using System.ComponentModel.DataAnnotations;
using GeziBlogum.Entity;

namespace GeziBlogum.Models{

    public class PostCreateViewModel{

        public int PostId {get;set;}
        [Required]
        [Display(Name = "Başlık")]
        public string? Title {get;set;}
        [Required]
        [Display(Name = "Açıklama")]
        public string? Description {get;set;}
        [Required]
        [Display(Name = "İçerik")]
        public string? Content {get;set;}
        [Required]
        [Display(Name = "Url")]
        public string? Url {get;set;}
        [Required(ErrorMessage = "Lütfen bir görsel dosyası seçiniz.")]
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }  
        public string? Image { get; set; } 
        public bool IsActive {get;set;}
        public int SelectedTagId { get; set; }
        public List<Tag>? Tags { get; set; }

    }
}