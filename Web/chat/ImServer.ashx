<%@ WebHandler Language="C#" Class="ImServer" %>

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using Chat.System.Core.Model;
using Newtonsoft.Json;

public class ImServer : IHttpHandler {

    HttpContext _context;

    public HttpResponse Response
    {
        get
        {
            return _context.Response;
        }
    }

    public HttpRequest Request
    {
        get
        {
            return _context.Request;
        }
    }

    private List<ChatUser> UserList { get; set; }

    private void Init(HttpContext context)
    {
        _context = context;
    }
    
    public void ProcessRequest (HttpContext context) {
        Init(context);

        string eventStr = Request["action"];

        switch (eventStr)
        {
            case "addMessage":
                AddMessage();
                break;
        }
        
    }

    private void AddMessage()
    {
        string userId = Request["userid"];
        string message = Request["msg"];
        ChatMessage model = new ChatMessage
        {
            Mid = Guid.NewGuid().ToString(),
            UserId = userId,
            Message = message,
            CreateDate = DateTime.Now
        };

        ChatMessage.AddMessage(model);

        PushText("sendMsg", JsonConvert.SerializeObject(model));
    }

    private void PushText(string eventStr, string text)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("event:{0}\r\n", eventStr);
        sb.AppendFormat("data:{0}\r\n", text);
        sb.AppendFormat("\r\n");
        Response.ContentType = "text/event-stream";
        Response.Write(sb.ToString());
        Response.Flush();
    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}