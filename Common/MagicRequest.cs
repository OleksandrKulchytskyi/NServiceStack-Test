using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	[Route("/magic", "POST")]
	[Route("/magic/{Id}", "POST")]
	public class MagicRequest : IReturn<MagicResponse>
	{
		public int Id { get; set; }
	}

	public class MagicResponse
	{
		public int InId { get; set; }
		public int OutId { get; set; }
	}
}
