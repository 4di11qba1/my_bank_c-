using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public class BeneficiaryController : Controller
    {
        private readonly MyBankContext _context;
        private readonly AccountRepository accRepo;
        private readonly UserRepository userRepo;
        private readonly BeneficiaryRepository beneRepo;

        public BeneficiaryController() {
            _context = new MyBankContext();
            accRepo = new AccountRepository(_context);
            userRepo = new UserRepository(_context);
            beneRepo = new BeneficiaryRepository(_context);
        }

        public ViewResult Index()
        {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            List<object> beneficiaries = getBeneficiaries(sessionId);
            return View("beneficiary", beneficiaries);
        }

        public ViewResult AddBeneficiaryPage() {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            List<object> accountsWithUsers = getAccountsWithUsers(sessionId);
            return View("addBeneficiary", accountsWithUsers);
        }

        [HttpPost]
        public ActionResult AddBeneficiary([FromBody] object data) {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            decimal acc = JsonSerializer.Deserialize<decimal>(data.ToString());

            User user = accRepo.GetUserByAccountId(acc);
            Beneficiary temp = new Beneficiary();

            temp.AccountId = accRepo.GetAccountIdByUserId(sessionId);
            temp.UserId = user.Id;

            beneRepo.Add(temp);

            TempData["Message"] = "Beneficiary Successfully Added.";
            return Ok(); // Return a 200 OK status code
        }

        [HttpPost]
        public ActionResult SaveChangedData([FromBody] object data)
        {
            string responseText = data.ToString();

            // Deserialize the JSON string into a dynamic object
            dynamic responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject(responseText);

            // Access the backendData property
            dynamic backendData = responseObject.backendData;

            // Extract the values from the backendData array
            List<int> accountIDs = new List<int>();
            foreach (var item in backendData)
            {
                int value = item[3];
                accountIDs.Add(value);
            }

            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            decimal mainAccID = accRepo.GetAccountIdByUserId(sessionId);
            beneRepo.DeleteByAccountID(mainAccID);

            foreach(var item in accountIDs) {
                int userID = accRepo.GetUserIdByAccountId(item);
                Beneficiary temp = new Beneficiary();
                temp.AccountId = mainAccID;
                temp.UserId = userID;
                beneRepo.Add(temp);
            }

            TempData["message"] = "Changes Saved Successfully.";
            return Ok(); // Return a 200 OK status code
        }

        public List<object> getBeneficiaries(int id) {
            List<object> beneficiaries = new List<object>();

            decimal accountID = accRepo.GetAccountIdByUserId(id);
            List<Beneficiary> ben = beneRepo.GetAllBeneficiaries(accountID);

            foreach(Beneficiary b in ben) {
                Account acc = accRepo.GetAccountByUserId(b.UserId.Value);
                User user = userRepo.GetUserById(b.UserId.Value);

                List<object> temp = new List<object>();

                temp.Add(user.Name);
                temp.Add(user.Address + ", " + user.City);
                temp.Add(user.Email);
                temp.Add(acc.Id);

                beneficiaries.Add(temp);
            }

            return beneficiaries;
        }

        public List<object> getAccountsWithUsers(int id) {
            List<object> list = new List<object>();

            List<User> users = userRepo.GetAllUsersExcept(id);

            foreach(User u in users) {
                List<object> temp = new List<object>();
                temp.Add(u.Name);
                temp.Add(u.Address + ", " + u.City);
                temp.Add(u.Email);
                temp.Add(u.Accounts.ToList()[0].Id);
                list.Add(temp);
            }

            return list;
        }
    }
}
