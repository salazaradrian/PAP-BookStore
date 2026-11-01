using System.Collections.Generic;

namespace PAP.Generators
{
    /// <summary>
    /// What the Admin screen needs to know about one entity: its name, whether it already has data
    /// and whether the button can be enabled (all the tables it depends on already have data).
    /// </summary>
    public class GeneratorStatus
    {
        public string EntityName { get; set; }
        public string TableName { get; set; }
        public bool HasData { get; set; }
        public bool CanGenerate { get; set; }
        public IList<string> MissingDependencies { get; set; }
    }
}