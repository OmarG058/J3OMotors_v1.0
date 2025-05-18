namespace J3OMotors_v1._0.Models.Venta
{
    public class Venta
    {

        public string? Id { get; set; }
        public int IdCotizacion { get; set; }

        public string IdAuto { get; set; }

        public int IdCliente { get; set; }

        public int? IdSeguro { get; set; }

        public DateTime Fecha { get; set; }

        public VentaAuto VentaAuto { get; set; }

        public VentaSeguro VentaSeguro { get; set; }
    }
}
