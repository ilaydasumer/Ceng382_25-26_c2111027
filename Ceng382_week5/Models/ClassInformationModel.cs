using System.ComponentModel.DataAnnotations;

namespace Ceng382_week5.Models
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        
 
        public string ClassName { get; set; }
        
     
        public int StudentCount { get; set; }
        
        public string Description { get; set; }
    }
}