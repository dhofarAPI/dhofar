using dhofarAPI.DTOS.ComplaintFiles;
using dhofarAPI.Model;

namespace dhofarAPI.DTOS.Complaint
{
    public class GetComplaintdto
    {

        public string State { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public DateTime Time { get; set; }
        public List<ComplaintsFile> Files { get; set; } = new List<ComplaintsFile>();



    }
}
