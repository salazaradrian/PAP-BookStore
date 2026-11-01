using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class RolesGenerator : GeneratorBase<Role>
    {
        public RolesGenerator() : base(new RoleRepository()) { }

        public override string EntityName { get { return "Roles"; } }

        protected override List<Role> Build()
        {
            var list = new List<Role>();
            foreach (string name in new[] { "Administrador", "Gerente", "Vendedor", "Bodeguero", "Soporte" })
                list.Add(new Role { Name = name });
            return list;
        }
    }
}