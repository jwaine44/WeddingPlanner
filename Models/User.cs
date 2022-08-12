#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WeddingPlanner.Models;

public class User
{
    [Key]
    public int UserId {get;set;}

    [Required(ErrorMessage = "First name is required!")]
    [Display(Name = "First Name:")]
    public string FirstName {get;set;}

    [Required(ErrorMessage = "Last name is required!")]
    [Display(Name = "Last Name:")]
    public string LastName {get;set;}

    [Required(ErrorMessage = "Email is required!")]
    [EmailAddress]
    public string Email {get;set;}

    [Required(ErrorMessage = "Password is required!")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters!")]
    [DataType(DataType.Password)]
    [Display(Name = "Password:")]
    public string Password {get;set;}

    [NotMapped]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Doesn't match password!")]
    [Display(Name = "Confirm Password:")]
    public string ConfirmPassword {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    // Navigation property for related Wedding objects
    public List<Wedding> CreatedWeddings {get;set;} = new List<Wedding>();

    public List<Connection> Connections {get;set;} = new List<Connection>();

    public string FullName()
    {
        return FirstName + " " + LastName;
    }
}