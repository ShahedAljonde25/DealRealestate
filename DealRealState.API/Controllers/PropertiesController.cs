using DealRealState.Business.Services;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Models;
using BusinessLayer.Services;
using Microsoft.AspNetCore.Authorization;
using DataLayer.DataAccess;
using Microsoft.AspNetCore.Hosting;

namespace DealRealState.API.Controllers
{
    [ApiController]
    [Route("api/properties")]
    public class PropertiesController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly PropertyService _propertyService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        // --- قم بإضافة هذا الـ Constructor هنا ---
        public PropertiesController(
            IConfiguration configuration,
            PropertyService propertyService,
            IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _propertyService = propertyService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/properties

        [HttpGet]
        public async Task<IActionResult> GetProperties([FromQuery] string propertyType = null)
        {
            try
            {
                var properties = await PropertyService.GetAllProperties(propertyType);

                if (properties == null || properties.Count == 0)
                {
                    return NotFound("No properties found with the specified criteria.");
                }

                return Ok(properties);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }






        [HttpPost("{id}/upload-panorama")]
        [AllowAnonymous]
        public async Task<IActionResult> UploadPanorama(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // قراءة المسار الآمن من ملف الإعدادات
            string uploadsFolder = _configuration["FileSettings:UploadsFolderPath"];

            if (string.IsNullOrEmpty(uploadsFolder))
                return StatusCode(500, "Server upload path is not configured securely.");

            // التأكد من وجود المجلد أو إنشائه
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            // توليد اسم فريد للملف لتجنب التضارب
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // حفظ الملف في المسار المعزول
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // الرابط الذي سيتم حفظه في القاعدة وعرضه للواجهة الأمامية (يمكن ربطه بـ Static File Middleware مخصص أو مسار آمن)
            string fileUrl = $"/uploads/panoramas/{uniqueFileName}";

            // حفظ الرابط في قاعدة البيانات عبر الـ Stored Procedure
            try
            {
                bool isSaved = await _propertyService.AddPanoramaToPropertyAsync(id, fileUrl);

                if (!isSaved)
                    return StatusCode(500, "Database returned false when saving panorama.");

                return Ok(new { filePath = fileUrl });
            }
            catch (Exception ex)
            {
                // أرجع رسالة الخطأ الحقيقية للـ Response لترى المشكلة فوراً
                return StatusCode(500, $"Database Error: {ex.Message}");
            };
        }


        // GET: api/properties/5

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPropertyByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var property = await _propertyService.GetPropertyByIDAsync(id);

            if (property == null)
                return NotFound();

            return Ok(property);
        }



        // PUT: api/properties
        [Authorize(Roles = "Seller")]
        [HttpPut]
        public IActionResult UpdateProperty(Property property)
        {

            var result =
                _propertyService.UpdateProperty(property);


            if (!result)
                return BadRequest();


            return Ok();

        }



        // DELETE: api/properties/5
        [Authorize(Roles = "Seller")]
        [HttpDelete("{id}")]
        public IActionResult DeleteProperty(int id)
        {
            if (id <= 0)
            {
                return BadRequest();

            }
            var result =
                _propertyService.DeleteProperty(id);


            if (!result)
                return BadRequest();


            return Ok();

        }

    }
}