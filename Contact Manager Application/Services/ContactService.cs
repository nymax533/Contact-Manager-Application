using Contact_Manager_Application.DTOs;
using Contact_Manager_Application.Models;
using Contact_Manager_Application.Models.Csv;
using Contact_Manager_Application.Mappings;
using Contact_Manager_Application.Mappings.Csv;
using Contact_Manager_Application.Repositories;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace Contact_Manager_Application.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly ILogger<ContactService> _logger;

        public ContactService(IContactRepository contactRepository, ILogger<ContactService> logger)
        {
            _contactRepository = contactRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ContactDto>> GetAllContactsAsync()
        {
            var contacts = await _contactRepository.GetAllAsync();
            return contacts.Select(ContactMapper.ToDto);
        }

        public async Task<ContactDto?> GetContactByIdAsync(int id)
        {
            var contact = await _contactRepository.GetByIdAsync(id);
            return contact != null ? ContactMapper.ToDto(contact) : null;
        }

        public async Task<bool> UpdateContactAsync(UpdateContactDto contactDto)
        {
            var existingContact = await _contactRepository.GetByIdAsync(contactDto.Id);
            if (existingContact == null)
            {
                _logger.LogWarning("Contact with ID {ContactId} not found for update", contactDto.Id);
                return false;
            }

            ContactMapper.UpdateEntity(existingContact, contactDto);
            await _contactRepository.UpdateAsync(existingContact);
            
            _logger.LogInformation("Contact updated with ID: {ContactId}", contactDto.Id);
            
            return true;
        }

        public async Task<bool> DeleteContactAsync(int id)
        {
            var exists = await _contactRepository.ExistsAsync(id);
            if (!exists)
            {
                _logger.LogWarning("Contact with ID {ContactId} not found for deletion", id);
                return false;
            }

            await _contactRepository.DeleteAsync(id);
            _logger.LogInformation("Contact deleted with ID: {ContactId}", id);
            
            return true;
        }

        public async Task<IEnumerable<ContactDto>> ImportContactsFromCsvAsync(Stream csvStream)
        {
            var contacts = new List<Contact>();

            try
            {
                using var reader = new StreamReader(csvStream);
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    BadDataFound = null
                };

                using var csv = new CsvReader(reader, config);
                csv.Context.RegisterClassMap<ContactCsvMap>();
                
                var records = csv.GetRecords<ContactCsvRecord>();
                
                foreach (var record in records)
                {
                    try
                    {
                        var contact = new Contact
                        {
                            Name = record.Name?.Trim() ?? string.Empty,
                            DateOfBirth = record.DateOfBirth,
                            Married = record.Married,
                            Phone = record.Phone?.Trim() ?? string.Empty,
                            Salary = record.Salary
                        };

                        contacts.Add(contact);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing CSV record for contact: {Name}", record.Name);
                    }
                }

                if (contacts.Any())
                {
                    await _contactRepository.AddRangeAsync(contacts);
                    _logger.LogInformation("Imported {Count} contacts from CSV", contacts.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing contacts from CSV");
                throw new InvalidOperationException("Failed to import contacts from CSV file", ex);
            }

            return contacts.Select(ContactMapper.ToDto);
        }

        public async Task<bool> ContactExistsAsync(int id)
        {
            return await _contactRepository.ExistsAsync(id);
        }
    }
}
