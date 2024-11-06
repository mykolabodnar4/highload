namespace GraphStore.Models;

public record Order
{
    public required string Id { get; set; }
    public required string CustomerId { get; set; }
    public List<Item> Items { get; set; } = [];

    public void AddItem(Item item)
    {
        Items.Add(item);
    }
}
