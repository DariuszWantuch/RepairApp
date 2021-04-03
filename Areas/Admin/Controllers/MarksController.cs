using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;

        [BindProperty]
        public RepairViewModel RepairViewModel { get; set; }

        public MarksController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment, ApplicationDbContext context)
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
                Mark = new Models.Mark()
            };

            if (id != null)
            {
                RepairViewModel.Mark = _unitOfWork.Mark.Get(id);
            }           

            return View(RepairViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Manage()
        {         
            if (ModelState.IsValid)
            {
                if (RepairViewModel.Mark.Id == null)
                {                                    
                    _unitOfWork.Mark.Add(RepairViewModel.Mark);
                }
                else
                {
                    var mark = _unitOfWork.Mark.Get(RepairViewModel.Mark.Id);
                    _unitOfWork.Mark.Update(RepairViewModel.Mark);
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
            return Json(new { data = _unitOfWork.Mark.GetAll() });
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            var mark = _unitOfWork.Mark.Get(id);
            var repairWithMark = _unitOfWork.Repair.GetFirstOrDefault(filter: x => x.Mark.MarkName == mark.MarkName);

            if (mark == null)
            {
                return Json(new { success = false, message = "Błąd podczas usuwania!" });
            }
            if (repairWithMark == null)
            {
                _unitOfWork.Mark.Remove(mark);
                _unitOfWork.Save();
            }
            else
            {
                return Json(new { success = false, message = "Istnieje naprawa z tą marką! Nie ma możliwośći usunięcia." });
            }
           
            return Json(new { success = true, message = "Usunięcie przebiegło pomyślnie." });
        }
    }
}
