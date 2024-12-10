CREATE TABLE IF NOT EXISTS orders.orders_by_clients (
    id ASCII,
    client_id INT,
    client_name TEXT,
    item_ids LIST<INT>, 
    order_total DECIMAL,
    submitted_date DATE,
    PRIMARY KEY((client_id), submitted_date, id)
)
WITH CLUSTERING ORDER BY (submitted_date DESC);

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o1', 1, 'Keir Starmer', [1, 5, 10, 26], 2867.00, '2024-12-10');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o6', 1, 'Keir Starmer', [33, 22], 919.00, '2024-12-13');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o7', 1, 'Keir Starmer', [35], 278.00, '2024-12-02');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o2', 2, 'John Lopata', [4], 1999.00, '2024-11-24');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o8', 2, 'John Lopata', [35], 276.00, '2024-12-03');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o3', 3, 'Mike Blumenthal', [18, 31], 398.00, '2024-11-27');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o4', 4, 'Chris Murphy', [7, 12], 398.00, '2024-12-04');

INSERT INTO orders.orders_by_clients (id, client_id, client_name, item_ids, order_total, submitted_date)
VALUES ('o5', 5, 'Kim Scott', [5, 17, 34], 1837.00, '2024-10-03');
