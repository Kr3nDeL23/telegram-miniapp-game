using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Domain.Enums;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models.Response;

namespace Presentation.Controllers;
[Authorize]
[ApiController, Route("[Controller]")]
public class BoostController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public BoostController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> Levels()
    {
        var levels = _unitOfWork.LevelRepository.Queryable(
            orderBy: o => o.OrderBy(x => x.LevelType).OrderBy(x => x.Level));

        var response = new Response(true)
        {
            Result = _unitOfWork.Mapper.Map<List<LevelEntity>>(await levels.ToListAsync())
        };

        return Ok(response);
    }

    [HttpPost("[Action]/{levelType}")]
    public async Task<IActionResult> LevelUp(LevelTypeEnum levelType)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId);

        var currenctLevel = await _unitOfWork.LevelRepository.GetAsync(
            predicate: x => x.LevelType == levelType & x.Id == user.Id);

        if (levelType == LevelTypeEnum.LimitEnergyLevel)
            currenctLevel = user.LimitEnergyLevel;

        else if (levelType == LevelTypeEnum.MultipleClickLevel)
            currenctLevel = user.MultipleClickLevel;

        else if (levelType == LevelTypeEnum.RechargeEnergyLevel)
            currenctLevel = user.RechargeEnergyLevel;

        else if (levelType == LevelTypeEnum.RoBotLevel)
            currenctLevel = user.RoBotLevel;

        else return NotFound(
            new Response(false) { Result = MessageConstant.ExceptionNotFound });


        var targetLevel = await _unitOfWork.LevelRepository.GetAsync(
            predicate: x => x.LevelType == levelType
                & x.Level == (currenctLevel != null ? currenctLevel.Level + 1 : 1));

        if (targetLevel == null) return NotFound(
            new Response(false) { Result = MessageConstant.ExceptionNotFound });

        if (targetLevel.AvailableCoin > user.BalanceCoin) return BadRequest(new Response(false)
        { Result = MessageConstant.ExceptionBalanceNotEnough });
        
        var leagues = await _unitOfWork.LeagueRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

        user.League = leagues.FirstOrDefault(
            x => x.AvailableCoin < user.BalanceCoin);

        user.BalanceCoin -= targetLevel.AvailableCoin;

        if (levelType == LevelTypeEnum.LimitEnergyLevel)
            user.LimitEnergyLevel = targetLevel;

        else if (levelType == LevelTypeEnum.MultipleClickLevel)
            user.MultipleClickLevel = targetLevel;

        else if (levelType == LevelTypeEnum.RechargeEnergyLevel)
            user.RechargeEnergyLevel = targetLevel;

        else user.RoBotLevel = targetLevel;

        await _unitOfWork.SaveChangesAsync();

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<UserModel>(user) });
    }

}
