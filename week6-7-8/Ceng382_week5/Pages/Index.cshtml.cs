using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Ceng382_week5.Models;
using Ceng382_week5.Data;

namespace Ceng382_week5.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        // Properties for data display
        public PaginatedList<ClassInformationModel> PaginatedClasses { get; set; }
        public IList<ClassInformationModel> ClassList { get; set; }

        // Filter properties
        [BindProperty(SupportsGet = true)]
        public string ClassNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStudentCount { get; set; }

        // Form model properties
        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public int EditId { get; set; }

        // Pagination settings
        public int PageSize { get; set; } = 5;

        public async Task OnGetAsync(int? pageIndex, int? id)
        {
            IQueryable<ClassInformationModel> query = _context.Classes;
             // Edit modunda ise ilgili kaydı yükle
            if (id.HasValue)
            {
                EditId = id.Value;
                var classToEdit = await _context.Classes.FindAsync(id.Value);
                if (classToEdit != null)
                {
                    NewClass = classToEdit;
                }
            }
            // Apply filters
            if (!string.IsNullOrEmpty(ClassNameFilter))
            {
                query = query.Where(c => c.ClassName.Contains(ClassNameFilter));
            }

            if (MinStudentCount.HasValue)
            {
                query = query.Where(c => c.StudentCount >= MinStudentCount.Value);
            }

     
         

            // Get data for both paginated and full list views
            PaginatedClasses = await PaginatedList<ClassInformationModel>.CreateAsync(
                query.AsNoTracking(),
                pageIndex ?? 1,
                PageSize);

            ClassList = await query.AsNoTracking().ToListAsync();
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("auth_token");
            Response.Cookies.Delete("remember_me");
            Response.Cookies.Delete("saved_username");
            return RedirectToPage("/Login");
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync(1,EditId);
                return Page();
            }

            _context.Classes.Add(NewClass);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }


        public async Task<JsonResult> OnGetEditClassAsync(int id)
        {
            var classToEdit = await _context.Classes.FindAsync(id);
            if (classToEdit == null)
            {
                return new JsonResult(new { success = false });
            }

            return new JsonResult(new {
                success = true,
                id = classToEdit.Id,
                className = classToEdit.ClassName,
                studentCount = classToEdit.StudentCount,
                description = classToEdit.Description
            });
        }
 
public async Task<IActionResult> OnPostEditClassAsync(int id)
{
    Console.WriteLine($"OnPostEditClassAsync çağrıldı, id: {id}");
    // Veritabanından güncellenmek istenen sınıfı al
    var classToUpdate = await _context.Classes
        .FirstOrDefaultAsync(c => c.Id == id);

    if (classToUpdate == null)
    {
        // Eğer sınıf bulunamazsa, hata mesajı dönebiliriz
        return NotFound();
    }
     Console.WriteLine($"Yeni Sınıf Adı: {NewClass.ClassName}, Öğrenci Sayısı: {NewClass.StudentCount}");


    // Verileri güncelle
    classToUpdate.ClassName = NewClass.ClassName;
    classToUpdate.StudentCount = NewClass.StudentCount;
    classToUpdate.Description = NewClass.Description;

    // Değişiklikleri kaydet
    _context.Classes.Update(classToUpdate);
    await _context.SaveChangesAsync();

    // Sayfayı yeniden yükle
    return RedirectToPage();  // Sayfa yeniden yüklenir
}
public IActionResult OnPostEdit(int id, string className, int studentCount, string description)
{
    var classToUpdate = _context.Classes.FirstOrDefault(c => c.Id == id);
    if (classToUpdate == null) return NotFound();

    // Değişen alanları güncelle
    if (!string.IsNullOrEmpty(className)) classToUpdate.ClassName = className;
    if (studentCount > 0) classToUpdate.StudentCount = studentCount;
    if (!string.IsNullOrEmpty(description)) classToUpdate.Description = description;

    // Değişiklikleri kaydet
    _context.SaveChanges();

    return RedirectToPage("./Index"); // İlgili sayfaya yönlendir
}


       public async Task<IActionResult> OnPostEditAsync()
        {
            // Model doğrulama kontrolü
            if (!ModelState.IsValid)
            {
                // Eğer form hatalıysa, sayfayı güncelle ve mevcut veriyi geri yükle
                await OnGetAsync(1, EditId); 
                return Page();
            }

            // Güncelleme yapılacak sınıfı veritabanından bul
            var classToEdit = await _context.Classes.FindAsync(EditId);

            // Eğer sınıf bulunmuşsa
            if (classToEdit != null)
            {
                // Formdan alınan yeni değerlerle sınıfı güncelle
                classToEdit.ClassName = NewClass.ClassName;
                classToEdit.StudentCount = NewClass.StudentCount;
                classToEdit.Description = NewClass.Description;
                classToEdit.IsActive = NewClass.IsActive;

                try
                {
                    // Veritabanını kaydet
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Hata durumunda mesaj göster
                    ModelState.AddModelError(string.Empty, $"Error updating class: {ex.Message}");
                    return Page();
                }
            }
            else
            {
                // Eğer sınıf bulunamazsa hata mesajı göster
                ModelState.AddModelError(string.Empty, "Class not found.");
                return Page();
            }

            // Güncelleme başarılı ise, ana sayfaya yönlendir
            return RedirectToPage("./Index"); 
        }


        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var classToDelete = await _context.Classes.FindAsync(id);
            if (classToDelete != null)
            {
                _context.Classes.Remove(classToDelete);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReorderAsync()
        {
            // var classes = await _context.Classes.OrderBy(c => c.DisplayOrder).ToListAsync();
            
            // // for (int i = 0; i < classes.Count; i++)
            // {
            //     classes[i].DisplayOrder = i + 1;
            // }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportFilteredJson([FromBody] ExportRequest request)
        {
            var query = _context.Users.AsQueryable();
            
            if (!string.IsNullOrEmpty(request.RoleFilter))
            {
                query = query.Where(u => u.Role == request.RoleFilter);
            }
            
            if (request.IsActiveFilter.HasValue)
            {
                query = query.Where(u => u.IsActive == request.IsActiveFilter.Value);
            }
            
            var filteredUsers = await query.AsNoTracking().ToListAsync();
            
            var jsonData = JsonSerializer.Serialize(filteredUsers, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            
            return new FileContentResult(Encoding.UTF8.GetBytes(jsonData), "application/json")
            {
                FileDownloadName = $"users_{DateTime.Now:yyyyMMddHHmmss}.json"
            };
        }

        public IActionResult OnGetExportAll(string? selectedColumns)
        {
            var data = _context.Classes
                .OrderBy(c => c.Id)
                .Select(c => new ClassInformationModel
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description,
                    IsActive = c.IsActive,
                
                });

            return ExportJson(data, selectedColumns?.Split(','));
        }

        private IActionResult ExportJson(IEnumerable<ClassInformationModel> data, string[]? selectedColumns = null)
        {
            selectedColumns ??= Array.Empty<string>();
            object jsonData;

            if (selectedColumns.Length > 0)
            {
                var filteredData = new List<ExpandoObject>();
                foreach (var item in data)
                {
                    dynamic expando = new ExpandoObject();
                    var dict = (IDictionary<string, object>)expando;

                    foreach (var prop in selectedColumns)
                    {
                        var value = item.GetType().GetProperty(prop)?.GetValue(item);
                        if (value != null)
                        {
                            dict.Add(prop, value);
                        }
                    }
                    filteredData.Add(expando);
                }
                jsonData = filteredData;
            }
            else
            {
                jsonData = data;
            }

            var json = JsonSerializer.Serialize(jsonData, new JsonSerializerOptions { 
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            return Content(json, "application/json");
        }

        public class ExportRequest
        {
            public string RoleFilter { get; set; }
            public bool? IsActiveFilter { get; set; }
        }

        public class PaginatedList<T> : List<T>
        {
            public int PageIndex { get; private set; }
            public int TotalPages { get; private set; }
            public bool HasPreviousPage => PageIndex > 1;
            public bool HasNextPage => PageIndex < TotalPages;

            public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
            {
                PageIndex = pageIndex;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                this.AddRange(items);
            }

            public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
            {
                var count = await source.CountAsync();
                var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
        }
    }
}