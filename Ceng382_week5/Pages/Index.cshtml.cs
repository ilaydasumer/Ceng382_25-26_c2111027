using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ceng382_week5.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ceng382_week5.Pages
{
    public class IndexModel : PageModel
    {
        public List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInformationModel ClassInfo { get; set; }

        public void OnGet()
        {
           
        }

        public IActionResult OnPostAdd()
        {
            if (ClassInfo != null && !string.IsNullOrWhiteSpace(ClassInfo.ClassName) && ClassInfo.StudentCount > 0)
            {
                ClassInfo.Id = ClassList.Count + 1;
                ClassList.Add(ClassInfo);  // Veriyi ekle
            }
            return RedirectToPage();  // Sayfayı yenile
        }

        public IActionResult OnPostDelete(int id)
        {
            var classToDelete = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToDelete != null)
            {
                ClassList.Remove(classToDelete); // Silme işlemi
            }
            return RedirectToPage(); // Sayfayı yenile
        }

        public IActionResult OnPostEdit(int id)
        {
            var classToEdit = ClassList.FirstOrDefault(c => c.Id == id);
            if (classToEdit != null)
            {
                ClassInfo = classToEdit;  // Düzenlenecek sınıfı al
            }
            return Page();  // Sayfayı düzenlemek için döndür
        }
    }
}
