using AppContracts;
using InfrastructeProvider;
using System.Net;

namespace Core
{
    public class ChatClient : ChatBase
    {
        private readonly User _user;
        private readonly IPEndPoint _iPEndPoint;
        private readonly IMessageSource _source;
        private IEnumerable<User> _users = [];

        public ChatClient(string userName, IPEndPoint iPEndPoint, IMessageSource messageSource)
        {
            _user = new User { Name = userName } ;
            _iPEndPoint = iPEndPoint;
            _source = messageSource;
        }

        public override async Task Start()
        {
            var join = new Message { Text = _user.Name, Command = Command.Join };
            await _source.Send(join, _iPEndPoint, CancellationToken);

            Task.Run(Listener);

            while (!CancellationToken.IsCancellationRequested)
            {
                string input = (await Console.In.ReadLineAsync()) ?? string.Empty;
                Message message = new Message() { Text = input, SenderId = _user.Id, Command = Command.None };
                await _source.Send(message, _iPEndPoint, CancellationToken);
            }
        }
     

        protected override async Task Listener()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    ResiveResult? resiveResult = await _source.Resive(CancellationToken);

                    if (resiveResult.Message is null)
                        throw new Exception("Message is null");

                    if(resiveResult.Message.Command == Command.Join)
                    {
                        JoinHandler(resiveResult.Message);
                    }
                    else if (resiveResult.Message.Command == Command.Users)
                    {
                        UsersHandler(resiveResult.Message);
                    }
                    else if(resiveResult.Message.Command == Command.None)
                    {
                       MessageJoinHandler(resiveResult.Message);
                    }
                }
                catch (Exception exception)
                {
                    await Console.Out.WriteLineAsync(exception.Message);
                }
            }
        }

        private void MessageJoinHandler(Message message)
        {
            Console.WriteLine($"{_users.First(u=> u.Id == message.SenderId)} : {message.Text}");
        }

        private void UsersHandler(Message message)
        {
            _users = message.Users;
        }

        private void JoinHandler(Message message)
        {
           _user.Id = message.RecepentId;
            Console.WriteLine("Join succes");
        }
    }
}
