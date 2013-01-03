using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.WebHost;
using ServiceStack.WebHost.Endpoints;
using Common;
using ServiceStack.Redis;
using ServiceStack.Redis.Messaging;

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

			_mqHost.Start();
		}

		public void SendMessage()
		{
			using (var mqClient=_mqHost.CreateMessageQueueClient())
			{
				mqClient.Publish(new Entry() { Amount = 24, Time = DateTime.Now });
			}

		}
	}
}
