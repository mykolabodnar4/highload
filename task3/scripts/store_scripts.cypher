// Define some data
//picnic-slalom-joel-passive-numeric-4056
CREATE CONSTRAINT unique_order_id    IF NOT EXISTS FOR (n:Order)    REQUIRE n.id IS UNIQUE;
CREATE CONSTRAINT unique_item_id     IF NOT EXISTS FOR (n:Item)     REQUIRE n.id IS UNIQUE;
CREATE CONSTRAINT unique_customer_id IF NOT EXISTS FOR (n:Customer) REQUIRE n.id IS UNIQUE;

MERGE (o1:Order {id:"1"})
MERGE (o2:Order {id:"2"})
MERGE (c:Customer {id:"1", name: "Mykola"})
MERGE (i1:Item {id:"1", name:"1", price: 2.5, likes: 5})
MERGE (i2:Item {id:"2", name:"2", price: 3, likes: 10})
MERGE (i3:Item {id:"3", name:"3", price: 1.5, likes: 12})
MERGE (i4:Item {id:"4", name:"4", price: 10, likes: 7})
MERGE (c)-[:PLACED]->(o1)
MERGE (c)-[:PLACED]->(o2)
MERGE (o1)-[:CONTAINS]->(i1)
MERGE (o1)-[:CONTAINS]->(i3)
MERGE (o2)-[:CONTAINS]->(i1)
MERGE (o2)-[:CONTAINS]->(i2)
MERGE (o2)-[:CONTAINS]->(i3)
MERGE (c)-[:VIEWED]->(i1)
MERGE (c)-[:VIEWED]->(i2)
MERGE (c)-[:VIEWED]->(i3)
MERGE (c)-[:VIEWED]->(i4)

// set param
:param {orderId: 1}

// Знайти Items які входять в конкретний Order
// Find items in an order
MATCH (o:Order {id:$orderId})-[:CONTAINS]->(i:Item)
RETURN i

// Підрахувати вартість конкретного Order
// Find a total price for an order
MATCH (o:Order {id:$orderId})-[:CONTAINS]->(i:Item)
RETURN o.id, sum(i.price)

// Знайти всі Orders конкретного Customer
// Find all orders for a customer
:param { customerId: 1}
MATCH (c:Customer {id:$customerId})-[:PLACED]->(o:Order)
RETURN o

// Знайти всі Items куплені конкретним Customer (через Order)
// Find all items bought by a customer
MATCH (c:Customer {id:$customerId})-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item)
RETURN i

// Знайти кількість Items куплені конкретним Customer (через Order)
// Find total number of items bought by a customer
MATCH (c:Customer)-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item)
WHERE c.id = $customerId
RETURN count(i.price) AS totalItems

// Знайти для Customer на яку суму він придбав товарів (через Order)
// Find total spent amount by a customer
MATCH (c:Customer)-[:PLACED]->(o:Order)-[:CONTAINS]->(i:Item)
WHERE c.id = $customerId
RETURN sum(i.price) AS totalAmountSpent

// Знайті скільки разів кожен товар був придбаний, відсортувати за цим значенням
// Find how many times each item was bought, sort by desc
MATCH (i:Item)<-[r:CONTAINS]-(o:Order)
RETURN i.id, count(i) AS totalBought
ORDER BY totalBought DESC


// Знайти всі Items переглянуті (view) конкретним Customer
MATCH (c:Customer)-[:VIEWED]->(i:Item)
WHERE c.id = $customerId
RETURN i

:param {itemId: 1}
// Знайти інші Items що купувались разом з конкретним Item (тобто всі Items що входять до Order-s разом з даними Item)
MATCH (item:Item {id:$itemId})<-[:CONTAINS]-(o:Order)-[:CONTAINS]->(i:Item)
RETURN DISTINCT i

// Знайти Customers які купили даний конкретний Item
MATCH (i:Item)-[r]-{2}(c:Customer)
WHERE i.id = $itemId
RETURN c, r, i

// Знайти для певного Customer(а) товари, які він переглядав, але не купив
MATCH (c:Customer)-[v:VIEWED]->(i:Item)
WHERE c.id = $customerId AND NOT EXISTS {
  MATCH (c)-[:PLACED]->(o)-[:CONTAINS]->(i)
}
RETURN c, v, i