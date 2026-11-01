using PAP.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAP.Generators
{
    /// <summary>
    /// Template for all generators: check the table is empty, build the rows, insert them in bulk.
    /// </summary>
    public abstract class GeneratorBase<T> : IGenerator where T : class
    {
        protected readonly RepositoryBase<T> Repository;

        protected GeneratorBase(RepositoryBase<T> repository)
        {
            Repository = repository;
        }

        public abstract string EntityName { get; }

        /// <summary>Builds the in-memory rows. Foreign keys must come from rows that already exist.</summary>
        protected abstract List<T> Build();

        public bool HasData()
        {
            return Repository.Any();
        }

        public int Generate()
        {
            if (HasData())
                throw new InvalidOperationException("La tabla " + EntityName + " ya contiene datos. No se generó nada.");

            List<T> items = Build();
            Insert(items);
            return items.Count;
        }

        /// <summary>
        /// Returns the ids of a parent table, or fails with a clear message when it is still empty.
        /// </summary>
        protected static List<int> RequireIds(IEnumerable<int> ids, string parentName)
        {
            List<int> list = ids.ToList();
            if (list.Count == 0)
                throw new InvalidOperationException(
                    "Primero debe generar los datos de " + parentName + ", porque esta tabla depende de ella.");
            return list;
        }
        /// <summary>Default bulk insert. Generators with extra links (e.g. Books) override it.</summary>
        protected virtual void Insert(List<T> items)
        {
            Repository.AddRange(items);
        }

        /// <summary>Reads all rows of a parent table and releases its repository.</summary>
        protected static List<TParent> Load<TParent>(RepositoryBase<TParent> parentRepository) where TParent : class
        {
            using (parentRepository)
            {
                return parentRepository.GetAll().ToList();
            }
        }

        public void Dispose()
        {
            Repository.Dispose();
        }
    }
}