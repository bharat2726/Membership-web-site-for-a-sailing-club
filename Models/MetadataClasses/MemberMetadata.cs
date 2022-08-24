using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sail.Models
{
    [ModelMetadataType(typeof(MemberMetadata))]
    public partial class Member : IValidatableObject
    {  
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            SailContext _context = new SailContext();
            int flag = 0;
            //Names 
            if(FirstName == null)
            {
                yield return new ValidationResult("First Name is a required field. ", new[] { nameof(FirstName) });
            }
            if (LastName == null)
            {
                yield return new ValidationResult("Last Name is a required field. ", new[] { nameof(LastName) });
            }

            //Province Code Validation
            if (ProvinceCode != null)
            {
                var SavedRecord = _context.Province.Where(j => j.ProvinceCode.Equals(ProvinceCode.ToUpper()) && ProvinceCode.Length == 2);

                if(SavedRecord.Any() == false)
                {
                    yield return new ValidationResult("Province Code not found in the file! Note: Province Code should be of length 2 (XX).", new[] { nameof(ProvinceCode) });
                }
                ProvinceCode = ProvinceCode.ToUpper();
                flag = 1;
            }
            else
            {
                yield return new ValidationResult("Province Code is required field!", new[] { nameof(ProvinceCode) });
            }

            //Postal Code Validation
            string validationString = "[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]";
            string validationString1 = "[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ]?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]";
            if (flag == 1)
            {
                var PostalCodeTemp = PostalCode.ToUpper();
                int lengthPostalCode = PostalCode.Length;
                if (!Regex.IsMatch(PostalCodeTemp, validationString) && !Regex.IsMatch(PostalCodeTemp, validationString1))
                {
                    yield return new ValidationResult("Invalid Postal Code!", new[] { nameof(PostalCode) });
                }
                else if(Regex.IsMatch(PostalCodeTemp, validationString1))
                {
                    PostalCodeTemp = PostalCodeTemp.Insert(3, " ");
                    PostalCode = PostalCodeTemp;
                }
            }

            //Home Phone validation
            if (HomePhone != null)
            {
                string phoneString = "[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]";
                string phoneString1 = "[0-9][0-9][0-9][-][0-9][0-9][0-9][0-9][0-9][0-9][0-9]";
                string phoneString2 = "[0-9][0-9][0-9][0-9][0-9][0-9][-][0-9][0-9][0-9][0-9]";
                string phoneString3 = "[0-9][0-9][0-9][-][0-9][0-9][0-9][-][0-9][0-9][0-9][0-9]";

                if (!Regex.IsMatch(HomePhone, phoneString) && !Regex.IsMatch(HomePhone, phoneString1) && !Regex.IsMatch(HomePhone, phoneString2) && !Regex.IsMatch(HomePhone, phoneString3))
                    yield return new ValidationResult("Home Phone, if provided, must be 10 digits: 123 - 123 - 1234 or 1231231234.", new[] { nameof(HomePhone) });
                else
                {
                    if(Regex.IsMatch(HomePhone, phoneString))
                    {
                        HomePhone = HomePhone.Insert(3, "-");
                        HomePhone = HomePhone.Insert(7, "-");
                    }
                    else if (Regex.IsMatch(HomePhone, phoneString1))
                    {
                        HomePhone = HomePhone.Insert(7, "-");
                    }
                    else if (Regex.IsMatch(HomePhone, phoneString2))
                    {
                        HomePhone = HomePhone.Insert(3, "-");
                    }
                }
            }

            //full name validation
            if(SpouseFirstName == null && SpouseLastName == null)
            {
                FullName = LastName + " & " + FirstName;
            }
            else if(SpouseLastName == null || SpouseLastName == LastName && SpouseFirstName!=null)
            {
                FullName = LastName+", "+FirstName+" & "+SpouseFirstName;
            }
            else if(SpouseLastName != null || SpouseLastName != LastName && SpouseFirstName != null)
            {
                FullName = LastName + ", " + FirstName + " & " + SpouseLastName + ", " + SpouseFirstName;
            }

            if(UseCanadaPost == true)
            {
                if(Street == null)
                    yield return new ValidationResult("Street is a required field. ", new[] { nameof(Street) });
                if(City == null)
                    yield return new ValidationResult("City is a required field. ", new[] { nameof(City) });
            }
            else if(UseCanadaPost == false)
            {
                if (Email == null)
                    yield return new ValidationResult("Email is a required field. ", new[] { nameof(Email) });
            }
            //throw new System.NotImplementedException();
        }
    }

    public class MemberMetadata
    {
        public MemberMetadata()
        {
            Boat = new HashSet<Boat>();
            MemberTask = new HashSet<MemberTask>();
            Membership = new HashSet<Membership>();
        }

        public int MemberId { get; set; }
        
        public string FullName { get; set; }
        
        public string FirstName { get; set; }
       
        public string LastName { get; set; }
        public string SpouseFirstName { get; set; }
        public string SpouseLastName { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        
        public string ProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string HomePhone { get; set; }
        public string Email { get; set; }
        public int? YearJoined { get; set; }
        public string Comment { get; set; }
        public bool TaskExempt { get; set; }
        public bool UseCanadaPost { get; set; }

        public virtual Province ProvinceCodeNavigation { get; set; }
        public virtual ICollection<Boat> Boat { get; set; }
        public virtual ICollection<MemberTask> MemberTask { get; set; }
        public virtual ICollection<Membership> Membership { get; set; }
    }
}
