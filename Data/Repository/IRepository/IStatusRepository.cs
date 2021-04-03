using Microsoft.AspNetCore.Mvc.Rendering;
using RepairApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository.IRepository
{
    public interface IStatusRepository : IRepository<Status>
    {
        string GetStatusIdByName(string name);
        IEnumerable<SelectListItem> GetStatusListFromDropDown();
    }
}
