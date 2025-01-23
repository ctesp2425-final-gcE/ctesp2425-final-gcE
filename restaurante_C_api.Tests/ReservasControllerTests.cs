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
    public class ReservasControllerTests
    {
        private readonly Mock<IReservaService> _reservaServiceMock;
        private readonly ReservasController _controller;

        public ReservasControllerTests()
        {
            _reservaServiceMock = new Mock<IReservaService>();
            _controller = new ReservasController(_reservaServiceMock.Object);
        }

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
        public void CriarReserva_DeveRetornar400QuandoNumeroPessoasInvalido()
        {
            // Arrange
            var reserva = new ReservaModel
            {
                NomeCliente = "João Silva",
                DataReserva = DateTime.Now.Date,
                HorarioReserva = TimeSpan.FromHours(19),
                NumeroMesa = 5,
                NumeroPessoas = 25 // Número inválido
            };

            // Act
            var result = _controller.CriarReserva(reserva);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Número de pessoas inválido. Cada mesa suporta de 1 a 20 pessoas.", badRequestResult.Value);
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

        public IActionResult ObterTodasReservas(string filtro, int pagina, int tamanhoPagina)
        {
            var reservas = _reservaService.ObterTodasReservas(); // Obtém todas as reservas

            var response = new
            {
                TotalRegistros = reservas.Count,
                Reservas = reservas
            };

            return Ok(response);
        }


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
            var result = _controller.ObterTodasReservas(null, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            // Verificar a estrutura do objeto retornado
            var response = okResult.Value as dynamic;
            Assert.NotNull(response);

            // Testar as propriedades do objeto retornado
            Assert.Equal(2, (int)response.TotalRegistros);
            Assert.Equal(2, ((IEnumerable<ReservaModel>)response.Reservas).Count());
        }


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
    }
}