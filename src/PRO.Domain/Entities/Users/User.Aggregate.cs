using PRO.Domain.Base;
using PRO.Domain.Entities.Departments;
using PRO.Domain.Entities.Users.Events;
using System;
using System.Linq;

namespace PRO.Domain.Entities.Users
{
    public partial class User
    {

        public User(string userName
            , string firstName
            , string lastName
            , string address
            , DateTime? birthDate
            , short departmentId)
        {
            UserName = userName;

            this.Update(
                firstName
                , lastName
                , address
                , birthDate
                , departmentId
            );
        }

        public void Update(string firstName
            , string lastName
            , string address
            , DateTime? birthDate
            , short departmentId)
        {
            FirstName = firstName;
            LastName = lastName;
            Address = address;
            BirthDate = birthDate;
            DepartmentId = departmentId;
        }

        public User RegisterNewUser(User user){
            var addEvent = new OnRegisteredEvent()
            {
                User = user
            };
            AddEvent(addEvent);
            return user;
        }

        public void AddDepartment(short departmentId)
        {
            DepartmentId = departmentId;
        }

        public Payslip AddPayslip(DateTime date
            , float workingDays
            , decimal bonus
            , bool isPaid
            )
        {
            // Make sure there's only one payslip  per month
            var exist = PaySlips.Any(_ => _.Date.Month == date.Month && _.Date.Year == date.Year);
            if (exist)
                throw new Exception("Payslip for this month already exist.");

            var payslip = new Payslip(this.Id, date, workingDays, bonus);
            if (isPaid)
            {
                payslip.Pay(this.CoefficientsSalary);
            }

            PaySlips.Add(payslip);

            var addEvent = new OnPayslipAddedEvent()
            {
                Payslip = payslip
            };

            AddEvent(addEvent);

            return payslip;
        }
    }
}