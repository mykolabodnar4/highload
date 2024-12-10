USE repl_two;

CONSISTENCY ONE;

SELECT * FROM repl_two.items;

INSERT INTO repl_two.items(id, name) VALUES (11, 'two_item_11');

--

CONSISTENCY TWO;

SELECT * FROM repl_two.items;

INSERT INTO repl_two.items(id, name) VALUES (12, 'two_item_12');