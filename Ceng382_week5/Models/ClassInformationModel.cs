using System.ComponentModel.DataAnnotations;

namespace Ceng382_week5.Models
{
    public class ClassInformationModel
    {
        private static int idCounter = 1;

        public ClassInformationModel()
        {
            Id = idCounter++;
        }

        public int Id { get; set; }

        [Required]
        public string? ClassName { get; set; } // Nullable yapılır

        [Required]
        [Range(1, 1000, ErrorMessage = "Student count must be between 1 and 1000.")]
        public int StudentCount { get; set; }

        public string? Description { get; set; } // Nullable yapılır
    }
}
