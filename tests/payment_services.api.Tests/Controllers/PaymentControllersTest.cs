using Xunit;
using Moq;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using payments_services.api.Controllers;
using payments_services.application.Commands.Commands;
using payments_services.application.DTOs;
using payments_services.application.Queries.Queries;
using payments_services.domain.Interfaces;
using payments_services.domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace payments_services.tests.Controllers
{
    public class PaymentsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IHistorialPagosRepositoryPostgres> _historialRepoMock;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _historialRepoMock = new Mock<IHistorialPagosRepositoryPostgres>();
            _controller = new PaymentsController(_mediatorMock.Object, _historialRepoMock.Object);
        }

        [Fact]
        public async Task RegistrarMedioDePago_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new RegistrarMedioDePagoDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarMedioPagoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.RegistrarMedioDePago(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResultadoDTO>(okResult.Value);
            Assert.True(response.Exito);
        }

        [Fact]
        public async Task RegistrarMedioDePago_ShouldReturnBadRequest_WhenFailed()
        {
            var dto = new RegistrarMedioDePagoDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarMedioPagoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.RegistrarMedioDePago(dto);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var response = Assert.IsType<ResultadoDTO>(badRequestResult.Value);
            Assert.False(response.Exito);
        }

        [Fact]
        public async Task ObtenerMedioDePago_ShouldReturnOk()
        {
            var dto = new ConsultarMediosDePagoDTO();
            var expectedResponse = new MedioDePagoDTO
            {
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarMedioDePagoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse); 

            var result = await _controller.ObtenerMedioDePago(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task ObtenerMediosPagoUsuario_ShouldReturnOk()
        {
            var correo = "test@test.com";
            var expectedResponse = new List<MedioDePagoDTO>
            {
                new MedioDePagoDTO
                {
                    idMedioPago = "pm_123",
                    tipoMedioPago = "visa",
                    medioPredeterminado = true,
                    ultimosCuatroDigitos = "4242"
                }
            };

            _mediatorMock.Setup(m => m.Send(It.IsAny<ConsultarMediosDePagoQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            var result = await _controller.ObtenerMediosPagoUsuario(correo);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedResponse, okResult.Value);
        }

        [Fact]
        public async Task RealizarPago_ShouldReturnOk_WhenSuccessful()
        {
            var dto = new RegistrarPagoDTO();

            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPagoCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var result = await _controller.RealizarPago(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<ResultadoDTO>(okResult.Value);
            Assert.True(response.Exito);
        }

        [Fact]
        public async Task RealizarPago_ShouldReturnBadRequest_WhenApplicationExceptionOccurs()
        {
            var dto = new RegistrarPagoDTO();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RegistrarPagoCommand>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new ApplicationException("Error de negocio"));

            var result = await _controller.RealizarPago(dto);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetHistorialDePagoByEvento_ShouldReturnOk_WithHistorial()
        {
            var idEvento = Guid.NewGuid();
            var historial = new List<HistorialPagos> { new HistorialPagos() };
            _historialRepoMock.Setup(s => s.GetHistorialDePagosByEvento(idEvento, It.IsAny<CancellationToken>()))
                .ReturnsAsync(historial);

            var result = await _controller.GetHistorialDePagoByEvento(idEvento, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(historial, okResult.Value);
        }

        [Fact]
        public async Task GetHistorialDePagoByEvento_ShouldReturnEmptyList_WhenNoData()
        {
            var idEvento = Guid.NewGuid();
            _historialRepoMock.Setup(s => s.GetHistorialDePagosByEvento(idEvento, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<HistorialPagos>());

            var result = await _controller.GetHistorialDePagoByEvento(idEvento, CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<HistorialPagosDTO>>(okResult.Value);
            Assert.Empty(value);
        }

        [Fact]
        public async Task GetHistorialDePagoByEvento_ShouldReturn500_OnException()
        {
            var idEvento = Guid.NewGuid();
            _historialRepoMock.Setup(s => s.GetHistorialDePagosByEvento(idEvento, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Fatal error"));

            var result = await _controller.GetHistorialDePagoByEvento(idEvento, CancellationToken.None);

            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }
    }
}