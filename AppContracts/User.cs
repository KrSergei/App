using System.Net;
using System.Text.Json.Serialization;

namespace AppContracts
{
    public record User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [JsonIgnore]
        public IPEndPoint? EndPoint { get; set; }
    }
}