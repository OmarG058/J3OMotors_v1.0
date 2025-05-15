namespace J3OMotors_v1._0.Helpers.Rutas
{
    public class Routes
    {
        public static string UrlBaseApi { get; set; } = "https://localhost:7174/";

        //Authenticacion
        public static string UrlBaseApiAuth { get; set; } = UrlBaseApi + "api/auth/login";

        //USUARIO
        public static string UrlBaseApiUsuario { get; set; } = UrlBaseApi + "api/usuario";

        public static string UrlBaseApiUsuarioRegister { get; set; } = UrlBaseApi + "api/usuario/Register";
        public static string UsuarioPorId(int id) => $"{UrlBaseApiUsuario}/{id}";

        //CLIENTE
        public static string UrlBaseApiCliente { get; set; } = UrlBaseApi + "api/Cliente";
        public static string BuscarClientePorNombreYApellido(string nombre, string apellidos)
        {
            return $"{UrlBaseApi}api/Cliente/buscar?nombre={Uri.EscapeDataString(nombre)}&apellidos={Uri.EscapeDataString(apellidos)}";
        }
        public static string ActualizarClientePorId(int id)
        {
            return $"{UrlBaseApi}api/Cliente/{id}";
        }   
        public static string BuscarClientePorIdUsuario(int idUsuario)
        {
            return $"{UrlBaseApi}Cliente/usuario/{idUsuario}";
        }
        public static string ClientePorId(int id) => $"{UrlBaseApiCliente}/{id}";

    }
}
