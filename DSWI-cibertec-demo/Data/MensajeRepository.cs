using DSWI_cibertec_demo.Models;
using Microsoft.Data.SqlClient;

namespace DSWI_cibertec_demo.Data
{
    public class MensajeRepository
    {
        private readonly string _connectionString;

        public MensajeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("CibertecConnection");
        }

        // Insertar mensaje y devolver con Id y Fecha
        public async Task<MensajeModel> InsertarMensajeAsync(MensajeModel mensaje)
        {
            const string sql = @"INSERT INTO Mensajes (Usuario, Mensaje) VALUES (@Usuario, @Mensaje); SELECT CAST(SCOPE_IDENTITY() AS int) AS NewId;";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Usuario", mensaje.Usuario);
                cmd.Parameters.AddWithValue("@Mensaje", mensaje.Mensaje);
                await conn.OpenAsync();

                var newId = await cmd.ExecuteScalarAsync();
                if (newId != null)
                {
                    mensaje.Id = (int)newId;
                    using (var cmd2 = new SqlCommand("SELECT FechaCreacion FROM Mensajes WHERE Id = @Id", conn))
                    {
                        cmd2.Parameters.AddWithValue("@Id", mensaje.Id);
                        var obj = await cmd2.ExecuteScalarAsync();
                        if (obj != null && obj != DBNull.Value)
                        {
                            mensaje.FechaCreacion = (DateTime)obj;
                        }
                    }
                }
                return mensaje;
            }
        }

        // Obtener últimos N mensajes ordenados ascendentemente por FechaCreacion (para mostrar desde el más antiguo)
        public async Task<List<MensajeModel>> ObtenerUltimosMensajesAsync(int cantidad = 50)
        {
            var lista = new List<MensajeModel>();
            var sql = @"SELECT TOP (@Top) Id, Usuario, Mensaje, FechaCreacion FROM Mensajes ORDER BY FechaCreacion DESC;";
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Top", cantidad);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new MensajeModel
                        {
                            Id = reader.GetInt32(0),
                            Usuario = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                            Mensaje = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                            FechaCreacion = reader.IsDBNull(3) ? DateTime.UtcNow : reader.GetDateTime(3)
                        });
                    }
                }
            }
            lista.Reverse();
            return lista;
        }
    }
}
