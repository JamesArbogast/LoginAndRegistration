
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LoginAndRegistration.Models
{
  [NotMapped]
  public class LoginUser
  {
    // No other fields!
    [EmailAddress]
    [Required]
    public string LoginEmail {get; set;}
    [DataType(DataType.Password)]
    [Required]
    [MinLength(8, ErrorMessage="Password must be 8 characters or longer!")]
    public string LoginPassword { get; set; }
  }
}