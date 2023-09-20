using System;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Components
{
	public class BalanceCard: ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			return View();
		}
	}

    public class DebitCard : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

    public class CreditCard : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }

    public class NetCard : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}

