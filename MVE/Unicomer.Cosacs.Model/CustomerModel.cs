using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Unicomer.Cosacs.Model
{
    //New class for customer MVE
    public class Contact
    {
        public string ContactLocation { get; set; }
        public string ContactNumber { get; set; }
        public string Ext { get; set; }
    }

    public class Address
    {
        public string AddressType { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Postalcode { get; set; }
        public string Email { get; set; }
        public string DeliveryArea { get; set; }
        public string DateIn { get; set; }
        public string Notes { get; set; }
    }

    public class UserJson
    {
        public string ResourceType { get; set; }
        public string Source { get; set; }
        public string CustomerID { get; set; }
        public string IdType { get; set; }
        public string ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public long Mobile { get; set; }
        public string DOB { get; set; }
        public string Title { get; set; }
        public int BranchNo { get; set; }
        public List<Contact> Contact { get; set; }
        public List<Address> Address { get; set; }
    }
    //New class for customer MVE

    public class User
    {
        public string CustId { get; set; }
        public string extUId { get; set; } //unipayId created @ YP end
        [Required]
        //[MaxLength(4)]
        public string idType { get; set; }
        [Required]
        //[MaxLength(30)]
        public string id { get; set; }
        [Required]
        //[MaxLength(30)]
        public string firstName { get; set; }
        [Required]
        //[MaxLength(60)]
        public string lastName { get; set; }
        public string middleName { get; set; }
        //[Required]
        //[MaxLength(20)]
        //[RegularExpression(@"^[0-9\-]*$", ErrorMessage = "Please provide correct phone number.")]
        public string phoneNumber { get; set; }
        //[Required]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$", ErrorMessage = "Please provide correct email id.")]
        [DataType(DataType.EmailAddress)]
        //[MaxLength(60)]
        public string email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string DOB { get; set; }
        [Required]
        [MaxLength(25)]
        public string Title { get; set; }
        [Required]
        [MaxLength(2)]
        public string AddressType { get; set; }
        [Required]
        [MaxLength(50)]
        public string Address { get; set; }
        [Required]
        [MaxLength(8)]
        public string DeliveryArea { get; set; }
        [Required]
        public short? BranchNoHdle { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public string DateIn { get; set; }
        [Required]
        [MaxLength(60)]
        public string Notes { get; set; }
        private bool _NewRecord;
        public bool NewRecord //{ get; set; } = true;
        {
            get { return this._NewRecord; }
            set { this._NewRecord = true; }
        }

    }
    public class ValidatetUser
    {
        /// <summary>
        /// Identity type
        /// </summary>
        public string IdType { get; set; }

        /// <summary>
        /// Identity Number
        /// </summary>
        [Required]
        public string IdNumber { get; set; }

        /// <summary>
        /// Phone Number
        /// </summary>
        //[Required]
        //[RegularExpression(@"^\(?\d{3}\)?-? *\d{3}-? *-?\d{4}$", ErrorMessage = "Please provide correct phone number.")]
        public string PhNumber { get; set; }
    }
    public class UpdateUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string YPUserID { get; set; }
        public string CustID { get; set; }
        private bool _NewRecord;
        public bool NewRecord //{ get; set; } = true;
        {
            get { return this._NewRecord; }
            set { this._NewRecord = false; }
        }
    }
    public class ValidateCustomerResult
    {
        public ValidateUserResult UserResult { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
    }
    public class ValidateUserResult
    {
        public string idType { get; set; }
        public string id { get; set; }
        public string extUId { get; set; }
        public string firstName { get; set; }
        public string middleName { get { return null; } }
        public string lastName { get; set; }
        //public Nullable<System.DateTime> DateOfBirth { get; set; }
        public string email { get; set; }
    }

    public class GetAuthQAndA
    {
        public List<questionsAndAnswers> questionsAndAnswers { get; set; }
        //public List<ListBranch> listBranch { get; set; }
        public int numCorrectRequired { get; set; }
    }
    public class ListBranch
    {
        public int BranchNo { get; set; }
        public string BranchName { get; set; }
    }
    public class questionsAndAnswers
    {
        public int qId { get; set; }
        public string question { get; set; }
        public List<string> answers { get; set; }
        public string inputType { get; set; }
        public string inputCategory { get; set; }
    }
}
