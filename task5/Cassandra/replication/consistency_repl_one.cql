USE repl_one;

CONSISTENCY ONE;

SELECT * FROM items WHERE id = 1;

SELECT * FROM repl_one.items;

INSERT INTO repl_one.items(id, name) VALUES (11, 'one_item_11');