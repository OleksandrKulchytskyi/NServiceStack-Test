using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	[Route("/status")]
	//[Route("/status","POST")]
	[Route("/status/{Date}","POST")]
	[ServiceStack.ServiceInterface.Authenticate]
	//[ServiceStack.ServiceInterface.RequiredRole("StatusUser")]
	//[ServiceStack.ServiceInterface.RequiredPermission("GetStatus")]
	public class StatusQuery
	{
		public DateTime Date { get; set; }
	}

	public class StatusResponse
	{
		public int Total { get; set; }
		public int Goal { get; set; }

		public ServiceStack.ServiceInterface.ServiceModel.ResponseStatus ResponseStatus { get; set; }

		public string Message { get; set; }
	}
}
