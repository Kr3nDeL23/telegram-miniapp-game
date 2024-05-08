
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Models.Response;
using Presentation.Common.Domain.Models.Request;

namespace Presentation.Controllers;

[Authorize]
[ApiController, Route("[Controller]")]
public class UserController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public UserController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId);

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<UserModel>(user) });
    }
    [HttpPost("[Action]")]
    public async Task<IActionResult> Transfer(TransferRequest request)
    {

        if (request.Count < 10_000) return BadRequest(new Response(false));

        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var target = await _unitOfWork.UserRepository.GetAsync(x => x.TelegramId == request.UserId);

        if (target == null) return NotFound(new Response(false)
        { Result = MessageConstant.ExceptionNotFound });

        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId);

        if (user.BalanceCoin < request.Count) return BadRequest(new Response(false)
        { Result = MessageConstant.ExceptionBalanceNotEnough });

        var leagues = await _unitOfWork.LeagueRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

        user.BalanceCoin -= request.Count;
        target.BalanceCoin += request.Count;

        user.League = leagues.FirstOrDefault(
            x => x.AvailableCoin < user.BalanceCoin);

        target.League = leagues.FirstOrDefault(
            x => x.AvailableCoin < target.BalanceCoin);

        await _unitOfWork.SaveChangesAsync();

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<UserModel>(user) });
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> Members([FromQuery] SearchRequest filter, [FromQuery] PaginationRequest pagination)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var users = _unitOfWork.UserRepository.Queryable(
            predicate: x => x.ParentId == userId & x.Name.ToLower().Contains(filter.Query),
            orderBy: o => o.OrderByDescending(x => x.BalanceCoin),
            skip: (pagination.Page - 1) * pagination.Size, take: pagination.Size);

        var response = new PaginationResponse<UserModel>(
            _unitOfWork.Mapper.Map<List<UserModel>>(await users.ToListAsync()), pagination);

        return Ok(response);
    }
}

public class TransferRequest
{
    [Required]
    public long Count { get; set; }
    [Required]
    public long UserId { get; set; }
}
