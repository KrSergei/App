using Core;
using InfrastructeProvider;
using System.Net;
using System.Net.Sockets;
using InfrastructePersistence.Context;
using Microsoft.EntityFrameworkCore;

namespace App;

class Program
{
    public static async Task Main(string[] args)
    {
        //var optionBuilder = new DbContextOptionsBuilder<Context>().UseNpgsql().UseLazyLoadingProxies();

        IPEndPoint serverendPoint = new(IPAddress.Parse("127.0.0.1"), 12000);
        IMessageSource source;
        if (args.Length == 0)
        {
            //server
            source = new MessageSource(new UdpClient(serverendPoint));

            var chat = new ChatServer(source, new ChatContext());
            await chat.Start();
        }
        else
        {
            //client
            source = new MessageSource(new UdpClient());
            var chat = new ChatClient(args[0], serverendPoint, source);
            await chat.Start();
        }
    }
}