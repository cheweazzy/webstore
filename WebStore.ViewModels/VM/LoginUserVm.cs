using System.ComponentModel.DataAnnotations;

namespace WebStore.ViewModels.VM;

public class LoginUserVm
{
    [Required]
    [Display(Name = "Email")]
    public string Login { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = default!;
}