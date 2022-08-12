using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers;

public class WeddingController : Controller
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

    public WeddingController(WeddingPlannerContext context)
    {
        _context = context;
    }

    [HttpGet("/dashboard")]
    public IActionResult Dashboard()
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        List<Wedding> AllWeddings = _context.Weddings
            .Include(w => w.Creator)
            .Include(w => w.Connections)
            .ToList();

        return View("Dashboard", AllWeddings);
    }

    [HttpGet("/wedding/plan")]
    public IActionResult ShowWeddingPlan()
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }
        return View("ShowWeddingPlan");
    }

    [HttpPost("/wedding/create")]
    public IActionResult CreateWedding(Wedding newWedding)
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        if(ModelState.IsValid == false)
        {
            return ShowWeddingPlan();
        }

        if(uid != null)
        {
            newWedding.UserId = (int)uid;
        }

        _context.Weddings.Add(newWedding);
        _context.SaveChanges();

        return RedirectToAction("Wedding");
    }

    [HttpGet("/wedding/{weddingId}")]
    public IActionResult Wedding(int weddingId)
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? wedding = _context.Weddings
            .Include(w => w.Creator)
            .Include(w => w.Connections)
            .ThenInclude(conn => conn.User)
            .FirstOrDefault(w => w.WeddingId == weddingId);

        if(wedding == null)
        {
            return RedirectToAction("Dashboard");
        }

        return View("Wedding", wedding);
    }

    [HttpPost("/wedding/{deletedWeddingId}")]
    public IActionResult Delete(int deletedWeddingId)
    {
        if(!loggedIn)
        {
            return RedirectToAction("Index", "User");
        }

        Wedding? weddingToDelete = _context.Weddings.FirstOrDefault(w => w.WeddingId == deletedWeddingId);

        if(weddingToDelete != null)
        {
            if(weddingToDelete.UserId == uid)
            {
                _context.Weddings.Remove(weddingToDelete);
                _context.SaveChanges();
            }
        }

        return RedirectToAction("Dashboard");
    }

    [HttpPost("/wedding/{weddingId}/rsvp")]
    public IActionResult Rsvp(int weddingId)
    {
        if(!loggedIn || uid == null)
        {
            return RedirectToAction("Index", "User");
        }

        // Comparing the userId of the RSVP to the uid in session
        Connection? existingRsvp = _context.Connections.FirstOrDefault(conn => conn.WeddingId == weddingId && conn.UserId == uid);

        if(existingRsvp == null)
        {
            Connection newRsvp = new Connection(){
                WeddingId = weddingId,
                UserId = (int)uid
            };
            _context.Connections.Add(newRsvp);
        }
        else
        {
            _context.Remove(existingRsvp);
        }

        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
