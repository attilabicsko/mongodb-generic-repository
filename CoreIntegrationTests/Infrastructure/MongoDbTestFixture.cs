﻿using MongoDbGenericRepository;
using MongoDbGenericRepository.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDbGenericRepository.Abstractions;

namespace CoreIntegrationTests.Infrastructure
{
    public class MongoDbTestFixture<T, TKey> : IDisposable
        where T : IDocument<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        public IMongoDbContext Context;

        public MongoDbTestFixture()
        {
        }

        public string PartitionKey { get; set; }

        public static ConcurrentBag<T> DocsToDelete { get; set; } = new ConcurrentBag<T>();

        public virtual void Dispose()
        {
            if (DocsToDelete.Any())
            {
                TestRepository.Instance.DeleteMany<T, TKey>(DocsToDelete.ToList());
            }
        }

        public T CreateTestDocument()
        {
            var doc = new T();
            DocsToDelete.Add(doc);
            return doc;
        }

        public List<T> CreateTestDocuments(int numberOfDocumentsToCreate)
        {
            var docs = new List<T>();
            for (var i = 0; i < numberOfDocumentsToCreate; i++)
            {
                var doc = new T();
                docs.Add(doc);
                DocsToDelete.Add(doc);
            }

            return docs;
        }
    }
}