using System.ComponentModel.DataAnnotations;

namespace Lab2.DataAccess
{
    public class Bread
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Type { get; set; } 

        [Required]
        public double Size { get; set; }
        public int BasketId { get; set; }
        public Basket Basket { get; set; }
    }
}
