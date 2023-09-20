using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebApplication1.Models;
using WebApplication1.Models.Repositories;

namespace WebApplication1.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly MyBankContext _context = new MyBankContext();
        private readonly TransactionRepository transRepo;
        private readonly UserRepository userRepo;
        private readonly AccountRepository accRepo;
        private readonly BeneficiaryController benCont;
        public PaymentsController() {
            transRepo = new TransactionRepository(_context);
            userRepo = new UserRepository(_context);
            accRepo = new AccountRepository(_context);
            benCont = new BeneficiaryController();
        }
        public ViewResult Index()
        {
            return View("payments");
        }

        public ActionResult TransferToBeneficiaryPage() {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            List<object> list = benCont.getBeneficiaries(sessionId);
            return View("transferToBeneficiary", list);
        }

        public ActionResult TransferToNonBeneficiaryPage() {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            List<object> list = benCont.getAccountsWithUsers(sessionId);
            return View("transferToNonBeneficiary", list);
        }

        [HttpPost]
        public ActionResult makeTransfer([FromBody] object data)
        {
            string[] transaction = data.ToString().Split(',');

            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            Account senderAccount = accRepo.GetAccountByUserId(sessionId);
            Account recieverAccount = accRepo.GetAccountById(decimal.Parse(transaction[0]));

            if(decimal.Parse(transaction[1]) <= senderAccount.Balance) {
                Transaction senderTransaction = new Transaction();
                Transaction recieverTransaction = new Transaction();

                senderTransaction.AccountId = senderAccount.Id;
                senderTransaction.Amount = decimal.Parse(transaction[1]);
                senderTransaction.SenderId = senderAccount.Id;
                senderTransaction.RecieverId = recieverAccount.Id;
                senderTransaction.Type = "Debit";
                senderTransaction.TimeStamp = DateTime.Now;
                senderAccount.Balance -= decimal.Parse(transaction[1]);

                recieverTransaction.AccountId = recieverAccount.Id;
                recieverTransaction.Amount = decimal.Parse(transaction[1]);
                recieverTransaction.SenderId = senderAccount.Id;
                recieverTransaction.RecieverId = recieverAccount.Id;
                recieverTransaction.Type = "Credit";
                recieverTransaction.TimeStamp = DateTime.Now;
                recieverAccount.Balance += decimal.Parse(transaction[1]);

                accRepo.Update(senderAccount);
                accRepo.Update(recieverAccount);

                transRepo.Add(senderTransaction);
                transRepo.Add(recieverTransaction);

                TempData["TransferMessage"] = "Credits Successfully Transfered.";
                return Ok();
            }
            else {
                TempData["TransferMessage"] = "Not Enough Amount.";
                return Ok();
            }
        }

        public ViewResult Transactions()
        {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            User user = userRepo.GetUserById(sessionId);

            List<object> data = new List<object>();
            List<List<object>> transactions = getAllTransactionsWithUser(sessionId);
            data.Add(transactions);
            return View("transactions", data);
        }

        [HttpGet]
        public string Details()
        {
            int sessionId = int.Parse(HttpContext.Session.GetString("user"));
            List<Transaction> trans = transRepo.GetTransactionsByDate(sessionId, DateTime.Today, DateTime.Today.Date.AddDays(1).AddTicks(-1));

            decimal? totalDebit = 0;
            decimal? totalCredit = 0;
            foreach (Transaction transaction in trans)
            {
                if (transaction.Type == "Debit")
                    totalDebit += transaction.Amount;
                if (transaction.Type == "Credit")
                    totalCredit += transaction.Amount;
            }

            var viewModel = new 
            {
                Debit = totalDebit,
                Credit = totalCredit
            };
            string data = JsonSerializer.Serialize(viewModel);
            return data;
        }



        public List<List<object>> getAllTransactionsWithUser(int id)
        {
            List<List<object>> transactions = new List<List<object>>();
            List<Transaction> trans = transRepo.GetTransactionsByUserId(id);

            foreach (Transaction t in trans)
            {
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