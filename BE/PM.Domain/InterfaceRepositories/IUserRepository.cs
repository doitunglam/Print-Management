using PM.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PM.Domain.InterfaceRepositories
{
    public interface IUserRepository
    {
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByPhoneNumber (string username);
        Task<User> GetUserById(long Id);
        Task AddRoleToUserAsync(User user, List<string> Roles);
        Task<IEnumerable<string>> GetRolesOfUserAsync(User user);

        Task<IEnumerable<User>> GetUsersByTeamId(long teamId);

        Task<User> UpdateAsync(User user);

        Task<User> DeleteAsync(long id);
    }
}
