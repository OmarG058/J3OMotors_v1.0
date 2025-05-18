namespace J3OMotors_v1._0.Models.Venta
{
    public class VentaSeguro
    {

        public string Estado { get; set; }

        public DateTime FechaContratacion { get; set; }

        public DateTime FechaFinalizacion { get; set; }

        public decimal PrecioTotal { get; set; }

        public decimal SaldoPendiente { get; set; }

        public List<Pago>? PagosSeguro { get; set; } = new List<Pago>();
    }
}
