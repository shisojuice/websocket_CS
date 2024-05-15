using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace websocket_CS.Controllers;

public class WebSocketController : ControllerBase
{
    public static List<string> pastLogs = new List<string>() { "past_message1", "past_message2", "past_message3" };

    [HttpGet("/api/messages")]
    public IEnumerable<string> GetMessages()
    {
        return pastLogs;
    }

    [Route("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            WebSocketManager.AddSocket(webSocket);
            await Echo(webSocket);
            WebSocketManager.RemoveSocket(webSocket);
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }


    private static async Task Echo(WebSocket webSocket)
    {
        var buffer = new byte[1024 * 4];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            string utf8String = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            pastLogs.Add(utf8String);
            await WebSocketManager.BroadcastAsync(utf8String);

            //await webSocket.SendAsync(
            //new ArraySegment<byte>(buffer, 0, receiveResult.Count),
            //    receiveResult.MessageType,
            //    receiveResult.EndOfMessage,
            //    CancellationToken.None);

            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
    }
}