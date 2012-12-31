using Common;
using ServiceStack.CacheAccess;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;

namespace WebApplication.ServiceStack
{
	public class RecordIpFilter:RequestFilterAttribute
	{
		public ICacheClient Cache { get; set; }

		public override void Execute(IHttpRequest req, IHttpResponse res, object requestDto)
		{
			Cache.Add("lastIP", req.UserHostAddress);
		}
	}

	public class LastIpFilter : ResponseFilterAttribute
	{
		public ICacheClient Cache { get; set; }

		public override void Execute(IHttpRequest req, IHttpResponse res, object responseDto)
		{
			var status = responseDto as StatusResponse;
			if(status!=null)
			{
				status.Message += "last IP:" + Cache.Get<string>("lastIP");
			}
		}
	}
}