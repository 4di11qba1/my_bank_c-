using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        private readonly MyBankContext _context = new MyBankContext();
        private readonly UserRepository userRepo;
        private readonly AccountRepository accRepo;
        private readonly TransactionRepository transRepo;
        public UserController()
        {
            userRepo = new UserRepository(_context);
            accRepo = new AccountRepository(_context);
            transRepo = new TransactionRepository(_context);
        }

        public IActionResult Index()
        {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            User user = userRepo.GetUserById(sessionId);

            List<object> data = new List<object>();

            List<List<Balance>> balances = GetBalances(sessionId);
            List<decimal> decimals = transactionDetails(sessionId);
            List<List<object>> transactions = getAllTransactionsWithUser(sessionId);

            data.Add(balances);
            data.Add(decimals);
            data.Add(transactions);

            if (sessionId != null)
            {
                return View("dashboard", data);
            }
            else
            {
                TempData["msg"] = "You are not logged in";
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult SignOut()
        {
            TempData.Clear();
            HttpContext.Session.Clear();
            TempData["msg"] = "You Have Successfully Logged Out.";
            return RedirectToAction("Index", "Home");
        }

        public List<List<Balance>> GetBalances(int id)
        {

            DateTime created = accRepo.GetAccountByUserId(id).CreatedAt.Value;
            Random random = new Random();

            List<Balance> tempBalances = transRepo.CalculateBalancesHourly(id, created, DateTime.Now);

            List<Balance> overAllBalances = new List<Balance>();
            HashSet<DateTime> uniqueDates = new HashSet<DateTime>(); // Track unique dates

            while (overAllBalances.Count < 12 && tempBalances.Count > 0)
            {
                int randomIndex = random.Next(0, tempBalances.Count);
                Balance temp = tempBalances[randomIndex];

                if (temp.balance != 0 && uniqueDates.Add(temp.date))
                {
                    overAllBalances.Add(temp);
                }

                // Remove the selected balance from the temporary list to avoid duplicates
                tempBalances.RemoveAt(randomIndex);
            }

            overAllBalances.Sort((b1, b2) => DateTime.Compare(b1.date, b2.date));

            List<Balance> yearBalances = transRepo.CalculateBalancesHourly(id, new DateTime(2023, 7, 8, 16, 0, 0), DateTime.Now);

            List<List<Balance>> balances = new List<List<Balance>>();
            balances.Add(overAllBalances);
            balances.Add(yearBalances);
            return balances;
        }

        public string Details()
        {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            Account acc = accRepo.GetAccountByUserId(sessionId);
            User usr = userRepo.GetUserById(sessionId);

            acc.User = null;

            var merge = new { Account = acc, User = usr };
            return JsonSerializer.Serialize(merge);
        }

        public List<decimal> transactionDetails(int id) {
            List<Transaction> data = transRepo.GetTransactionsByUserId(id);
            List<decimal> decimals = new List<decimal>();

            decimal credits = 0;
            decimal debits = 0;
            decimal nets = 0;

            foreach (Transaction t in data) {

                if (t.Type == "Credit")
                    credits += t.Amount.Value;
                else if (t.Type == "Debit")
                    debits += t.Amount.Value;

                nets += t.Amount.Value;
            }

            decimals.Add(nets);
            decimals.Add(credits);
            decimals.Add(debits);
            return decimals;
        }

        public List<List<object>> getAllTransactionsWithUser(int id) {
            List<List<object>> transactions = new List<List<object>>();
            List<Transaction> trans = transRepo.GetTransactionsByUserId(id);

            foreach(Transaction t in trans) {
                decimal senderAccID = t.SenderId.Value;
                decimal recieverAccID = t.RecieverId.Value;

                int senderID = accRepo.GetUserIdByAccountId(senderAccID);
                int recieverID = accRepo.GetUserIdByAccountId(recieverAccID);

                User sender = userRepo.GetUserById(senderID);
                User reciever = userRepo.GetUserById(recieverID);

                List<object> transaction = new List<object>();
                transaction.Add(sender.Name);
                transaction.Add(senderAccID);
                transaction.Add(reciever.Name);
                transaction.Add(recieverAccID);
                transaction.Add(t.Amount);
                transaction.Add(t.Type);
                transaction.Add(t.TimeStamp);

                transactions.Add(transaction);
            }

            return transactions;
        }
    }
}