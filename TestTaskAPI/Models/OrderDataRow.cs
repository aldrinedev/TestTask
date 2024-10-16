namespace TestTask.Models;

public class OrderDataRow
{
    public string PickupStoreNumber { get; set; } = string.Empty;
    public string PickupStoreName { get; set; } = string.Empty;
    public string PickupLat { get; set; } = string.Empty;
    public string PickupLon { get; set; } = string.Empty;
    public string PickupFormattedAddress { get; set; } = string.Empty;
    public string PickupContactFirstName { get; set; } = string.Empty;
    public string PickupContactLastName { get; set; } = string.Empty;
    public string PickupContactEmail { get; set; } = string.Empty;
    public string PickupContactMobileNumber { get; set; } = string.Empty;
    public string PickupEnableSmsNotification { get; set; } = string.Empty;
    public string PickupTime { get; set; } = string.Empty;
    public string PickupToleranceMin { get; set; } = string.Empty;
    public string PickupServiceTime { get; set; } = string.Empty;
    public string DeliveryStoreNumber { get; set; } = string.Empty;
    public string DeliveryStoreName { get; set; } = string.Empty;
    public string DeliveryLat { get; set; } = string.Empty;
    public string DeliveryLon { get; set; } = string.Empty;
    public string DeliveryFormattedAddress { get; set; } = string.Empty;
    public string DeliveryContactFirstName { get; set; } = string.Empty;
    public string DeliveryContactLastName { get; set; } = string.Empty;
    public string DeliveryContactEmail { get; set; } = string.Empty;
    public string DeliveryContactMobileNumber { get; set; } = string.Empty;
    public string DeliveryEnableSmsNotification { get; set; } = string.Empty;
    public string DeliveryTime { get; set; } = string.Empty;
    public string DeliveryToleranceMin { get; set; } = string.Empty;
    public string DeliveryServiceTime { get; set; } = string.Empty;
    public string OrderDetails { get; set; } = string.Empty;
    public string AssignedDriver { get; set; } = string.Empty;
    public string CustomerReference { get; set; } = string.Empty;
    public string Payer { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string Weight { get; set; } = string.Empty;
    public string Price { get; set; } = string.Empty;
}