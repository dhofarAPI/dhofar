using dhofarAPI.Data;
using dhofarAPI.DTOS.Category;
using dhofarAPI.InterFaces;
using dhofarAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace dhofarAPI.Services
{
    public class CategoryServices : ICategory
    {
        private readonly dhofarDBContext _db;
        public CategoryServices(dhofarDBContext db)
        {
            _db = db;
        }

        public async Task<Categorydto> Create(Categorydto category)
        {
            Category newCategory = new Category()
            {
                Name = category.Name,

            };
            foreach (var subCategoryDto in category.subcategories)
            {
                var subCategory = new SubCategory
                {


                    Name = subCategoryDto.Name
                };
                newCategory.subcategories.Add(subCategory);
                _db.SubCategories.AddAsync(subCategory);
            }

            Categorydto categorydto = new Categorydto
            {

              Name = newCategory.Name ,
              subcategories= newCategory.subcategories
            };


            await _db.Categories.AddAsync(newCategory);
            await _db.SaveChangesAsync();
            return categorydto;
        }


        public async Task Delete(int Id)
        {
            var deletedCategory = await _db.Categories.FindAsync(Id);
            if (deletedCategory != null)
            {
                _db.Entry(deletedCategory).State = EntityState.Deleted;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<List<Categorydto>> GetAll()
        {
            var allCategories = await _db.Categories
                .Include(c => c.subcategories)
                .ToListAsync();

            var categoryDtos = new List<Categorydto>();

            foreach (var category in allCategories)
            {
                var subcategoryDtos = new List<SubCategory>();

                foreach (var subcategory in category.subcategories)
                {
                    var subcategoryDto = new SubCategory
                    {
                        
                        Name = subcategory.Name,
                        CategoryId= subcategory.CategoryId
                    };

                    subcategoryDtos.Add(subcategoryDto);
                }

                var categoryDto = new Categorydto
                {
                    Name = category.Name,
                    subcategories = subcategoryDtos
                };

                categoryDtos.Add(categoryDto);
            }

            return categoryDtos;
        }

        public async Task<Categorydto> GetById(int id)
        {
            var category = await _db.Categories
                .Include(c => c.subcategories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return null; 
            }

            var categoryDto = new Categorydto
            {
                Id = category.Id,

                Name = category.Name,
                subcategories = category.subcategories.Select(subcategory => new SubCategory
                {
                    Id= subcategory.Id,
                    Name= subcategory.Name,
                }).ToList()
            };

            return categoryDto;
        }

        public async Task<Categorydto> Update(Categorydto categoryDto)
        {
            var category = await _db.Categories.FindAsync(categoryDto.Id);

            if (category == null)
            {
                return null; 
            }

            category.Name = categoryDto.Name;

            await _db.SaveChangesAsync();

            return categoryDto;
        }
    }
}
