using ShoppingCart.Models;
using ShoppingCart.Tests.TestCases;

namespace ShoppingCart.Tests.Generators;
public class BasketCalculationDataGenerator
{
    public static IEnumerable<object[]> Baskets()
    {
        yield return BuildTestCaseObject(
            "Single Item",
            new ShoppingBasket()
            {
                Contents = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Champagne",
                        Price = 20d,
                        Quantity = 1,
                    }
                },
            },
            new List<Discount>()
            {

            },
            20d);

        yield return BuildTestCaseObject(
            "2 Items",
            new ShoppingBasket()
            {
                Contents = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Champagne",
                        Price = 20d,
                        Quantity = 2,
                    }
                },
            },
            new List<Discount>()
            {

            },
            40d);

        yield return BuildTestCaseObject(
            "Multiple Products",
            new ShoppingBasket()
            {
                Contents = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Champagne",
                        Price = 20d,
                        Quantity = 2,
                    },
                    new Product()
                    {
                        Name = "Crisps",
                        Price = 1.50d,
                        Quantity = 1,
                    },
                },
            },
            new List<Discount>()
            {

            },
            41.50d);

        yield return BuildTestCaseObject(
            "Single Product - Buy 5 get 1 free",
            new ShoppingBasket()
            {
                Contents = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Beer",
                        Price = 2.00d,
                        Quantity = 6,
                    },
                },
            },
            new List<Discount>()
            {
                new Discount()
                {
                    Name = "Buy 5 get 1 free",
                    Deduction = 2.00d,
                    ProductId = "Beer",
                    QuantityThreshold = 5,
                }
            },
            10.00d);

        yield return BuildTestCaseObject(
            "Multiple Products - Buy 5 get 1 free on Beer",
            new ShoppingBasket()
            {
                Contents = new List<Product>()
                {
                    new Product()
                    {
                        Name = "Beer",
                        Price = 2.00d,
                        Quantity = 6,
                    },
                    new Product()
                    {
                        Name = "Crisps",
                        Price = 1.50d,
                        Quantity = 1,
                    },
                    new Product()
                    {
                        Name = "Salsa",
                        Price = 0.50d,
                        Quantity = 1,
                    },
                },
            },
            new List<Discount>()
            {
                new Discount()
                {
                    Name = "Buy 5 get 1 free",
                    Deduction = 2.00d,
                    ProductId = "Beer",
                    QuantityThreshold = 5,
                }
            },
            12.00d);
    }

    /// <summary>
    /// Wraps the test case data into a <see cref="TestDataObject"/>.
    /// </summary>
    private static object[] BuildTestCaseObject(string testCaseName, ShoppingBasket basket, List<Discount> discounts, double expectedTotal)
    {
        var testData = new BasketCalculationTestCase()
        {
            Name = testCaseName,
            Basket = basket,
            Discounts = discounts,
            ExpectedTotal = expectedTotal,
        };

        return new object[] { new TestDataObject(testData) };
    }
}
