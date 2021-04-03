using RepairApp.Data.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;

        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;          
            Mark = new MarkRepository(_db);
            Repair = new RepairRepository(_db);
            Status = new StatusRepository(_db);
            Address = new AddressRepository(_db);
            RepairCost = new RepairCostRepository(_db);
            DeviceType = new DeviceTypeRepository(_db);
           
        }
        
        public IMarkRepository Mark { get; private set; }
        public IRepairRepository Repair { get; private set; }
        public IStatusRepository Status { get; private set; }
        public IAddressRepository Address { get; private set; }
        public IRepairCostRepository RepairCost { get; private set; }
        public IDeviceTypeRepository DeviceType { get; private set; }

        public void Dispose()
        {
            _db.Dispose();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
