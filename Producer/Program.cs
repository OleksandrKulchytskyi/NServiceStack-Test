using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost;
using ServiceStack.WebHost.Endpoints;
using Common;
using ServiceStack.Redis;
using ServiceStack.Redis.Messaging;
using ServiceStack.Messaging;

namespace Producer
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("StartingProducer");
			var host = new ClientAppHost();
			host.Init();
			host.Start("http://localhost:81/");
			while (true)
			{
				Console.WriteLine("Press NTER to send a message");
				Console.ReadLine();
				host.SendMessage();
			}
		}
	}
	public class ClientAppHost : AppHostHttpListenerBase
	{
		private RedisMqHost _mqHost;

		public ClientAppHost()
			: base("Producer Console App", typeof(Entry).Assembly)
		{

		}

		public override void Configure(Funq.Container container)
		{
			var redisfactrory = new PooledRedisClientManager("localhost:6379");
			_mqHost = new RedisMqHost(redisfactrory);

			_mqHost.RegisterHandler<EntryResponse>(message =>
			{
				Console.WriteLine("Got message id {0}", message.GetBody().Id);
				return null;
			});

			_mqHost.Start();
		}

		public void SendMessage()
		{
			using (var mqClient = _mqHost.CreateMessageQueueClient())
			{
				var uniqueQ = "mq:c1" + ":" + Guid.NewGuid().ToString("N");
				var message = new Message<Entry>(new Entry() { Amount = 24, Time = DateTime.Now }) { ReplyTo = uniqueQ };

				mqClient.Publish(message);
				var response = mqClient.Get(uniqueQ, new TimeSpan(0, 0, 1, 0)).ToMessage<EntryResponse>();
				Console.WriteLine("Got response with id {0}", response.GetBody().Id);

			}

		}
	}
}
