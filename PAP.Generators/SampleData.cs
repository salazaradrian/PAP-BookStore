using System;
using System.Collections.Generic;

namespace PAP.Generators
{
    /// <summary>
    /// Shared pools of realistic values and random helpers. The seed is fixed so runs are repeatable.
    /// </summary>
    public static class SampleData
    {
        public static readonly Random Rng = new Random(2026);

        public static readonly string[] FirstNames =
        {
            "Maria", "Jose", "Ana", "Luis", "Carmen", "Carlos", "Laura", "Miguel", "Sofia", "Daniel",
            "Valeria", "Andres", "Camila", "Jorge", "Paula", "Ricardo", "Elena", "Fernando", "Isabel", "Diego",
            "Lucia", "Pablo", "Gabriela", "Mateo", "Natalia", "Sergio", "Adriana", "Hector", "Monica", "Oscar"
        };

        public static readonly string[] LastNames =
        {
            "Garcia", "Rodriguez", "Martinez", "Lopez", "Gonzalez", "Perez", "Sanchez", "Ramirez", "Torres", "Flores",
            "Rivera", "Gomez", "Diaz", "Cruz", "Morales", "Reyes", "Jimenez", "Vargas", "Castro", "Mora",
            "Rojas", "Salazar", "Vega", "Solano", "Campos", "Araya", "Chaves", "Quesada", "Brenes", "Alvarado"
        };

        public static readonly string[] Countries =
        {
            "Costa Rica", "Mexico", "Colombia", "Argentina", "Chile", "Peru", "Spain", "United States", "United Kingdom", "Panama"
        };

        public static readonly string[] Cities =
        {
            "San Jose", "Heredia", "Alajuela", "Cartago", "Liberia", "Puntarenas", "Limon", "Madrid", "Bogota", "Lima"
        };

        public static T Pick<T>(IList<T> items)
        {
            return items[Rng.Next(items.Count)];
        }

        public static int Between(int min, int max)
        {
            return Rng.Next(min, max + 1);
        }

        public static decimal Money(int min, int max)
        {
            return Math.Round((decimal)(min + Rng.NextDouble() * (max - min)), 2);
        }

        public static bool Chance(int percent)
        {
            return Rng.Next(100) < percent;
        }

        /// <summary>Random date between January 1 of yearFrom and December 28 of yearTo.</summary>
        public static DateTime DateBetween(int yearFrom, int yearTo)
        {
            DateTime start = new DateTime(yearFrom, 1, 1);
            DateTime end = new DateTime(yearTo, 12, 28);
            return start.AddDays(Rng.Next((end - start).Days + 1));
        }

        /// <summary>Random moment within the last daysBack days.</summary>
        public static DateTime Recent(int daysBack)
        {
            return DateTime.UtcNow.AddDays(-Rng.Next(daysBack)).AddMinutes(-Rng.Next(1440));
        }

        public static string Email(string first, string last, int index)
        {
            return (first + "." + last + index).ToLowerInvariant() + "@example.com";
        }

        public static string Phone()
        {
            return "+506 " + Between(2000, 8999) + "-" + Between(1000, 9999);
        }
    }
}