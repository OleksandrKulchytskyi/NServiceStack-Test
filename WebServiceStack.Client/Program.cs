using Common;
using ServiceStack.ServiceClient.Web;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.ServiceInterface.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebServiceStack.Client
{
	class Program
	{
		static void Main(string[] args)
		{
			int amount = -1;
			JsonServiceClient client = null;
			client = new JsonServiceClient("http://localhost:4057") { UserName = "jsuser", Password = "password1" };
			var assignRResp = client.Send<AssignRolesResponse>(new AssignRoles
			{
				UserName = "jsuser",
				Roles = new ArrayOfString("StatusUser"),
				Permissions = new ArrayOfString("GetStatus")
			});

			try
			{
				var repsspp = client.Send<StatusResponse>(new StatusQuery() { Date = DateTime.Now });
			}
			catch (WebServiceException ex)
			{
				if (ex.Message != null) { }
			}


			client.PostAsync<MagicResponse>(new MagicRequest { Id = 1 },
				resp => Console.WriteLine(resp.OutId), (resp, err) => { Console.WriteLine(err.Message); });


			while (amount != 0)
			{
				Console.WriteLine("Enter a protein amount (press 0 to exit)");
				if (int.TryParse(Console.ReadLine(), out amount))
				{
					var response = client.Send<EntryResponse>(new Entry { Amount = amount, Time = DateTime.Now });
					Console.WriteLine("Response, ID: " + response.Id);
				}
			}

			var reps2 = client.Post<StatusResponse>("status", new StatusQuery { Date = DateTime.Now });
			Console.WriteLine("{0} / {1}", reps2.Total, reps2.Goal);
			Console.WriteLine(reps2.Message);
			Console.ReadLine();
		}
	}
}
