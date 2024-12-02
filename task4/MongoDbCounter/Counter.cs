    using MongoDB.Bson;

    namespace MongoDbCounter;

    public record Counter {
        public Counter(string name, int value) {
            Name = name;
            Value = value;

        }
        public Counter()
        {
        }

        public ObjectId Id {get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
    };

