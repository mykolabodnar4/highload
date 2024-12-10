# first check the correct name with `docker network ls`
docker network disconnect cassandra_cassandra-net cassandra-2
docker network disconnect cassandra_cassandra-net cassandra-3

# reconnect
docker network connect cassandra_cassandra-net cassandra-2
docker network connect cassandra_cassandra-net cassandra-3

# on node 1 (cassandra-1)
# INSERT INTO items(id, name) VALUES (101, 'three_item_one')

# on node 2 (cassandra-2)
# INSERT INTO items(id, name) VALUES (101, 'three_item_two')

# on node 3 (cassandra-3)
# INSERT INTO items(id, name) VALUES (101, 'three_item_three')
