using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SocialMix.DataLayer;
using SocialMix.BusinessLayer.Managers;
using SocialMix.Models.Models;

namespace SocialMix.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {

        private readonly UserManager userManager;
        private readonly PersonRepository userRepository;

        public RegistrationController(UserManager userManager, PersonRepository userRepository = null)
        {
            this.userManager = userManager;
            this.userRepository = userRepository;
        }



        [Route("Register"), HttpPost]
        public Person Register(Person user)
        {
            return this.userManager.RegistgerUser(user);
        }
    }
}