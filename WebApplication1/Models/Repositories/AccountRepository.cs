using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Repositories
{
    public class AccountRepository
    {
        private readonly MyBankContext _context;

        public AccountRepository(MyBankContext context)
        {
            _context = context;
        }

        public void Add(Account account)
        {
            _context.Accounts.Add(account);
            _context.SaveChanges();
        }

        public void Update(Account account)
        {
            _context.Accounts.Update(account);
            _context.SaveChanges();
        }

        public void Delete(Account account)
        {
            _context.Accounts.Remove(account);
            _context.SaveChanges();
        }

        public List<Account> GettAllAccounts() {
            return _context.Accounts.ToList();
        }

        public Account GetAccountById(decimal id)
        {
            return _context.Accounts.Find(id);
        }

        public Account GetAccountByUserId(int id)
        {
            return _context.Accounts.FirstOrDefault(a => a.UserId == id);
        }

        public IEnumerable<Account> GetAllAccounts()
        {
            return _context.Accounts;
        }

        public int GetUserIdByAccountId(decimal accountId)
        {
            int? userId = _context.Accounts
                .Where(a => a.Id == accountId)
                .Select(a => a.UserId)
                .SingleOrDefault();

            return userId.Value;
        }

        public User GetUserByAccountId(decimal accountId)
        {
            User user = _context.Accounts
                .Where(a => a.Id == accountId)
                .Select(a => a.User)
                .FirstOrDefault();

            return user;
        }



        public decimal GetAccountIdByUserId(int userId)
        {
            decimal? accountId = _context.Accounts
                .Where(a => a.UserId == userId)
                .Select(a => a.Id)
                .SingleOrDefault();

            return accountId.Value;
        }
    }
}
