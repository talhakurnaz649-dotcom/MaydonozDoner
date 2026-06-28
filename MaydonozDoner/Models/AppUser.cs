using Microsoft.AspNetCore.Identity;

namespace MaydonozDoner.Models
{
    public class AppUser : IdentityUser
    {
        public string Address { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string CardExpiry { get; set; } = string.Empty;
        public string CardCvv { get; set; } = string.Empty;
    }
}
