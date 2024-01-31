using dhofarAPI.DTOS.Category;
using dhofarAPI.DTOS.Complaint;
using dhofarAPI.Model;

namespace dhofarAPI.InterFaces
{
    public interface ICategory
    {
        public Task<List<Categorydto>> GetAll();
        public Task<Categorydto> GetById(int id);  
        public Task<Categorydto> Create(Categorydto category);
        public Task<Categorydto> Update(Categorydto category);
        public Task Delete(int Id);


    }
}
