using KuroOpti.Common.DTO;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;

namespace KuroOpti.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository userRepository;

        public AdminService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await userRepository.GetAllAsync();

            return users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Role = u.Role,
                })
                .ToList();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            return await userRepository.CountAsync();
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            user.Email = userDto.Email;
            user.Role = userDto.Role;

            return await userRepository.UpdateAsync(user);
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await userRepository.GetByIdAsync(id);
            if (user == null)
                return false;

            return await userRepository.DeleteAsync(user);
        }
    }
}
