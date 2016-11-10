namespace TotalDefense.Concordance.Analysis
{
    using System.Collections.Concurrent;
    using System.Linq;

    using Tokens;

    // In a perfect world, we wouldn't have any shared state between our instances but this is a special case 
    // needed to avoid memory churn which can be hard on the garbage collector. Instead of creating a new character
    // token for every character in our file, we're going to cheat and share instances for duplicate characters.
    // This avoids beating up the garbage collector on ridiculously large files and keeps our memory footprint low.
    public static class TokenFactory
    {
        // NOTE: We're using `ConcurrentDictionary` here because the generic `Dictionary` class is not thread-safe and 
        //       we don't know how this class will be used in the future.
        private static readonly ConcurrentDictionary<char, IToken> CharTokenCache =
            new ConcurrentDictionary<char, IToken>();
        private static readonly ConcurrentDictionary<char, IToken> NewlineTokenCache =
            new ConcurrentDictionary<char, IToken>();

        private static readonly IToken WhitespaceToken = new WhitespaceToken();

        private static readonly IToken UnreadableToken = new UnreadableToken();

        public static IToken Create(char chr)
        {
            if (new[] { '.', '?', '!'}.Contains(chr))
            {
                return NewlineTokenCache.GetOrAdd(chr, x => new NewlineToken(x));
            }

            if (new[] { '\t', ' ' }.Contains(chr))
            {
                return WhitespaceToken;
            }

            // If the character is a letter, digit, or symbol generate a token for it.
            if (char.IsLetterOrDigit(chr) || char.IsSymbol(chr))
            {
                return CharTokenCache.GetOrAdd(chr, x => new CharToken(x));
            }

            return UnreadableToken;
        }
    }
}