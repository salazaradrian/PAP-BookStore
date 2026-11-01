using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Business
{
    public partial class EmployeeManager : IDisposable
    {
        private readonly EmployeeRepository _repository = new EmployeeRepository();
        private readonly RoleRepository _roleRepository = new RoleRepository();

        public IList<Employee> GetAll()
        {
            return _repository.GetAll().ToList();
        }

        public Employee GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IList<string> Create(Employee entity, string password)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity, password, true);
            if (errors.Count > 0)
                return errors;

            DateTime now = DateTime.UtcNow;
            entity.PasswordHash = PasswordHasher.Hash(password);
            entity.CreatedAt = now;
            entity.Id = 0;
            _repository.Add(entity);
            return errors;
        }

        public IList<string> Update(Employee entity, string password)
        {
            Normalize(entity);
            IList<string> errors = Validate(entity, password, false);
            if (errors.Count > 0)
                return errors;

            Employee existing = _repository.GetById(entity.Id);
            if (existing == null)
            {
                errors.Add("El registro ya no existe.");
                return errors;
            }

            DateTime now = DateTime.UtcNow;
            entity.PasswordHash = string.IsNullOrEmpty(password) ? existing.PasswordHash : PasswordHasher.Hash(password);
            entity.CreatedAt = existing.CreatedAt;
            _repository.Update(entity.Id, entity);
            return errors;
        }

        public void Delete(int id)
        {
            _repository.Delete(id);
        }

        public IList<LookupItem> GetRolesOptions()
        {
            return _roleRepository.GetAll()
                .Select(x => new LookupItem(x.Id, x.Name))
                .OrderBy(i => i.Text)
                .ToList();
        }

        private static void Normalize(Employee entity)
        {
            entity.FirstName = ValidationRules.Clean(entity.FirstName);
            entity.LastName = ValidationRules.Clean(entity.LastName);
            entity.Email = ValidationRules.Clean(entity.Email);
        }

        private IList<string> Validate(Employee entity, string password, bool isNew)
        {
            var errors = new List<string>();
            ValidationRules.Selected(errors, "Rol", entity.RoleId);
            ValidationRules.Required(errors, "Nombre", entity.FirstName, 100);
            ValidationRules.Required(errors, "Apellido", entity.LastName, 100);
            ValidationRules.Email(errors, "Correo electrónico", entity.Email, true, 150);
            ValidationRules.Password(errors, password, isNew);
            return errors;
        }

        public void Dispose()
        {
            _repository.Dispose();
            _roleRepository.Dispose();
        }
    }
}
