using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WeddingPlanner.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WeddingPlanner.Controllers;
    
public class UserController : Controller
{
    private int? uid
    {
        get
        {
            return HttpContext.Session.GetInt32("uid");
        }
    }

    private bool loggedIn
    {
        get
        {
            return uid != null;
        }
    }

    private WeddingPlannerContext _context;

    public UserController(WeddingPlannerContext context)
    {
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        if(loggedIn)
        {
            return RedirectToAction("Dashboard", "Wedding");
        }
        
        return View("Index");
    }

    [HttpPost("/register")]
    public IActionResult Register(User newUser)
    {
        if(ModelState.IsValid)
        {
            if(_context.Users.Any(user => user.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "already exists!");
            }
        }

        if(ModelState.IsValid == false)
        {
            return Index();
        }
        
        PasswordHasher<User> hasher = new PasswordHasher<User>();
        newUser.Password = hasher.HashPassword(newUser, newUser.Password);

        _context.Users.Add(newUser);
        _context.SaveChanges();

        HttpContext.Session.SetInt32("uid", newUser.UserId);
        HttpContext.Session.SetString("Name", newUser.FullName());

        return RedirectToAction("Dashboard", "Wedding");
    }

    [HttpPost("/login")]
    public IActionResult Login(LoginUser loginUser)
    {
        if(ModelState.IsValid == false)
        {
            return Index();
        }

        User? dbUser = _context.Users.FirstOrDefault(user => user.Email == loginUser.LoginEmail);

        if(dbUser == null)
        {
            ModelState.AddModelError("LoginEmail", "Invalid entry!");
            return Index();
        }

        PasswordHasher<LoginUser> hasher = new PasswordHasher<LoginUser>();
        PasswordVerificationResult result = hasher.VerifyHashedPassword(loginUser, dbUser.Password, loginUser.LoginPassword);

        if(result == 0)
        {
            ModelState.AddModelError("LoginPassword", "Invalid entry!");
            return Index();
        }

        // no returns, therefore no errors
        HttpContext.Session.SetInt32("uid", dbUser.UserId);
        HttpContext.Session.SetString("Name", dbUser.FullName());
        return RedirectToAction("Dashboard", "Wedding");
    }

    [HttpPost("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }
}