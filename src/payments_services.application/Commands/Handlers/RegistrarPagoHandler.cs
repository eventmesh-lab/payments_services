using Events.Shared;
using MassTransit;
using MediatR;
using payments_services.application.Commands.Commands;
using payments_services.application.Interfaces;
using payments_services.domain.Entities;
using payments_services.domain.Factory;
using payments_services.domain.Interfaces;
using Polly;
using Polly.Retry;
using Stripe.Forwarding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using payments_services.application.DTOs;
using static MassTransit.ValidationResultExtensions;


namespace payments_services.application.Commands.Handlers
{
    public class RegistrarPagoHandler : IRequestHandler<RegistrarPagoCommand, bool>
    {
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un usuario en el Microservicio Usuarios, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IUsuarioService _usuarioService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un pago o medio de pago con Stripe, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly IStripeService _stripeService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre un historial de pagos, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly  IHistorialPagosRepositoryPostgres _historialPagosService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre las notificaciones en el Microservicio Notificaciones, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly INotificationServices _notificacionService;
        /// <summary>
        /// Atributo que corresponde a las operaciones posibles que se pueden realizar sobre una reserva en el Microservicio Reserva, el cual será inyectado por inversión de dependencias.
        /// </summary>
        private readonly ICouponServices _couponServices;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IActivityService _activityService;

        public RegistrarPagoHandler(IUsuarioService usuarioService, IStripeService stripeService, IHistorialPagosRepositoryPostgres historialPagosService,
                                    INotificationServices notificacionService, IActivityService activityService, IReservaService reservaService, ICouponServices couponServices, IPublishEndpoint publishEndpoint)
        {
            _stripeService = stripeService;
            _usuarioService = usuarioService;
            _historialPagosService = historialPagosService;
            _notificacionService = notificacionService;
            _couponServices = couponServices;
            _publishEndpoint = publishEndpoint;
            _activityService = activityService;
        }

        public async Task<bool> Handle(RegistrarPagoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var idUsuario = await _usuarioService.ObtenerUsuarioPorEmailAsync(request.medioDePagoDTO.correo);

                if (idUsuario == Guid.Empty)
                    throw new ApplicationException("El usuario no existe en la base de datos.");

                var idUsuarioStripe = await _stripeService.ObtenerUsuarioStripeAsync(idUsuario);
                var montoLong = (long)(request.medioDePagoDTO.monto * 100);

                var retryPolicy = Policy
                    .Handle<Exception>()
                    .WaitAndRetryAsync(
                        retryCount: 2,
                        sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                        onRetry: (exception, timespan, attempt, context) =>
                        {
                            Console.WriteLine($"Reintento Stripe {attempt} tras error: {exception.Message}");
                        });

                var pagoRealizado = await retryPolicy.ExecuteAsync(async () =>
                {
                    return await _stripeService.RegistrarPagoAsync(
                        idUsuarioStripe,
                        request.medioDePagoDTO.stripeMedioPagoId,
                        request.medioDePagoDTO.idEvento,
                        idUsuario,
                        request.medioDePagoDTO.moneda,
                        montoLong);
                });

                if (pagoRealizado.Status != "succeeded")
                    throw new ApplicationException("No se ha podido realizar el pago de la reserva.");

                var medioPago = await _stripeService.ObtenerMedioDePagoStripeAsync(idUsuarioStripe, request.medioDePagoDTO.stripeMedioPagoId);
              
                var historialPagos = HistorialPagosFactory.CrearHistorialPagos(
                    idUsuario,
                    request.medioDePagoDTO.idEvento,
                    request.medioDePagoDTO.stripeMedioPagoId,
                    request.medioDePagoDTO.monto,
                    medioPago.UltimosDigitosTarjeta,
                    medioPago.TipoTarjeta.tipoPago);

                var historialPagosId = await _historialPagosService.RegistrarHistorialPagosAsync(historialPagos);

                if (historialPagosId == Guid.Empty)
                    throw new ApplicationException("Fallo al registrar el historial de pago en PostgreSQL");

                var monto = request.medioDePagoDTO.monto.ToString() + request.medioDePagoDTO.moneda;
                await _notificacionService.EnviarPagoExitoso(request.medioDePagoDTO.correo, monto);

                if (request.medioDePagoDTO.idCoupon != Guid.Empty)
                {
                    await _couponServices.MarcarCuponComoUsado(request.medioDePagoDTO.idCoupon);
                }
                await _publishEndpoint.Publish<IAuditLogCreated>(new
                {
                    Id = Guid.NewGuid(),
                    ServicioOrigen = "payment_services",
                    UsuarioId = idUsuario.ToString(),
                    TipoAccion = "Registro de pago",
                    Datos = JsonSerializer.Serialize(new { Accion = $"Pago realizado por {request.medioDePagoDTO.monto}{request.medioDePagoDTO.moneda}" }),
                    FechaOcurrencia = DateTime.UtcNow,
                    Nivel = "Info"
                }, cancellationToken);
                await _activityService.RegisterActivityAsync(
                    email: request.medioDePagoDTO.correo,
                    action: $"Pago regitrado por {request.medioDePagoDTO.monto}{request.medioDePagoDTO.moneda}",
                    category: "Seguridad"
                );
                var datosCorreo = new EnviarCorreoPagoExitosoDto
                {
                    Destinatario = request.medioDePagoDTO.correo,
                    MontoPago = $"{request.medioDePagoDTO.monto}{request.medioDePagoDTO.moneda}",
                    FechaPago = DateTime.UtcNow
                };
                await _notificacionService.EnviarCorreoPagoExitosoDetallado(datosCorreo);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}
