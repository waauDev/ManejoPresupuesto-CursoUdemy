using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioCategorias
    {
        Task Actualizar(Categoria categoria);
        Task Borrar(int id);
        Task Crear(Categoria categoria);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId);
        Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId);
        Task<Categoria> ObtenerPorId(int id, int usuarioId);
    }
    public class RepositorioCategorias: IRepositorioCategorias
    {
        private readonly string connectionString;
        public RepositorioCategorias(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task Crear(Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(
                @"INSERT INTO CATEGORIAS (NOMBRE, TIPO_OPERACION_ID, USUARIOID) VALUES(@NOMBRE,@TipoOperacionId, @UsuarioId);
                SELECT SCOPE_IDENTITY();", categoria);

            categoria.Id = id;
        }

        public async Task<IEnumerable<Categoria>> Obtener (int usuarioId) 
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>(@"SELECT ID,NOMBRE, TIPO_OPERACION_ID AS TipoOperacionId, USUARIOID  FROM CATEGORIAS WHERE USUARIOID =@usuarioId;", new { usuarioId });

        }

        public async Task<IEnumerable<Categoria>> Obtener(int usuarioId, TipoOperacion tipoOperacionId)
        {
            using var connection = new SqlConnection(connectionString);

            return await connection.QueryAsync<Categoria>(@"
            SELECT ID,NOMBRE, TIPO_OPERACION_ID AS TipoOperacionId, USUARIOID  
            FROM CATEGORIAS WHERE USUARIOID =@usuarioId
            AND TIPO_OPERACION_ID = @tipoOperacionId;", new { usuarioId, tipoOperacionId });

        }

        public async Task<Categoria> ObtenerPorId (int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Categoria>(
                @"SELECT * FROM CATEGORIAS WHERE ID=@id AND USUARIOID=@usuarioID",
                new {id, usuarioId});

        }

        public async Task Actualizar (Categoria categoria)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(
                @"UPDATE CATEGORIAS SET NOMBRE=@NOMBRE, TIPO_OPERACION_ID=@TipoOperacionId
                WHERE ID=@ID;", 
                categoria);
        }

        public async Task Borrar(int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync(@"DELETE FROM CATEGORIAS WHERE ID=@id", new { id });

        }

    }
}
