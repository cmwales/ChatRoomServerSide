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
        PersonRepository userRepository;

        public UserManager(PersonRepository userRepository = null)
        {
            this.userRepository = userRepository;
        }

        public Person RegistgerUser(Person user)
        {
            user = this.userRepository.RegisterUser(user);

            return user;
        }


        public Person AuthenticateUser(string userName, string password)
        {

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                throw new Exception("Invalid Password or UserName");
            }

            Person user = this.userRepository.ValidateCredentials(userName, password);
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
