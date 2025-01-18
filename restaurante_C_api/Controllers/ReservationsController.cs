using Microsoft.AspNetCore.Mvc;
using restaurante_C_api.Models;
using restaurante_C_api.Services;
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
        public IActionResult CriarReserva([FromBody] ReservaModel reserva)
        {
            if (reserva == null)
            {
                return BadRequest("Dados da reserva inválidos.");
            }

            // Verificar conflitos de horário
            var reservasExistentes = _reservaService.ObterReservasPorMesa(reserva.NumeroMesa);

            if (reservasExistentes.Any(r =>
                r.DataReserva == reserva.DataReserva &&
                Math.Abs((r.HorarioReserva - reserva.HorarioReserva).TotalMinutes) < 90))
            {
                return Conflict("Já existe uma reserva para esta mesa em um intervalo de 1 hora e 30 minutos.");
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
        public IActionResult ObterTodasReservas([FromQuery] DateTime? data, [FromQuery] int pagina = 1, [FromQuery] int tamanho = 10)
        {
            if (pagina <= 0 || tamanho <= 0)
            {
                return BadRequest("Página e tamanho devem ser maiores que zero.");
            }

            var query = _reservaService.ObterTodasReservas().AsQueryable();

            if (data.HasValue)
            {
                query = query.Where(r => r.DataReserva.Date == data.Value.Date);
            }

            var totalRegistros = query.Count();
            var reservasPaginadas = query
                .Skip((pagina - 1) * tamanho)
                .Take(tamanho)
                .ToList();

            var resultado = new
            {
                TotalRegistros = totalRegistros,
                PaginaAtual = pagina,
                TamanhoPagina = tamanho,
                TotalPaginas = (int)Math.Ceiling(totalRegistros / (double)tamanho),
                Reservas = reservasPaginadas
            };

            return Ok(resultado);
        }

        // Endpoint para obter detalhes de uma reserva específica
        [HttpGet("{id}")]
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
        public IActionResult ObterReservasPorData([FromQuery] DateTime data)
        {
            var reservas = _reservaService.ObterReservasPorData(data);
            return Ok(reservas);
        }
    }
}
