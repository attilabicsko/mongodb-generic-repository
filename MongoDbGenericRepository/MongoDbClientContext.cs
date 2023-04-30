using MongoDB.Driver;
using MongoDbGenericRepository.Abstractions;

namespace MongoDbGenericRepository
{
    /// <summary>
    /// Shared mongoDB client. Should be configured as singleton
    /// </summary>
    public class MongoDbClientContext : IMongoDbClientContext
    {
        public IMongoClient Client { get; }

        public MongoDbClientContext(string connectionString)
        {
            InitializeGuidRepresentation();
            this.Client = new MongoClient(connectionString);
        }

        /// <summary>
        /// Initialize the Guid representation of the MongoDB Driver.
        /// Override this method to change the default GuidRepresentation.
        /// </summary>
        private void InitializeGuidRepresentation()
        {
            // by default, avoid legacy UUID representation: use Binary 0x04 subtype.
            MongoDefaults.GuidRepresentation = MongoDB.Bson.GuidRepresentation.Standard;
        }

        /// <summary>
        /// Sets the Guid representation of the MongoDB Driver.
        /// </summary>
        /// <param name="guidRepresentation">The new value of the GuidRepresentation</param>
        public virtual void SetGuidRepresentation(MongoDB.Bson.GuidRepresentation guidRepresentation)
        {
            MongoDefaults.GuidRepresentation = guidRepresentation;
        }
    }
}