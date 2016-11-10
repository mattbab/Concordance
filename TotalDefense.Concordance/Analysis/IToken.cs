namespace TotalDefense.Concordance.Analysis
{
    public interface IToken
    {
        void Accept(IDispatcher dispatcher);
    }
}