namespace AppContracts
{
    public class Messasge
    {
         public int Id { get; set; }

        public string Text { get; set; } = string.Empty;

        public int SenderId { get; set; }

        public int? RecepentId { get; set; }
    }
}