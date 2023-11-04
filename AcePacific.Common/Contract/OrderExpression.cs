using Newtonsoft.Json;

namespace AcePacific.Common.Contract
{
    public class OrderExpression
    {
        public OrderExpression()
        {

        }
        public int Column { get; set; }
        public int Direction { get; set; }

        public static OrderExpression Deserilizer(string orderExpression)
        {
            OrderExpression orderDeserilizer = null;
            if (orderExpression != null)
            {
                orderDeserilizer = JsonConvert.DeserializeObject<OrderExpression>(orderExpression);
            }
            return orderDeserilizer ?? new OrderExpression();
        }
    }
}
