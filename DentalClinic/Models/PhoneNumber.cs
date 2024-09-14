using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;




namespace DentalClinic.Models
{
    [PrimaryKey(nameof(Phonenumber), nameof(BranchID))]

    public class PhoneNumber
    {
       
        private string _phoneNumber;

        [Column(TypeName = "nchar(11)")]
        [Required(ErrorMessage = "من فضلك ادخل رقم الهاتف")]
        [RegularExpression(@"^(?:\+20|0)?1[0125]\d{8}$", ErrorMessage = "عليك إدخال رقم هاتف مصري مكون من 11 رقم ")]
        public string Phonenumber
        {
            get { return _phoneNumber; }
            set
            {
                // Validation logic
                if (!Regex.IsMatch(value, @"^(?:\+20|0)?1[0125]\d{8}$"))
                {
                    throw new ArgumentException("عليك إدخال رقم هاتف مصري مكون من 11 رقم");
                }
                _phoneNumber = value;
            }
        }
        [Key,ForeignKey("Branch")]
        public int BranchID { get; set; }
        public virtual Branch Branch { get; set; }
    }


}
