using DotNetCoreAssignments.Enums;
using DotNetCoreAssignments.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DotNetCoreAssignments.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticateController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginUser loginUser)
        {
            var user = await userManager.FindByNameAsync(loginUser?.Username);
            if (user != null && await userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                var userRoles = await userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }
            return NotFound("User does not exist...");
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUser registerUser)
        {
            var userExists = await userManager.FindByNameAsync(registerUser.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username
            };
            var result = await userManager.CreateAsync(user, registerUser.Password);

            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    sb.AppendLine(item.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed. " + sb });
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));

            if (await roleManager.RoleExistsAsync(UserRoles.User.ToString()))
            {
                switch(registerUser.Role)
                {
                    case "User":
                        await userManager.AddToRoleAsync(user, UserRoles.User.ToString());
                        break;
                    case "User1":
                        await userManager.AddToRoleAsync(user, UserRoles.User1.ToString());
                        break;
                }
            }
                
            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterUser registerUser)
        {
            var userExists = await userManager.FindByNameAsync(registerUser.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User already exists!" });

            ApplicationUser user = new ApplicationUser()
            {
                Email = registerUser.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerUser.Username
            };
            var result = await userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var item in result.Errors)
                {
                    sb.AppendLine(item.Description);
                }
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed. " + sb });
            }
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin.ToString()));
            if (!await roleManager.RoleExistsAsync(UserRoles.User.ToString()))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.User.ToString()));

            if (await roleManager.RoleExistsAsync(UserRoles.Admin.ToString()))
            {
                await userManager.AddToRoleAsync(user, UserRoles.Admin.ToString());
            }

            return Ok(new Response { Status = "Success", Message = "User created successfully!" });
        }
    }
}
