using Contact_Manager_Application.Data;
using Contact_Manager_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Contact_Manager_Application.Repositories
{
    public class ContactRepository : Repository<Contact>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Contact>> GetContactsByNameAsync(string name)
        {
            return await _dbSet
                .Where(c => c.Name.Contains(name))
                .ToListAsync();
        }

        public async Task<IEnumerable<Contact>> GetMarriedContactsAsync(bool married)
        {
            return await _dbSet
                .Where(c => c.Married == married)
                .ToListAsync();
        }

        public async Task<IEnumerable<Contact>> GetContactsBySalaryRangeAsync(decimal minSalary, decimal maxSalary)
        {
            return await _dbSet
                .Where(c => c.Salary >= minSalary && c.Salary <= maxSalary)
                .ToListAsync();
        }
    }
}
