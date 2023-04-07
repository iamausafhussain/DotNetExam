using ExamApiTwo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ExamApiTwo.Controllers
{
    [Route("employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private ExamContext _context;
        public EmployeeController(ExamContext context)
        {
            _context = context;
        }
        [HttpGet("getEmployeeCount")]
        public object GetEmployeeCount()
        {
            var result = from e in _context.Employees
                         join d in _context.Departments
                         on e.DepartmentId equals d.DepartmentId
                         group e by d.DepartmentName into g
                         select new
                         {
                             DepartmentName = g.Key,
                             Employee_Count = g.Count()
                         };
            return result;
        }
        /*[HttpGet("AvgEmpYears")]
        *//*Not Executable*//*
        public object GetAvgEmpYears()
        {
            var getAvgEmpYears = from e in _context.Employees
                                 join d in _context.Departments
                                 on e.DepartmentId equals d.DepartmentId
                                 group e by new { d.DepartmentName, e.HireDate } into g
                                 select new
                                 {
                                     Department_Name = g.Key.DepartmentName,
                                     Avg_Year_of_Service = (int)g.Average(e => (int)(DateTime.Today - e.HireDate).TotalDays / 365)
                                 };
            return getAvgEmpYears;
        }*/
        [HttpGet("highOrders")]
        public object GetTopEmployee()
        {
            var top_emp = (from d in _context.Departments
                           join o in _context.Orders
                           on d.DepartmentId equals o.DepartmentId
                           join e in _context.Employees
                           on d.DepartmentId equals e.DepartmentId
                           group new { d, o } by new { d.DepartmentName, o.TotalAmount } into g
                           orderby g.Key.TotalAmount descending
                           select new
                           {
                               Department_Name = g.Key.DepartmentName,
                               Total_Amount = g.Key.TotalAmount
                           }).Take(5);

            return top_emp;
        }
        [HttpGet("departmentWithAmount")]
        public object GetDepartmentWithAmount()
        {
            var deptAmt = from d in _context.Departments
                          join o in _context.Orders
                          on d.DepartmentId equals o.DepartmentId
                          group o by d.DepartmentName into g
                          select new
                          {
                              DepartmentName = g.Key,
                              Avg_Total = g.Average(o => o.TotalAmount)
                          };
            return deptAmt;
        }
        [HttpGet("employeeOrder")]
        public object GetEmployeeOrder()
        {
            var empOrd = from o in _context.Orders
                         join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                         group o by new { e.FirstName, e.LastName } into g
                         select new
                         {
                             FirstName = g.Key.FirstName,
                             LastName = g.Key.LastName,
                             TotalAmountPerEmployee = g.Sum(o => o.TotalAmount)
                         };
            return empOrd;
        }
        [HttpGet("employeeDeptOrd")]
        public object GetEmployeeDeptOrd()
        {
            var empDeptOrd = from o in _context.Orders
                             join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                             join d in _context.Departments on o.DepartmentId equals d.DepartmentId
                             group o by new { Name = e.FirstName + " " + e.LastName, d.DepartmentName } into g
                             select new
                             {
                                 Name = g.Key.Name,
                                 DepartmentName = g.Key.DepartmentName,
                                 HighestTotal = g.Sum(o => o.TotalAmount)
                             };
            return empDeptOrd;
        }
        [HttpGet("highestEmpOrd")]
        public object GetHighestEmpOrd()
        {
            var highestEmpOrd = (from ord in _context.Orders
                                 join dept in _context.Departments on ord.DepartmentId equals dept.DepartmentId
                                 group ord by dept.DepartmentName into g
                                 orderby g.Average(o => o.TotalAmount) descending
                                 select new
                                 {
                                     DepartmentName = g.Key,
                                     AvgTotal = g.Average(ord => ord.TotalAmount)
                                 }).Take(3);
            return highestEmpOrd;
        }
        [HttpGet("empDeptTa")]
        public object GetEmployeeDeptTA()
        {
            var empDeptTa = from o in _context.Orders
                            join e in _context.Employees on o.EmployeeId equals e.EmployeeId
                            join d in _context.Departments on o.DepartmentId equals d.DepartmentId
                            orderby d.DepartmentName, o.TotalAmount descending
                            group o by new { Name = e.FirstName + " " + e.LastName, d.DepartmentName, o.TotalAmount } into g
                            select new
                            {
                                Name = g.Key.Name,
                                DepartmentName = g.Key.DepartmentName,
                                TotalAmount = g.Key.TotalAmount
                            };
            return empDeptTa;
        }
        [HttpGet("getSpecDeptEmp/{dept}")]
        public IEnumerable<Employee> GetSpecificDeptEmp(string dept)
        {
            return _context.Employees.FromSqlRaw($"GetAllSalesEmp @dept='{dept}'").ToList();
        }
        [HttpGet("getDeptEmpOrdDesc")]
        public object GetDeptEmpOrdDesc()
        {
            var deptEmpOrd = from e in _context.Employees
                             join d in _context.Departments on e.DepartmentId equals d.DepartmentId
                             where d.DepartmentName == "Engineering"
                             orderby e.FirstName descending
                             select new
                             {
                                 Name = e.FirstName + " " + e.LastName
                             };
            return deptEmpOrd;
        }
    }
}
