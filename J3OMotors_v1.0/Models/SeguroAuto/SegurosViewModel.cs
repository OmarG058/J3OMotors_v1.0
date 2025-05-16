using System.ComponentModel.DataAnnotations;

namespace J3OMotors_v1._0.Models.Seguro
{
    public class SegurosViewModel
    {
        [Key]
        public int IdSeguro { get; set; }
        [Required]
        public string Tipo { get; set; }
        [Required]
        public decimal Costo { get; set; }
        [Required]
        public string Proveedor { get; set; }
        [Required]
        public int Duracion { get; set; }
    }
}
