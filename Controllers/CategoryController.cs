using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Categories;
//using Blog.ViewModels.Categories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;


// Health Check
namespace Blog.Controllers
{
    [ApiController]

    public class CategoryController : ControllerBase

    {
        [HttpGet("v1/categories")]
        public async Task<IActionResult> GetAsync(
            [FromServices] IMemoryCache cache,
            [FromServices]BlogDataContext context)
        {
            try
            {
                //var categories = await context.Categories.ToListAsync();
                var categories = cache.GetOrCreate("CategoriesCache", entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                    return GetCategories(context);
                });
                return Ok(new ResultViewModel<List<Category>>(categories));
            }
            catch //(Exception ex)
            { 
                return StatusCode(500, new ResultViewModel<List<Category>>("05x01 - Falha Interna no Servidor"));
            }
        }

        private List<Category> GetCategories(BlogDataContext context)
        {
            return context.Categories.ToList();
        }

        [HttpGet("v1/categories/{id:int}")]
        public async Task<IActionResult> GetByIdAsync(
            [FromRoute] int id,
            [FromServices]BlogDataContext context)
        {
            try
            { 
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("04X01 - Conteudo não encontrado"));

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new ResultViewModel<Category>("05x02 - Falha Interna no servidor"));
            }
        }


        [HttpPost("v1/categories")]
        public async Task<IActionResult> PostAsync(
            [FromBody] EditorCategoryViewModel model,
            [FromServices]BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<Category>(ModelState.GetErrors()));
            
            try
            { 
                var category = new Category
                {
                    Id = 0,
                    Posts = null,
                    Name = model.Name,
                    Slug = model.Slug.ToLower(),
                };
                await context.Categories.AddAsync(category);
                await context.SaveChangesAsync();

                return Created($"v1/categories/{category.Id}", new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            { 
                //return BadRequest("Não foi possivel incluir a categoria");
                return StatusCode(500, new ResultViewModel<Category>("04X02 - Não Foi Possivel Incluir a Categoria"));
            }
            catch //(Exception ex)
            { 
                return StatusCode(500, new ResultViewModel<Category>("05X02 - Conteudo não encontrado"));
            }
        }


        [HttpPut("v1/categories/{id:int}")]
        public async Task<IActionResult> PutAsync(
            [FromRoute] int id,
            [FromBody] EditorCategoryViewModel model,
            [FromServices]BlogDataContext context)
        {
            try
            {
                var category = await context
                .Categories
                .FirstOrDefaultAsync(x => x.Id == id);

                if (category == null)
                    return NotFound(new ResultViewModel<Category>("03x01 - Conteúdo não encontrado"));

                category.Name = model.Name;
                category.Slug = model.Slug;

                context.Categories.Update(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            { 
                //return BadRequest("Não foi possivel incluir a categoria");
                return StatusCode(500, new ResultViewModel<Category>("05XE8 - Não foi possivel alterar a categoria"));
            }
            catch //(Exception ex)
            { 
                return StatusCode(500, new ResultViewModel<Category>("05XE11 - Falha interna no servidor"));
            }
            
        }

        [HttpDelete("v1/categories/{id:int}")]
        public async Task<IActionResult> DeleteAsync(
            [FromRoute] int id,
            [FromServices]BlogDataContext context)
        {
            try 
            { 
                var category = await context
                    .Categories
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new ResultViewModel<Category>("03x01 - Conteúdo não encontrado"));

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<Category>(category));
            }
            catch (DbUpdateException ex)
            { 
                //return BadRequest("Não foi possivel incluir a categoria");
                return StatusCode(500, new ResultViewModel<Category>("05XE7 - Não foi possivel excluir a categoria"));
            }
            catch (Exception ex)
            { 
                return StatusCode(500, new ResultViewModel<Category>("05XE12 - Falha interna no servidor"));
            }
        }
    }
}





   /* public class CategoryController : ControllerBase

    {
       
         [HttpGet("v2/categories")]
        public IActionResult Get2(
            [FromServices]BlogDataContext context)
        {
            var categories = context.Categories.ToList();
            return Ok();
        }
    }

}
   */

