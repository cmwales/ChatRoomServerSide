using SocialMix.DataLayer;
using SocialMix.Models.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace SocialMix.BusinessLayer.Managers
{
    public class UserManager
    {
        UserRepository userRepository;
   

        public UserManager(UserRepository userRepository = null)
        {
            this.userRepository = userRepository;
           
        }

        public User RegistgerUser(User user)
        {
            user = this.userRepository.RegisterUser(user);

            return user;
        }


        public User AuthenticateUser(string userName, string password)
        {
            
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Invalid Password or UserName");
            }

            User user = this.userRepository.ValidateCredentials(userName, password);
            if (user is null)
            {
                throw new Exception("Invalid Password or UserName");
            }
            else
            {

                return user;
            }

        }


    }
}
