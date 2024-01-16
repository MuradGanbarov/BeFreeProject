using BeFree.Models;
using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels
{
    public class EmployeeCreateVM
    {
        [Required(ErrorMessage ="Name is required")]
        [MaxLength(25,ErrorMessage ="Name can contain maximum 25 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(25, ErrorMessage = "Surname can contain maximum 25 characters")]
        public string Surname { get; set; }
        public string? Facebook { get; set; }
        public string? Twitter { get; set; }
        public string? GooglePlus { get; set; }
        public IFormFile Photo { get; set; }

        public int? PositionId { get; set; }
        public List<Position>? Positions { get; set; }
    }
}
