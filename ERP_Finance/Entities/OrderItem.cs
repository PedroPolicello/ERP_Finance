namespace ERP_Finance.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public decimal Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public decimal Subtotal { get; private set; }

    public OrderItem(Guid orderId, Guid productId, decimal quantity, decimal unitPrice)
    {
        ValidateId(orderId);
        ValidateId(productId);
        ValidateQuantity(quantity);
        ValidateUnitPrice(unitPrice);


        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;

        CalculateSubtotal(Quantity, UnitPrice);
    }

    public void UpdateQuantity(decimal quantity)
    {
        ValidateQuantity(quantity);
        Quantity = quantity;

        CalculateSubtotal(Quantity, UnitPrice);
    }

    private void CalculateSubtotal(decimal quantity, decimal unitPrice)
    {
        Subtotal = quantity * unitPrice;
    }

    private static void ValidateId(Guid id)
    {
        if(id == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(id));
    }

    private static void ValidateQuantity(decimal quantity)
    {
        if(quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity has to be a positive value.");
    }

    private static void ValidateUnitPrice(decimal unitPrice)
    {
        if (unitPrice <= 0)
            throw new ArgumentOutOfRangeException(nameof(unitPrice), "Unit price has to be a positive value.");
    }
}
