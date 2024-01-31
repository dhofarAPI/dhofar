using Microsoft.AspNetCore.Identity;

namespace dhofarAPI.Model
{
    public class User : IdentityUser
    {
        public string  FullName { get; set; }

        public string? ImageURL { get; set; }

        public string CodeNumber { get; set; }

        public List<Complaint>? Complaints { get; set; }

        public List<FavoriteSubject>? FavoriteSubjects { get; set; }

        public List<CommentSubject>? CommentSubjects { get; set; }

        public List<RatingSubject>? RatingSubjects { get; set; }
    }   
}
