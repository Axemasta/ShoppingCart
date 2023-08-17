using ShoppingCart.Models;

namespace ShoppingCart.Services;
public class BasketService
{
    private readonly List<Discount> discounts = new List<Discount>();


    public void SetDiscounts(List<Discount> discounts)
    {
        this.discounts.Clear();
        this.discounts.AddRange(discounts);
    }

    public double CalculateBasketTotal(ShoppingBasket basket)
    {
        var grossSum = basket.Contents.Sum(b => b.Price * b.Quantity);

        var total = grossSum;

        // Do any discounts apply to our basket?
        if (basket.Contents.Any(b => discounts.Any(d => d.ProductId == b.Name)))
        {
            var applicableDiscounts = discounts.Where(d => basket.Contents.Any(b => b.Name == d.ProductId));

            foreach (var applicableDiscount in applicableDiscounts)
            {
                var applicableItem = basket.Contents.First(b => b.Name ==  applicableDiscount.ProductId);

                double discountQuantity = applicableItem.Quantity / applicableDiscount.QuantityThreshold;

                var applicationTimes = Convert.ToInt32(Math.Floor(discountQuantity));

                total = total - applicableDiscount.Deduction * applicationTimes;
            }
        }

        return total;
    }
}
