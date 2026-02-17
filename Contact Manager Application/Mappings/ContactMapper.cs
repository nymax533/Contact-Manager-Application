using Contact_Manager_Application.DTOs;
using Contact_Manager_Application.Models;

namespace Contact_Manager_Application.Mappings
{
    public static class ContactMapper
    {
        public static ContactDto ToDto(Contact contact)
        {
            return new ContactDto
            {
                Id = contact.Id,
                Name = contact.Name,
                DateOfBirth = contact.DateOfBirth,
                Married = contact.Married,
                Phone = contact.Phone,
                Salary = contact.Salary
            };
        }

        public static Contact ToEntity(CreateContactDto dto)
        {
            return new Contact
            {
                Name = dto.Name,
                DateOfBirth = dto.DateOfBirth,
                Married = dto.Married,
                Phone = dto.Phone,
                Salary = dto.Salary
            };
        }

        public static void UpdateEntity(Contact contact, UpdateContactDto dto)
        {
            contact.Name = dto.Name;
            contact.DateOfBirth = dto.DateOfBirth;
            contact.Married = dto.Married;
            contact.Phone = dto.Phone;
            contact.Salary = dto.Salary;
        }
    }
}
