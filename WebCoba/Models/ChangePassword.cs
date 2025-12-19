using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace WebCoba.Models {
    public class ChangePassword {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password Baru")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Konfirmasi Password Baru")]
        [Compare("NewPassword", ErrorMessage = "Password baru dan konfirmasi tidak cocok.")]
        public string ConfirmPassword { get; set; }
    }
}