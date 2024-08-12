using System.Net;
using System.Text.Json.Serialization;

namespace AppContracts
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }
        [JsonIgnore]
        public IPEndPoint? IPEndPoint { get; set; }
    }
}