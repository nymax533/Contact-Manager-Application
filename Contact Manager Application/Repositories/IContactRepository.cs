using Contact_Manager_Application.Models;

namespace Contact_Manager_Application.Repositories
{
    public interface IContactRepository : IRepository<Contact>
    {
        Task<IEnumerable<Contact>> GetContactsByNameAsync(string name);
        Task<IEnumerable<Contact>> GetMarriedContactsAsync(bool married);
        Task<IEnumerable<Contact>> GetContactsBySalaryRangeAsync(decimal minSalary, decimal maxSalary);
    }
}
