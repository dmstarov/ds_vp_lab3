using System.ComponentModel.DataAnnotations;

namespace Lab2.DataAccess
{
    public class Basket
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BasketDiscount { get; set; }

        [Required]
        [StringLength(100)]
        public string Client { get; set; }

        [Required]
        public double Store { get; set; }

        [Required]
        public int Priority { get; set; }

        public List<Delivery> Delivery { get; set; } = new List<Delivery>();
    }
}
