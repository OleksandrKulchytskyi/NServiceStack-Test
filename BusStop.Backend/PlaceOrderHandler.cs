using BusStop.API.Contracts;
using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusStop.Backend
{
	public class PlaceOrderHandler:IHandleMessages<PlaceOrder>
	{
		public void Handle(PlaceOrder message)
		{
			Console.WriteLine("Order received " + message.OrderID);
		}
	}
}
