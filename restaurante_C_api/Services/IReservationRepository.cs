using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using restaurante_C_api.Data;
using restaurante_C_api.Models;

namespace restaurante_C_api.Services
{
    public interface IReservaService
    {
        ReservaModel CriarReserva(ReservaModel reserva);
        IEnumerable<ReservaModel> ObterTodasReservas();
        ReservaModel ObterReservaPorId(int id);
        IEnumerable<ReservaModel> ObterReservasPorMesa(int numeroMesa);
        IEnumerable<ReservaModel> ObterReservasPorData(DateTime data);
        ReservaModel AtualizarReserva(ReservaModel reserva);
        bool CancelarReserva(int id);
    }

    public class ReservaService : IReservaService
    {
        private readonly AppDbContext _context;

        public ReservaService(AppDbContext context)
        {
            _context = context;
        }

        public ReservaModel CriarReserva(ReservaModel reserva)
        {
            reserva.DataCriacao = DateTime.Now;
            _context.Reservas.Add(reserva);
            _context.SaveChanges();
            return reserva;
        }

        public IEnumerable<ReservaModel> ObterTodasReservas()
        {
            return _context.Reservas.AsNoTracking().ToList();
        }

        public ReservaModel ObterReservaPorId(int id)
        {
            return _context.Reservas.AsNoTracking().FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<ReservaModel> ObterReservasPorMesa(int numeroMesa)
        {
            return _context.Reservas.Where(r => r.NumeroMesa == numeroMesa).ToList();
        }

        public IEnumerable<ReservaModel> ObterReservasPorData(DateTime data)
        {
            return _context.Reservas.Where(r => r.DataReserva.Date == data.Date).ToList();
        }

        public ReservaModel AtualizarReserva(ReservaModel reserva)
        {
            var reservaExistente = _context.Reservas.Find(reserva.Id);
            if (reservaExistente == null)
                return null;

            reservaExistente.NomeCliente = reserva.NomeCliente;
            reservaExistente.DataReserva = reserva.DataReserva;
            reservaExistente.HorarioReserva = reserva.HorarioReserva;
            reservaExistente.NumeroMesa = reserva.NumeroMesa;
            reservaExistente.NumeroPessoas = reserva.NumeroPessoas;

            _context.SaveChanges();
            return reservaExistente;
        }

        public bool CancelarReserva(int id)
        {
            var reserva = _context.Reservas.Find(id);
            if (reserva == null)
                return false;

            _context.Reservas.Remove(reserva);
            _context.SaveChanges();
            return true;
        }
    }
}