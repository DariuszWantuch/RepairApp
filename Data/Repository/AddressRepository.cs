using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        private readonly ApplicationDbContext _db;

        public AddressRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
