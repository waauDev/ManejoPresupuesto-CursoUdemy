﻿using Dapper;
using ManejoPresupuesto.Models;
using Microsoft.Data.SqlClient;

namespace ManejoPresupuesto.Servicios
{
    public interface IRepositorioTransacciones
    {
        Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnterior);
        Task Borrar(int id);
        Task Crear(Transaccion transaccion);
        Task<IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo);
        Task<Transaccion> ObtenerPorId(int id, int usuarioId);
    }

    public class RepositorioTransacciones : IRepositorioTransacciones
    {
        private readonly string connectionString;

        public RepositorioTransacciones(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task Crear(Transaccion transaccion)
        {
            using var connection = new SqlConnection(connectionString);

            var id = await connection.QuerySingleAsync<int>(
                @"Transacciones_Insertar", new
                {
                    transaccion.UsuarioId,
                    transaccion.FechaTransaccion,
                    transaccion.Monto,
                    transaccion.CategoriaId,
                    transaccion.CuentaId,
                    transaccion.Nota
                }, commandType: System.Data.CommandType.StoredProcedure);

            transaccion.Id = id;

        }

        public async Task Actualizar(Transaccion transaccion, decimal montoAnterior, int cuentaAnteriorId)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Actualizar", new
            {
                transaccion.Id,
                transaccion.FechaTransaccion,
                transaccion.Monto,
                transaccion.CategoriaId,
                transaccion.CuentaId,
                transaccion.Nota,
                montoAnterior,
                cuentaAnteriorId
            }, commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<Transaccion> ObtenerPorId(int id, int usuarioId)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryFirstOrDefaultAsync<Transaccion>(
                @"SELECT TRANSACCIONES.*, CAT.TIPO_OPERACION_ID as TipoOperacionId FROM TRANSACCIONES
                INNER JOIN CATEGORIAS CAT
                ON CAT.ID = TRANSACCIONES.CATEGORIA_ID
                WHERE TRANSACCIONES.ID = @id AND TRANSACCIONES.USUARIOID =@usuarioId;
                ", new { id, usuarioId });
        }

        public async Task Borrar (int id)
        {
            using var connection = new SqlConnection(connectionString);
            await connection.ExecuteAsync("Transacciones_Borrar", new {id}, commandType: System.Data.CommandType.StoredProcedure);

        }

        public async Task <IEnumerable<Transaccion>> ObtenerPorCuentaId(ObtenerTransaccionesPorCuenta modelo)
        {
            using var connection = new SqlConnection(connectionString);
            return await connection.QueryAsync<Transaccion>(@"
            SELECT T.ID,T.MONTO, T.FECHATRANSACCION, c.NOMBRE AS CATEGORIA,
            CU.NOMBRE AS CUENTA, C.TIPO_OPERACION_ID
            FROM TRANSACCIONES T
            INNER JOIN CATEGORIAS C
            ON C.ID= T.CATEGORIA_ID
            INNER JOIN CUENTAS CU
            ON CU.ID = T.CUENTA_ID
            WHERE T.CUENTA_ID = @CuentaId AND T.USUARIOID = @usuarioId 
            AND FECHATRANSACCION BETWEEN @FechaInicio AND @FechaFin
            ", modelo);
        }

    }
}