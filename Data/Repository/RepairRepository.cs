using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository
{
    public class RepairRepository : Repository<Repair>, IRepairRepository
    {
        private readonly ApplicationDbContext _db;

        public RepairRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void ChangeRepairStatus(string repairId, string statusId)
        {
            var obj = _db.Repair.FirstOrDefault(s => s.Id == repairId);
            obj.StatusId = statusId;
            _db.SaveChanges();
        }    

        public int RepairID()
        {
            var repairID = _db.Repair.Count() + 1;

            return repairID;
        }

        public void Update(Repair repair)
        {
            var obj = _db.Repair.FirstOrDefault(s => s.Id == repair.Id);

            obj.Describe = repair.Describe;
            obj.DeviceModel = repair.DeviceModel;
            obj.Warranty = repair.Warranty;
            obj.MarkId = repair.MarkId;
            obj.PickupDate = repair.PickupDate;

            _db.SaveChanges();
        }
    }
}
