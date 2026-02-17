using Contact_Manager_Application.Models.Csv;
using CsvHelper.Configuration;

namespace Contact_Manager_Application.Mappings.Csv
{
    public class ContactCsvMap : ClassMap<ContactCsvRecord>
    {
        public ContactCsvMap()
        {
            Map(m => m.Name).Name("Name");
            Map(m => m.DateOfBirth).Name("Date of birth", "DateOfBirth");
            Map(m => m.Married).Name("Married");
            Map(m => m.Phone).Name("Phone");
            Map(m => m.Salary).Name("Salary");
        }
    }
}
