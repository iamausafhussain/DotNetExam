using System;
using System.Collections.Generic;

namespace ExamApiTwo.Models;

public partial class Order
{
    public int? OrderId { get; set; }

    public int? EmployeeId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Employee? Employee { get; set; }
}
