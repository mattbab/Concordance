namespace TotalDefense.Concordance.Analysis.Tokens
{
    public abstract class SpecialToken : IToken
    {
        public void Accept(IDispatcher dispatcher)
        {
            dispatcher.Dispatch(this);
        }
    }
}