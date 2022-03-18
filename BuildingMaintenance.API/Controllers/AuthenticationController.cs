using BuildingMaintenance.API.Response;
using BuildingMaintenance.Common.Messages;
using BuildingMaintenance.Domain.DTO;
using BuildingMaintenance.Domain.Entities.Authentication;
using BuildingMaintenance.Repositories.IRepository.IFileLogging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BuildingMaintenance.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private readonly IFileLoggingRepository _fileLogging;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config, IFileLoggingRepository fileLogging)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _fileLogging = fileLogging;
        }

        [Route("ApplicationRoles/{Role}")]
        [HttpGet]
        public async Task<IActionResult> ApplicationRoles(string Role)
        {
            var roleExists = await _roleManager.RoleExistsAsync(Role);

            if (roleExists == true)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityRole>(Role + " " + AppMessages.RoleExist, AppConstant.Information));
            }

            var newRole = await _roleManager.CreateAsync(new IdentityRole(Role));

            if (!newRole.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityRole>(Role + " " + AppMessages.RoleFailed, AppConstant.Error));
            }

            return Ok(new ApiResponse<IdentityRole>(AppConstant.Success, Role + " " + AppMessages.RoleSuccess));
        }

        [Route("UserRegistration")]
        [HttpPost]
        public async Task<IActionResult> UserRegistration([FromBody] UserRegistrationDTO registrationDTO)
        {
            var userExists = await _userManager.FindByNameAsync(registrationDTO.UserName);
            var roleId = false;

            if(userExists != null)
            {
                roleId = await _userManager.IsInRoleAsync(userExists, AppConstant.Admin);
            }            

            if (userExists != null && roleId)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityUser>(AppMessages.UserExist, AppConstant.Information));
            }

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = registrationDTO.UserName,
                Email = registrationDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(applicationUser, registrationDTO.Password);

            if (result.Succeeded && await _roleManager.RoleExistsAsync(AppConstant.User))
            {
                await _userManager.AddToRoleAsync(applicationUser, AppConstant.User);
            }

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityUser>(AppMessages.UserFailed, AppConstant.Error));
            }

            return Ok(new ApiResponse<IdentityUser>(AppMessages.UserSuccess, AppConstant.Success));

        }

        [Route("AdminRegistration")]
        [HttpPost]
        public async Task<IActionResult> AdminRegistration([FromBody] UserRegistrationDTO registrationDTO)
        {
            var userExists = await _userManager.FindByNameAsync(registrationDTO.UserName);

            if (userExists != null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityUser>(AppMessages.AdminExist, AppConstant.Information));
            }

            ApplicationUser applicationUser = new ApplicationUser()
            {
                UserName = registrationDTO.UserName,
                Email = registrationDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(applicationUser, registrationDTO.Password);

            if (result.Succeeded && await _roleManager.RoleExistsAsync(AppConstant.Admin))
            {
                await _userManager.AddToRoleAsync(applicationUser, AppConstant.Admin);
            }

            if (!result.Succeeded)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<IdentityUser>(AppMessages.AdminFailed, AppConstant.Error));
            }

            return Ok(new ApiResponse<IdentityUser>(AppMessages.AdminSuccess, AppConstant.Success));

        }

        [Route("UserLogin")]
        [HttpPost]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDTO userLoginDTO)
        {
            var user = await _userManager.FindByNameAsync(userLoginDTO.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, userLoginDTO.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                foreach (var role in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, role));
                }

                var authSignKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SymentricKey"]));

                var token = new JwtSecurityToken(
                    issuer: _config["JWT:ValidIssuers"],
                    audience: _config["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(4),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSignKey, SecurityAlgorithms.HmacSha256)
                );

                //Write file logging
                //_fileLogging.Information("User Login_" + DateTime.Now);

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });

            }
            return Unauthorized();
        }
    }
}