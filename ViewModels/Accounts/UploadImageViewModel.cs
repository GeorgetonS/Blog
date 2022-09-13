using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Accounts;

public class UploadImageViewModel
{
    [Required(ErrorMessage = "Imagem Invalida")]
    //public byte Base64Image { get; set; }
    public string Base64Image { get; set; }
}
