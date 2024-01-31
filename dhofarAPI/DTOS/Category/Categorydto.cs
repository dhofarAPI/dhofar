using dhofarAPI.Model;

namespace dhofarAPI.DTOS.Category
{
    public class Categorydto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubCategory> subcategories { get; set; }

    }
}
