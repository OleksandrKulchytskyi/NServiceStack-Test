using BusStop.API.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BusStop.API.Controllers
{
	public class OrdersController : ApiController
	{
		//public NServiceBus.IBus Bus { get; set; }

		public Guid Get()
		{
			var oreder = new PlaceOrder
			{
				OrderID = Guid.NewGuid(),
				CustomerID = Guid.NewGuid(),
				ProductID = Guid.NewGuid()
			};

			MvcApplication.Bus.Send(oreder);
			return oreder.OrderID;
		}
	}
}
