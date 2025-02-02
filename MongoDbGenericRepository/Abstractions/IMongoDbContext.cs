﻿using MongoDB.Driver;

namespace MongoDbGenericRepository.Abstractions
{
    public interface IMongoDbClientContext
    {
        /// <summary>
        /// The IMongoClient from the official MongoDb driver
        /// </summary>
        IMongoClient Client { get; }

        /// <summary>
        /// Sets the Guid representation of the MongoDb Driver.
        /// </summary>
        /// <param name="guidRepresentation">The new value of the GuidRepresentation</param>
        void SetGuidRepresentation(MongoDB.Bson.GuidRepresentation guidRepresentation);
    }


    /// <summary>
    /// This is the interface of the IMongoDbContext which is managed by the <see cref="BaseMongoRepository"/>.
    /// </summary>
    public interface IMongoDbContext
    {
        IMongoDbClientContext ClientContext { get; }

        /// <summary>
        /// The IMongoDatabase from the official Mongodb driver
        /// </summary>
        IMongoDatabase Database { get; }

        /// <summary>
        /// Returns a collection for a document type that has a partition key.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="partitionKey">The value of the partition key.</param>
        IMongoCollection<TDocument> GetCollection<TDocument>(string partitionKey = null);

        /// <summary>
        /// Drops a collection having a partitionkey, use very carefully.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        void DropCollection<TDocument>(string partitionKey = null);
    }
}