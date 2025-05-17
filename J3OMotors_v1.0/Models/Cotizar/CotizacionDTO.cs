namespace J3OMotors_v1._0.Models.Cotizar
{
    public class CotizacionDTO
    {
        public int? IdCotizacion { get; set; }
        public DateTime Fecha { get; set; }
        public AutoDTO Auto { get; set; }
        public SeguroDTO Seguro { get; set; }
        public AccesorioDTO Asiento { get; set; }
        public AccesorioDTO Color { get; set; }
        public AccesorioDTO Rin { get; set; }
        public string TipoPago { get; set; }
        public int? PlazoMeses { get; set; }
        public decimal? Mensualidad { get; set; }
        public decimal? CostoTotal { get; set; }
        public int IdCliente { get; set; }
    }
}
