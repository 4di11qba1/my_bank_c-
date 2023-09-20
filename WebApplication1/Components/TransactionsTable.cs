using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Components
{
	public class AllTransactionsTable: ViewComponent
	{
		public IViewComponentResult Invoke() {
			return View();
		}
	}

	public class TransactionsSummary: ViewComponent {
		public IViewComponentResult Invoke() {
			return View();
		}
	}
}

