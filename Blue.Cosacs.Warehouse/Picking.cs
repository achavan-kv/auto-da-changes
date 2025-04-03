using System.ComponentModel.DataAnnotations.Schema;
using Blue.Cosacs.Warehouse.Utils;

namespace Blue.Cosacs.Warehouse
{
    partial class Picking
    {
        private string GetEmployeeName(int? employeeCode)
        {
            if (employeeCode == null)
                return null;

            var user = Admin.UserRepository.GetUserById(employeeCode.Value);

            if (user == null)
                return null;

            return user.FullName;
            //var emp = new EmployeeListProvider();

            //var value = emp.Load().Cast<PickListRow>()
            //    .FirstOrDefault(p => p.k == employeeCode.ToString());

            //if (value != null)
            //    return value.v;

            //return null;
        }

        string pickedByName = null;
        [NotMapped]
        public string PickedByName
        {
            get
            {
                if (pickedByName == null)
                {
                    this.pickedByName = GetEmployeeName(this.PickedBy);
                }

                return this.pickedByName;
            }
            set
            {
                this.pickedByName = value;
            }
        }

        string checkedByName = null;
        [NotMapped]
        public string CheckedByName
        {
            get
            {
                if (checkedByName == null)
                    this.checkedByName = GetEmployeeName(this.CheckedBy);

                return this.checkedByName;
            }
            set
            {
                this.checkedByName = value;
            }
        }

        string confirmedByName = null;
        [NotMapped]
        public string ConfirmedByName
        {
            get
            {
                if (confirmedByName == null)
                    this.confirmedByName = GetEmployeeName(this.ConfirmedBy);

                return this.confirmedByName;
            }
            set
            {
                this.confirmedByName = value;
            }
        }

        public void AutoPick(IClock clock)
        {
            PickedBy = ConfirmedBy = CheckedBy = AutoUser.AutoPickConfirm.Id;
            ConfirmedOn = PickedOn = clock.UtcNow;          // #13985
            Comment = AutoUser.AutoPickConfirm.Comment;
        }
    }
}
