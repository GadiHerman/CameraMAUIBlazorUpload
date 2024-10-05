using Microsoft.AspNetCore.Mvc;

namespace ImageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        public class Upload
        {
            public IFormFile? File { get; set; }
        }

        // POST api/upload
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] Upload uploadFile)
        {
            IFormFile? file = uploadFile.File;

            if (file == null || file.Length == 0)
                return BadRequest("No file selected");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) ||
                (extension != ".pdf" && extension != ".png" && extension != ".jpg" && extension != ".jpeg"))
                return BadRequest("Invalid file type");

            // הוספת בדיקת גודל קובץ
            if (file.Length > 10 * 1024 * 1024) // 10MB
                return BadRequest("File too large");

            // הוספת בדיקת MIME type
            var allowedMimeTypes = new[] { "application/pdf", "image/jpeg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType))
                return BadRequest("Invalid file type");


            // Generate a new file name with a GUID
            var newFileName = Path.GetRandomFileName() + extension;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            var filePath = Path.Combine(folderPath, newFileName);

            // יצירת התיקייה אם לא קיימת
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = Url.Content($"~/uploads/{newFileName}");
            return Ok(new { url = fileUrl });
        }
    }
}
