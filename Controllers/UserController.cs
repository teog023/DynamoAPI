using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DynamoAPI.Models;

namespace DynamoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IDataAccessProvider _dataAccessProvider;
        public UserController(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        [HttpGet("infoUsers")]
        public async Task<IEnumerable<User>> Get()
        {
            return await _dataAccessProvider.GetUserRecords();
        }

        [HttpGet("infoUser/{userid}")]
        public async Task<User> Details(string userid)
        {
            return await _dataAccessProvider.GetUserSingleRecord(userid);
        }

        [HttpPost("insUser")]
        public async Task Create([FromBody] User User)
        {
            if (ModelState.IsValid)
            {
                await _dataAccessProvider.AddUserRecord(User);
            }
        }

        [HttpPut("updUser")]
        public async Task Edit([FromBody] User User)
        {
            if (ModelState.IsValid)
            {
                await _dataAccessProvider.UpdateUserRecord(User);
            }
        }
        [HttpDelete("delUser/{userid}")]
        public async Task DeleteConfirmed(string userid)
        {
            await _dataAccessProvider.DeleteUserRecord(userid);
        }
    }
}