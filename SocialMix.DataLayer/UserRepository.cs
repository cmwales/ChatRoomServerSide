using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using SocialMix.Models.Models;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace SocialMix.DataLayer
{
    public class UserRepository : BaseRepository
    {



        public UserRepository(IConfiguration configuration) : base(configuration)
        {

        }

        public User Login(string emailOrUsername, string password)
        {
           
            using (var connection = this.CreateConnection())
            {
                connection.Open();

                // Prepare the SQL query
                var query = @"
                SELECT *
                FROM User
                WHERE Email = @EmailOrUsername OR UserName = @EmailOrUsername";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@EmailOrUsername", emailOrUsername);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        // Retrieve the user details from the reader
                        var user = new User
                        {
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Salt = reader.GetString(reader.GetOrdinal("Salt")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age"))

                        };

                        // Hash the provided password
                        string hashedPassword = HashPassword(password, user.Salt);


                        // Check if the password matches
                        if (user.Password == hashedPassword)
                        {
                            return user; // Login successful
                        }
                    }
                    else
                    {
                        throw new Exception("Invalid Username or password");
                    }
                }
            }

            throw new Exception("Invalid Username or password");
        }

        public User RegisterUser(User user)
        {
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(user.Password, salt);


            using (var connection = this.CreateConnection())
            {
                connection.Open();

                var query = @"
                            INSERT INTO User (Email, Password, UserName, Country, State, Gender, Age)
                            OUTPUT INSERTED.UserId
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

                    // Execute the query and retrieve the generated UserId
                    user.UserId = (int)command.ExecuteScalar();
                }
            }

            user.Password = null;
            return user;
        }

        public User ValidateCredentials(string username, string password)
        {
            using (var connection = this.CreateConnection())
            {
                connection.Open();

                // Create a SQL command to retrieve the user data
                var command = new SqlCommand("SELECT UserId, Email, Password, Salt, UserName, Country, State, Gender, Age FROM User WHERE UserName = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        User user = new User
                        {
                            UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            Password = reader.GetString(reader.GetOrdinal("Password")),
                            Salt = reader.GetString(reader.GetOrdinal("Salt")),
                            UserName = reader.GetString(reader.GetOrdinal("UserName")),
                            Country = reader.GetString(reader.GetOrdinal("Country")),
                            State = reader.GetString(reader.GetOrdinal("State")),
                            Gender = reader.GetString(reader.GetOrdinal("Gender")),
                            Age = reader.GetInt32(reader.GetOrdinal("Age"))
                        };

                        // Verify the provided password against the stored hashed password
                        bool isValidPassword = VerifyPassword(password, user.Password, user.Salt);

                        if (isValidPassword)
                        {
                            return user;
                        }
                    }
                }
            }

            return null; // Invalid credentials or user not found
        }

        private bool VerifyPassword(string password, string storedHash, string salt)
        {
            string hashedPassword = HashPassword(password, salt);
            return string.Equals(hashedPassword, storedHash);
        }

        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var saltedPassword = password + salt;
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private string GenerateSalt()
        {
            // Generate a random salt value using a cryptographic RNG
            byte[] saltBytes = new byte[32];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }

            // Convert the salt bytes to a Base64 string
            string salt = Convert.ToBase64String(saltBytes);
            return salt;
        }

    }

}