namespace TestTask.Models;

public class Order
{
    public int PickupStoreNumber { get; set; }
    public string PickupStoreName { get; set; } = string.Empty;
    public double PickupLat { get; set; }
    public double PickupLon { get; set; }
    public string PickupFormattedAddress { get; set; } = string.Empty;
    public string PickupContactFirstName { get; set; } = string.Empty;
    public string PickupContactLastName { get; set; } = string.Empty;
    public string PickupContactEmail { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public int DeliveryTolerance { get; set; }
    public int DeliveryServiceTime { get; set; }
    public string OrderDetails { get; set; } = string.Empty;
    public string AssignedDriver { get; set; } = string.Empty;
    public string CustomerReference { get; set; } = string.Empty;
    public string Payer { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public double Weight { get; set; }
    public decimal Price { get; set; }
}