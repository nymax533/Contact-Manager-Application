using Contact_Manager_Application.DTOs;

namespace Contact_Manager_Application.Services
{
    public interface IContactService
    {
        Task<IEnumerable<ContactDto>> GetAllContactsAsync();
        Task<ContactDto?> GetContactByIdAsync(int id);
        Task<bool> UpdateContactAsync(UpdateContactDto contactDto);
        Task<bool> DeleteContactAsync(int id);
        Task<IEnumerable<ContactDto>> ImportContactsFromCsvAsync(Stream csvStream);
        Task<bool> ContactExistsAsync(int id);
    }
}
