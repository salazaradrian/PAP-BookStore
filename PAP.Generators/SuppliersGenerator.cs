using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class SuppliersGenerator : GeneratorBase<Supplier>
    {
        public SuppliersGenerator() : base(new SupplierRepository()) { }

        public override string EntityName { get { return "Suppliers"; } }

        protected override List<Supplier> Build()
        {
            var list = new List<Supplier>();
            string[] names = { "Distribuidora Central", "Libros del Sur", "Mayorista Andina", "Importadora Letras", "Logistica Editorial", "Papeleria Norte", "Distribuciones Maya", "Comercial Pacifico" };
            for (int i = 0; i < names.Length; i++)
            {
                string first = SampleData.Pick(SampleData.FirstNames);
                string last = SampleData.Pick(SampleData.LastNames);
                list.Add(new Supplier
                {
                    Name = names[i],
                    ContactName = first + " " + last,
                    Email = "ventas" + i + "@proveedor.example.com",
                    Phone = SampleData.Phone(),
                    Address = SampleData.Pick(SampleData.Cities) + ", " + SampleData.Pick(SampleData.Countries),
                    IsActive = true
                });
            }
            return list;
        }
    }
}