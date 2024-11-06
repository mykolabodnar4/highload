// See https://aka.ms/new-console-template for more information

using GraphStore;
using Neo4j.Driver;
using GraphStore.Models;

var dockerUri = "neo4j://localhost";
var user = "neo4j";
var dockerPassword = "your_password";
IDriver driver = GraphDatabase.Driver(
    dockerUri,
    AuthTokens.Basic(user, dockerPassword), 
    builder => builder.WithSocketKeepAliveEnabled(true));

var repository = new Repository(driver);
string orderId = "1";
string customerId = "1";
string itemId = "1";

Console.WriteLine("All items for orderId: {0}", orderId);
var allItemsForOrder = await repository.GetAllItemsForOrder(orderId);
foreach (var item in allItemsForOrder)
{
    Console.WriteLine(item);
}
Console.WriteLine();

Console.WriteLine("Get total price for orderId: {0}", orderId);
var totalPrice = await repository.GetTotalPrice("1");
Console.WriteLine(totalPrice);
Console.WriteLine();

Console.WriteLine("Get all orders for a customer with id: {0}", customerId);
var allOrdersForCustomer = await repository.GetAllOrdersForCustomer(customerId);
foreach (var order in allOrdersForCustomer)
{
    Console.WriteLine(order);
}
Console.WriteLine();

Console.WriteLine("Get all items bought by a customer with id: {0}", customerId);
var allBoughtItems = await repository.GetAllBoughtItemsForCustomer(customerId);
foreach (var item in allBoughtItems)
{
    Console.WriteLine(item);
}
Console.WriteLine();

Console.WriteLine("Get total number of bought items for a customer with id: {0}", customerId);
var totalBought = await repository.GetTotalItemsBoughtForCustomer(customerId);
Console.WriteLine(totalBought);
Console.WriteLine();

Console.WriteLine("Get total spent amount for a customer with id: {0}", customerId);
var totalAmountSpent = await repository.GetTotalSpentAmount(customerId);
Console.WriteLine(totalAmountSpent);
Console.WriteLine();

Console.WriteLine("Get how many times each item was bought");
var itemsPopularity = await repository.GetAllItemsPopularity();
foreach (var value in itemsPopularity)
{
    Console.WriteLine(value);
}
Console.WriteLine();

Console.WriteLine("Get all items viewed by a customer with id: {0}", customerId);
var viewedItems = await repository.GetAllItemsViewedByCustomer(customerId);
foreach (var item in viewedItems)
{
    Console.WriteLine(item);
}
Console.WriteLine();

Console.WriteLine("Get all items bought along with an item with id: {0}", itemId);
var boughtAlong = await repository.GetAllBoughtAlongWith(itemId);
foreach (var item in boughtAlong)
{
    Console.WriteLine(item);
}
Console.WriteLine();

Console.WriteLine("Get all customers that bought an item with id: {0}", itemId);
var customers = await repository.GetAllCustomersWhoBoughtItem(itemId);
foreach (var customer in customers)
{
    Console.WriteLine(customer);
}
Console.WriteLine();

Console.WriteLine("Get all items viewed but not bought by a customer with id: {0}", customerId);
var viewedAndNotBoughtItems = await repository.GetAllViewedAndNotBoughtItems(customerId);
foreach (var item in viewedAndNotBoughtItems)
{
    Console.WriteLine(item);
}
Console.WriteLine();


await MassiveLikesUpdate.UpdateLikes(driver, itemId);
var likes = await MassiveLikesUpdate.GetItemLikes(driver, itemId);
Console.WriteLine($"Final number of likes: {likes}");