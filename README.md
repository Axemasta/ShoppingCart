# Shopping Cart
This is a little proof of concept app showcasing `IXunitSerializable` usage in unit tests.

This repo is mainly for my own purposes playing around with patterns that I feel will be simple and easy to replicate in other projects.



## Test Case Pattern

Using the approach seen in [botbuilder-dotnet](https://github.com/microsoft/botbuilder-dotnet), the [TestDataObject](https://github.com/microsoft/botbuilder-dotnet/blob/main/libraries/Microsoft.Bot.Builder.Testing/XUnit/TestDataObject.cs) pattern seemed to solve most of my issues!

The only real difference is the `TestDataObject` doesn't get passed a generic type, it has a method which expects the generic, everything else is functionally very similar.

This repo is built on the pattern seen in this repo:

- Use `TestDataObject` class

- Test cases are simple poco's:
  ```csharp
  public class BasketCalculationTestCase
  {
      public string Name { get; set; }
  
      public ShoppingBasket Basket { get; set; }
  
      public List<Discount> Discounts { get; set; }
  
      public double ExpectedTotal { get; set; }
  }
  ```

- Use a class for member data (previously I put this data within a test class)

- Generator classes have a builder method to create test cases & wrap in `TestDataObject`:
  ```csharp
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
  ```

- All test cases are written using this wrapper method:
  ```csharp
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
  }
  ```

- Tests reference generator class
  ```csharp
  [Theory]
  [MemberData(nameof(BasketCalculationDataGenerator.Baskets), MemberType = typeof(BasketCalculationDataGenerator))]
  public void CalculateBasketTotal_WhenProvidedTestCase_ShouldMatchExpected(TestDataObject testData)
  ```

- Tests pass in `TestDataObject` and retrieve test case via `GetObject<T>()` method
  ```
  [Theory]
  [MemberData(nameof(BasketCalculationDataGenerator.Baskets), MemberType = typeof(BasketCalculationDataGenerator))]
  public void CalculateBasketTotal_WhenProvidedTestCase_ShouldMatchExpected(TestDataObject testData)
  {
      var testCase = testData.GetObject<BasketCalculationTestCase>();
  
      // Do something...
  }
  ```

  

## Previous Approaches

 I had toiled with this implementation since Xunit [doesn't support test case serialization](https://github.com/xunit/xunit/discussions/2570#discussioncomment-3351945). I had followed guides such as [this](https://darchuk.net/2019/04/12/serializing-xunit-test-cases/), but i wasn't happy with the usage in my codebase. I eventually settles on a json approach but it required a ton of boilerplate:



**Base Class:**

```csharp
public abstract class MemberDataSerializer<T> : IXunitSerializable
{
    public MemberDataSerializer()
    {
    }

    private const string objValue = "objValue";

    public T Object { get; set; }

    public void Deserialize(IXunitSerializationInfo info)
    {
        var objJson = info.GetValue<string>(objValue);

        Object = JsonConvert.DeserializeObject<T>(objJson);
    }

    public void Serialize(IXunitSerializationInfo info)
    {
        var json = JsonConvert.SerializeObject(this);
        info.AddValue(objValue, json);
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
}
```



**Test Case:**

```csharp
public class BasketCalculationTestCase : MemberDataSerializer<BasketCalculationTestCase>
{
    private string name;

    public string Name
    {
        get
        {
            if (Object != null)
            {
                return Object.Name;
            }
            
            return name;
        }
        set => name = value;
    }

    private ShoppingBasket basket;

    public ShoppingBasket Basket
    {
        get
        {
            if (Object != null)
            {
                return Object.Basket;
            }
            
            return basket;
        }
        set => basket = value;
    }

    private List<Discount> discounts;

    public List<Discount> Discounts
    {
        get
        {
            if (Object != null)
            {
                return Object.Discounts;
            }
            
            return discounts;
        }
        set => discounts = value;
    }

    private double expectedTotal;

    public double ExpectedTotal
    {
        get
        {
            if (Object != null)
            {
                return Object.ExpectedTotal;
            }
            
            return expectedTotal;
        }
        set => expectedTotal = value;
    }
}
```

As you can see the test case contains insane amounts of boilerplate and they become very tedious to write and maintain. I could have written a template or source generator to manage them but I knew there would be a better way!

The main advantage here is that the tests are ridiculously easy to use:

```csharp
[Theory]
[MemberData(nameof(BasketCalculationDataGenerator.Baskets), MemberType = typeof(BasketCalculationDataGenerator))]
public void CalculateBasketTotal_WhenProvidedTestCase_ShouldMatchExpected(BasketCalculationTestCase testCase)
{
    // Arrange
    _sut.SetDiscounts(testCase.Discounts);

    // Act
    var total = _sut.CalculateBasketTotal(testCase.Basket);

    // Assert
    Assert.Equal(testCase.ExpectedTotal, total);
}
```

Without investing too much time I wanted the flexibility of serialized tests for complex cases without the boilerplate of my implementation, not to mention a better syntax than: `public class BasketCalculationTestCase : MemberDataSerializer<BasketCalculationTestCase>` for test cases.



