using System;

namespace GraphStore.Models;

public record Customer
{
    public required string Id { get; set; }
    public List<Order> Orders { get; } = [];
}
