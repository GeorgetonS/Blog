using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels.Categories
{

    public class EditorCategoryViewModel
    {
        [Required(ErrorMessage = "O nome é Obrigatorio!")]
        [StringLength(40, MinimumLength = 3, ErrorMessage = "Este campo deve contar entre 3 40 caracteres")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Slug é Obrigatorio!")]
        public string Slug { get; set; }
    }
}

