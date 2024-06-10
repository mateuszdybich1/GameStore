using System.Diagnostics;
using System.Text;
using GameStore.Application.Dtos;
using GameStore.Application.IUserServices;
using GameStore.Domain.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStore.Web.Controllers;

[Route("api/users")]
[ApiController]
public class UserController(IHttpClientFactory httpClientFactory, IUserService userService, IUserCheckService userCheckService, IRolesService roleService) : ControllerBase
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("AuthMicroservice");
    private readonly IRolesService _rolesService = roleService;
    private readonly IUserService _userService = userService;
    private readonly IUserCheckService _userCheckService = userCheckService;

    [HttpPost("login")]
    public async Task<IActionResult> Login(AuthUserModelDto model)
    {
        if (!model.Model.InternalAuth)
        {
            try
            {
                var jsonContent = JsonConvert.SerializeObject(new { Email = model.Model.Login, Password = model.Model.Password });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/auth", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var microserviceUserModel = JsonConvert.DeserializeObject<UserModelMicroserviceDto>(responseString);

                    if (microserviceUserModel != null)
                    {
                        var userExists = await _userService.UserExists(microserviceUserModel.Email);

                        if (userExists)
                        {
                            var token = await _userService.LoginUser(model.Model.Login, model.Model.Password);
                            return Ok(new { Token = token });
                        }
                        else
                        {
                            var roleId = await _rolesService.GetDefaultRole(Domain.UserEntities.DefaultRoles.User);
                            var roles = new List<Guid> { roleId };
                            var token = await _userService.RegisterUser(new(microserviceUserModel.Email, model.Model.Password, roles));
                            return Ok(new { Token = token });
                        }
                    }
                }

                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        else
        {
            var token = await _userService.LoginUser(model.Model.Login, model.Model.Password);
            return Ok(new { Token = token });
        }
    }

    [HttpPost]
    public async Task<IActionResult> RegisterUser(UserRegisterDto userRegisterDto)
    {
        try
        {
            return Ok(await _userService.RegisterUser(userRegisterDto));
        }
        catch (EntityNotFoundException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPut]
    public async Task<IActionResult> UpdateUser(UserRegisterDto userRegisterDto)
    {
        if (_userCheckService.IsCurrentUser(userRegisterDto.User.ID) || _userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.UpdateUser }))
        {
            try
            {
                return Ok(await _userService.RegisterUser(userRegisterDto));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Users }))
        {
            IEnumerable<UserModelMicroserviceDto> response = null;
            try
            {
                response = await _httpClient.GetFromJsonAsync<IEnumerable<UserModelMicroserviceDto>>($"{_httpClient.BaseAddress}/users");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            try
            {
                var users = await _userService.GetAllUsers();

                if (response != null)
                {
                    response = response.Where(x => !users.Any(y => y.Name == x.Email));
                    return Ok(users.Concat(response.Select(x => new UserModelDto(x)).ToList()).ToList());
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Internal server error: {ex.Message}");
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.User }))
        {
            try
            {
                return Ok(await _userService.GetUser(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.DeleteUser }))
        {
            try
            {
                return Ok(await _userService.RemoveUser(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpGet("{id}/roles")]
    public async Task<IActionResult> GetUserRoles([FromRoute] Guid id)
    {
        if (_userCheckService.CanUserAccess(new AccessPageDto() { TargetPage = Domain.UserEntities.Permissions.Roles }))
        {
            try
            {
                return Ok(await _userService.GetUserRoles(id));
            }
            catch (EntityNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        else
        {
            return Unauthorized();
        }
    }

    [AllowAnonymous]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [HttpPost("access")]
    public IActionResult CheckAccess(AccessPageDto accessPageDto)
    {
        return Ok(_userCheckService.CanUserAccess(accessPageDto));
    }
}
