namespace J3OMotors_v1._0.Models.Venta
{
    public class VentaAuto
    {
 
        public decimal PrecioTotal { get; set; }

        public string TipoPago { get; set; }
 
        public decimal SaldoPendiente { get; set; }

        public List<Pago> PagosAuto { get; set; } = new List<Pago>();
    }
}
