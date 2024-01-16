using System.ComponentModel.DataAnnotations;

namespace BeFree.Areas.Admin.ViewModels.Setting
{
    public class SettingUpdateVM
    {
        [Required(ErrorMessage ="Key is required")]
        [MaxLength(25,ErrorMessage ="Key can contain maximum 25 characters")]
        public string Key { get; set; }
        [Required(ErrorMessage = "Value is required")]
        [MaxLength(25, ErrorMessage = "Value can contain maximum 25 characters")]

        public string Value { get; set; }
    }
}
