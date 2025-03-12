using System.ComponentModel.DataAnnotations;

namespace ByteCore.Web.Models
{
    public class MentorModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Expertise")]
        public string Expertise { get; set; }

        [Required]
        [Display(Name = "Hourly Rate (USD)")]
        public decimal Rate { get; set; }

        [Display(Name = "Rating")]
        public double Rating { get; set; }

        [Display(Name = "Profile Image URL")]
        public string ImageUrl { get; set; }
    }
}