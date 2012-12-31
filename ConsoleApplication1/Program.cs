using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;

namespace ConsoleApplication1
{
	class Program
	{
		static void ProcessNumbers(int numb)
		{
			Console.WriteLine("{0} ThraedId {1}", numb, Thread.CurrentThread.ManagedThreadId);
		}

		static int TimeConsumingComputation(int numb)
		{
			Thread.Sleep(100);
			if (numb == 4)
				throw new ArgumentException("Number contains unacceptable data");
			return numb;
		}

		static IEnumerable<string> FakeFetch()
		{
			Thread.Sleep(300);
			Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId);
			yield return "message 1";
			yield return "message 2";
			yield return "message 3";
		}

		static void DoFakeFetch(long interval)
		{
			if (interval == 3)
				throw new ArgumentException("Smth was wrong");
			Console.WriteLine(interval);

			foreach (string data in FakeFetch())
			{
				Console.WriteLine(data);
			}
		}

		static void Main(string[] args)
		{
			Console.WriteLine("Begin on Thread {0}", Thread.CurrentThread.ManagedThreadId);
			CancellationTokenSource _cts = new CancellationTokenSource();

			//var timer=Observable.Interval(TimeSpan.FromMilliseconds(1000));
			//timer.SubscribeOn(System.Reactive.Concurrency.Scheduler.NewThread).Subscribe(DoFakeFetch, ex => Console.WriteLine(ex.Message), _cts.Token);

			ManualResetEvent event2 = new ManualResetEvent(false); 

			var link = new List<string> { "http://www.codeproject.com/Tips/514529/Better-serialization-deserialization-similar-to-JS", 
				"http://www.codeproject.com/Tips/513522/Providing-session-state-in-ASP-NET-WebAPI"};

			var subscr = link.ToObservable();
			subscr.Repeat(700).ObserveOn(System.Reactive.Concurrency.Scheduler.NewThread).SubscribeOn(System.Reactive.Concurrency.Scheduler.NewThread).Subscribe(lnk =>
				{
					var webCl = new System.Net.WebClient();
					var siteData = webCl.DownloadString(lnk);
					if (siteData != null)
					{
						Console.WriteLine(siteData.Length.ToString());

					}
					webCl.Dispose();
					webCl = null;
					Console.WriteLine("+  " + lnk);
				}, () => { Console.WriteLine("Completed"); event2.Set(); });


			event2.WaitOne();
			return;
			Console.ReadLine();
			var oserv3 = Observable.Timer(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(1)).Timestamp();
			using (oserv3.Subscribe(x => Console.WriteLine("{0}: {1}", x.Value, x.Timestamp)))
			{
				Console.WriteLine("Press any key to unsubscribe");
				Console.ReadKey();
			}


			var observable1 = (from item in FakeFetch() select item).ToObservable();
			observable1.SubscribeOn(System.Reactive.Concurrency.Scheduler.NewThread).Subscribe(Console.WriteLine, wx => Console.WriteLine(wx.Message),
																								() => Console.WriteLine("Completed"));


			Thread.Sleep(700);
			Console.WriteLine("Thread {0}", Thread.CurrentThread.ManagedThreadId);
			var input = Console.ReadLine();
			if (input == "1")
			{
				_cts.Cancel();
				Console.WriteLine("Break on Thread {0}", Thread.CurrentThread.ManagedThreadId);
			}

			var query = from i in Enumerable.Range(1, 12) select TimeConsumingComputation(i);

			var observable = query.ToObservable(System.Reactive.Concurrency.Scheduler.NewThread);
			observable.ObserveOn(System.Reactive.Concurrency.Scheduler.CurrentThread).Subscribe(ProcessNumbers,
																								(ex) => Console.WriteLine(ex.Message),
																								() => Console.WriteLine("I'm done!!!"));

			Console.ReadLine();

			var file = @"C:\Users\vin26935\Desktop\ServLocLazy.txt";

			var obsrStrings = Observable.Using<char, System.IO.StreamReader>(() =>
				new StreamReader(new FileStream(file, FileMode.Open)),
				sr => (from str in sr.ReadToEnd() select str).ToObservable());

			using (obsrStrings.Subscribe(Console.Write, () => Console.WriteLine("Done!!")))
			{

			}

			var fdata2 = from s in File.ReadAllText(file) select s;

			FileStream fs = File.OpenRead(file);
			Func<byte[], int, int, IObservable<int>> read =
				Observable.FromAsyncPattern<byte[], int, int, int>(fs.BeginRead, fs.EndRead);

			byte[] bs = new byte[2048];

			using (read(bs, 0, bs.Length).Subscribe(bytesRead =>
			{
				Console.WriteLine("+");

				string text = Encoding.UTF8.GetString(bs, 0, bytesRead);
				Console.WriteLine(text);
				Console.WriteLine(bytesRead);
			}))
			{

			}

			Console.ReadLine();

			IRepo<Department> _depRepo = new Imnp();

			Database.SetInitializer<MyContext>(new System.Data.Entity.DropCreateDatabaseIfModelChanges<MyContext>());
			using (MyContext cont = new MyContext())
			{
				var person = cont.Persons.Include(X => X.Departments).FirstOrDefault(x => x.Name.Equals("Cawa", StringComparison.OrdinalIgnoreCase));
				if (person != null)
				{
					if (person.Address == null)
					{
						person.Address = new Address() { City = "vinnica", Street = "600-richia", PersonId = person.PersonId };
						cont.SaveChanges();
					}

					if (person.Departments.Count == 0)
					{
						person.Departments.Add(new Department() { Name = "It" });
						person.Departments.Add(new Department() { Name = "Dev" });
						cont.SaveChanges();
					}

				}
			}
		}
	}

	public interface IRepo<T>
	{
		T Add(T ent);
	}

	public class Imnp : IRepo<Department>
	{

		public Department Add(Department ent)
		{
			throw new NotImplementedException();
		}
	}

	public class MyContext : DbContext
	{
		public MyContext()
			: base("name=MyContext")
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.Add(new PersonMapping());
			modelBuilder.Configurations.Add(new AddressnMapping());
			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Person> Persons { get; set; }
		public DbSet<Address> Addresses { get; set; }
	}

	public class Person
	{
		[Key]
		public int PersonId { get; set; }
		[Required]
		[MaxLength(100)]
		public string Name { get; set; }

		public virtual Address Address { get; set; }

		public virtual ICollection<Department> Departments { get; set; }
	}

	public class PersonMapping : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Person>
	{
		public PersonMapping()
		{
			this.ToTable("Persons");

			this.HasKey(p => p.PersonId);
			this.Property(p => p.Name).IsRequired();
		}
	}

	public class Address
	{
		public Address()
		{
		}

		[Key]
		public int AddressId { get; set; }

		[Required]
		public string City { get; set; }
		[Required]
		public string Street { get; set; }

		public virtual Person Person { get; set; }
		public int PersonId { get; set; }
	}

	public class AddressnMapping : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Address>
	{
		public AddressnMapping()
		{
			this.ToTable("Addresses");
			this.HasKey(p => p.AddressId);
			this.Property(a => a.Street).IsRequired();
			this.Property(a => a.City).IsRequired();

			this.HasRequired(a => a.Person).WithOptional(p => p.Address);
		}
	}

	public class Department
	{
		public Department()
		{
		}

		[Key]
		public int DepartmentId { get; set; }

		public string Name { get; set; }

		public virtual ICollection<Person> Pesrons { get; set; }
	}

	public class DepartmentMap : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Department>
	{
		public DepartmentMap()
		{
			this.ToTable("Departments");
			this.HasKey(d => d.DepartmentId);
			this.Property(d => d.Name).IsRequired();

			this.HasMany(d => d.Pesrons).WithMany(p => p.Departments).
				Map(m =>
				{
					m.ToTable("PersonsDepartments");
					m.MapLeftKey("DepartmentId");
					m.MapRightKey("PersonId");
				});
		}
	}
}
