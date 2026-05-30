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
	}
}
