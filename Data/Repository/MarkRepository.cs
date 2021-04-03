using Microsoft.AspNetCore.Mvc.Rendering;
using RepairApp.Data.Repository.IRepository;
using RepairApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Data.Repository
{
    public class MarkRepository : Repository<Mark>, IMarkRepository
    {
        private readonly ApplicationDbContext _db;

        public MarkRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public IEnumerable<SelectListItem> GetMarkListFromDropDown()
        {
            return _db.Mark.Select(i => new SelectListItem()
            {
                Text = i.MarkName,
                Value = i.Id.ToString()
            });
        }

        public bool IsMarkExist(string name)
        {
            return _db.Mark.Any(
                x => x.MarkName == name);
        }

        public void Update(Mark mark)
        {
            var obj = _db.Mark.FirstOrDefault(x => x.Id == mark.Id);

            obj.MarkName = mark.MarkName;

            _db.SaveChanges();
        }
    }
}
