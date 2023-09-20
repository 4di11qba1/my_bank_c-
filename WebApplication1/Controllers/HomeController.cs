using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyBankContext _context = new MyBankContext();
        private readonly UserRepository userRepo;
        private readonly AccountRepository accRepo;
        public HomeController()
        {
            userRepo = new UserRepository(_context);
            accRepo = new AccountRepository(_context);
        }
        public ViewResult Index()
        {
            ViewBag.Message = TempData["msg"];
            return View("sign-in");
        }

        [HttpPost]
        public ActionResult SignIn(string email, string password)
        {
            User user = userRepo.GetUserByEmail(email);
            if (user != null)
            {
                if (user.Password == password)
                {
                    HttpContext.Session.SetString("user", user.Id.ToString());
                    TempData["msg"] = "You Have Successfully Logged In.";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    TempData["msg"] = "Incorrect Password.";
                    return RedirectToAction("Index");
                }
            }
            else
            {
                TempData["msg"] = "Incorrect Email.";
                return RedirectToAction("Index");
            }
        }

        public ViewResult SignUpPage()
        {
            ViewBag.Message = TempData["msg"];
            return View("sign-up");
        }

        [HttpPost]
        public ActionResult SubmitForm(string name, string email, string password, string cnic, string phone, string address, string city, string type)
        {
            User user = new User();
            user.Name = name;
            user.Email = email;
            user.Password = password;
            user.Cnic = cnic;
            user.Phone = phone;
            user.Address = address;
            user.City = city;

            Account account = new Account();
            account.Id = generateAccountID();
            account.Balance = 0;
            account.Type = type;
            account.CreatedAt = DateTime.Now;
            
            if (userRepo.GetUserByCNIC(cnic) != null)
            {
                TempData["msg"] = "CNIC Already Exists.";
                return RedirectToAction("SignUpPage");
            }
            else if(userRepo.GetUserByEmail(email) != null){
                TempData["msg"] = "Email Already Exists.";  
                return RedirectToAction("SignUpPage");
            }
            else
            {
                userRepo.Add(user);
                user = userRepo.GetUserByEmail(user.Email);
                account.UserId = user.Id;
                accRepo.Add(account);
                TempData["msg"] = "You Account Has Been Created Successfully.";
                return RedirectToAction("Index");
            }
        }

        public long generateAccountID()
        {
            DateTime now = DateTime.Now;
            DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan timeSpan = now - unixEpoch;
            long timestamp = (long)timeSpan.TotalSeconds;

            // Convert the timestamp to an integer
            int uniqueInteger = (int)timestamp;
            return uniqueInteger;
        }
    }
}