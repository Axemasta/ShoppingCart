using ShoppingCart.Services;
using ShoppingCart.Tests.Generators;
using ShoppingCart.Tests.TestCases;

namespace ShoppingCart.Tests;

public class BasketServiceTests
{
    private readonly BasketService _sut;

    public BasketServiceTests()
    {
        _sut = new BasketService();
    }

    [Theory]
    [MemberData(nameof(BasketCalculationDataGenerator.Baskets), MemberType = typeof(BasketCalculationDataGenerator))]
    public void CalculateBasketTotal_WhenProvidedTestCase_ShouldMatchExpected(TestDataObject testData)
    {
        // Arrange
        var testCase = testData.GetObject<BasketCalculationTestCase>();

        _sut.SetDiscounts(testCase.Discounts);

        // Act
        var total = _sut.CalculateBasketTotal(testCase.Basket);

        // Assert
        Assert.Equal(testCase.ExpectedTotal, total);
    }
}