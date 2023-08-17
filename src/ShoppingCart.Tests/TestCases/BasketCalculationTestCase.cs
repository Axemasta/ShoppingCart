using ShoppingCart.Models;

namespace ShoppingCart.Tests.TestCases;

public class BasketCalculationTestCase
{
    public string Name { get; set; }

    public ShoppingBasket Basket { get; set; }

    public List<Discount> Discounts { get; set; }

    public double ExpectedTotal { get; set; }
}
