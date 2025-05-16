namespace J3OMotors_v1._0.Models.Cotizar
{
    public class CotizacionViewModel
    {
        public int? IdCotizacion { get; set; }
     
        public DateTime Fecha { get; set; } = DateTime.Now;

        public int? IdSeguro { get; set; }
     
        public string IdAuto { get; set; }

        public decimal? CostoTotal { get; set; }
       
        public int IdCliente { get; set; }
        public string? IdAsiento { get; set; }
        public string? IdColor { get; set; }
        public string? IdRin { get; set; }
        public string TipoPago { get; set; }  //contado // o "Credito"
        public int? PlazoMeses { get; set; } // 12, 24 o 36 (si aplica)
        public decimal? Mensualidad { get; set; } // Si es crédito
    }
}
