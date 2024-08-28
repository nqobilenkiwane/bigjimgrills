using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devDynast.Models
{
    [Table("cart")]
    public class Cart
    {
        [Key]
        [Column("id")] 
        public int Id { get; set; }

        [Required]
        [Column("user_id")] 
        public string? UserId { get; set; }

        [Required]
        [Column("product_id")] 
        public String? ProductId { get; set; }

        [Required]
        [Column("quantity")] 
        public int Quantity { get; set; }

        [Required]
        [Column("price")] 
        public double Price { get; set; }

        [Required]
        [Column("status")] 
        public string? Status { get; set; }


        [Required]
        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
