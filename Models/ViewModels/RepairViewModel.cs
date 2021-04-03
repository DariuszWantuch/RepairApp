using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Models.ViewModels
{
    public class RepairViewModel
    {
        public Repair Repair { get; set; }
        public Address Address { get; set; }
        public Mark Mark { get; set; }
        public RepairCost RepairCost { get; set; }
        public Status Status { get; set; }
        public DeviceType DeviceType { get; set;}

        public IEnumerable<SelectListItem> DeviceTypeList { get; set; }
        public IEnumerable<SelectListItem> MarkList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
    }
}
