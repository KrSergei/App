using System.Data;

namespace AppContracts
{
    public class Message
    {
         public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public int SenderId { get; set; }

        public int RecepentId { get; set; } = -1;

        public Command Command { get; set; } = Command.None;

        public IEnumerable<User> Users { get; set; } = [];

    }

    public enum Command
    {
        None,
        Join, 
        Exit,
        Users,
        Confirm
    }
}