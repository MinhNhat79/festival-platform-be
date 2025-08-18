using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FestivalFlatform.Data.Models
{
    public class Festival
    {
        [Key]
        public int FestivalId { get; set; }

        [Required]
        public int SchoolId { get; set; }
        [Required]
        public string FestivalName { get; set; } = null!;

        public string? Theme { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? RegistrationStartDate { get; set; }
        public DateTime? RegistrationEndDate { get; set; }
        public string? Location { get; set; }

        [Required]
        public int MaxFoodBooths { get; set; } = 0;

        [Required]
        public int MaxBeverageBooths { get; set; } = 0;

        public int RegisteredFoodBooths { get; set; } = 0;

        public int RegisteredBeverageBooths { get; set; } = 0;

        public string Status { get; set; } = "draft";
        public decimal TotalRevenue { get; set; } = 0;
        public string? Description { get; set; }

        public string? cancellationReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual School School { get; set; } = null!;

        public virtual ICollection<AccountFestivalWallet> AccountFestivalWallets { get; set; } = new List<AccountFestivalWallet>();



        public virtual ICollection<Image> Images { get; set; } = new List<Image>();

        public virtual ICollection<Booth> Booths { get; set; } = new List<Booth>();
        public virtual ICollection<FestivalIngredient> FestivalIngredients { get; set; } = new List<FestivalIngredient>();
        public virtual ICollection<FestivalMap> FestivalMaps { get; set; } = new List<FestivalMap>();
        public virtual ICollection<FestivalSchool> FestivalSchools { get; set; } = new List<FestivalSchool>();

    }
}
