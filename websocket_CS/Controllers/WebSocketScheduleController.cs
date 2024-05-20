using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace websocket_CS.Controllers;

public class ScheduleDto
{
    public string id { get; set; } = "";
    public decimal x { get; set; } = 0;
    public decimal y { get; set; } = 0;
    public decimal z { get; set; } = 1;//zindex
    public decimal w { get; set; } = 0;//width
    public string bkc { get; set; } = "green";
    public string rowId { get; set; } = "";
    public decimal from { get; set; } = 0;
    public decimal to { get; set; } = 0;
    public string value { get; set; } = "";// & < > ` " ' この6記号は使用不可
}
public class rowDto
{
    public string rowId { get; set; } = "";
    public int rowOrder { get; set; } = 0;
    public string name { get; set; } = "";
}
public class BoardDto
{
    public string gridhtml { get; set; } = "";
    public string objhtml { get; set; } = "";
}
public class WebSocketScheduleController : ControllerBase
{
    public static List<ScheduleDto> pastLogs =
        new List<ScheduleDto>() {
            new ScheduleDto() { id = "obj1", x = 235, y = 110, z = 1, w = 90, bkc = "green" , rowId = "rowId0" , from = 200, to = 338, value = "test1" },
            new ScheduleDto() { id = "obj2", x = 295, y = 140, z = 1, w = 90, bkc = "green" , rowId = "rowId1" , from = 300, to = 438, value = "test2"  },
            new ScheduleDto() { id = "obj3", x = 355, y = 170, z = 1, w = 90, bkc = "green" , rowId = "rowId2" , from = 400, to = 538, value = "test3"  }
        };
    public static List<rowDto> defrows =
    new List<rowDto>() {
            new rowDto() { rowId = "rowId0" , rowOrder = 0 , name = "aaa"  },
            new rowDto() { rowId = "rowId1" , rowOrder = 1 , name = "bbb"  },
            new rowDto() { rowId = "rowId2" , rowOrder = 2 , name = "ccc"  },
            new rowDto() { rowId = "rowId3" , rowOrder = 3 , name = "ddd"  },
            new rowDto() { rowId = "rowId4" , rowOrder = 4 , name = "eee"  },
            new rowDto() { rowId = "rowId5" , rowOrder = 5 , name = "fff"  },
            new rowDto() { rowId = "rowId6" , rowOrder = 6 , name = "ggg"  },
            new rowDto() { rowId = "rowId7" , rowOrder = 7 , name = "hhh"  }
    };

    [HttpGet("/api/schedules")]
    public BoardDto GetMessages()
    {
        BoardDto board = new BoardDto();
        string rowhtml = "";
        for (int i = 0; i < defrows.Count; i++) // defrows はrowOrder順で取得想定
        {
            rowhtml = rowhtml 
                + @"<div data-rowid=" + defrows[i].rowId + @" data-roworder=" + defrows[i].rowOrder + @" >" + defrows[i].name + @"</div>";
        }
        board.gridhtml = board.gridhtml
            + @"<div class=""grid-rowcontainer"" data-rowcount=" + defrows.Count + @" style=""grid-template-rows: repeat(" + defrows.Count + @", 1fr);"" >" + rowhtml + @"</div>";
        string cellhtml = "";
        for (int i = 0; i < defrows.Count * 24; i++)
        {
            cellhtml = cellhtml + "<div></div>";
        }
        board.gridhtml = board.gridhtml
            + @"<div class=""grid-maincontainer"" ondragover=""setDragOverStyle(event)"" style=""grid-template-rows: repeat(" + defrows.Count + @", 1fr);"" >" + cellhtml + @"</div>";

        for (int i = 0; i < pastLogs.Count; i++)
        {
            var log = pastLogs[i];
            string attr = " id=" + log.id
                        + @" style=""height=15px;width:" + log.w + "px; position:absolute;left:" + log.x + "px;top:" + log.y + "px;z-index:" + log.z + @"; cursor:pointer; background-color:" + log.bkc + @";"""
                        + " value=" + log.value
                        + @" readonly=true draggable=true  ondragstart=""setDraggedStyle(event)"" ondragend=""setDraggedEnd(event)""  ontouchstart=""setTDraggedStyle(event)"" ontouchmove=""setTDraggedEnd(event)""  ontouchend=""setTDraggedEnd2(event)"" onfocus=""wideFocus(event)"" onblur=""wideBlur(event)"" data-rowid=" + log.rowId
                        + " title=" + strTime(log.from) + "～" + strTime(log.to) + "_" + log.value;
            board.objhtml = board.objhtml + @"<input" + attr + "></input>";
        }
        return board;
    }

    [Route("/ws_schedules")]
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
        var buffer = new byte[1024 * 16];
        var receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        while (!receiveResult.CloseStatus.HasValue)
        {
            string utf8String = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            //pastLogs.Add(utf8String);
            await WebSocketManager.BroadcastAsync(utf8String);

            receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        }

        await webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
    }
    private static string strTime(decimal time)
    {
        string ret = time.ToString().PadLeft(4, '0');
        return ret.Substring(0, 2) + ":" + ret.Substring(2, 2);
    }

}