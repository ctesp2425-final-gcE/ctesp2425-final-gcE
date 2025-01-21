using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<ReservaModel> _reservas;

        public ReservaService()
        {
            _reservas = new List<ReservaModel>();
        }

        public ReservaModel CriarReserva(ReservaModel reserva)
        {
            reserva.Id = _reservas.Count > 0 ? _reservas.Max(r => r.Id) + 1 : 1;
            reserva.DataCriacao = DateTime.Now;
            _reservas.Add(reserva);
            return reserva;
        }

        public IEnumerable<ReservaModel> ObterTodasReservas()
        {
            return _reservas;
        }

        public ReservaModel ObterReservaPorId(int id)
        {
            return _reservas.FirstOrDefault(r => r.Id == id);
        }

        public IEnumerable<ReservaModel> ObterReservasPorMesa(int numeroMesa)
        {
            return _reservas.Where(r => r.NumeroMesa == numeroMesa);
        }

        public IEnumerable<ReservaModel> ObterReservasPorData(DateTime data)
        {
            return _reservas.Where(r => r.DataReserva.Date == data.Date);
        }

        public ReservaModel AtualizarReserva(ReservaModel reserva)
        {
            var reservaExistente = _reservas.FirstOrDefault(r => r.Id == reserva.Id);

            if (reservaExistente == null)
            {
                return null;
            }

            reservaExistente.NomeCliente = reserva.NomeCliente;
            reservaExistente.DataReserva = reserva.DataReserva;
            reservaExistente.HorarioReserva = reserva.HorarioReserva;
            reservaExistente.NumeroMesa = reserva.NumeroMesa;
            reservaExistente.NumeroPessoas = reserva.NumeroPessoas;

            return reservaExistente;
        }

        public bool CancelarReserva(int id)
        {
            var reserva = _reservas.FirstOrDefault(r => r.Id == id);

            if (reserva == null)
            {
                return false;
            }

            _reservas.Remove(reserva);
            return true;
        }
    }
}
