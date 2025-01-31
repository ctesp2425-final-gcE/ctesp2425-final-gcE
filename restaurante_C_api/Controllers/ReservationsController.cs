using Microsoft.AspNetCore.Mvc;
using restaurante_C_api.Models;
using restaurante_C_api.Services;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;

namespace restaurante_C_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservasController : ControllerBase
    {
        private readonly IReservaService _reservaService;

        public ReservasController(IReservaService reservaService)
        {
            _reservaService = reservaService;
        }

        // Endpoint para criar uma nova reserva
        [HttpPost]
        [SwaggerOperation(Summary = "Cria uma nova reserva", Description = "Adiciona uma nova reserva com as informações fornecidas.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CriarReserva([FromBody] ReservaModel reserva)
        {
            if (reserva == null)
            {
                return BadRequest("Dados da reserva inválidos.");
            }

            if (reserva.NumeroMesa < 1 || reserva.NumeroMesa > 30)
            {
                return BadRequest("Número da mesa inválido. O restaurante possui mesas numeradas de 1 a 30.");
            }

            if (reserva.NumeroPessoas < 1 || reserva.NumeroPessoas > 20)
            {
                return BadRequest("Número de pessoas inválido. Cada mesa suporta de 1 a 20 pessoas.");
            }

            // Verifica conflitos de horário
            var reservasExistentes = _reservaService.ObterReservasPorMesa(reserva.NumeroMesa);

            if (reservasExistentes.Any(r =>
                r.DataReserva == reserva.DataReserva &&
                Math.Abs((r.HorarioReserva - reserva.HorarioReserva).TotalMinutes) < 90))
            {
                return Conflict("Já existe uma reserva para esta mesa num intervalo de 1 hora e 30 minutos.");
            }

            var novaReserva = _reservaService.CriarReserva(reserva);

            if (novaReserva != null)
            {
                return CreatedAtAction(nameof(ObterReservaPorId), new { id = novaReserva.Id }, novaReserva);
            }
            return StatusCode(500, "Ocorreu um erro ao criar a reserva.");
        }

        // Endpoint para listar todas as reservas com paginação e filtros
        [HttpGet]
        [SwaggerOperation(Summary = "Lista todas as reservas", Description = "Obtém todas as reservas com suporte a paginação e filtros.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult ObterTodasReservas([FromQuery] DateTime? data)
        {
            var query = _reservaService.ObterTodasReservas().AsQueryable();

            if (data.HasValue)
            {
                query = query.Where(r => r.DataReserva.Date == data.Value.Date);
            }
            var reservas = query.ToList();
            return Ok(reservas);
        }

        // Endpoint para obter detalhes de uma reserva específica
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Obtém detalhes de uma reserva", Description = "Retorna os detalhes de uma reserva específica pelo ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult ObterReservaPorId(int id)
        {
            var reserva = _reservaService.ObterReservaPorId(id);

            if (reserva == null)
            {
                return NotFound("Reserva não encontrada.");
            }
            return Ok(reserva);
        }

        // Endpoint para atualizar uma reserva existente
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Atualiza uma reserva", Description = "Atualiza os detalhes de uma reserva existente pelo ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult AtualizarReserva(int id, [FromBody] ReservaModel reserva)
        {
            if (reserva == null || reserva.Id != id)
            {
                return BadRequest("Dados da reserva inválidos.");
            }

            var reservaAtualizada = _reservaService.AtualizarReserva(reserva);

            if (reservaAtualizada == null)
            {
                return NotFound("Reserva não encontrada para atualização.");
            }
            return Ok(reservaAtualizada);
        }

        // Endpoint para cancelar uma reserva
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Cancela uma reserva", Description = "Remove uma reserva específica pelo ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult CancelarReserva(int id)
        {
            var sucesso = _reservaService.CancelarReserva(id);
            if (!sucesso)
            {
                return NotFound("Reserva não encontrada para cancelamento.");
            }
            return NoContent();
        }

        // Endpoint para obter reservas por data
        [HttpGet("data")]
        [SwaggerOperation(Summary = "Lista reservas por data", Description = "Obtém todas as reservas para uma data específica.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult ObterReservasPorData([FromQuery] DateTime data)
        {
            var reservas = _reservaService.ObterReservasPorData(data);
            return Ok(reservas);
        }
    }
}
