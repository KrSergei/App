using AppContracts;
using System.Net;

namespace InfrastructeProvider
{
    public interface IMessageSource
    {
        Task<ResiveResult> Resive(CancellationToken cancellationToken);
        Task Send(Message message, IPEndPoint iPEndPoint, CancellationToken cancellationToken);
    }
}