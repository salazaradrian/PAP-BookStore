using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    public class EmployeesGenerator : GeneratorBase<Employee>
    {
        public EmployeesGenerator() : base(new EmployeeRepository()) { }

        public override string EntityName { get { return "Employees"; } }

        protected override List<Employee> Build()
        {
            List<int> roleIds = RequireIds(Load(new RoleRepository()).Select(x => x.Id), "Roles");
            string hash = PAP.Business.PasswordHasher.Hash("Empleado123!");
            var list = new List<Employee>();
            for (int i = 0; i < 12; i++)
            {
                string first = SampleData.Pick(SampleData.FirstNames);
                string last = SampleData.Pick(SampleData.LastNames);
                list.Add(new Employee
                {
                    RoleId = SampleData.Pick(roleIds),
                    FirstName = first,
                    LastName = last,
                    Email = SampleData.Email(first, last, i).Replace("@example.com", "@libreria.example.com"),
                    PasswordHash = hash,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
            }
            return list;
        }
    }
}