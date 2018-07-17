namespace Robot.Trans2Quik.Entities.Transaction.Order
{
    public class Security
    {
        public string ClassCode { get; set; }
        public string SecCode { get; set; }

        public Security() { }

        public Security(string classCode, string secCode)
        {
            ClassCode = classCode;
            SecCode = secCode;
        }
    }
}
