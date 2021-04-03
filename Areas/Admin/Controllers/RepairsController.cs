using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RepairApp.Data;
using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using RepairApp.Models.ViewModels;
using RepairApp.Services;
using RepairApp.Utility;

namespace RepairApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class RepairsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IEmailSender _emailSender;

        [BindProperty]
        public RepairViewModel RepairViewModel { get; set; }

        public RepairsController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext context, IEmailSender emailSender)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
        }

        // GET: Admin/Repairs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Repair.Include(r => r.Address).Include(r => r.DeviceType).Include(r => r.IdentityUser).Include(r => r.Mark).Include(r => r.RepairCost).Include(r => r.Status);
            return View(await applicationDbContext.ToListAsync());
        }

        public IActionResult Details(string id)
        {
            RepairViewModel repair = new RepairViewModel()
            {
                Repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "Status,Mark,RepairCost,Address,DeviceType")
            };
            return View(repair);
        }    
          
        public async Task<IActionResult> UpdateStatus(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repair = await _context.Repair.FindAsync(id);
            if (repair == null)
            {
                return NotFound();
            }
            ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id", repair.AddressId);
            ViewData["DeviceTypeId"] = new SelectList(_context.DeviceType, "Id", "Id", repair.DeviceTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", repair.UserId);
            ViewData["MarkId"] = new SelectList(_context.Mark, "Id", "Id", repair.MarkId);
            ViewData["RepairCostId"] = new SelectList(_context.RepairCost, "Id", "Id", repair.RepairCostId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "RepairStatus", repair.StatusId);
            return View(repair);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(string id, [Bind("Id,RepairId,ReportDate,PickupDate,DeviceModel,Describe,Warranty,Tracking,StatusId,RepairCostId,AddressId,DeviceTypeId,MarkId,UserId")] Repair repair)
        {
            if (id != repair.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(repair);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepairExists(repair.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AddressId"] = new SelectList(_context.Address, "Id", "Id", repair.AddressId);
            ViewData["DeviceTypeId"] = new SelectList(_context.DeviceType, "Id", "Id", repair.DeviceTypeId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", repair.UserId);
            ViewData["MarkId"] = new SelectList(_context.Mark, "Id", "Id", repair.MarkId);
            ViewData["RepairCostId"] = new SelectList(_context.RepairCost, "Id", "Id", repair.RepairCostId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "RepairStatus", repair.StatusId);
            return View(repair);
        }

        public IActionResult RepairCost(string id)
        {
            var repairCostId = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id).RepairCostId;

            RepairViewModel repair = new RepairViewModel()
            {
                Repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id),
                RepairCost = _unitOfWork.RepairCost.GetFirstOrDefault(x => x.Id == repairCostId)
            };

            return View(repair);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RepairCost(string id, [Bind("Id,FaultDescription,Cost,IsAccepted,IsRejected")] RepairCost repairCost)
        {       
            if (ModelState.IsValid)
            {
                try
                {
                    var repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "IdentityUser,Address");
                    repair.StatusId = _unitOfWork.Status.GetStatusIdByName(StatusSD.Valuation);                   
                    _context.Update(repair);
                    _context.Update(repairCost);

                    await _emailSender.SendEmailAsync(repair.IdentityUser.Email, "Wprowadzono koszt naprawy",
                     $"Koszt naprawy oraz opis usterki został wprowadzony. Zaloguj się oraz akceptuj lub odrzuć koszt naprawy.<br>Nr zgłoszenia: {repair.RepairId}.");

                    var send = new SmsSender();
                    send.SendSms(repair.Address.PhoneNumber, "Koszt naprawy oraz opis usterki został wprowadzony. Zaloguj się oraz akceptuj lub odrzuć koszt naprawy. Nr zgłoszenia: " + repair.RepairId);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepairCostExists(repairCost.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(repairCost);
        }

        public IActionResult AddTracking(string id)
        {
            var repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id);  

            return View(repair);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTracking(string id, [Bind("Tracking")] Repair repairT)
        {       
                try
                {
                    var  repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "IdentityUser,DeviceType,Address");
                    repair.Tracking = repairT.Tracking;
                    repair.StatusId = _unitOfWork.Status.GetStatusIdByName(StatusSD.SentCourier);
                    _context.Update(repair);
                    await _emailSender.SendEmailAsync(repair.IdentityUser.Email, "Sprzęt został wysłany",
                    $"{repair.DeviceType.DeviceName} został/a spakowana i wyruszyła w drogę powrotną." +
                    $" Dziękujemy za skorzystanie z naszych usług. Mamy nadzieję, że jeszcze do nas wrócisz :)." +
                    $" <br> Link do śledznienia: <a href='https://tracktrace.dpd.com.pl/parcelDetails?typ=1&p1={repair.Tracking}+&p2=&p3=&p4=&p5=&p6=&p7=&p8=&p9=&p10='>Śledzenie</a>" +
                    $" <br>Nr zgłoszenia: {repair.RepairId}.");

                    var send = new SmsSender();
                    send.SendSms(repair.Address.PhoneNumber, "Został wprowadzony numer śledzenia. Twój sprzęt wkrótce wyruszy w drogę! Nr zgłoszenia: " + repair.RepairId + ". Link do śledzenia: https://tracktrace.dpd.com.pl/parcelDetails?typ=1&p1=" + repair.Tracking + "+&p2=&p3=&p4=&p5=&p6=&p7=&p8=&p9=&p10=");
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepairCostExists(repairT.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));       
        }

        private bool RepairCostExists(string id)
        {
            return _context.RepairCost.Any(e => e.Id == id);
        }

        private bool RepairExists(string id)
        {
            return _context.Repair.Any(e => e.Id == id);
        }

        public IActionResult GetAll()
        {
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            return Json(new { data = _unitOfWork.Repair.GetAll(includeProperties: "Status,DeviceType") });
        }
    }
}
