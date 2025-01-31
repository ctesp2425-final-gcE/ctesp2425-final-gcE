using Microsoft.AspNetCore.Mvc;
using Moq;
using restaurante_C_api.Controllers;
using restaurante_C_api.Models;
using restaurante_C_api.Services;
using Xunit;

namespace restaurante_C_api.Tests
{
    public class UpdateReservasTests
    {
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ReservasController _controller;

        public UpdateReservasTests()
        {
            _reservaServiceMock = new Mock<IReservaService>();
            _controller = new ReservasController(_reservaServiceMock.Object);
        }

        #region Testes de AtualizarReserva

        [Fact]
        public void AtualizarReserva_DeveRetornar200QuandoAtualizadaComSucesso()
        {
            var reservaId = 1;
            var reserva = new ReservaModel { Id = reservaId, NomeCliente = "João Silva" };

            _reservaServiceMock.Setup(s => s.AtualizarReserva(reserva))
                .Returns(reserva);

            var result = _controller.AtualizarReserva(reservaId, reserva); // Ação

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(reserva, okResult.Value);
        }

        [Fact]
        public void AtualizarReserva_DeveRetornar400QuandoDadosNulos()
        {
            var result = _controller.AtualizarReserva(1, null); // Ação

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Dados da reserva inválidos.", badRequestResult.Value);
        }

        [Fact]
        public void AtualizarReserva_DeveRetornar400QuandoIdNaoCorresponde()
        {
            var reserva = new ReservaModel { Id = 2, NomeCliente = "João Silva" };

            var result = _controller.AtualizarReserva(1, reserva); // Ação

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Dados da reserva inválidos.", badRequestResult.Value);
        }

        [Fact]
        public void AtualizarReserva_DeveRetornar404QuandoNaoEncontrada()
        {
            var reserva = new ReservaModel { Id = 1, NomeCliente = "João Silva" };

            _reservaServiceMock.Setup(s => s.AtualizarReserva(reserva))
                .Returns((ReservaModel)null);

            var result = _controller.AtualizarReserva(1, reserva); // Ação

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Reserva não encontrada para atualização.", notFoundResult.Value);
        }

        #endregion
    }
}
