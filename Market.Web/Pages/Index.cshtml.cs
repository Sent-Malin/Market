using Market_Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Market_Web.Pages
{
    public class LoginModel : PageModel
    {
        public Errors Error { get; set; }
        public string ErrorMessage { get; set; }
        public LoginModel(string errorMessage="", Errors error=Errors.None)
        {
            ErrorMessage = errorMessage;
            Error = error;
        }
    }
}
