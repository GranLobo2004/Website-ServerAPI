// Product.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerAPI.Entities
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Characteristics { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public float Rating { get; set; }
        
        public int NRatings { get; set; }
    }
}
