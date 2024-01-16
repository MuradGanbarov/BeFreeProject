using BeFree.Models;
using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels.Employee
{
    public class EmployeeUpdateVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(25, ErrorMessage = "Surname can contain maximum 25 characters")]
        public string Surname { get; set; }
        public string? ImageURL { get; set; }
        public IFormFile? Photo { get; set; }
        public string? Facebook { get; set; }
        public string? Instagram { get; set; }
        public string? GooglePlus { get; set; }
        public int? PositionId { get; set; }
        public List<Position>? Positions { get; set; }
    }
}
