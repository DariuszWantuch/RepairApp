using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using RepairApp.Data;
using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using RepairApp.Models.ViewModels;
using RepairApp.Services;
using RepairApp.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace RepairApp.Areas.Customer.Controllers
{
    [Authorize]
    [Area("Customer")]
    public class RepairController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment; 
        private readonly IEmailSender _emailSender;
      

        [BindProperty]
        public RepairViewModel RepairViewModel { get; set; }

        public RepairController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
            _emailSender = emailSender;
           

        }

        public IActionResult Index()
        {           
            return View();
        }

        public IActionResult Create()
        {
            DateTime now = DateTime.Now;

            RepairViewModel = new RepairViewModel()
            {
                Repair = new Models.Repair(),
                Address = new Models.Address(),
                RepairCost = new Models.RepairCost(),
                MarkList = _unitOfWork.Mark.GetMarkListFromDropDown(),
                DeviceTypeList = _unitOfWork.DeviceType.GetDeviceTypeListFromDropDown()
            };
               
            RepairViewModel.Repair.PickupDate = now;

            return View(RepairViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string? id)
        {        
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var currentUserEmail = User.FindFirst(ClaimTypes.Email).Value;
            DateTime now = DateTime.Now;
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                         
                if (RepairViewModel.Repair.Id == null)
                {
                    if(files.Count > 0)
                    {
                        var size = files[0].Length;
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(webRootPath, @"img\warranty");
                        var extension = Path.GetExtension(files[0].FileName);

                        if ((extension == ".pdf" || extension == ".jpg" || extension == ".png" || extension == ".jpeg") && size <= 2097152)
                        {
                            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                            {
                                files[0].CopyTo(fileStreams);
                            }
                            RepairViewModel.Repair.Warranty = @"\img\warranty\" + fileName + extension;
                        }                     
                        else
                        {
                            ModelState.AddModelError("Repair.Warranty", "Nieprawidłowe rozszerzenie pliku lub jego rozmiar jest zbyt duży! Prawidłowe rozszerzenia: pdf, jpg, jpeg lub png. Waga pliku maksymalnie 2MB.");
                            RepairViewModel.MarkList = _unitOfWork.Mark.GetMarkListFromDropDown();
                            RepairViewModel.DeviceTypeList = _unitOfWork.DeviceType.GetDeviceTypeListFromDropDown();
                            return View(RepairViewModel);
                        }                    
                    }
                    
                    RepairViewModel.RepairCost.Id = _unitOfWork.RepairCost.GenerateId();
                    RepairViewModel.Address.Id = _unitOfWork.Address.GenerateId();
                    RepairViewModel.Repair.RepairCostId = RepairViewModel.RepairCost.Id;
                    RepairViewModel.Repair.AddressId = RepairViewModel.Address.Id;
                    RepairViewModel.Repair.StatusId = _unitOfWork.Status.GetStatusIdByName(StatusSD.Start);
                    RepairViewModel.Repair.UserId = currentUserID;
                    RepairViewModel.Repair.ReportDate = now;

                    RepairViewModel.Repair.RepairId = _unitOfWork.Repair.RepairID();

                    _unitOfWork.RepairCost.Add(RepairViewModel.RepairCost);
                    _unitOfWork.Address.Add(RepairViewModel.Address);
                    _unitOfWork.Repair.Add(RepairViewModel.Repair);
                    
                }

                await _emailSender.SendEmailAsync(currentUserEmail, "Zgłoszono naprawę",
                         $"Naprawa została poprawnie zgłoszona. <br>Nr zgłoszenia: {RepairViewModel.Repair.RepairId}.");

                await _emailSender.SendEmailAsync("repair@int.pl", "Nowa naprawa",
                         $"Została zgłoszona nowa naprawa. <br>Nr zgłoszenia: {RepairViewModel.Repair.RepairId}.");
                try
                {
                    var send = new SmsSender();
                    send.SendSms(RepairViewModel.Repair.Address.PhoneNumber, "Naprawa została zgłoszona poprawnie.");
                }
                catch (Exception e)
                {
                    RepairViewModel.MarkList = _unitOfWork.Mark.GetMarkListFromDropDown();
                    RepairViewModel.DeviceTypeList = _unitOfWork.DeviceType.GetDeviceTypeListFromDropDown();
                    ModelState.AddModelError("Address.PhoneNumber", "Nie ma takiego numeru!");
                    return View(RepairViewModel);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                RepairViewModel.MarkList = _unitOfWork.Mark.GetMarkListFromDropDown();
                RepairViewModel.DeviceTypeList = _unitOfWork.DeviceType.GetDeviceTypeListFromDropDown();
                return View(RepairViewModel);
            }
        }

        public IActionResult Details(string id)
        {
            RepairViewModel repair = new RepairViewModel()
            {
                Repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "Status,Mark,RepairCost,Address,DeviceType")
            };         
            return View(repair);        
        }

        public IActionResult RepairCost(string id)
        {
            RepairViewModel repair = new RepairViewModel()
            {
                Repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "RepairCost")
            };
            return View(repair);
        }

        public async Task<IActionResult> RepairCostDecision(string id, string decision)
        {
            var repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "RepairCost,IdentityUser");

            if (decision == "Accept")
            {
                repair.RepairCost.IsAccepted = true;
                repair.StatusId = _unitOfWork.Status.GetStatusIdByName(StatusSD.Accepted);

                await _emailSender.SendEmailAsync(repair.IdentityUser.Email, "Naprawa urządzenia",
                        $"Koszt naprawy został ackeptowany. Zabieramy się za naprawę urządzenia!<br>Nr zgłoszenia: {repair.RepairId}. <br> Koszt naprawy: {repair.RepairCost.Cost}");

                await _emailSender.SendEmailAsync("repair@int.pl", "Akceptowano koszt naprawy",
                         $"Koszt naprawy został akceptowany przez kleinta. <br>Nr zgłoszenia: {repair.RepairId}.");
            }
            else
            {
                repair.RepairCost.IsRejected = true;
                repair.StatusId = _unitOfWork.Status.GetStatusIdByName(StatusSD.Rejected);

                await _emailSender.SendEmailAsync(repair.IdentityUser.Email, "Odrzucenie kosztów naprawy",
                        $"Koszt naprawy został odrzucony. W kolejnym dniu roboczym zostanie nadana przesyłka zwrotna. Przykro nam, że urządznie nie zostało naprawione. Mamy nadzieję, że jescze do nas wrócisz :)<br>Nr zgłoszenia: {repair.RepairId}.");

                await _emailSender.SendEmailAsync("repair@int.pl", "Odrzucono koszt naprawy naprawy",
                         $"Koszt naprawy został odrzucony przez klienta. <br>Nr zgłoszenia: {repair.RepairId}.");
            }

            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetAll()
        {
            var currentUserID = User.FindFirst(ClaimTypes.NameIdentifier).Value;
       
            return Json(new { data = _unitOfWork.Repair.GetAll(filter: x => x.UserId == currentUserID, includeProperties: "Status,DeviceType") });
        }
        
        public async Task<IActionResult> Delete(string id)
        {
            var repair = _unitOfWork.Repair.GetFirstOrDefault(x => x.Id == id, includeProperties: "IdentityUser");

            await _emailSender.SendEmailAsync(repair.IdentityUser.Email, "Anulowanie naprawy",
                     $"Naprawa została anulowana. Mamy nadzieję, że jescze do nas wrócisz :)<br>Nr zgłoszenia: {repair.RepairId}.");

            await _emailSender.SendEmailAsync("repair@int.pl", "Anulowano naprawę",
                     $"Klient anulował naprawę. <br>Nr zgłoszenia: {repair.RepairId}.");

            _unitOfWork.Repair.ChangeRepairStatus(id, _unitOfWork.Status.GetStatusIdByName(StatusSD.Cancel));
            _unitOfWork.Save();

            return View(repair);
        }
    }
}
