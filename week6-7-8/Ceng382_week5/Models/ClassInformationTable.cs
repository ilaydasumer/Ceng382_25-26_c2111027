using System.ComponentModel.DataAnnotations;

namespace Ceng382_week5.Models
{
    public class ClassInformationTable
    {
        [Display(Name = "Class Name")]
        public string ClassName { get; set; } = string.Empty;

        [Display(Name = "Student Count")]
        public int StudentCount { get; set; }

        public string Description { get; set; } = string.Empty;

        public int Id { get; set; }
        public int DisplayOrder { get; set; } 
    }
}