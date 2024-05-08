using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Presentation.Common;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Models.Response;
using Presentation.Common.Constants;

namespace Presentation.Controllers;
[Authorize]
[ApiController, Route("[Controller]")]
public class GameController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public GameController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpGet("[Action]")]
    public async Task<IActionResult> CheckRoBot()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId);

        if (user.RoBotLevel == null) return BadRequest(
            new Response(false) { Result = MessageConstant.ExceptionNotAccess });

        var difference = Math.Min(
            Math.Max((DateTime.UtcNow - user.LastClickDate).TotalMinutes, 0), user.RoBotLevel.Value * 60);

        if (difference < 30) return BadRequest(
            new Response(false) { Result = MessageConstant.ExceptionNotAccess });

        var differenceToSecond = difference * 60;

        var count = Convert.ToInt64(differenceToSecond) * user.MultipleClickLevel.Value;

        user.BalanceCoin += count;
        user.LastClickDate = DateTime.UtcNow;
        user.AvailableEnergy = user.LimitEnergyLevel.Value;

        var leagues = await _unitOfWork.LeagueRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

        user.League = leagues.FirstOrDefault(
            x => x.AvailableCoin < user.BalanceCoin);


        if (user.Squad != null)
        {
            user.Squad.BalanceCoin += count;
            user.Squad.League = leagues.FirstOrDefault(
                x => x.AvailableCoin < user.Squad.BalanceCoin);
        }

        await _unitOfWork.SaveChangesAsync();

        return Ok(new Response(true)
        {
            Result = new CheckRoBotResponse
            {
                Count = count,
                User = _unitOfWork.Mapper.Map<UserModel>(user)
            }
        });
    }
    [HttpPost("[Action]")]
    public async Task<IActionResult> Click(ClickRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId);

        if (request.Count <= user.AvailableEnergy)
        {
            user.BalanceCoin += request.Count;
            user.AvailableEnergy -= request.Count;
            user.LastClickDate = DateTime.UtcNow;

            var leagues = await _unitOfWork.LeagueRepository.Queryable(
                orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

            user.League = leagues.FirstOrDefault(
                x => x.AvailableCoin < user.BalanceCoin);

            if (user.Squad != null)
            {
                user.Squad.BalanceCoin += request.Count;
                user.Squad.League = leagues.FirstOrDefault(
                    x => x.AvailableCoin < user.Squad.BalanceCoin);
            }

            await _unitOfWork.SaveChangesAsync();
        }
        var response = new Response(true)
        {
            Result = _unitOfWork.Mapper.Map<UserModel>(user)
        };
        return Ok(response);
    }
}
public class ClickRequest
{
    [Required]
    public long Count { get; set; }
}
public class CheckRoBotResponse
{
    public long Count { get; set; }
    public UserModel User { get; set; }
}