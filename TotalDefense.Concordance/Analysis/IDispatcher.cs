namespace TotalDefense.Concordance.Analysis
{
    using System.Collections.Generic;

    using Tokens;

    public interface IDispatcher
    {
        IDictionary<string, IEnumerable<long>> Results { get; }

        void Dispatch(char token);

        void Dispatch(CharToken token);

        void Dispatch(WordToken token);

        void Dispatch(IToken token);

        void Dispatch(WhitespaceToken token);

        void Dispatch(NewlineToken token);

        void Dispatch(UnreadableToken token);
    }
}