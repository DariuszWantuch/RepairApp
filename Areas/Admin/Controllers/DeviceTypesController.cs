using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RepairApp.Data;
using RepairApp.Data.Repository.IRepository;
using RepairApp.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DeviceTypesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public RepairViewModel RepairViewModel { get; set; }

        public DeviceTypesController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Manage(string? id)
        {
            RepairViewModel = new RepairViewModel()
            {
                DeviceType = new Models.DeviceType()
            };

            if (id != null)
            {
                RepairViewModel.DeviceType = _unitOfWork.DeviceType.Get(id);
            }

            return View(RepairViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage()
        {
            if (ModelState.IsValid)
            {
                if (RepairViewModel.DeviceType.Id == null)
                {
                    _unitOfWork.DeviceType.Add(RepairViewModel.DeviceType);
                }
                else
                {
                    var mark = _unitOfWork.DeviceType.Get(RepairViewModel.DeviceType.Id);
                    _unitOfWork.DeviceType.Update(RepairViewModel.DeviceType);
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {              
                return View(RepairViewModel);
            }
        }


        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.DeviceType.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var deviceType = _unitOfWork.DeviceType.Get(id);
            var repairWithMark = _unitOfWork.Repair.GetFirstOrDefault(filter: x => x.DeviceType.DeviceName == deviceType.DeviceName);

            if (deviceType == null)
            {
                return Json(new { success = false, message = "Błąd podczas usuwania!" });
            }
            if (repairWithMark == null)
            {
                _unitOfWork.DeviceType.Remove(deviceType);
                _unitOfWork.Save();
            }
            else
            {
                return Json(new { success = false, message = "Istnieje naprawa z tym typem urządzenia! Nie ma możliwośći usunięcia." });
            }

            return Json(new { success = true, message = "Usunięcie przebiegło pomyślnie." });
        }
    }
}
