using MediatR;
using Microsoft.AspNetCore.Mvc;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;
using payments_services.application.Queries.Queries;
using Stripe.Forwarding;
using System.Threading;

namespace payments_services.api.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        /// <summary>
        /// Atributo que se encarga de enviar solicitudes (commands/queries) mediante el patrón mediador
        /// </summary>
        private readonly IMediator _mediator;
        public PaymentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Endpoint encargado de registrar un nuevo medio de pago.
        /// </summary>
        /// <param name="medioDePagoDto">Parametro de tipo DTO con los datos del medio de pago a registrar.</param>
        /// <returns>Resultado de la operación con mensaje y estado dependiendo del resultado.</returns>
        [HttpPost("registroMedioDePago")]
        public async Task<IActionResult> RegistrarMedioDePago([FromBody] RegistrarMedioDePagoDTO medioDePagoDto)
        {
            var resultado = await _mediator.Send(new RegistrarMedioPagoCommand(medioDePagoDto));
            if (resultado)
            {
                return Ok(new ResultadoDTO { Mensaje = "El medio de pago se registró exitosamente.", Exito = true });
            }

            return BadRequest(new ResultadoDTO { Mensaje = "El medio de pago no pudo ser registrada.", Exito = false });
        }
        /// <summary>
        /// Endpoint encargado de consultar un medio de pago.
        /// </summary>
        /// <param name="medioDePagoDTO">Parametro de tipo DTO con los datos del medio de pago a consultar.</param>
        /// <returns>Retorna un objeto Medio de Pago con su detalle.</returns>
        [HttpPost("obtenerMedioDePago")]
        public async Task<IActionResult> ObtenerMedioDePago([FromBody] ConsultarMediosDePagoDTO medioDePagoDTO)
        {
            var resultado = await _mediator.Send(new ConsultarMedioDePagoQuery(medioDePagoDTO));
            return Ok(resultado);
        }

        /// <summary>
        /// Endpoint encargado de consultar los medios de pago de un usuario.
        /// </summary>
        /// <param name="correo">Parametro de que corresponde al correo del usuario cuyos medios de pago se van a consultar.</param>
        /// <returns>Retorna una lista de objetos Medio de Pago con su detalle.</returns>
        [HttpGet("obtenerMediosDePagoUsuario/{correo}")]
        public async Task<IActionResult> ObtenerMediosPagoUsuario([FromRoute] string correo)
        {
            var resultado = await _mediator.Send(new ConsultarMediosDePagoQuery(correo));
            return Ok(resultado);
        }

        /// <summary>
        /// Endpoint encargado de registrar el pago de una resera.
        /// </summary>
        /// <param name="medioDePagoDto">Parametro de tipo DTO con los datos del pago a registrar.</param>
        /// <returns>Resultado de la operación con mensaje y estado dependiendo del resultado.</returns>

        [HttpPost("realizarPagoReserva")]
        public async Task<IActionResult> RealizarPago([FromBody] RegistrarPagoDTO medioDePagoDto)
        {
            try
            {
                var resultado = await _mediator.Send(new RegistrarPagoCommand(medioDePagoDto));
                return Ok(new ResultadoDTO { Mensaje = "El pago se ha registrado exitosamente.", Exito = true });
                
            }
            catch (ApplicationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("obtenertHistorialPagosUsuario/{correo}")]
        public async Task<IActionResult> obtenerHistorialPagosUsuario([FromRoute] string correo)
        {
            var resultado = await _mediator.Send(new ConsultarHistorialPagosUsuarioQuery(correo));
            return Ok(resultado);
        }

        [HttpGet("obtenertHistorialPagos")]
        public async Task<IActionResult> obtenerHistorialPagos()
        {
            var resultado = await _mediator.Send(new ConsultarPagosQuery());
            return Ok(resultado);
        }
    }
}
