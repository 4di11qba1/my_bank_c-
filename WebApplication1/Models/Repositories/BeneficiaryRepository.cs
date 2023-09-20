using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace WebApplication1.Models.Repositories
{
    public class BeneficiaryRepository
    {
        private readonly MyBankContext _context;

        public BeneficiaryRepository(MyBankContext context)
        {
            _context = context;
        }

        public void Add(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Add(beneficiary);
            _context.SaveChanges();
        }

        public void Update(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Update(beneficiary);
            _context.SaveChanges();
        }

        public void Delete(Beneficiary beneficiary)
        {
            _context.Beneficiaries.Remove(beneficiary);
            _context.SaveChanges();
        }

        public void DeleteByAccountID(decimal accID) {
            var entitiesToDelete = _context.Beneficiaries.Where(b => b.AccountId == accID).ToList();

            // Remove the retrieved entities from the context
            _context.Beneficiaries.RemoveRange(entitiesToDelete);

            // Save the changes to the database
            _context.SaveChanges();
        }

        public Beneficiary GetBeneficiaryById(int id)
        {
            return _context.Beneficiaries.Find(id);
        }

        public IEnumerable<Beneficiary> GetAllBeneficiaries()
        {
            return _context.Beneficiaries;
        }

        public List<Beneficiary> GetAllBeneficiaries(decimal aId) {
            return _context.Beneficiaries.Where(b => b.AccountId == aId).ToList();
        }
    }
}
