using FestivalFlatform.Data.Models;
using System.ComponentModel.DataAnnotations;

public class AccountFestivalWallet
{
    [Key]
    public int AccountFestivalWalletId { get; set; }

    public int FestivalId { get; set; }
    public int AccountId { get; set; }

    public string? Name { get; set; }
    public decimal Balance { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

  
    public virtual Festival Festival { get; set; } = null!;
    public virtual Account Account { get; set; } = null!;
}
