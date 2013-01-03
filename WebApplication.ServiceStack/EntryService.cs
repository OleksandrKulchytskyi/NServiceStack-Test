using Common;
using ServiceStack.CacheAccess;
using ServiceStack.OrmLite;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using System;
using ServiceStack.OrmLite.Sqlite;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.Common;

namespace WebApplication.ServiceStack
{
	[WebApplication.ServiceStack.RecordIpFilter]
	[LastIpFilter(ApplyTo = ApplyTo.Get | ApplyTo.Post)]
	public class EntryService : Service
	{
		public TrackedDataRepository2 Repos { get; set; }

		public object Post(Entry request)
		{
			var cacheKey = UrnId.Create<StatusQuery>(request.Time.ToShortDateString());
			base.RequestContext.RemoveFromCache(base.Cache, cacheKey);

			Repos.AddEntry(request);

			return new EntryResponse() { Id = 1 };
		}
	}

	public class TrackedDataRepository2
	{
		public IDbConnectionFactory DbConFactory { get; set; }

		public int  AddEntry(Entry entry)
		{
			entry.Time = entry.Time.Date;
			using (var db=DbConFactory.OpenDbConnection())
			{
				db.CreateTable<Entry>();
				db.Insert(entry);
				return (int)db.GetLastInsertId();
			}
		}

		public StatusResponse GetStatus(DateTime fullDate, ISession session, IAuthSession authSession)
		{
			using (var db = DbConFactory.OpenDbConnection())
			{
				var total=db.Select<Entry>(x=>x.Time==fullDate.Date).Sum(x=>x.Amount);

				return new StatusResponse { Total = total, Goal = 300, Message = authSession.DisplayName };
			}
		}
	}

	public class TrackedDataRepository
	{
		public IDbConnectionFactory DbConFactory { get; set; }

		public void AddEntry(int amount, DateTime time, ISession session)
		{
			var date = time.Date;
			var trackedData = (TrackedData)session[date.ToString()];
			if (trackedData == null)
				trackedData = new TrackedData { Goal = 300 };

			trackedData.Total += amount;
			session[date.ToString()] = trackedData;
		}

		public StatusResponse GetStatus(DateTime fullDate, ISession session, IAuthSession authSession)
		{
			var date = fullDate.Date;
			var trackedData = session[date.ToString()] as TrackedData;
			if (trackedData == null)
				trackedData = new TrackedData { Goal = 300, Total = 0 };

			var message = authSession.DisplayName;
			return new StatusResponse { Message = message, Total = trackedData.Total, Goal = trackedData.Goal };
		}
	}
}