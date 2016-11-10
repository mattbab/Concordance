namespace TotalDefense.Concordance.Analysis.Tokens
{
    using System.Collections.Generic;
    using System.Linq;

    public class WordToken : ValueToken<IEnumerable<ValueToken<char>>>
    {
        private readonly ICollection<ValueToken<char>> charTokens;

        public WordToken()
            : this(Enumerable.Empty<CharToken>())
        { }

        public WordToken(IEnumerable<ValueToken<char>> value)
        {
            this.charTokens = value.ToList();
        }

        public void AddChar(ValueToken<char> charToken)
        {
            this.charTokens.Add(charToken);
        }

        public long Length
        {
            get
            {
                return this.charTokens.Count;
            }
        }

        public override IEnumerable<ValueToken<char>> Value
        {
            get
            {
                return this.charTokens;
            }
        }

        public override string ToString()
        {
            return new string(this.charTokens.Select(x => x.Value).ToArray());
        }
    }
}