using DocumentManagementApp.Models.Entities;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;

namespace DocumentManagementApp.Models
{
    public class DatabaseContext
    {
        private readonly string _connectionString;

        public DatabaseContext(string connectionString)
        {
            _connectionString = connectionString;
            CreateDatabaseIfNotExists();
            CreateTablesIfNotExist();
        }

        private void CreateDatabaseIfNotExists()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
            }
        }

        private void CreateTablesIfNotExist()
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "CREATE TABLE IF NOT EXISTS Documents (" +
                                          "Id INTEGER PRIMARY KEY, " +
                                          "Type TEXT, " +
                                          "Date TEXT, " +
                                          "FirstName TEXT, " +
                                          "LastName TEXT, " +
                                          "City TEXT)";
                    command.ExecuteNonQuery();

                    command.CommandText = "CREATE TABLE IF NOT EXISTS DocumentItems (" +
                                          "DocumentId INTEGER, " +
                                          "Ordinal INTEGER, " +
                                          "Product TEXT, " +
                                          "Quantity INTEGER, " +
                                          "Price DECIMAL, " +
                                          "TaxRate INTEGER, " +
                                          "FOREIGN KEY (DocumentId) REFERENCES Documents (Id))";
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddDocument(Document document)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT OR REPLACE INTO Documents (Id, Type, Date, FirstName, LastName, City) " +
                                          "VALUES (@Id, @Type, @Date, @FirstName, @LastName, @City)";
                    command.Parameters.AddWithValue("@Id", document.Id);
                    command.Parameters.AddWithValue("@Type", document.Type);
                    command.Parameters.AddWithValue("@Date", document.Date);
                    command.Parameters.AddWithValue("@FirstName", document.FirstName);
                    command.Parameters.AddWithValue("@LastName", document.LastName);
                    command.Parameters.AddWithValue("@City", document.City);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddDocumentItem(DocumentItem documentItem)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO DocumentItems (DocumentId, Ordinal, Product, Quantity, Price, TaxRate) " +
                                          "VALUES (@DocumentId, @Ordinal, @Product, @Quantity, @Price, @TaxRate)";
                    command.Parameters.AddWithValue("@DocumentId", documentItem.DocumentId);
                    command.Parameters.AddWithValue("@Ordinal", documentItem.Ordinal);
                    command.Parameters.AddWithValue("@Product", documentItem.Product);
                    command.Parameters.AddWithValue("@Quantity", documentItem.Quantity);
                    command.Parameters.AddWithValue("@Price", documentItem.Price);
                    command.Parameters.AddWithValue("@TaxRate", documentItem.TaxRate);

                    command.ExecuteNonQuery();
                }
            }
        }

        public IEnumerable<JoinedDocument> GetJoinedDocuments()
        {
            var joinedDocuments = new List<JoinedDocument>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Documents.Id, Documents.Type, Documents.Date, " +
                                          "Documents.FirstName, Documents.LastName, Documents.City, " +
                                          "DocumentItems.Product, DocumentItems.Quantity, DocumentItems.Price, DocumentItems.TaxRate, DocumentItems.Ordinal " +
                                          "FROM Documents " +
                                          "LEFT JOIN DocumentItems ON Documents.Id = DocumentItems.DocumentId";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var joinedDocument = new JoinedDocument
                            {
                                DocumentId = reader.GetInt32(0),
                                Type = reader.GetString(1),
                                Date = reader.GetDateTime(2),
                                FirstName = reader.GetString(3),
                                LastName = reader.GetString(4),
                                City = reader.GetString(5),
                                Product = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                                Quantity = reader.IsDBNull(7) ? 0 : reader.GetInt32(7),
                                Price = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8),
                                TaxRate = reader.IsDBNull(9) ? 0 : reader.GetInt32(9),
                                Ordinal = reader.IsDBNull(10) ? 0 : reader.GetInt32(10)
                            };

                            joinedDocuments.Add(joinedDocument);
                        }
                    }
                }
            }

            return joinedDocuments;
        }
    }
}
