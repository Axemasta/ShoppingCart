namespace ShoppingCart.Models;
public class Discount
{
    public required string ProductId { get; set; }

    public double Deduction { get; set; }

    public required string Name { get; set; }

    public int QuantityThreshold { get; set; }
}
