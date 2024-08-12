using AppContracts;
using InfrastructeProvider;
using System.Net;
using System.Xml.Linq;

namespace Core
{
    public class ChatServer : ChatBase
    {

        private readonly IMessageSource _source;
        private HashSet<User> _users = [];

        public ChatServer(IMessageSource messageSource)
        {
            _source = messageSource;
        }

        public override async Task Start()
        {
            await Task.CompletedTask;
            Task.Run(Listener);
        }

        protected override async Task Listener()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                try
                {
                    ResiveResult? resiveResult = await _source.Resive(CancellationToken) ?? throw new Exception("Message is null");

                    switch (resiveResult.Message!.Command)
                    {
                        case Command.None:
                            await MessageHandler(resiveResult);
                            break;
                        case Command.Join: 
                            await JoinHandler(resiveResult);
                            break;
                        case Command.Exit:
                            break;
                        case Command.Users:
                            break;
                        case Command.Confirm:
                            break;
                    }

                }
                catch (Exception exception)
                {
                    await Console.Out.WriteLineAsync(exception.Message);
                }
            }
        }

        private async Task MessageHandler(ResiveResult resiveResult)
        {
            if(resiveResult.Message!.RecepentId < 0)
            {
                await SendAllAsync(resiveResult.Message);
            } 
            else
            {
                await _source.Send(
                    resiveResult.Message,
                    _users.First(u =>u.Id == resiveResult.Message.SenderId).EndPoint!,
                    CancellationToken);


                var repicientEndPoint = _users.FirstOrDefault(u => u.Id == resiveResult.Message.SenderId)?.EndPoint;

                if (repicientEndPoint is not null) 
                {
                    await _source.Send(
                       resiveResult.Message,
                       repicientEndPoint,
                       CancellationToken);

                }
            }
        }

        private async Task JoinHandler(ResiveResult result)
        {
            User? user = _users.FirstOrDefault(u => u.Name == result.Message!.Text);
            if (user is null) 
            {
                user = new User() { Name = result.Message!.Text, EndPoint = result.EndPoint };
                _users.Add(user);    
            }
            user.EndPoint = result.EndPoint;

            await _source.Send(
                new Message() { Command = Command.Join ,RecepentId = user.Id},
                user.EndPoint!,
                CancellationToken);

            await _source.Send(
                new Message() { Command = Command.Users, RecepentId = user.Id, Users = _users },
                user.EndPoint!,
                CancellationToken);            

            await SendAllAsync(new Message() { Command = Command.Confirm, Text = $"{user.Name} joined to chat"});

        }

        private async Task SendAllAsync(Message message)
        {
            foreach (var user in _users)
            {
                await _source.Send(
                    message,
                    user.EndPoint!,
                    CancellationToken);
            }
        }
    }
}
