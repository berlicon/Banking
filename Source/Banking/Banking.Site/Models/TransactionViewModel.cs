using System.ComponentModel.DataAnnotations;
using Banking.Data;

namespace Banking.Site.Models
{
    public class TransactionViewModel: Transaction
    {
        [Required]
        [StringLength(20, ErrorMessage = "The value {0} should be at least {1} symbols.", MinimumLength = 20)]
        [DataType(DataType.Text)]
        [Display(Name = "Account To number")]
        public string AccountToNumber { get; set; }
    }
}
