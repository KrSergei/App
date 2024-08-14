using AppContracts;
using InfrastructePersistence.Context;
using InfrastructeProvider;
using Microsoft.EntityFrameworkCore;

namespace Core
{
    public class ChatServer : ChatBase
    {

        private readonly IMessageSource _source;
        private readonly ChatContext _context;
        private HashSet<User> _users = [];

        public ChatServer(IMessageSource messageSource, ChatContext context)
        {
            _source = messageSource;
            _context = context;
        }

        public override async Task Start()
        {
            Console.WriteLine("Server started");
            await Task.CompletedTask;
            await Task.Run(Listener);
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
                            await ExitHandler(resiveResult);
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

        private async Task ExitHandler(ResiveResult resiveResult)
        {
            var user = User.FromDomain(await _context.Users.FirstAsync(x => x.Id == resiveResult.Message!.SenderId));
            user.LastOnLine = DateTime.Now;
            await _context.SaveChangesAsync();
            _users.Remove(_users.First(x => x.Id == resiveResult.Message!.SenderId));    
        }

        private async Task MessageHandler(ResiveResult resiveResult)
        {
            if(resiveResult.Message!.RecipientId < 0)
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
                new Message() { Command = Command.Join ,RecipientId = user.Id},
                user.EndPoint!,
                CancellationToken);

            await SendAllAsync(new Message() { Command = Command.Confirm, Text = $"{user.Name} joined to chat"});

            await SendAllAsync(new Message() { Command = Command.Users, RecipientId = user.Id, Users = _users });

            var unreadMsg = await _context.Messages.Where(m => m.RepicientId == user.Id).ToListAsync();

            foreach (var msg in unreadMsg)
            {
                await _source.Send(
                    Message.FromDomain(msg),
                    user.EndPoint,
                    CancellationToken);
            }
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
