using System.Collections.Generic;
using System.Threading.Tasks;

namespace DynamoAPI.Models
{
    public interface IDataAccessProvider
    {
        Task AddUserRecord(User user);
        Task UpdateUserRecord(User user);
        Task DeleteUserRecord(string userid);
        Task<User> GetUserSingleRecord(string userid);
        Task<IEnumerable<User>> GetUserRecords();
    }
}