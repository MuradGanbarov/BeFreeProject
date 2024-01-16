using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels
{
    public class CategoryUpdateVM
    {
        [Required(ErrorMessage="Name is required")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }
    }
}
