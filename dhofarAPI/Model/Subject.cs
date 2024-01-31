﻿namespace dhofarAPI.Model
{
    public class Subject
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public int SubjectTypeId { get; set; }

        public string PrimarySubject { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public int LikeCounter { get; set; }

        public int DisLikeCounter { get; set; }

        public List<SubjectFiles>? SubjectFiles { get; set; }

        public List<RatingSubject>? RatingSubjects { get; set; }

        public List<CommentSubject>? CommentSubjects { get; set; }

        public List<SubjectType>? SubjectTypes { get; set; }

        public List<FavoriteSubject>? FavoriteSubjects { get; set; }

        public User User { get; set; }

    }
}
