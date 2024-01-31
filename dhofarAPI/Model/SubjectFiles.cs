namespace dhofarAPI.Model
{
    public class SubjectFiles
    {
        public int Id { get; set; }

        public int SubjecId { get; set; }

        public string? ImageURL { get; set; }

        public Subject? Subject { get; set; }
    }
}
