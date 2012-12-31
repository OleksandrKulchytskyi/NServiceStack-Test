using NServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BusStop.API.Contracts
{
	[TimeToBeReceived("00:00:10")]//valid only 10 seconds
	public class PlaceOrder : IMessage
	{
		public Guid OrderID { get; set; }
		public Guid ProductID { get; set; }
		public Guid CustomerID { get; set; }
	}
}