CREATE KEYSPACE IF NOT EXISTS repl_one
WITH REPLICATION = {
    'class': 'SimpleStrategy',
    'replication_factor': 1
};

CREATE KEYSPACE IF NOT EXISTS repl_two
WITH REPLICATION = {
    'class': 'SimpleStrategy',
    'replication_factor': 2
};

CREATE KEYSPACE IF NOT EXISTS repl_three
WITH REPLICATION = {
    'class': 'SimpleStrategy',
    'replication_factor': 3
};