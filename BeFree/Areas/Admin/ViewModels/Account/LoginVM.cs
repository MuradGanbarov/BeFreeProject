using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels.Account
{
    public class LoginVM
    {
        [Required(ErrorMessage ="You need write username or email")]
        public string UserNameOrEmail { get; set; }

        [Required(ErrorMessage ="You need write Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool IsRemember { get; set; }

    }
}
