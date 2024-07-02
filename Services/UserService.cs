using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using webapi_ENVIA.Entities;

namespace webapi_ENVIA.Services
{
    public interface IUserService
    {
        Task <IEnumerable<User>> GetUsers();
        Task<User> Authenticate(string username, string password);
    }
    public class UserService : IUserService
    {
        private List<User> _users = new List<User>
        {
            new User{ ID = 1, Name = "ENVIA", Password = "ENVIA2Q2i!" },
            new User{ ID = 2, Name = "WebApi", Password = "InvitadoBOFASA2Q2!$" }
        };

        public async Task<User> Authenticate(string username, string password)
        {
            var user = await Task.Run(() => _users.SingleOrDefault(x => x.Name == username && x.Password == password));

            return user;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            return await Task.Run(() => _users);
        }
    }
}
