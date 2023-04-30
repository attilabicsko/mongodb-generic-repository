using MongoDB.Driver;
using MongoDbGenericRepository.Attributes;
using MongoDbGenericRepository.Utils;
using System.Linq;
using System.Reflection;
using MongoDbGenericRepository.Abstractions;

namespace MongoDbGenericRepository
{
    /// <summary>
    /// The MongoDb context
    /// </summary>
    public class MongoDbContext : IMongoDbContext
    {
        private IMongoDatabase? _database;
        public IMongoDbClientContext ClientContext { get; }

        protected IMongoClient Client => ClientContext.Client;

        /// <summary>
        /// The IMongoDatabase from the official MongoDB driver
        /// </summary>
        public virtual IMongoDatabase? Database
        {
            get => _database ?? throw new MongoDbContextDatabaseNotSetException();
            set => _database = value;
        }


        /// <summary>
        /// The constructor of the MongoDbContext, it needs a client context. Use is there's a child class defining databases per scope.. 
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        public MongoDbContext(IMongoDbClientContext clientContext)
        {
            ClientContext = clientContext;
        }

        /// <summary>
        /// The constructor of the MongoDbContext, it needs an object implementing <see cref="IMongoDatabase"/> Sets Database explicitly. Use for singleton scope with single database
        /// </summary>
        /// <param name="mongoDatabase">An object implementing IMongoDatabase</param>
        public MongoDbContext(IMongoDatabase mongoDatabase)
        {
            _database = mongoDatabase;
        }


        /// <summary>
        /// The constructor of the MongoDbContext, it needs a client context and a database name. Use for singleton scope with single database.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="databaseName">The name of your database.</param>
        public MongoDbContext(string connectionString, string databaseName)
        {
            ClientContext = new MongoDbClientContext(connectionString);
            _database = Client.GetDatabase(databaseName);
        }

        /// <summary>
        /// The constructor of the MongoDbContext, it needs a client context and a database name. Use for singleton scope with single database.
        /// </summary>
        /// <param name="clientContext">The client context.</param>
        /// <param name="databaseName">The name of your database.</param>
        public MongoDbContext(IMongoDbClientContext clientContext, string databaseName) : this(clientContext)
        {
            _database = Client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Returns a collection for a document type. Also handles document types with a partition key.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The optional value of the partition key.</param>
        public virtual IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null)
        {
            return Database.GetCollection<TDocument>(GetCollectionName<TDocument>(partitionKey));
        }

        /// <summary>
        /// Drops a collection, use very carefully.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        public virtual void DropCollection<TDocument>(string partitionKey = null)
        {
            Database.DropCollection(GetCollectionName<TDocument>(partitionKey));
        }


        /// <summary>
        /// Extracts the CollectionName attribute from the entity type, if any.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The name of the collection in which the TDocument is stored.</returns>
        protected virtual string GetAttributeCollectionName<TDocument>()
        {
            return (typeof(TDocument).GetTypeInfo()
                .GetCustomAttributes(typeof(CollectionNameAttribute))
                .FirstOrDefault() as CollectionNameAttribute)?.Name;
        }


        /// <summary>
        /// Given the document type and the partition key, returns the name of the collection it belongs to.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <param name="partitionKey">The value of the partition key.</param>
        /// <returns>The name of the collection.</returns>
        protected virtual string GetCollectionName<TDocument>(string partitionKey)
        {
            var collectionName = GetAttributeCollectionName<TDocument>() ?? Pluralize<TDocument>();
            if (string.IsNullOrEmpty(partitionKey))
            {
                return collectionName;
            }

            return $"{partitionKey}-{collectionName}";
        }

        /// <summary>
        /// Very naively pluralizes a TDocument type name.
        /// </summary>
        /// <typeparam name="TDocument">The type representing a Document.</typeparam>
        /// <returns>The pluralized document name.</returns>
        protected virtual string Pluralize<TDocument>()
        {
            return (typeof(TDocument).Name.Pluralize()).Camelize();
        }
    }
}