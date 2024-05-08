using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Extensions;
using Presentation.Common.Domain.Configurations;
using Presentation.Common.Domain.Models.Response;
using Presentation.Common.Domain.Models;

namespace Presentation.Controllers;

[ApiController, Route("[Controller]")]
public class TokenController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    private readonly TokenConfiguration _tokenConfiguration;
    private readonly TelegramConfiguration _telegramConfiguration;
    public TokenController(UnitOfWork unitOfWork, IOptions<TokenConfiguration> tokenConfiguration, IOptions<TelegramConfiguration> telegramConfiguration)
    {
        _unitOfWork = unitOfWork;
        _tokenConfiguration = tokenConfiguration.Value;
        _telegramConfiguration = telegramConfiguration.Value;
    }

    [HttpPost("[Action]")]
    public async Task<IActionResult> SignIn(SignInRequest request)
    {
        if (TelegramExtension.ValidateWebApp(
            request.WebAppData, _telegramConfiguration.Token, out WebAppUserRequest webAppUser))
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == webAppUser.ChatId);

            if (user != null)
            {
                var claims = new[] {
                    new Claim(ClaimTypes.Sid, user.Id),
                    new Claim(ClaimTypes.Name, user.Name),
                };
                var (accessToken, expiration) = TokenExtension.GenerateAccessToken(_tokenConfiguration, claims);

                var levels = await _unitOfWork.LevelRepository.Queryable(
                    orderBy: o => o.OrderBy(x => x.LevelType).OrderBy(x => x.Level)).ToListAsync();

                var leagues = _unitOfWork.LeagueRepository.Queryable(
                    orderBy: o => o.OrderBy(x => x.AvailableCoin));

                var result = new SignInResponse
                {
                    User = _unitOfWork.Mapper.Map<UserModel>(user),
                    Levels = _unitOfWork.Mapper.Map<List<LevelModel>>(levels),
                    Leagues = _unitOfWork.Mapper.Map<List<LeagueModel>>(leagues),

                    Token = new TokenResponse
                    {
                        AccessToken = accessToken,
                        AccessTokenExpiration = expiration
                    }
                };

                return Ok(new Response(true) { Result = result });
            }
        }
        return BadRequest(new Response(false) { Result = MessageConstant.ExceptionNotFound });
    }
}
public class SignInRequest
{
    [Required]
    public string WebAppData { get; set; }
}

public class TokenResponse
{
    public string AccessToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
}

public class SignInResponse
{

    public TokenResponse Token { get; set; }
    public UserModel User { get; set; }
    public List<LevelModel> Levels { get; set; }
    public List<LeagueModel> Leagues { get; set; }
}
