using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RepairApp.Models
{
    public class Address
    {
        [Key]       
        public string Id { get; set; }

        [Required(ErrorMessage = "Imię jest wymagane")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Numer telefonu jest wymagany")]
        [DataType(DataType.PhoneNumber)]
        public int PhoneNumber { get; set; }

        [Required(ErrorMessage = "Ulica jest wymagana")]
        [DataType(DataType.Text)]
        public string Street { get; set; }

        [Required(ErrorMessage = "Miasto jest wymagane")]
        [DataType(DataType.Text)]
        public string City { get; set; }    

        [Required(ErrorMessage = "Kod pocztowy jest wymagany")]
        [DataType(DataType.PostalCode)]
        public int PostalCode { get; set; }

        public virtual ICollection<Repair> Repairs { get; set; }
    }
}
