namespace REST_API_GNB.Models
{
    public class TransactionModel
    {
        public string Sku { get; set; }

        // Apply rounding when setting the amount
        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set => _amount = ApplyBankersRounding(value, 2); 
        }

        public string Currency { get; set; }

        private decimal ApplyBankersRounding(decimal value, int decimalPlaces)
        {
            return Math.Round(value, decimalPlaces, MidpointRounding.ToEven);
        }
    }
}
