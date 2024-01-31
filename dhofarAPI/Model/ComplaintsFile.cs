using System.Text.Json.Serialization;

namespace dhofarAPI.Model
{
    public class ComplaintsFile
    {
        public int Id { get; set; }
        public string? FilePaths { get; set; }
        public string? Filename { get; set; } 

        public int ComplaintId { get; set; }
         
        public Complaint? complaint { get; set; }
    }
}
