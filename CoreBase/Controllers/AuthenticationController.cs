using CoreBase.Authentication;
using CoreBase.Dtos;
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

namespace CoreBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        #region Declared Variables

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        #endregion

        #region Constructor
        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;

        }
        #endregion

        #region Register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto model)
        {
            try
            {
                var userExist = await _userManager.FindByNameAsync(model.UserName);

                //check if user exist
                if (userExist != null) return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Message = "User name already exists!", IsError = true });

                ApplicationUser user = new ApplicationUser
                {
                    Email = model.Email,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    UserName = model.UserName
                };

                //create user
                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded) return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Message = "Error creating user", IsError = true });

                //create user role if doesnot exist
                if (!await _roleManager.RoleExistsAsync(UserRoles.User)) await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

                //assign user role
                await _userManager.AddToRoleAsync(user, UserRoles.User);

                return Ok(new ResponseDto { Message = "User created successfully", IsError = false });
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Message = e.Message, IsError = true });
            }
        }
        #endregion

        #region Login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto model)
        {
            try
            {
                //check user by username
                var user = await _userManager.FindByNameAsync(model.UserName);

                // if user by username exists and password match
                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var userRoles = await _userManager.GetRolesAsync(user);
                    var authClaims = new List<Claim> {
                    new Claim(ClaimTypes.Name,user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())};

                    //add roles to claims
                    foreach(var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var authSigninKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
                    
                    var token = new JwtSecurityToken(
                        issuer : _configuration["JWT:ValidIssuer"],
                        audience : _configuration["JWT:ValidAudience"],
                        expires : DateTime.Now.AddDays(15),
                        claims : authClaims,
                        signingCredentials : new SigningCredentials(authSigninKey,SecurityAlgorithms.HmacSha256)
                        );
                    
                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiresAt= token.ValidTo
                    });
                }
                
                return Unauthorized();
            }
            catch (Exception e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDto { Message = e.Message, IsError = true });
            }
        }
        #endregion
    }
}
