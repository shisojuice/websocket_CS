using System.Net.WebSockets;
using System.Text;

namespace websocket_CS.Controllers
{
    public class WebSocketManager
    {
        //重複排除のためHashSet
        private static readonly HashSet<WebSocket> _sockets = new HashSet<WebSocket>();

        public static void AddSocket(WebSocket socket)
        {
            _sockets.Add(socket);
        }

        public static void RemoveSocket(WebSocket socket)
        {
            _sockets.Remove(socket);
        }
        public static async Task InitBroadcastAsync(WebSocket socket, List<string> messages, CancellationToken ct = default)
        {
            foreach (var message in messages)
            {
                var data = Encoding.UTF8.GetBytes(message);
                var segment = new ArraySegment<byte>(data);
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
                }
                else
                {
                    RemoveSocket(socket);
                }
            }
        }
        public static async Task BroadcastAsync(string message, CancellationToken ct = default)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(data);

            foreach (var socket in _sockets.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
                }
                else
                {
                    RemoveSocket(socket);
                }
            }
        }
    }
}
