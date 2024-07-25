using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext db, UserManager<ApplicationUser> userMgr, RoleManager<IdentityRole> roleMgr, IJwtTokenGenerator jwtTokenGenerator)
        {
            _db = db;
            _userManager = userMgr;
            _roleManager = roleMgr;
            _jwtTokenGenerator = jwtTokenGenerator;

        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => string.Compare(u.Email, email, StringComparison.OrdinalIgnoreCase) == 0);
            if(user != null)
            {
                if(!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => string.Compare(u.UserName, loginRequestDTO.UserName, StringComparison.OrdinalIgnoreCase) == 0);
            bool isValid = false;
            if (user != null) 
            {
                isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);
            }

            if(user == null || !isValid)
            {
                return new LoginResponseDto() { User = null, Token=""};
            }

            var userdto = new UserDTO()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };
            LoginResponseDto response = new()
            {
                User = userdto,
                Token = _jwtTokenGenerator.GenerateToken(user)
            };
            return response;
        }

        public async Task<string> Register(RegistrationRequestDTO registrationRequestDTO)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDTO.Email,
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper(),
                Name = registrationRequestDTO.Name,
                PhoneNumber = registrationRequestDTO.PhoneNumber
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDTO.Email);
                    return "";
                }
                else return result.Errors.FirstOrDefault().Description;
            }
            catch (Exception ex)
            {
                return $"error, {ex.Message}";
            }
        }
    }
}
