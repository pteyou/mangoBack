using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthAPIController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            var message = await _authService.Register(model);
            var response = new ResponseDto();
            if(!string.IsNullOrEmpty(message))
            {
                response.IsSuccess = false; 
                response.Message = message;
                return BadRequest(response);
            }
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDto)
        {
            var loginReponse = await _authService.Login(loginRequestDto);
            var response = new ResponseDto();
            if (loginReponse.User == null)
            {
                response.IsSuccess = false;
                response.Message = "Username or password is incorrect";
                return BadRequest(response);
            }
            else
            {
                response.Result = loginReponse;
                return Ok(response);
            }
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDTO model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            var response = new ResponseDto();
            if (!assignRoleSuccessful)
            {
                response.IsSuccess = false;
                response.Message = "Error encountered";
                return BadRequest(response);
            }
            else
            {
                return Ok(response);
            }
        }
    }
}
