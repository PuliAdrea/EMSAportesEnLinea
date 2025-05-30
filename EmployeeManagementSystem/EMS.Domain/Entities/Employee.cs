﻿using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }  
        public JobPosition Position { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
    }
}
