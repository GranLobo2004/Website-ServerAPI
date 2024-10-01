using System;
using System.Collections.Generic;

namespace ServerAPI.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
    }
}