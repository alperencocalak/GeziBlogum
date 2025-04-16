using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GeziBlogum.Models
{
    public class ProfileUpdateViewModel
    {
        public int UserId { get; set; }

        [Display(Name = "Ad Soyad")]
        public string? Name { get; set; }

        [Display(Name = "Kullanıcı Adı")]
        public string? Username { get; set; }

        [Display(Name = "Profil Fotoğrafı")]
        public string? Image { get; set; }

        [Display(Name = "Yeni Profil Fotoğrafı")]
        public IFormFile? ImageFile { get; set; }

        [Display(Name = "Mevcut Şifre")]
        [DataType(DataType.Password)]
        public string? CurrentPassword { get; set; }

        [Display(Name = "Yeni Şifre")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Display(Name = "Yeni Şifre (Tekrar)")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }
    }
}
