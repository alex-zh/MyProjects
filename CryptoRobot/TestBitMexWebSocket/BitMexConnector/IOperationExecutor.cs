using BitMexConnector.Tables.Classes;

namespace BitMexConnector
{
    public interface IOperationExecutor
    {
        void SetOrder(LimitOrder order);
    }
}