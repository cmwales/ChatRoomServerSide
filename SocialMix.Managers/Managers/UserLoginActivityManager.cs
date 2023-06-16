using SocialMix.DataLayer;
using SocialMix.Models.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static SocialMix.Models.Enums.Enums;

namespace SocialMix.BusinessLayer.Managers
{
    public class UserLoginActivityManager
    {
        private UserLoginActivityRepository userLoginActivityRepository;

        public UserLoginActivityManager(UserLoginActivityRepository userLoginActivityRepository) 
        {
            this.userLoginActivityRepository =  userLoginActivityRepository;
        }

        public int GetFailedLoginAttempts(string userName)
        {
            var count = this.userLoginActivityRepository.GetFailedLoginRequestsCount(userName);
            return count;
        }

        public void RecordFailedLoginAttempt(object username, string ipAddress, string message)
        {
            // Create a UserLoginActivity object with the necessary information
            var loginActivity = new UserLoginActivity
            {
                UserId = null,
                SessionID = string.Empty,
                Timestamp = DateTime.UtcNow,
                IPAddress = ipAddress,
                Status = (int)UserLoginStatus.Failed,
                @AdditionalInfo = message
            };

            // Call the repository method to save the login activity to the database
            userLoginActivityRepository.CreateUserLoginActivity(loginActivity);
        }

        public void RecordLoginSuccess(int userId, string sessionId, string ipaddress)
        {
            // Create a UserLoginActivity object with the necessary information
            var loginActivity = new UserLoginActivity
            {
                UserId = userId,
                SessionID = sessionId,
                Timestamp = DateTime.UtcNow,
                IPAddress = ipaddress,
                Status = (int)UserLoginStatus.Successful,
                @AdditionalInfo = string.Empty
        };

            // Call the repository method to save the login activity to the database
            userLoginActivityRepository.CreateUserLoginActivity(loginActivity);
        }

        public void LockoutUser(string username, string ipAddress)
        {
            var loginActivity = new UserLoginActivity
            {
                UserId = null,
                SessionID = null,
                Timestamp = DateTime.UtcNow,
                IPAddress = ipAddress,
                Status = (int)UserLoginStatus.AccountLocked,
                @AdditionalInfo = "Account Locked Out"
            };


            this.userLoginActivityRepository.CreateUserLoginActivity(loginActivity);
        }

        public bool IsAccountLocked(string userName)
        {
            return this.userLoginActivityRepository.IsAccountLocked(userName);
        }
    }
}
