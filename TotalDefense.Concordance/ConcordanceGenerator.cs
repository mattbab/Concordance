namespace TotalDefense.Concordance
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Analysis;

    // Simple implementation of the `IConcordanceGenerator` interface
    public class ConcordanceGenerator : IConcordanceGenerator
    {
        public IDictionary<string, IEnumerable<long>> Generate(TextReader reader)
        {
            if (reader == null)
            {
                // C# 6: Replace the constant string "reader" with `nameof(reader)` to keep them in sync should the 
                //       parameter name change
                throw new ArgumentNullException("reader");
            }

            // When working with `TextReader` children, there is no way to explicitly check if the underlying stream is 
            // still open. The preferred convention is to set the reader to `null` when closed but we don't want our
            // implementation to depend on any outside behavior so we'll do a sanity check to infer that the stream is
            // open and has data available.
            // NOTE: `reader.Peek` may throw an IOException but we're going to let that bubble up since we can't really
            //       do anything about it here.
            if (reader.Peek() == -1)
            {
                throw new ArgumentException("The specified file is empty or otherwise cannot be read.");
            }

            IDispatcher dispatcher = new TokenDispatcher();
            foreach (var token in reader.Chars(shouldLowercase: true).Select(TokenFactory.Create))
            {
                Debug.WriteLine(token.GetType());
                token.Accept(dispatcher);
            }

            return dispatcher.Results;
        }
    }
}
