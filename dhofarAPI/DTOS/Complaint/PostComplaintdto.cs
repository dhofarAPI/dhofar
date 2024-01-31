using dhofarAPI.DTOS.ComplaintFiles;
using dhofarAPI.Model;

namespace dhofarAPI.DTOS.Complaint
{
    public class PostComplaintdto
    {
        public int CategoryId { get; set; }
        public string UserId { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }

        public List<ComplaintsFile> Files { get; set; } = new List<ComplaintsFile>();

    }

}
