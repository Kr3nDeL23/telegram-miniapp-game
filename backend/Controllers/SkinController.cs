using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using System.Security.Claims;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Models.Request;
using Presentation.Common.Domain.Models.Response;
using Presentation.Common.Domain.Entities;

namespace Presentation.Controllers;

[Authorize]
[ApiController, Route("[Controller]")]
public class SkinController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public SkinController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var skins = _unitOfWork.SkinRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.DateCreation),
            include: x => x.Include(z => z.Users));

        List<SkinModel> models = new();

        foreach (SkinEntity skin in await skins.ToListAsync())
        {
            var model = _unitOfWork.Mapper.Map<SkinModel>(skin);

            model.IsBought = skin.Users.Any(x => x.Id == userId);

            models.Add(model);
        }

        return Ok(new Response(true) { Result = models });
    }

    [HttpPost("[Action]/{id}")]
    public async Task<IActionResult> Set(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId,
            include: x => x.Include(y => y.Skins));

        if (user.Skins.Any(x => x.Id == id) == false) return NotFound(new Response(false)
        { Result = MessageConstant.ExceptionNotFound });

        user.SkinId = id;
        await _unitOfWork.SaveChangesAsync();

        var response = new Response(true)
        {
            Result = _unitOfWork.Mapper.Map<UserModel>(user)
        };
        return Ok(response);
    }
    [HttpPost("[Action]/{id}")]
    public async Task<IActionResult> Buy(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var skin = await _unitOfWork.SkinRepository.GetAsync(x => x.Id == id);

        if (skin == null) return NotFound(new Response(false)
        { Result = MessageConstant.ExceptionNotFound });

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId, include: x => x.Include(y => y.Skins));

        if (user.Skins.Any(x => x.Id == id)) return Ok(new Response(true)
        { Result = MessageConstant.MessageAlreadyExists });

        if (user.BalanceCoin < skin.AvailableCoin) return BadRequest(new Response(true)
        { Result = MessageConstant.MessageAlreadyExists });
        var leagues = await _unitOfWork.LeagueRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

        user.Skins.Add(skin);
        user.BalanceCoin -= skin.AvailableCoin;

        user.League = leagues.FirstOrDefault(
            x => x.AvailableCoin < user.BalanceCoin);
            
        await _unitOfWork.SaveChangesAsync();

        var response = new Response(true)
        {
            Result = _unitOfWork.Mapper.Map<UserModel>(user)
        };
        return Ok(response);
    }
}

