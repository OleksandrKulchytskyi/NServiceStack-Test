using ServiceStack.DataAnnotations;
using ServiceStack.FluentValidation;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface.ServiceModel;
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
		public ResponseStatus ResponseStatus { get; set; }
	}

	public class EntryValidator : AbstractValidator<Entry>
	{
		public EntryValidator()
		{
			RuleFor(e => e.Amount).GreaterThan(0);
			RuleFor(e => e.Time).LessThan(DateTime.Now).WithMessage("Date must not be a future date.");

			RuleSet(ServiceStack.ServiceInterface.ApplyTo.Post, () =>
			{
				RuleFor(e => e.Amount).LessThanOrEqualTo(50);
			});

			RuleSet(ServiceStack.ServiceInterface.ApplyTo.Get, () =>
			{
				RuleFor(e => e.Time).LessThanOrEqualTo(DateTime.Now.AddDays(1));
			});
		}
	}
}
