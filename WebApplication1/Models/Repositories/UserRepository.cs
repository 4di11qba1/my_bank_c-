using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models.Repositories
{
    public class UserRepository
    {
        private readonly MyBankContext _context;
        public UserRepository(MyBankContext context)
        {
            _context = context;
        }
        public void Add(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
        public void Update(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
        }
        public void Delete(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        public List<User> GetAllUsers() {
            return _context.Users.Include(u => u.Accounts).ToList();
        }

        public List<User> GetAllUsersExcept(int id)
        {
            return _context.Users.Include(u => u.Accounts)
                .Where(u => u.Id != id)
                .ToList();
        }

        public User GetUserById(int id)
        {
            return _context.Users.Find(id);
        }

        public User GetUserByCNIC(string cnic)
        {
            return _context.Users.FirstOrDefault(u => u.Cnic == cnic);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email);
        }
    }
}
