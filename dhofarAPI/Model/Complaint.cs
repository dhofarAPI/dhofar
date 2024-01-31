using System.ComponentModel.DataAnnotations.Schema;

namespace dhofarAPI.Model
{
    public class Complaint
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public bool IsAccepted {  get; set; }
        public DateTime Time { get; set; }
        //ublic List<ComplaintsFile>? Files { get; set; }
        public List<ComplaintsFile> Files { get; set; } = new List<ComplaintsFile>();

        public User User { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
       public Category? Category { get; set; }



    }
}
