namespace TotalDefense.Concordance.Analysis.Tokens
{
    public class ValueToken<TValue> : IToken
    {
        protected internal ValueToken()
        { } 

        public ValueToken(TValue value)
        {
            this.Value = value;
        } 

        public virtual TValue Value { get; }

        public virtual void Accept(IDispatcher dispatcher)
        {
            dispatcher.Dispatch(this);
        }
    }
}
