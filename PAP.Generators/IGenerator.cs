using System;

namespace PAP.Generators
{
    /// <summary>
    /// Contract used by the Admin screen: every entity has one generator.
    /// </summary>
    public interface IGenerator : IDisposable
    {
        /// <summary>Name shown in the Admin table (e.g. "Authors").</summary>
        string EntityName { get; }

        /// <summary>True when the table already has at least one row.</summary>
        bool HasData();

        /// <summary>Inserts the generated rows. Throws if the table already has data.</summary>
        /// <returns>Number of rows inserted.</returns>
        int Generate();
    }
}