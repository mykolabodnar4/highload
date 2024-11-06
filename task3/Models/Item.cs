using System;

namespace GraphStore.Models;

public record Item
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required decimal Price { get; set; }
    public int Likes { get; set; }
}
