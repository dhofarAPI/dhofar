namespace dhofarAPI.Model
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubCategory> subcategories { get; set;}
        public List<Complaint>Complaints { get; set; }
    }
}
