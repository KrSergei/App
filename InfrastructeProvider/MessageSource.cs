using AppContracts;
using AppContracts.Extensions;
using System.Net;
using System.Net.Sockets;

namespace InfrastructeProvider
{
    public interface IMessageSource
    {
        Task<Message?> Resive();
        Task Send(Message message, IPEndPoint iPEndPoint);
        //IPEndPoint CreateEndpoint(string adress, int port);
        //IPEndPoint GetServerEndpoint();
    }

    public class MessageSource : IMessageSource
    {
        private readonly UdpClient _udpClient;

        public MessageSource(UdpClient udpClient)
        {
            _udpClient = udpClient;
        }

        public IPEndPoint CreateEndpoint(string adress, int port)
        {
            throw new NotImplementedException();
        }

        public async Task<Message?> Resive()
        {
           var data = await _udpClient.ReceiveAsync();
            
            return data.Buffer.ToMessage();
        }

        public async Task Send(Message message, IPEndPoint ep)
        {
            await _udpClient.SendAsync(message.ToBytes(), ep);
        }
    }
}