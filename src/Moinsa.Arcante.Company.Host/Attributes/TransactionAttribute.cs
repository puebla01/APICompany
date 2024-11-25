namespace Moinsa.Arcante.Company.Host.Attributes
{
    /// <see cref="https://dev.to/moesmp/transaction-middleware-in-aspnet-core-2608"/>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class TransactionAttribute : Attribute
    {
    }
}