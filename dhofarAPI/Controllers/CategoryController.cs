using Microsoft.AspNetCore.Mvc;
using dhofarAPI.DTOS.Category;
using dhofarAPI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using dhofarAPI.InterFaces;

namespace dhofarAPI.Controllers
{
    [Route("api/[controller]/[Action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategory _categoryService;

        public CategoryController(ICategory categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/Category
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Categorydto>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAll();
            return Ok(categories);
        }

        // GET: api/Category/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Categorydto>> GetCategoryById(int id)
        {
            var category = await _categoryService.GetById(id);

            if (category == null)
            {
                return NotFound(); 
            }

            return Ok(category);
        }

        // POST: api/Category
        [HttpPost]
        public async Task<ActionResult<Categorydto>> CreateCategory(Categorydto category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _categoryService.Create(category);
            return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.Id }, createdCategory);
        }

        // PUT: api/Category/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Categorydto category)
        {
            if (id != category.Id)
            {
                return BadRequest();
            }

            var updatedCategory = await _categoryService.Update(category);

            if (updatedCategory == null)
            {
                return NotFound(); 
            }

            return NoContent();
        }

        // DELETE: api/Category/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.Delete(id);
            return NoContent();
        }
    }
}
