using DSWI_cibertec_demo.Data;
using DSWI_cibertec_demo.Models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DSWI_cibertec_demo.Hubs
{
    public class ChatHub : Hub
    {
        // Mantener lista sencilla de usuarios conectados
        private static readonly ConcurrentDictionary<string, string> _usuarios = new();
        private readonly MensajeRepository _mensajeRepo;

        public ChatHub(MensajeRepository mensajeRepo)
        {
            _mensajeRepo = mensajeRepo;
        }

        // Método que llamarán los clientes para enviar mensaje a todos
        public async Task EnviarMensaje(string usuario, string mensaje)
        {
            var mensajeModel = new MensajeModel
            {
                Usuario = usuario,
                Mensaje = mensaje
            };
            var guardado = await _mensajeRepo.InsertarMensajeAsync(mensajeModel);
            await Clients.All.SendAsync("RecibirMensaje", guardado.Usuario, guardado.Mensaje, guardado.FechaCreacion);
        }

        // Mensaje privado a un ConnectionId concreto
        public async Task EnviarMensajePrivado(string connectionId, string remitente, string mensaje)
        {
            var mensajeModel = new MensajeModel
            {
                Usuario = remitente,
                Mensaje = mensaje
            };
            var guardado = await _mensajeRepo.InsertarMensajeAsync(mensajeModel);
            await Clients.Client(connectionId).SendAsync("RecibirMensajePrivado", guardado.Usuario, guardado.Mensaje, guardado.FechaCreacion);
        }

        // Manejar conexión y desconexión para mantener lista de usuarios
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Request.Query["user"].ToString();
            if (string.IsNullOrWhiteSpace(user))
            {
                user = "Anónimo";
            }
            _usuarios[Context.ConnectionId] = user;
            var historial = await _mensajeRepo.ObtenerUltimosMensajesAsync(50);
            await Clients.Caller.SendAsync("Conectado", Context.ConnectionId, user, historial);
            await Clients.All.SendAsync("UsuariosConectados", _usuarios.Select(u => new { connectionId = u.Key, nombre = u.Value }).ToList());
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _usuarios.TryRemove(Context.ConnectionId, out _);
            await Clients.All.SendAsync("UsuariosConectados", _usuarios.Select(u => new { connectionId = u.Key, nombre = u.Value }).ToList());
            await base.OnDisconnectedAsync(exception);
        }
    }
}
