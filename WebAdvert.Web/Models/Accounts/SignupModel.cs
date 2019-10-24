using System.ComponentModel.DataAnnotations;


namespace WebAdvert.Web.Models.Accounts
{
    public class SignupModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(6, ErrorMessage ="Password must be at least six characters")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password confirm doesn't match Password")]
        public string ConfirmPassword { get; set; }

    }
}
