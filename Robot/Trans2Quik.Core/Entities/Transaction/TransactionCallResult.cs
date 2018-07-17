using System.Text;
using Robot.Trans2Quik.ReturnValues;
using Trans2Quik.Core;

namespace Robot.Trans2Quik.Entities.Transaction
{    
    public class TransactionCallResult : CallResult
    {
        private const int OrderSuccessfullyRegisteredReplyCode = 3;

        public int ReplyCode { get; set; }
        public long TransactionId { get; set; }
        public long OrderNumber { get; set; }
        public string ResultMessage { get; set; }

        public TransactionCallResult(ReturnValue result,
                                    int errorCode,
                                    string errorMessage,
                                    int replyCode,
                                    long transactionId,
                                    long orderNumber,
                                    string resultMessage)
                                    : base(result, errorCode, errorMessage)
        {
            ReplyCode = replyCode;
            TransactionId = transactionId;
            OrderNumber = orderNumber;
            ResultMessage = resultMessage;
        }

        public bool IsSuccess
        {
            get { return ReturnValue.IsSuccess && ReplyCode == OrderSuccessfullyRegisteredReplyCode; }
        }

        public override string ToString()
        {
            var sb = new StringBuilder(base.ToString());

            sb.AppendFormat("\nReplyCode={0}; TransactionId={1}; OrderNumber={2}; ResultMessage={3};", ReplyCode, TransactionId, OrderNumber, ResultMessage);

            return sb.ToString().Trim();
        }
    }
}
