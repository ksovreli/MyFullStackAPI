using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BackpackStoreFS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController(IWebHostEnvironment env) : ControllerBase
    {
        private readonly IWebHostEnvironment _env = env;

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return BadRequest("ფაილი არ არის არჩეული.");
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    return BadRequest("არავალიდური ფაილის ფორმატი. დაშვებულია მხოლოდ ფოტოები (.jpg, .jpeg, .png, .webp).");
                }

                string wwwrootPath = _env.WebRootPath;
                string uploadsFolder = Path.Combine(wwwrootPath, "uploads");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string fileName = Guid.NewGuid().ToString() + extension;
                string filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileUrl = $"{Request.Scheme}://{Request.Host}/uploads/{fileName}";
                return Ok(new { url = fileUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"სისტემური შეცდომა: {ex.Message}");
            }
        }
    }
}