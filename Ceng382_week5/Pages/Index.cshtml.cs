using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Ceng382_week5.Models;
using System;

namespace Ceng382_week5.Pages
{
    public class IndexModel : PageModel
    {
        private static List<ClassInformationModel> Classes = new List<ClassInformationModel>();
        private static int NextId = 1;
 
        [BindProperty(SupportsGet = true)]
        public string ClassNameFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStudentCount { get; set; }
 
        public PaginatedList<ClassInformationTable> PaginatedClasses { get; set; }
        public int PageSize { get; set; } = 5;  

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new ClassInformationModel();

        [BindProperty]
        public int EditId { get; set; }

        public void OnGet(int? id, int? pageIndex)
        {
         
            if (!Classes.Any() && string.IsNullOrEmpty(ClassNameFilter) && !MinStudentCount.HasValue)
            {
                GenerateSampleData();
            }

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

         
            var filteredClasses = Classes.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).AsQueryable();

            if (!string.IsNullOrEmpty(ClassNameFilter))
            {
                filteredClasses = filteredClasses.Where(c => c.ClassName.Contains(ClassNameFilter));
            }

            if (MinStudentCount.HasValue)
            {
                filteredClasses = filteredClasses.Where(c => c.StudentCount >= MinStudentCount);
            }

            PaginatedClasses = PaginatedList<ClassInformationTable>.Create(
                filteredClasses, pageIndex ?? 1, PageSize);
        }

        private void GenerateSampleData()
        {
            var random = new Random();
            var classNames = new[] { "Math", "Physics", "Chemistry", "Biology", "Programming" };

            for (int i = 1; i <= 100; i++)
            {
                Classes.Add(new ClassInformationModel
                {
                    Id = NextId++,
                    ClassName = $"{classNames[random.Next(classNames.Length)]} {i}",
                    StudentCount = random.Next(10, 100),
                    Description = $"Description for class {i}"
                });
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
            
            return RedirectToPage(new { id = (int?)null });
        }

        public IActionResult OnPostDelete(int id)
        {
            Classes.RemoveAll(c => c.Id == id);
            return RedirectToPage();
        }
    }

    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}