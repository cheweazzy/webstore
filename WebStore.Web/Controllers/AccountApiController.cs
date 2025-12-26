using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebStore.Model.DataModels;
using WebStore.ViewModels.VM;

namespace WebStore.Web.Controllers;

public class AccountApiController : BaseApiController
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JwtOptionsVm _jwtOptions;

    public AccountApiController(ILogger<AccountApiController> logger, IMapper mapper,
        IOptions<JwtOptionsVm> jwtOptions,
        SignInManager<User> signInManager, UserManager<User> userManager) : base(logger, mapper)
    {
        _jwtOptions = jwtOptions.Value;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginUserVm applicationUser)
    {
        try
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model");

            var result = await _signInManager.PasswordSignInAsync(applicationUser.Login, applicationUser.Password, true, false);
            if (!result.Succeeded) return BadRequest("Invalid credentials");

            var user = await _userManager.FindByEmailAsync(applicationUser.Login);
            var userRoles = await _userManager.GetRolesAsync(user);

            // Budowanie Claimów (informacji o użytkowniku zaszytych w tokenie)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, applicationUser.Login),
                new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddMinutes(_jwtOptions.TokenExpirationMinutes)).ToUnixTimeSeconds().ToString()),
            };
            claims.AddRange(userRoles.Select(ur => new Claim(ClaimTypes.Role, ur)));

            // Generowanie tokena
            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey)),
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(claims));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { access_token = encodedJwt, expires_in = token.ValidTo });
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, ex.Message);
            return BadRequest("Error occurred");
        }
    }
}