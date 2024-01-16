using BeFree.Areas.Admin.Models.Utilities.Enums;
using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels.Account
{
    public class RegisterVM
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(25, ErrorMessage = "Name can contain maximum 25 characters")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required")]
        [MaxLength(25, ErrorMessage = "Surname can contain maximum 25 characters")]
        public string Surname { get; set; }
        [Required(ErrorMessage ="You need select Gender")]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [MaxLength(25, ErrorMessage = "Username can contain maximum 25 characters")]
        public string UserName { get; set; }
        [Required(ErrorMessage ="Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage ="Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage ="You need confirm password")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }

    }
}
