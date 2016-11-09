using System;

namespace Chat.System.Core.Model
{
    public class ChatMessage
    {
        public string Mid { get; set; }

        public string UserId { get; set; }

        public string Message { get; set; }

        public DateTime CreateDate { get; set; }

        public string NickName { get; set; }

        


        public static PageData GetPageData(int index, int size)
        {
            using (MySqlDbConnection con = MySqlDbConnection.Connect())
            {
                con.CommandText = "select * from chatmessage";

                return con.Pageing(index, size, "CreateDate", MySqlDbConnection.OrderBy.DESC);
            }
        }


        public static int AddMessage(ChatMessage model)
        {
            using (MySqlDbConnection con = MySqlDbConnection.Connect())
            {
                con.CommandText = "insert into chatmessage (Mid, UserId, Message, CreateDate) values (@Mid, @UserId, @Message, @CreateDate)";
                con.AddParameters("Mid", model.Mid);
                con.AddParameters("UserId", model.UserId);
                con.AddParameters("Message", model.Message);
                con.AddParameters("CreateDate", model.CreateDate);

                return con.Exec();
            }
        }
    }
}
