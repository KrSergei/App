using AppContracts;
using AppContracts.Extensions;
using System.Net;
using System.Net.Sockets;

namespace InfrastructeProvider
{
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

        public async Task<ResiveResult?> Resive(CancellationToken cancellationToken)
        {
           var data = await _udpClient.ReceiveAsync(cancellationToken);
            
            return new (data.RemoteEndPoint,  data.Buffer.ToMessage());
        }

        public async Task Send(Message message, IPEndPoint ep, CancellationToken cancellationToken)
        {
            await _udpClient.SendAsync(message.ToBytes(), ep, cancellationToken);
        }
    }
}
