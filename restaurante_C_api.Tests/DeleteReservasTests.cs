using Microsoft.AspNetCore.Mvc;
using Moq;
using restaurante_C_api.Controllers;
using restaurante_C_api.Models;
using restaurante_C_api.Services;
using Xunit;

namespace restaurante_C_api.Tests
{
    public class DeleteReservasTests
    {
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ReservasController _controller;

        public DeleteReservasTests()
        {
            _reservaServiceMock = new Mock<IReservaService>();
            _controller = new ReservasController(_reservaServiceMock.Object);
        }

        #region Testes de CancelarReserva

        [Fact]
        public void CancelarReserva_DeveRetornar204QuandoCanceladaComSucesso()
        {
            var reservaId = 1;

            _reservaServiceMock.Setup(s => s.CancelarReserva(reservaId))
                .Returns(true);

            var result = _controller.CancelarReserva(reservaId); // Ação

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void CancelarReserva_DeveRetornar404QuandoNaoEncontrada()
        {
            var reservaId = 1;

            _reservaServiceMock.Setup(s => s.CancelarReserva(reservaId))
                .Returns(false);

            var result = _controller.CancelarReserva(reservaId); // Ação

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Reserva não encontrada para cancelamento.", notFoundResult.Value);
        }

        #endregion
    }
}
