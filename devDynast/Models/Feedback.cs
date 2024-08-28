using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace devDynast.Models
{
    [Table("feedback")]
    public class Feedback
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("userid")]
        [StringLength(100)]
        public string? UserId { get; set; } 

        [Required]
        [Column("title")]
        [StringLength(255)]
        public string? Title { get; set; }

        [Required]
        [Column("content")]
        [StringLength(1000)]
        public string? Content { get; set; }

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
