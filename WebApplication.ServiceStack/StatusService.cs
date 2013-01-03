using Common;
using ServiceStack.Common;
using ServiceStack.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.ServiceHost;
namespace WebApplication.ServiceStack
{

	public class StatusService : Service
	{
		public TrackedDataRepository2 Repository { get; set; }

		public object Any(StatusQuery request)
		{
			//throw new NotImplementedException("This is a test");

			var cacheKey=UrnId.Create<StatusQuery>(request.Date.ToShortDateString());
			return RequestContext.ToOptimizedResultUsingCache(base.Cache, cacheKey, new TimeSpan(0,0,26), () =>
				{
					var status = Repository.GetStatus(request.Date, Session, this.GetSession());
					return status;
				});

			//var date = request.Date.Date;
			//var trackedData = (TrackedData)Session[date.ToString()];
			//if (trackedData == null)
			//	trackedData = new TrackedData { Goal = 300, Total = 0 };

			//var message = this.GetSession().DisplayName;

			//trackedData.Total++;

			//Session[date.ToString()] = trackedData;

			//return new StatusResponse() { Goal = trackedData.Goal, Total = trackedData.Total, Message=message };
		}
	}
}