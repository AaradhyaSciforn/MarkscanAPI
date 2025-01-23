// <copyright file="DatabaseConnection.cs" company="Sciforn Ltd">
// Copyright (C) 2021. Sciforn Ltd. All Rights Reserved
// </copyright>


using MySqlConnector;

namespace DbAccess
{
    public interface IDatabaseConnection
    {
        public MySqlConnection GetConnection();
        public string ConnectionString { get; set; }
    }

    public class DatabaseConnection : IDatabaseConnection
    {
        private string? connectionString;

        public string ConnectionString
        {
            get
            {
                return connectionString ?? string.Empty;
            }

            set
            {
                connectionString = value;
            }
        }

        public DatabaseConnection(string _connectionString)
        {
            ConnectionString = _connectionString;
        }

        public MySqlConnection GetConnection()
        {
            
                return new MySqlConnection(ConnectionString);
            
        }
    }
}
