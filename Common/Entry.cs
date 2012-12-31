using ServiceStack.DataAnnotations;
using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
	[Route("/entry", "POST")]
	[Route("/entry/{Amount}/{Time}", "POST")]
	public class Entry
	{
		[AutoIncrement]
		public int Id { get; set; }
		public DateTime Time { get; set; }
		public int Amount { get; set; }
	}

	public class EntryResponse
	{
		public int Id { get; set; }
	}
}
