#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
namespace WeddingPlanner.Models;

public class Wedding
{
    [Key]
    public int WeddingId {get;set;}

    [Required(ErrorMessage = "This is required!")]
    [Display(Name = "Wedder One")]
    public string WedderOne {get;set;}

    [Required(ErrorMessage = "This is required!")]
    [Display(Name = "Wedder Two")]
    public string WedderTwo {get;set;}

    [Required(ErrorMessage = "Date is required!")]
    [DataType(DataType.Date)]
    [DateValidation]
    public DateTime Date {get;set;}

    [Required(ErrorMessage = "Address is required!")]
    [Display(Name = "Wedding Address")]
    public string Address {get;set;}

    // Foreign key for User model
    public int UserId {get;set;}

    // Navigation property for related User object
    public User? Creator {get;set;}

    public DateTime CreatedAt {get;set;} = DateTime.Now;
    public DateTime UpdatedAt {get;set;} = DateTime.Now;

    public List<Connection> Connections {get;set;} = new List<Connection>();
}