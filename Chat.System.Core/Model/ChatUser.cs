using System;

namespace Chat.System.Core.Model
{
    public class ChatUser
    {
        public string UserId { get; set; }

        public string UserHead { get; set; }

        public string UserName { get; set; }

        public DateTime LastOnlineDate { get; set; }
    }
}