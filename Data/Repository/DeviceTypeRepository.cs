using Microsoft.AspNetCore.Mvc.Rendering;
using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository
{
    public class DeviceTypeRepository : Repository<DeviceType>, IDeviceTypeRepository
    {
        private readonly ApplicationDbContext _db;

        public DeviceTypeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public IEnumerable<SelectListItem> GetDeviceTypeListFromDropDown()
        {
            return _db.DeviceType.Select(i => new SelectListItem()
            {
                Text = i.DeviceName +" | Koszt transportu: " +i.TransportCost + " zł",
                Value = i.Id.ToString()
            });
        }

        public void Update(DeviceType deviceType)
        {
            var obj = _db.DeviceType.FirstOrDefault(x => x.Id == deviceType.Id);

            obj.DeviceName = deviceType.DeviceName;
            obj.TransportCost = deviceType.TransportCost;

            _db.SaveChanges();
        }
    }
}
