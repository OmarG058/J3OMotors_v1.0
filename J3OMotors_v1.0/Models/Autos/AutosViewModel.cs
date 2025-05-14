namespace J3OMotors_v1._0.Models.Autos
{
    public class AutosViewModel
    {

        public string? Id { get; set; }
        public string Modelo { get; set; }
        public required string FechaModelo { get; set; }
        public decimal Costo { get; set; }
        public string Fabricante { get; set; }
        public string NumeroSerie { get; set; }
        public string? RutaImagen { get; set; }
    }
}
