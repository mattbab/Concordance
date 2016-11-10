namespace TotalDefense.Concordance.Analysis
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Tokens;

    public class TokenDispatcher : IDispatcher
    {
        #region Common abbreviations

        private static readonly string[] CommonAbbreviations = new[] { "i.e." };

        #endregion

        private bool isWhitespace = false;

        private bool isNewline = false;

        private long currentLineCount = 1;

        private long currentLineWordCount = 0;

        private WordToken currentWordToken = new WordToken();

        // `ConcurrentDictionary` may be overkill in this case since we know how this is going to be used (it's private)
        // but it has some nice functions for atomic updates.
        private readonly ConcurrentDictionary<string, IEnumerable<long>> results =
            new ConcurrentDictionary<string, IEnumerable<long>>();

        public IDictionary<string, IEnumerable<long>> Results
        {
            get
            {
                return this.results;
            }
        }

        public void Dispatch(char token)
        {
            IToken newToken = TokenFactory.Create(token);

            this.Dispatch(newToken);
        }

        public void Dispatch(IToken token)
        {
            // If we've been asked to dispatch a generic `IToken` instance, send it to the right place.
            if (token is CharToken)
            {
                this.Dispatch((CharToken)token);
            }
            else if (token is WordToken)
            {
                this.Dispatch((WordToken)token);
            }
            else if (token is WhitespaceToken)
            {
                this.Dispatch((WhitespaceToken)token);
            }
            else if (token is NewlineToken)
            {
                this.Dispatch((NewlineToken)token);
            }
            else if (token is UnreadableToken)
            {
                this.Dispatch((UnreadableToken)token);
            }
        }

        public void Dispatch(CharToken token)
        {
            this.currentWordToken.AddChar(token);

            this.isWhitespace = false;
            this.isNewline = false;
        }

        public void Dispatch(WordToken token)
        {
            // Convert word token to string and look it up to record the current line number
            string word = token.ToString();

            this.results.AddOrUpdate(
                word,
                key => new[] { this.currentLineCount },
                (key, existing) => existing.Concat(new[] { this.currentLineCount }));
        }

        public void Dispatch(WhitespaceToken token)
        {
            if (this.currentWordToken.Length == 0)
            {
                // We haven't read any actual data from the current line yet so lets ignore leading spaces
                return;
            }

            if (this.isWhitespace)
            {
                // The last token was whitespace too so let's skip this one
                return;
            }

            // Increment the line word count
            this.currentLineWordCount++;

            // Save off the current word token and create a new instance for the next word.
            var wordToken = this.currentWordToken;
            this.currentWordToken = new WordToken();

            // Dispatch the current word token so it can be processed
            this.Dispatch(wordToken);

            this.isWhitespace = true;
        }

        public void Dispatch(NewlineToken token)
        {
            if (this.currentLineCount == 0 && this.currentLineWordCount == 0)
            {
                // We haven't read any actual data from the file yet so lets ignore leading blank lines
                return;
            }

            if (this.isNewline)
            {
                // The last token was a newline too so let's skip this one
                return;
            }

            var currentWord = this.currentWordToken.ToString() + token.Value;

            if (!CommonAbbreviations.Any(x => x.StartsWith(currentWord)))
            {
                this.currentLineCount++;
                this.currentLineWordCount = 0;

                this.isWhitespace = false;
                this.isNewline = true;
            }
            else
            {
                this.currentWordToken.AddChar(token);
            }
        }

        public void Dispatch(UnreadableToken token)
        {
            // The assignment was to work with a text file which shouldn't have unreadable characters so we're just 
            // going to ignore them if they exist.
        }
    }
}