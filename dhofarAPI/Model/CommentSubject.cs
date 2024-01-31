namespace dhofarAPI.Model
{
    public class CommentSubject
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public string UserId { get; set; }

        public string Comment { get; set; }

        public DateTime CommentTime { get; set; }

        public Subject Subject { get; set; }    

        public User User { get; set; }
    }
}
