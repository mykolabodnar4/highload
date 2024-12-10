USE orders;

DESCRIBE orders.items_by_category;

--Напишіть запит, який виводить усі товари в певній категорії відсортовані за ціною
SELECT
    name,
    category,
    price,
    manufacturer,
    short_description,
    custom_attributes['processor'] AS processor
FROM orders.items_by_category
WHERE category = 'Laptop'
ORDER BY price ASC;


-- Напишіть запити, які вибирають товари за різними критеріями в межах певної категорії
-- 1) назва
SELECT 
    name,
    category,
    price
FROM orders.items_by_category
WHERE category = 'Gaming Console' AND name = 'Xbox Series X';
-- WHERE category =? AND name =?

-- 2) ціна в проміжку
SELECT 
    name,
    category,
    price,
    manufacturer,
    short_description,
    custom_attributes['storage'] as storage
FROM orders.items_by_category
WHERE category = 'Gaming Console' AND price > 400 AND price < 500;

-- 3) ціна та виробник
SELECT  
    id, 
    name, 
    category, 
    price, 
    manufacturer 
FROM orders.items_by_category 
WHERE category = 'Smartphone' AND manufacturer = 'Apple' AND price < 4000;