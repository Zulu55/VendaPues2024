using System.ComponentModel;

namespace VendaPues.Shared.Enums
{
    public enum OrderType
    {
        [Description("Pago contra entrega")]
        PaymentAgainstDelivery,

        [Description("Pago en línea")]
        PayOnLine
    }
}