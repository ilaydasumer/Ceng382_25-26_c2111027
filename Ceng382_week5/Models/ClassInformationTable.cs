using System.ComponentModel.DataAnnotations;

namespace Ceng382_week5.Models
{
    public class ClassInformationTable
    {
        [Display(Name = "Class Name")]
        public string ClassName { get; set; }

        [Display(Name = "Student Count")]
        public int StudentCount { get; set; }

        public string Description { get; set; }
 
        public int Id { get; set; }
    }
}