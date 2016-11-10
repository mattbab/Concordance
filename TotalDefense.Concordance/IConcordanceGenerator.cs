namespace TotalDefense.Concordance
{
    using System.Collections.Generic;
    using System.IO;

    public interface IConcordanceGenerator
    {
        IDictionary<string, IEnumerable<long>> Generate(TextReader reader);
    }
}
