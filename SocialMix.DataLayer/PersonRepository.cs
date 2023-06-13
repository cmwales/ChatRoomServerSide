using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using SocialMix.Models.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SocialMix.DataLayer
{
    public class PersonRepository : BaseRepository
    {



        public PersonRepository(IConfiguration configuration) : base(configuration)
        {

        }

            public Person Login(string emailOrUsername, string password)
        {
            // Hash the provided password
            string hashedPassword = HashPassword(password);

            using (var connection = this.CreateConnection())
            {
                connection.Open();

                // Prepare the SQL query
                var query = @"
                SELECT *
                FROM Person
                WHERE Email = @EmailOrUsername OR UserName = @EmailOrUsername";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailOrUsername", emailOrUsername);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Retrieve the user details from the reader
                        var user = new Person
                        {
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age"))

                        };

                        // Check if the password matches
                        if (user.Password == hashedPassword)
                        {
                            return user; // Login successful
                        }
                    }
                }
            }

            return null; // Invalid credentials
        }

        public Person RegisterUser(Person user)
        {
            string hashedPassword = HashPassword(user.Password);
   

            using (var connection = this.CreateConnection())
            {
                connection.Open();

                var query = @"
                            INSERT INTO Person (Email, Password, UserName, Country, State, Gender, Age)
                            OUTPUT INSERTED.PersonId
                            VALUES (@Email, @Password, @UserName, @Country, @State, @Gender, @Age)";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", hashedPassword);
                    command.Parameters.AddWithValue("@Username", user.UserName);
                    command.Parameters.AddWithValue("@Country", user.Country);
                    command.Parameters.AddWithValue("@State", user.State);
                    command.Parameters.AddWithValue("@Gender", user.Gender);
                    command.Parameters.AddWithValue("@Age", user.Age);

                    // Execute the query and retrieve the generated PersonId
                    user.PersonId = (int)command.ExecuteScalar();
                }
            }

            user.Password = null;
            return user;
        }


        public Person ValidateCredentials(string username, string password)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();

                // Create a SQL command to retrieve the user data
                var command = new SqlCommand("SELECT PersonId, Email, Password, UserName, Country, State, Gender, Age FROM Person WHERE UserName = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Person user = new Person
                        {
                            PersonId = reader.GetInt32(reader.GetOrdinal("PersonId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age"))
                        };

                        // Verify the provided password against the stored hashed password
                        bool isValidPassword = VerifyPassword(password, user.Password);

                        if (isValidPassword)
                        {
                            return user;
                        }
                    }
                }
            }

            return null; // Invalid credentials or user not found
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            string hashedPassword = HashPassword(password);
            return string.Equals(hashedPassword, storedHash);
        }



        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }

}