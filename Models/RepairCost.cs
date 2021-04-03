using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Models
{
    public class RepairCost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [DataType(DataType.MultilineText)]
        public string FaultDescription { get; set; }

        [DataType(DataType.Currency)]
        public float Cost { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsRejected { get; set; }
    }
}
