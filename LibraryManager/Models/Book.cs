﻿namespace LibraryManager.Models
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string ISBN { get; set; } = string.Empty;
        public int YearPublished { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? BorrowedDate { get; set; }

        public Book()
        {

        }
    }
}
