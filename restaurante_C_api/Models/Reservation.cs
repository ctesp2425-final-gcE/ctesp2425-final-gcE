using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace restaurante_C_api.Models
{
    [Table("Reservations")]
    public class ReservaModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do cliente não pode exceder 100 caracteres.")]
        public string NomeCliente { get; set; }

        [Required(ErrorMessage = "A data da reserva é obrigatória.")]
        [DataType(DataType.Date, ErrorMessage = "A data da reserva deve estar num formato válido.")]
        public DateTime DataReserva { get; set; }

        [Required(ErrorMessage = "O horário da reserva é obrigatório.")]
        [DataType(DataType.Time, ErrorMessage = "O horário da reserva deve estar num formato válido.")]
        public TimeSpan HorarioReserva { get; set; }

        [Required(ErrorMessage = "O número da mesa é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O número da mesa deve ser maior que zero.")]
        public int NumeroMesa { get; set; }

        [Required(ErrorMessage = "O número de pessoas é obrigatório.")]
        [Range(1, int.MaxValue, ErrorMessage = "O número de pessoas deve ser maior que zero.")]
        public int NumeroPessoas { get; set; }

        [Required(ErrorMessage = "A data de criação é obrigatória.")]
        [DataType(DataType.DateTime, ErrorMessage = "A data de criação deve estar num formato válido.")]
        public DateTime DataCriacao { get; set; } = DateTime.Now;
    }
}
