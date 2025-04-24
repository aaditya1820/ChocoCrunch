using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Aaditya.Models
{
    public class OTPModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OTP { get; set; }
    }
}
