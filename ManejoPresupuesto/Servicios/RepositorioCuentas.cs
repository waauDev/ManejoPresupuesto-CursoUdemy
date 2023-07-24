using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{

    public interface IRepositorioCuentas
    {
        Task Actualizar(CuentaCreacionViewModel cuenta);
        Task Borrar(int id);
        Task<IEnumerable<Cuenta>> Buscar(int usuarioId);
        Task Crear(Cuenta cuenta);
        Task<Cuenta> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCuentas:IRepositorioCuentas
    {
        private readonly string connectionString;

        public RepositorioCuentas(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Cuenta cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            var id = await connection.QuerySingleAsync<int>(@"INSERT INTO CUENTAS (NOMBRE, TIPO_CUENTA_ID, DESCRIPCION,BALANCE) 
                                                       VALUES (@NOMBRE, @TIPOCUENTAID,@DESCRIPCION,@BALANCE) SELECT SCOPE_IDENTITY();", cuenta);

            cuenta.Id = id;
        }

        public async Task<IEnumerable<Cuenta>> Buscar(int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Cuenta>(@"SELECT CUENTAS.ID, CUENTAS.NOMBRE, BALANCE, TC.NOMBRE AS TIPO_CUENTA
                                                          FROM CUENTAS
                                                          INNER JOIN TIPOS_CUENTAS TC
                                                          ON TC.ID= CUENTAS.TIPO_CUENTA_ID
                                                          WHERE TC.USUARIO_ID = @usuarioId
                                                          ORDER BY TC.ORDEN", new { usuarioId });
        }

        public async Task<Cuenta> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Cuenta>(
                @"SELECT CUENTAS.ID, CUENTAS.NOMBRE, BALANCE, DESCRIPCION, TC.ID 
                 FROM CUENTAS
                 INNER JOIN TIPOS_CUENTAS TC
                 ON TC.ID= CUENTAS.TIPO_CUENTA_ID
                 WHERE TC.USUARIO_ID = @usuarioId
                 AND CUENTAS.ID =@id ", new { id,usuarioId });
        }

        public async Task Actualizar(CuentaCreacionViewModel cuenta)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"UPDATE CUENTAS SET NOMBRE=@NOMBRE, 
                                            BALANCE=@BALANCE, DESCRIPCION=@DESCRIPCION, TIPO_CUENTA_ID= @TIPOCUENTAID WHERE ID=@Id ", cuenta);
        }


        public async Task Borrar (int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("DELETE FROM CUENTAS WHERE ID=@Id", new { id });
        }
    }
}
