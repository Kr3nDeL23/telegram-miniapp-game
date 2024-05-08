using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Models.Request;
using Presentation.Common.Domain.Models.Response;

namespace Presentation.Controllers;

[Authorize]
[ApiController, Route("[Controller]")]
public class SquadController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public SquadController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> Search([FromQuery] SearchRequest filter, [FromQuery] PaginationRequest pagination)
    {
        var squads = _unitOfWork.SquadRepository.Queryable(
            predicate: x => x.Name.ToLower().Contains(filter.Query)
                || x.UserName.ToLower().Contains(filter.Query),
            orderBy: o => o.OrderByDescending(x => x.BalanceCoin),
            skip: (pagination.Page - 1) * pagination.Size, take: pagination.Size);

        var response = new PaginationResponse<SquadModel>(
            _unitOfWork.Mapper.Map<List<SquadModel>>(await squads.ToListAsync()), pagination);

        return Ok(response);
    }
    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> Members(string id, [FromQuery] SearchRequest filter, [FromQuery] PaginationRequest pagination)
    {
        var users = _unitOfWork.UserRepository.Queryable(
            predicate: x => x.SquadId == id & x.Name.ToLower().Contains(filter.Query),
            orderBy: o => o.OrderByDescending(x => x.BalanceCoin),
            skip: (pagination.Page - 1) * pagination.Size, take: pagination.Size);

        var response = new PaginationResponse<UserModel>(
            _unitOfWork.Mapper.Map<List<UserModel>>(await users.ToListAsync()), pagination);

        return Ok(response);
    }
    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var squad = await _unitOfWork.SquadRepository.GetAsync(
            predicate: x => x.Id == id);

        if (squad == null) return NotFound(new Response(false)
        { Result = MessageConstant.ExceptionNotFound });

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<SquadModel>(squad) });
    }
    [HttpPost("[Action]/{id}")]
    public async Task<IActionResult> Join(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var squad = await _unitOfWork.SquadRepository.GetAsync(
            predicate: x => x.Id == id);

        if (squad == null) return NotFound(new Response(false)
        { Result = MessageConstant.ExceptionNotFound });

        var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId);

        user.Squad = squad;

        await _unitOfWork.SaveChangesAsync();

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<UserModel>(user) });
    }
    [HttpPost("[Action]")]
    public async Task<IActionResult> Left()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId);

        if (user.Squad != null)
            user.Squad.Members.Remove(user);

        await _unitOfWork.SaveChangesAsync();

        return Ok(new Response(true)
        { Result = _unitOfWork.Mapper.Map<UserModel>(user) });
    }
}
