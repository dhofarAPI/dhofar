    namespace dhofarAPI.Model
{
    public class RatingSubject
    {
        public int Id { get; set; }

        public int SubjectId { get; set; }

        public string UserId { get; set; }

        public bool IsLike { get; set; }

        public bool IsDisLike { get; set; }

        public Subject Subject { get; set; }

        public User User { get; set; }
    }
}
