using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Automation_Website.Models
{
    public class LoginModel
    {

            

            [Display(Name = "Username")]
            [Required(ErrorMessage = "Username is empty.")]
            public string Username { get; set; }

            [Display(Name = "Password")]
            [Required(ErrorMessage = "Password is empty.")]
            public string Password { get; set; }

            public override string ToString()
            {
                return $"Username: {Username}, Password: {Password}";
            }
        
    }
}
