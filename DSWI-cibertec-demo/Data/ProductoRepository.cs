using System.Collections.Generic;
using System.Threading.Tasks;
using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DSWI_cibertec_demo.Data
{
    public class ProductoRepository
    {
        private readonly string _connectionString;

        public ProductoRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CibertecConnection");
        }

        // Función para obtener Todos los Productos de golpe
        public async Task<List<ProductoModel>> ObtenerProductosAsync()
        {
            var lista = new List<ProductoModel>();
            var query = "SELECT Id, Nombre, Precio, Cantidad, Estado FROM Producto";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ProductoModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Cantidad = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4)
                        });
                    }
                }
            }
            return lista;
        }

        public async Task<(List<ProductoModel> Productos, int TotalRegistros)> ObtenerProductosPaginadoAsync(int page, int pageSize)
        {
            var lista = new List<ProductoModel>();
            // Cálculo del offset
            int offset = (page -  1) * pageSize;
            // Consulta principal con paginación
            var query = $@"
                SELECT Id, Nombre, Precio, Cantidad, Estado
                FROM Producto
                ORDER BY Id
                OFFSET {offset} ROWS FETCH NEXT {pageSize} ROWS ONLY;
                
                SELECT COUNT(*) FROM Producto;
            ";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new ProductoModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Cantidad = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4)
                        });
                    }
                    // Ir al segundo conjunto de resultados (total de registros)
                    await reader.NextResultAsync();
                    int total = 0;
                    if (await reader.ReadAsync())
                    {
                        total = reader.GetInt32(0);
                    }
                    return (lista, total);
                }
            }
        }

        public async Task<ProductoModel> AgregarProductoAsync(ProductoModel p)
        {
            var sql = "INSERT INTO Producto(Nombre, Precio, Cantidad, Estado) VALUES (@Nombre, @Precio, @Cantidad, @Estado); SELECT SCOPE_IDENTITY();";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Precio", p.Precio);
                cmd.Parameters.AddWithValue("@Cantidad", p.Cantidad);
                cmd.Parameters.AddWithValue("@Estado", p.Estado);

                await conn.OpenAsync();

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        p.Id = Convert.ToInt32(reader[0]);
                    }
                }
                return p;
            }
        }

        public async Task<bool> ActualizarProductoAsync(ProductoModel p)
        {
            var sql = @"UPDATE Producto 
                SET Nombre = @Nombre, Precio = @Precio, 
                    Cantidad = @Cantidad, Estado = @Estado 
                WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", p.Id);
                cmd.Parameters.AddWithValue("@Nombre", p.Nombre);
                cmd.Parameters.AddWithValue("@Precio", p.Precio);
                cmd.Parameters.AddWithValue("@Cantidad", p.Cantidad);
                cmd.Parameters.AddWithValue("@Estado", p.Estado);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<bool> EliminarProductoAsync(int id)
        {
            var sql = "DELETE FROM Producto WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                return await cmd.ExecuteNonQueryAsync() > 0;
            }
        }

        public async Task<ProductoModel?> ObtenerProductoPorIdAsync(int id)
        {
            var sql = "SELECT Id, Nombre, Precio, Cantidad, Estado FROM Producto WHERE Id = @Id";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Id", id);

                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new ProductoModel
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Precio = reader.GetDecimal(2),
                            Cantidad = reader.GetInt32(3),
                            Estado = reader.GetBoolean(4)
                        };
                    }
            return null;
                }
            }
        }
    }
}
