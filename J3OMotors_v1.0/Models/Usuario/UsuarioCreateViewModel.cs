using System.ComponentModel.DataAnnotations;

namespace J3OMotors_v1._0.Models.Usuario
{
    public class UsuarioCreateViewModel
    {
        public int idUsuario  { get; set; }   
        [Required]
        [EmailAddress]

        public string correo { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string contrasenia { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int idTipo { get; set; } = 2; // Por defecto cliente

        
    }
}
