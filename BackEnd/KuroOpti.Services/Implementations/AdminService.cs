using AutoMapper;
using KuroOpti.Common.DTO;
using KuroOpti.Repositories.Interfaces;
using KuroOpti.Services.Interfaces;


namespace KuroOpti.Services.Implementations
{
	public class AdminService : IAdminService
	{
		private readonly IUserRepository userRepository;
		private readonly IMapper mapper;

		public AdminService(IUserRepository userRepository, IMapper mapper)
		{
			this.userRepository = userRepository;
			this.mapper = mapper;
		}

		public async Task<List<UserDto>> GetAllUsersAsync()
		{
			var users = await userRepository.GetAllAsync();
			return mapper.Map<List<UserDto>>(users);

		}

		public async Task<int> GetTotalUsersAsync()
		{
			return await userRepository.CountAsync();
		}

		public async Task<bool> UpdateUserAsync(int id, UserDto userDto)
		{
			var user = await userRepository.GetByIdAsync(id);
			if (user == null) return false;

			// mapper.Map(userDto, user);
			user.Email = userDto.Email;
			user.Role = userDto.Role;
			user.Id = id; 

			return await userRepository.UpdateAsync(user);
		}

		public async Task<bool> DeleteUserAsync(int id)
		{
			var user = await userRepository.GetByIdAsync(id);
			if (user == null) return false;

			return await userRepository.DeleteAsync(user);
		}
	}
}
