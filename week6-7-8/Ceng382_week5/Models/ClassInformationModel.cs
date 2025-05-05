using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ceng382_week5.Models
{
    [Table("Classes")]
    public class ClassInformationModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // ID'nin otomatik artmasın
        public int Id { get; set; }

    [Required]
    [Column("Name")] // Tablodaki sütun adı
    public string ClassName { get; set; } = string.Empty;

    [Required]
    [Column("PersonCount")] // Tablodaki sütun adı
    public int StudentCount { get; set; }

    [Column("Description")] // Tablodaki sütun adı
    public string Description { get; set; } = string.Empty;

    [Required]
    public bool IsActive { get; set; } = true; 
 
    }
}