using PAP.DataAccess;
using PAP.Repositories;
using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    public class GenresGenerator : GeneratorBase<Genre>
    {
        public GenresGenerator() : base(new GenreRepository()) { }

        public override string EntityName { get { return "Genres"; } }

        protected override List<Genre> Build()
        {
            var list = new List<Genre>();
            string[] names = { "Novela", "Misterio", "Fantasia", "Ciencia ficcion", "Romance", "Terror", "Biografia", "Poesia", "Aventura", "Drama" };
            foreach (string name in names)
                list.Add(new Genre { Name = name, Description = "Obras del genero " + name });
            return list;
        }
    }
}