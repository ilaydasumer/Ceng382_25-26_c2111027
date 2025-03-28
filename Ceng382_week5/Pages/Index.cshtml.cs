using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ceng382_week5.Models;

namespace Ceng382_week5.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> Classes = new List<ClassInformationModel>();
        private static int NextId = 1;

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public int EditId { get; set; }

        public List<ClassInformationModel> ClassList => Classes;

        public void OnGet(int? id)
        {
            if (id.HasValue)
            {
                var classToEdit = Classes.FirstOrDefault(c => c.Id == id.Value);
                if (classToEdit != null)
                {
                    NewClass = new ClassInformationModel
                    {
                        Id = classToEdit.Id,
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
                    EditId = classToEdit.Id;
                }
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid) return Page();
            
            NewClass.Id = NextId++;
            Classes.Add(NewClass);
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid) return Page();

            var classToEdit = Classes.FirstOrDefault(c => c.Id == EditId);
            if (classToEdit != null)
            {
                classToEdit.ClassName = NewClass.ClassName;
                classToEdit.StudentCount = NewClass.StudentCount;
                classToEdit.Description = NewClass.Description;
            }
            
            // I got help from ai to reset the form after the update.
            return RedirectToPage(new { id = (int?)null });
        }

        public IActionResult OnPostDelete(int id)
        {
            Classes.RemoveAll(c => c.Id == id);
            return RedirectToPage();
        }
    }
}