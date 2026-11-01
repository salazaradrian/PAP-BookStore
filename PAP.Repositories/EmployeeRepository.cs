using PAP.DataAccess;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace PAP.Repositories
{
    public class EmployeeRepository : RepositoryBase<Employee>
    {
        private IQueryable<Employee> WithRelations()
        {
            return _set
                .Include(x => x.Role);
        }

        public override IEnumerable<Employee> GetAll()
        {
            return WithRelations().OrderBy(x => x.Id).ToList();
        }

        public override Employee GetById(int id)
        {
            return WithRelations().FirstOrDefault(x => x.Id == id);
        }
    }
}
