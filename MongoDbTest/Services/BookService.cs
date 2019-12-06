using MongoDB.Driver;
using MongoDbTest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MongoDbTest.Services
{
    public class BookService
    {
        private readonly IMongoCollection<Book> _books;

        public BookService(IBookstoreDatabaseSettings settings, HelperService helperService)
        {
#if (DEBUG)
            string connectionString = $"mongodb://localhost:27017";
#else
            string mongodbServerList = helperService.GetEnvFileValue("MONGODB_SERVER_LIST", null);
            if (string.IsNullOrEmpty(mongodbServerList))
            {
                throw new ArgumentNullException("MONGODB_SERVER_LIST or MONGODB_SERVER_LIST_FILE missing");
            }

            string username = helperService.GetEnvFileValue("MONGODB_SERVER_USERNAME", null);
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentNullException("MONGODB_SERVER_USERNAME or MONGODB_SERVER_USERNAME_FILE missing");
            }

            string password = helperService.GetEnvFileValue("MONGODB_SERVER_PASSWORD", null);
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentNullException("MONGODB_SERVER_PASSWORD or MONGODB_SERVER_PASSWORD_FILE missing");
            }

            string replicaSet = helperService.GetEnvFileValue("MONGODB_REPLICA_SET", settings.ReplicaSet);
            string connectionString = $"mongodb://{username}:{password}@{mongodbServerList}/admin?replicaSet={replicaSet}&readPreference=secondaryPreferred";
#endif
            string databaseName = helperService.GetEnvFileValue("MONGODB_DATABASE_NAME", settings.DatabaseName);
            string booksCollectionName = helperService.GetEnvFileValue("MONGODB_BOOKS_COLLECTION_NAME", settings.BooksCollectionName);

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _books = database.GetCollection<Book>(booksCollectionName);
        }

        public async Task<List<Book>> Get()
        {
            return (await _books.FindAsync(book => true)).ToList();
        }

        public async Task<Book> Get(string id)
        {
            return (await _books.FindAsync(book => book.Id == id)).FirstOrDefault();
        }

        public async Task<Book> Create(Book book)
        {
            await _books.InsertOneAsync(book);
            return book;
        }

        public async Task Update(string id, Book bookIn)
        {
            await _books.ReplaceOneAsync(book => book.Id == id, bookIn);
        }

        public async Task Remove(Book bookIn)
        {
            await _books.DeleteOneAsync(book => book.Id == bookIn.Id);
        }

        public async Task Remove(string id)
        {
            await _books.DeleteOneAsync(book => book.Id == id);
        }
    }
}