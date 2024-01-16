using BeFree.Models;
using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels.Product
{
    public class ProductUpdateVM
    {
        [Required(ErrorMessage ="Name is required")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }
        public IFormFile? Photo { get; set; }
        public string? ImageURL { get; set; }
        public int? CategoryId { get; set; }
        public List<Category>? Categories { get; set; }
    }
}
