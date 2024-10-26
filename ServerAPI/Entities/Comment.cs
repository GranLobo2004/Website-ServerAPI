
namespace ServerAPI.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string UserImage { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
    }
}