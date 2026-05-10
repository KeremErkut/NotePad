using System;

namespace NotePad.Models
{
    // The model class must be public for serialization
    public class Note
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}