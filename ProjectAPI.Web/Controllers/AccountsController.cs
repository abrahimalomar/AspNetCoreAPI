using Core.Model;
using Core.ViewModel;
using Microsoft.AspNetCore.Authorization;
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

namespace ProjectAPI.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AccountsController(UserManager<User> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }



        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] UserView userView)
        {
            if (ModelState.IsValid)
            {
                User user = new User
                {
                    UserName = userView.UserName,
                    Email = userView.Email,
                    PhoneNumber = userView.PhoneNumber,
                };
                IdentityResult result = await _userManager.CreateAsync(user, userView.Password);

                if (result.Succeeded)
                {
                    return Ok("Succeeded");
                }
                else
                {
                    foreach (var item in result.Errors)
                    {
                        ModelState.AddModelError("", item.Description);
                    }
                }
            }
            return BadRequest();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> LogIn([FromForm]LoginView login)
        {
            if (ModelState.IsValid)
            {
                User user = await _userManager.FindByNameAsync(login.UserName);
                if (user!=null)
                {
                    var check = await _userManager.CheckPasswordAsync (user, login.Password);
                    if (check)
                    {
                        var claims = new List<Claim>();
                        claims.Add(new Claim("name","value"));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id));
                        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                        var roles = await _userManager.GetRolesAsync(user);
                        foreach (var role in roles)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, role.ToString()));
                        }
                        //signingCredentials
                        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]));
                        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                        var token = new JwtSecurityToken
                            (


                            claims: claims,
                            issuer:_configuration["JWT:Issuer"],
                            audience:_configuration["JWT:Audience"],
                            expires: DateTime.Now.AddSeconds(30),
                            signingCredentials:signingCredentials
                            ) ;

                        var _token = new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        };
                        return Ok(_token);
                      
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
                else
                {
                    ModelState.AddModelError("", " User Name is Invalid");  
                }
            }
            return BadRequest();
        }



        [HttpDelete("Delete/{userId}")]
        public async Task<IActionResult> DeleteById(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok("User deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete user");
            }
        }



        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteByUsername([FromForm] string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user == null)
            {
                return NotFound("User not found");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return Ok("User deleted successfully");
            }
            else
            {
                return BadRequest("Failed to delete user");
            }
        }



        [HttpPut("Update")]
 
        public async Task<IActionResult> UpdateUserByUsername([FromForm] UserView update)
        {
            User user = await _userManager.FindByNameAsync(update.UserName);

            if (user == null)
            {
                return NotFound("User not found");
            }

            user.UserName = update.UserName;
            user.PhoneNumber = update.PhoneNumber;
            user.Email = update.Password;
          
            if (!string.IsNullOrWhiteSpace(update.Password))
            {
                var newPasswordHash = _userManager.PasswordHasher.HashPassword(user, update.Password);
                user.PasswordHash = newPasswordHash;
            }

            
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok("User updated successfully");
            }
            else
            {
                return BadRequest("Failed to update user");
            }
        }

    }
}
