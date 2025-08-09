using MongoDB.Driver;

namespace blogpost_website_api.DB
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;
        public MongoDBContext(IConfiguration configuration)
        {
            var connectionString = configuration["MongoDB:ConnectionString"];
            var databaseName = configuration["MongoDB:DatabaseName"];

            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentException("MongoDB connection string or database name is missing in configuration.");
            }

            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);

            Console.WriteLine($"✅ MongoDB connected to: {databaseName}");
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
