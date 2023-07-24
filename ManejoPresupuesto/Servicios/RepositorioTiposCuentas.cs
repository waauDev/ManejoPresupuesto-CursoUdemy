using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;
using System.Data.Common;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTiposCuentas
    {
        Task Actualizar(TipoCuenta tipoCuenta);
        Task Borrar(int id);
        Task Crear(TipoCuenta tipoCuenta);
        Task<bool> Existe(string nombre, int usuarioId);
        Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId);
        Task<TipoCuenta> ObtenerPorID(int id, int usuarioID);
        Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenados);
    }
    public class RepositorioTiposCuentas : IRepositorioTiposCuentas
    {
        private readonly string connectionString;
        public RepositorioTiposCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>($@"TiposCuentas_Insertar",
                                            new {usuarioId = tipoCuenta.USUARIO_ID, nombre= tipoCuenta.NOMBRE},
                                                        commandType: System.Data.CommandType.StoredProcedure);

            tipoCuenta.ID = id;
        }

        public async Task<bool> Existe(string nombre, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            var existe = await connection.QueryFirstOrDefaultAsync<int>($@"SELECT 1 FROM TIPOS_CUENTAS 
                                                        WHERE NOMBRE=@NOMBRE AND USUARIO_ID=@usuarioId;",
                                                        new { nombre, usuarioId });
            return existe == 1;
        }

        public async Task<IEnumerable<TipoCuenta>> Obtener(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<TipoCuenta>(@"SELECT ID, NOMBRE, ORDEN FROM TIPOS_CUENTAS WHERE USUARIO_ID=@usuarioId ORDER BY ORDEN ASC",
                                                            new { usuarioId });
        }

        public async Task Actualizar(TipoCuenta tipoCuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE TIPOS_CUENTAS SET NOMBRE=@NOMBRE WHERE ID=@Id ", tipoCuenta);
        }

        public async Task<TipoCuenta> ObtenerPorID(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<TipoCuenta>(@"SELECT ID, NOMBRE, USUARIO_ID FROM TIPOS_CUENTAS WHERE
                                                                          ID=@id and USUARIO_ID=@usuarioId", new {id,usuarioId});
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM TIPOS_CUENTAS WHERE ID=@id", new { id });
        }

        public async Task Ordenar(IEnumerable<TipoCuenta> tipoCuentaOrdenados)
        {
            var query = "UPDATE TIPOS_CUENTAS SET ORDEN=@ORDEN WHERE ID=@ID";
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(query, tipoCuentaOrdenados);
        }

    }


}
