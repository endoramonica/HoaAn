namespace VietCommerce.Core.Enums.Payments
{
    public enum PaymentMethodType
    {
        CASH = 1,
        CREDIT_CARD = 2,
        BANK_TRANSFER = 3,
        E_WALLET = 4,
        COD = 5,
        // INSTALLMENT = 6, // kkhoan tra gop 
        PENDING = 7,// status
        CONFIRMED = 8,// status
        FAILED = 9 // status
    }
}
