using Microsoft.AspNetCore.Mvc;
using Moq;
using restaurante_C_api.Controllers;
using restaurante_C_api.Models;
using restaurante_C_api.Services;
using System;
using System.Collections.Generic;
using Xunit;

namespace restaurante_C_api.Tests
{
    public class CreateGetReservasTests
    {
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ReservasController _controller;

        public CreateGetReservasTests()
        {
            _reservaServiceMock = new Mock<IReservaService>();
            _controller = new ReservasController(_reservaServiceMock.Object);
        }

        #region Testes de CriarReserva

        [Fact]
        public void CriarReserva_DeveRetornar201QuandoCriadaComSucesso()
        {
            // Arrange
            var reserva = new ReservaModel
            {
                NomeCliente = "João Silva",
                DataReserva = DateTime.Now.Date,
                HorarioReserva = TimeSpan.FromHours(19),
                NumeroMesa = 5,
                NumeroPessoas = 4
            };

            _reservaServiceMock.Setup(s => s.ObterReservasPorMesa(reserva.NumeroMesa))
                .Returns(new List<ReservaModel>());

            _reservaServiceMock.Setup(s => s.CriarReserva(reserva))
                .Returns(reserva);

            // Act
            var result = _controller.CriarReserva(reserva);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdResult.StatusCode);
            Assert.Equal(reserva, createdResult.Value);
        }

        [Fact]
        public void CriarReserva_DeveRetornar400QuandoNumeroMesaInvalido()
        {
            // Arrange
            var reserva = new ReservaModel
            {
                NomeCliente = "João Silva",
                DataReserva = DateTime.Now.Date,
                HorarioReserva = TimeSpan.FromHours(19),
                NumeroMesa = 35, // Número inválido
                NumeroPessoas = 4
            };

            // Act
            var result = _controller.CriarReserva(reserva);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Número da mesa inválido. O restaurante possui mesas numeradas de 1 a 30.", badRequestResult.Value);
        }

        [Fact]
        public void CriarReserva_DeveRetornar409QuandoConflitoDeHorario()
        {
            // Arrange
            var reserva = new ReservaModel
            {
                NomeCliente = "João Silva",
                DataReserva = DateTime.Now.Date,
                HorarioReserva = TimeSpan.FromHours(19),
                NumeroMesa = 5,
                NumeroPessoas = 4
            };

            var reservasExistentes = new List<ReservaModel>
            {
                new ReservaModel
                {
                    NomeCliente = "Maria Silva",
                    DataReserva = DateTime.Now.Date,
                    HorarioReserva = TimeSpan.FromHours(18.5),
                    NumeroMesa = 5,
                    NumeroPessoas = 2
                }
            };

            _reservaServiceMock.Setup(s => s.ObterReservasPorMesa(reserva.NumeroMesa))
                .Returns(reservasExistentes);

            // Act
            var result = _controller.CriarReserva(reserva);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal(409, conflictResult.StatusCode);
            Assert.Equal("Já existe uma reserva para esta mesa em um intervalo de 1 hora e 30 minutos.", conflictResult.Value);
        }

        #endregion

        #region Testes de ObterTodasReservas

        [Fact]
        public void ObterTodasReservas_DeveRetornar200ComListaDeReservas()
        {
            // Arrange
            var reservas = new List<ReservaModel>
            {
                new ReservaModel { NomeCliente = "João Silva", NumeroMesa = 1, DataReserva = DateTime.Now.Date },
                new ReservaModel { NomeCliente = "Maria Silva", NumeroMesa = 2, DataReserva = DateTime.Now.Date }
            };

            _reservaServiceMock.Setup(s => s.ObterTodasReservas())
                .Returns(reservas);

            // Act
            var result = _controller.ObterTodasReservas(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<List<ReservaModel>>(okResult.Value); // Trabalhe diretamente com a lista
            Assert.Equal(2, response.Count); // Verifique o número de itens na lista
        }

        [Fact]
        public void ObterTodasReservas_DeveRetornar200ComListaVaziaSeNaoExistiremReservas()
        {
            // Arrange
            _reservaServiceMock.Setup(s => s.ObterTodasReservas())
                .Returns(new List<ReservaModel>());

            // Act
            var result = _controller.ObterTodasReservas(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            var response = Assert.IsType<List<ReservaModel>>(okResult.Value);
            Assert.Empty(response); // Certifique-se de que a lista está vazia
        }

        #endregion

        #region Testes de ObterReservaPorId

        [Fact]
        public void ObterReservaPorId_DeveRetornar404QuandoNaoEncontrada()
        {
            // Arrange
            var reservaId = 1;

            _reservaServiceMock.Setup(s => s.ObterReservaPorId(reservaId))
                .Returns((ReservaModel)null);

            // Act
            var result = _controller.ObterReservaPorId(reservaId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            Assert.Equal("Reserva não encontrada.", notFoundResult.Value);
        }

        [Fact]
        public void ObterReservaPorId_DeveRetornar200QuandoEncontrada()
        {
            // Arrange
            var reservaId = 1;
            var reserva = new ReservaModel { Id = reservaId, NomeCliente = "João Silva" };

            _reservaServiceMock.Setup(s => s.ObterReservaPorId(reservaId))
                .Returns(reserva);

            // Act
            var result = _controller.ObterReservaPorId(reservaId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(reserva, okResult.Value);
        }

        #endregion
    }
}
