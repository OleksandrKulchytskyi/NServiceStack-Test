using Common;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.ServiceStack
{
	public class MagicService : Service
	{
		public object Magic(MagicRequest request)
		{
			return new MagicResponse { InId = request.Id, OutId = request.Id++ };
		}
	}
}