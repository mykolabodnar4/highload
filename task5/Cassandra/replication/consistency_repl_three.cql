USE repl_three;

CONSISTENCY ONE;

SELECT * FROM repl_three.items;

INSERT INTO repl_three.items(id, name) VALUES (11, 'three_item_11');

--

CONSISTENCY TWO;

SELECT * FROM repl_three.items;

INSERT INTO repl_three.items(id, name) VALUES (12, 'three_item_12');

--

CONSISTENCY THREE;
SELECT * FROM repl_three.items;

INSERT INTO repl_three.items(id, name) VALUES (13, 'three_item_13');

--
CONSISTENCY ALL ;

CONSISTENCY;

SELECT * FROM repl_three.items;

INSERT INTO repl_three.items(id, name) VALUES (13, 'three_item_13');
