using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Repositories
{
    public class TransactionRepository
    {
        private readonly MyBankContext _context;
        public TransactionRepository(MyBankContext context)
        {
            _context = context;
        }
        public void Add(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            _context.SaveChanges();
        }
        public void Update(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            _context.SaveChanges();
        }
        public void Delete(Transaction transaction)
        {
            _context.Transactions.Remove(transaction);
            _context.SaveChanges();
        }
        public Transaction GetTransactionById(int id)
        {
            return _context.Transactions.Find(id);
        }
        public List<Transaction> GetTransactionsByUserId(int userId)
        {
            var transactions = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Where(t => t.Account.UserId == userId)
                .ToList();

            return transactions;
        }

        public List<Transaction> GetTransactionsByDate(int userID, DateTime startDate, DateTime endDate)
        {
            var transactions = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Where(t => t.Account.User.Id == userID && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                .OrderBy(t => t.TimeStamp)
                .ToList();

            return transactions;
        }

        public List<Balance> CalculateBalancesHourly(int userId, DateTime startDate, DateTime endDate)
        {
            var balances = new List<Balance>();

            var transactions = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Where(t => (t.Account.UserId == userId || t.AccountId == userId)
                            && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                .OrderBy(t => t.TimeStamp)
                .ToList();

            decimal balance = 0;
            DateTime currentTime = startDate;

            while (currentTime <= endDate)
            {
                decimal hourBalance = transactions
                    .Where(t => t.TimeStamp >= currentTime && t.TimeStamp < currentTime.AddHours(1))
                    .Sum(t => t.Type == "Credit" ? t.Amount ?? 0 : -(t.Amount ?? 0));

                balances.Add(new Balance
                {
                    date = currentTime,
                    balance = balance + hourBalance
                });

                balance += hourBalance;
                currentTime = currentTime.AddHours(1);
            }

            return balances;
        }

        public List<Balance> CalculateBalancesDaily(int userId, DateTime startDate, DateTime endDate)
        {
            var balances = new List<Balance>();

            var transactions = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Where(t => (t.Account.UserId == userId || t.AccountId == userId)
                            && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                .OrderBy(t => t.TimeStamp)
                .ToList();

            decimal balance = 0;
            DateTime currentDate = startDate.Date;

            while (currentDate <= endDate)
            {
                decimal dayBalance = transactions
                    .Where(t => t.TimeStamp.Value.Date == currentDate)
                    .Sum(t => t.Type == "Credit" ? t.Amount ?? 0 : -(t.Amount ?? 0));

                balances.Add(new Balance
                {
                    date = currentDate,
                    balance = balance + dayBalance
                });

                balance += dayBalance;
                currentDate = currentDate.AddDays(1);
            }

            return balances;
        }


        public List<Balance> CalculateBalances(int userId, DateTime startDate, DateTime endDate)
        {
            var balances = new List<Balance>();

            var transactions = _context.Transactions
                .Include(t => t.Account)
                    .ThenInclude(a => a.User)
                .Where(t => (t.Account.UserId == userId || t.AccountId == userId)
                            && t.TimeStamp >= startDate && t.TimeStamp <= endDate)
                .OrderBy(t => t.TimeStamp)
                .ToList();

            decimal balance = 0;
            DateTime currentMonth = startDate.Date;

            while (currentMonth <= endDate)
            {
                decimal monthBalance = transactions
                    .Where(t => t.TimeStamp.Value.Year == currentMonth.Year && t.TimeStamp.Value.Month == currentMonth.Month)
                    .Sum(t => t.Type == "Credit" ? t.Amount ?? 0 : -(t.Amount ?? 0));

                balances.Add(new Balance
                {
                    date = currentMonth,
                    balance = balance + monthBalance
                });

                balance += monthBalance;
                currentMonth = currentMonth.AddMonths(1);
            }

            return balances;
        }
    }
}
