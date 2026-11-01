using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class CategoriesGenerator : GeneratorBase<Category>
    {
        public CategoriesGenerator() : base(new CategoryRepository()) { }

        public override string EntityName { get { return "Categories"; } }

        protected override List<Category> Build()
        {
            var list = new List<Category>();
            string[] names = { "Ficcion", "No ficcion", "Infantil", "Juvenil", "Academico", "Tecnologia", "Historia", "Ciencia", "Arte", "Negocios" };
            foreach (string name in names)
                list.Add(new Category { Name = name, Description = "Libros de la categoria " + name });
            return list;
        }
    }
}