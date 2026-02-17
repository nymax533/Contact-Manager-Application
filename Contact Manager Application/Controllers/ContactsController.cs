using Contact_Manager_Application.DTOs;
using Contact_Manager_Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Contact_Manager_Application.Controllers
{
    public class ContactsController : Controller
    {
        private readonly IContactService _contactService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(IContactService contactService, ILogger<ContactsController> logger)
        {
            _contactService = contactService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var contacts = await _contactService.GetAllContactsAsync();
                return View(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading contacts");
                TempData["Error"] = "Failed to load contacts";
                return View(new List<ContactDto>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Please select a valid CSV file";
                return RedirectToAction(nameof(Index));
            }

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
            {
                TempData["Error"] = "Only CSV files are allowed";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                using var stream = file.OpenReadStream();
                var contacts = await _contactService.ImportContactsFromCsvAsync(stream);
                
                TempData["Success"] = $"Successfully imported {contacts.Count()} contacts";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading CSV file");
                TempData["Error"] = $"Failed to import contacts: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var contacts = await _contactService.GetAllContactsAsync();
                return Json(new { success = true, data = contacts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all contacts");
                return Json(new { success = false, message = "Failed to retrieve contacts" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var contact = await _contactService.GetContactByIdAsync(id);
                if (contact == null)
                {
                    return Json(new { success = false, message = "Contact not found" });
                }

                return Json(new { success = true, data = contact });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contact by ID: {ContactId}", id);
                return Json(new { success = false, message = "Failed to retrieve contact" });
            }
        }

        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update([FromBody] UpdateContactDto contactDto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid contact data", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            try
            {
                var result = await _contactService.UpdateContactAsync(contactDto);
                if (!result)
                {
                    return Json(new { success = false, message = "Contact not found" });
                }

                return Json(new { success = true, message = "Contact updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact with ID: {ContactId}", contactDto.Id);
                return Json(new { success = false, message = $"Failed to update contact: {ex.Message}" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _contactService.DeleteContactAsync(id);
                if (!result)
                {
                    return Json(new { success = false, message = "Contact not found" });
                }

                return Json(new { success = true, message = "Contact deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact with ID: {ContactId}", id);
                return Json(new { success = false, message = $"Failed to delete contact: {ex.Message}" });
            }
        }
    }
}
