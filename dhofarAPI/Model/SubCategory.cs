﻿namespace dhofarAPI.Model
{
    public class SubCategory
    {
        public int Id { get; set; }

        public string  Name { get; set; }

        public int CategoryId { get; set; }
        public Category category { get; set; }
    }
}
