using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace DSWI_cibertec_demo.Hubs
{
    public class ChatHub : Hub
    {
        // Mantener lista sencilla de usuarios conectados
        private static readonly ConcurrentDictionary<string, string> _usuarios = new();

        // Método que llamarán los clientes para enviar mensaje a todos
        public async Task EnviarMensaje(string usuario, string mensaje)
        {
            await Clients.All.SendAsync("RecibirMensaje", usuario, mensaje);
        }

        // Mensaje privado a un ConnectionId concreto
        public async Task EnviarMensajePrivado(string connectionId, string remitente, string mensaje)
        {
            await Clients.Client(connectionId).SendAsync("RecibirMensajePrivado", remitente, mensaje);
        }

        // Manejar conexión y desconexión para mantener lista de usuarios
        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user = httpContext.Request.Query["user"].ToString() ?? "Anónimo";
            _usuarios[Context.ConnectionId] = user;
            await Clients.Caller.SendAsync("Conectado", Context.ConnectionId, user, _usuarios.Select(u => new {connectionId = u.Key, nombre = u.Value}).ToList());
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
