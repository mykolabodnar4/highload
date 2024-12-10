DESCRIBE orders.orders_by_clients;

-- Для замовника виведіть всі його замовлення відсортовані за часом коли вони були зроблені

SELECT
    id,
    client_id,
    client_name,
    item_ids,
    order_total,
    submitted_date
FROM orders.orders_by_clients
WHERE client_id = 1
ORDER BY submitted_date;

-- Для кожного замовників визначте суму на яку були зроблені усі його замовлення
SELECT 
    client_id,
    client_name,
    SUM(order_total) AS total_spent
FROM orders.orders_by_clients
GROUP BY client_id;

-- Для кожного замовлення виведіть час коли його ціна були занесена в базу (SELECT WRITETIME)
SELECT
    id AS "Order Id",
    -- we need to divide by 1000 because writetime returns value in microseconds and all other time functions use milliseconds
    totimestamp(minTimeuuid(writetime(order_total)/1000)) AS "Price Last Updated At",
    order_total AS "Order Total"
FROM orders.orders_by_clients;
-- WHERE client_id =?;
