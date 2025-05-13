using System.ComponentModel.DataAnnotations;

namespace J3OMotors_v1._0.Models.Usuario
{
    public class UsuarioCreateViewModel
    {
        int? idUsuario  { get; set; } = null ;    
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
