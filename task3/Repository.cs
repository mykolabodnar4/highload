using GraphStore.Models;
using Neo4j.Driver;

namespace GraphStore;

public interface IRepository
{
    Task<IEnumerable<Item>> GetAllItemsForOrder(string orderId);
    Task<OrderTotalPriceResult> GetTotalPrice(string orderId);
    Task<IEnumerable<Order>> GetAllOrdersForCustomer(string customerId);
    Task<IEnumerable<Item>> GetAllBoughtItemsForCustomer(string customerId);
    Task<int> GetTotalItemsBoughtForCustomer(string customerId);
    Task<decimal> GetTotalSpentAmount(string customerId);
    Task<List<(int itemId, int totalBought)>> GetAllItemsPopularity();
    Task<List<Item>> GetAllItemsViewedByCustomer(string customerId);
    Task<List<Item>> GetAllBoughtAlongWith(string itemId);
    Task<List<Customer>> GetAllCustomersWhoBoughtItem(string itemId);
    Task<List<Item>> GetAllViewedAndNotBoughtItems(string customerId);
}

public class Repository : IRepository
{
    private readonly IDriver _driver;
    private readonly QueryConfig queryConfig;
    public Repository(IDriver driver)
    {
        _driver = driver;
        queryConfig = new QueryConfig(database: "neo4j");    
    }

    public async Task<IEnumerable<Item>> GetAllItemsForOrder(string orderId)
    {
        var queryResults = await _driver
            .ExecutableQuery(
                "MATCH (o:Order {id:$orderId})-[:CONTAINS]->(i:Item) " +
                "RETURN i as item")
            .WithParameters(new { orderId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var nodes = queryResults.Result.Select(record => record["item"].As<INode>());
        var items = nodes.Select(node => GetItemFromNode(node)).ToList();
        return items;
    }

    public async Task<OrderTotalPriceResult> GetTotalPrice(string orderId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (o:Order {id:$orderId})-[:CONTAINS]->(i:Item) " +
                "RETURN o.id as orderId, sum(i.price) as totalPrice"
            )
            .WithParameters(new { orderId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var result = queryResult
            .Select(record => new OrderTotalPriceResult
            (
                record["orderId"].As<string>(),
                record["totalPrice"].As<decimal>()
            ))
            .Single();
        return result;
    }

    public async Task<IEnumerable<Order>> GetAllOrdersForCustomer(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer {id:$customerId})-[:PLACED]->(o:Order) " +
                "RETURN o as order")
            .WithParameters(new { customerId }) 
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var orders = queryResult
            .Select(record => record["order"].As<INode>())
            .Select(node => new Order
            {
                Id = node.Properties["id"].As<string>(),
                CustomerId = customerId
            })
            .ToList();
        return orders;
    }

    public async Task<IEnumerable<Item>> GetAllBoughtItemsForCustomer(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer {id:$customerId})-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item)" +
                "RETURN i as item")
            .WithParameters(new { customerId }) 
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var items = queryResult.Select(record => record["item"].As<INode>())
            .Select(node => GetItemFromNode(node))
            .ToList();
        return items;
    }

    public async Task<int> GetTotalItemsBoughtForCustomer(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer)-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item) " +
                "WHERE c.id = $customerId " +
                "RETURN count(i.price) AS totalItems"
            )
            .WithParameters(new { customerId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var totalItems = queryResult
            .Select(record => record["totalItems"].As<int>())
            .Single();
        return totalItems;
    }
    
    public async Task<decimal> GetTotalSpentAmount(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer)-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item) " +
                "WHERE c.id = $customerId " +
                "RETURN sum(i.price) AS totalAmountSpent "
            )
            .WithParameters(new { customerId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var totalSpentAmount = queryResult
            .Select(record => record["totalAmountSpent"].As<decimal>())
            .Single();
        return totalSpentAmount;
    }

    public async Task<List<(int itemId, int totalBought)>> GetAllItemsPopularity()
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (i:Item)<-[r:CONTAINS]-(o:Order) " +
                "RETURN i.id as itemId, count(i) AS totalBought " +
                "ORDER BY totalBought DESC"
            )
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var totalSpentAmount = queryResult
            .Select(record => (
                itemId: record["itemId"].As<int>(),
                totalBought: record["totalBought"].As<int>())
            )
            .ToList();
        return totalSpentAmount;
    }

    public async Task<List<Item>> GetAllItemsViewedByCustomer(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer)-[:VIEWED]->(i:Item) " +
                "WHERE c.id = $customerId " +
                "RETURN i as item"
            )
            .WithParameters(new { customerId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var viewedItems = queryResult
            .Select(record => record["item"].As<INode>())
            .Select(node => GetItemFromNode(node))
            .ToList();
        return viewedItems;
    }

    public async Task<List<Item>> GetAllBoughtAlongWith(string itemId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (searchItem:Item {id:$itemId})<-[:CONTAINS]-(o:Order)-[:CONTAINS]->(i:Item) " +
                "RETURN DISTINCT i as item"
            )
            .WithParameters(new { itemId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var boughtAlongWith = queryResult
            .Select(record => record["item"].As<INode>())
            .Select(node => GetItemFromNode(node))
            .ToList();
        return boughtAlongWith;
    }

    public async Task<List<Customer>> GetAllCustomersWhoBoughtItem(string itemId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (i:Item)-[r]-{2}(c:Customer) " +
                "WHERE i.id = $itemId " +
                "RETURN DISTINCT c as customer"
            )
            .WithParameters(new { itemId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var customers = queryResult
            .Select(record => record["customer"].As<INode>())
            .Select(node => new Customer { Id = node["id"].As<string>() })
            .ToList();
        return customers;
    }

    public async Task<List<Item>> GetAllViewedAndNotBoughtItems(string customerId)
    {
        var (queryResult, _) = await _driver
            .ExecutableQuery(
                "MATCH (c:Customer)-[v:VIEWED]->(i:Item) " +
                "WHERE c.id = $customerId AND NOT EXISTS {" +
                "  MATCH (c)-[:PLACED]->(o)-[:CONTAINS]->(i)" +
                "} " +
                "RETURN i as item"
            )
            .WithParameters(new { customerId })
            .WithConfig(queryConfig)
            .ExecuteAsync();
        var viewedItems = queryResult
            .Select(record => record["item"].As<INode>())
            .Select(node => GetItemFromNode(node))
            .ToList();
        return viewedItems;
    }

    private Item GetItemFromNode(INode node) => new Item
    {
        Id = node.Properties["id"].As<string>(),
        Name = node.Properties["name"].As<string>(),
        Price = node.Properties["price"].As<decimal>(),
        Likes = node.Properties["likes"].As<int>(),
    };
}