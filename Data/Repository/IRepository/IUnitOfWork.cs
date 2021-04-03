using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository.IRepository
{
    public interface IUnitOfWork : IDisposable
    {    
        IMarkRepository Mark { get; }
        IRepairRepository Repair { get; }
        IStatusRepository Status { get; }
        IAddressRepository Address { get; }
        IRepairCostRepository RepairCost { get; }
        IDeviceTypeRepository DeviceType { get; }

        void Save();
    }
}
