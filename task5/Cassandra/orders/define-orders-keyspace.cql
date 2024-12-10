CREATE KEYSPACE IF NOT EXISTS orders
WITH REPLICATION = {
    'class': 'SimpleStrategy',
    'replication_factor': 3
};
