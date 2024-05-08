using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

using Presentation.Common;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models.Request;
using Presentation.Common.Domain.Models.Response;


namespace Presentation.Controllers;

[Authorize]
[ApiController, Route("[Controller]")]
public class LeagueController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public LeagueController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> List()
    {
        var leagues = _unitOfWork.LeagueRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.AvailableCoin));

        var response = new Response(true)
        {
            Result = _unitOfWork.Mapper.Map<List<LeagueModel>>(await leagues.ToListAsync())
        };

        return Ok(response);
    }
    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> Members(string id, [FromQuery] SearchRequest filter, [FromQuery] PaginationRequest pagination)
    {
        var users = _unitOfWork.UserRepository.Queryable(
            predicate: x => x.LeagueId.Equals(id) & x.Name.ToLower().Contains(filter.Query),
            orderBy: o => o.OrderByDescending(x => x.BalanceCoin),
            skip: (pagination.Page - 1) * pagination.Size, take: pagination.Size);

        var response = new PaginationResponse<UserModel>(
            _unitOfWork.Mapper.Map<List<UserModel>>(await users.ToListAsync()), pagination);

        return Ok(response);
    }
    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> Squads(string id, [FromQuery] SearchRequest filter, [FromQuery] PaginationRequest pagination)
    {
        var squads = _unitOfWork.SquadRepository.Queryable(
            predicate: x => x.LeagueId.Equals(id) & (x.Name.ToLower().Contains(filter.Query)
                || x.UserName.ToLower().Contains(filter.Query)),
            orderBy: o => o.OrderByDescending(x => x.BalanceCoin),
            skip: (pagination.Page - 1) * pagination.Size, take: pagination.Size);
        var response = new PaginationResponse<SquadModel>(
            _unitOfWork.Mapper.Map<List<SquadModel>>(await squads.ToListAsync()), pagination);

        return Ok(response);
    }
}
