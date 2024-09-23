using DentalClinic.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace DentalClinic.DTO
{
    public class UpdateDoctorDto
    {
        private string _phoneNumber;
        public string PhoneNumber
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
    }

}
