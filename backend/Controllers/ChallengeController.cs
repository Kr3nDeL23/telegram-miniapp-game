using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Presentation.Common;
using Presentation.Common.Constants;
using Presentation.Common.Domain.Entities;
using Presentation.Common.Domain.Models;
using Presentation.Common.Domain.Models.Response;

namespace Presentation.Controllers;

[Authorize]
[ApiController, Route("[Controller]")]
public class ChallengeController : ControllerBase
{
    private readonly UnitOfWork _unitOfWork;
    public ChallengeController(UnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("[Action]")]
    public async Task<IActionResult> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var challenges = _unitOfWork.ChallengeRepository.Queryable(
            orderBy: o => o.OrderByDescending(x => x.DateCreation),
            include: x => x.Include(z => z.Users));

        List<ChallengeModel> models = new();

        foreach (ChallengeEntity challenge in await challenges.ToListAsync())
        {
            var model = _unitOfWork.Mapper.Map<ChallengeModel>(challenge);

            model.IsCompleted = challenge.Users.Any(x => x.Id == userId);
            models.Add(model);
        }
        return Ok(new Response(true)
        { Result = models });
    }


    [HttpGet("[Action]/{id}")]
    public async Task<IActionResult> GetTasks(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var tasks = _unitOfWork.TaskRepository.Queryable(
            predicate: x => x.ChallengeId == id,
            orderBy: o => o.OrderByDescending(x => x.DateCreation),
            include: x => x.Include(z => z.Users));

        List<TaskModel> models = new();

        foreach (TaskEntity task in await tasks.ToListAsync())
        {
            var model = _unitOfWork.Mapper.Map<TaskModel>(task);

            model.IsCompleted = task.Users.Any(x => x.Id == userId);
            models.Add(model);
        }
        return Ok(new Response(true)
        { Result = models });
    }
    [HttpPost("[Action]/{id}")]
    public async Task<IActionResult> CompletTask(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var task = await _unitOfWork.TaskRepository.GetAsync(
            predicate: x => x.Id == id,
            include: i => i.Include(x => x.Users)
        );


        if (task == null) return NotFound(
            new Response(false) { Result = MessageConstant.ExceptionNotFound });

        if (!task.Users.Any(x => x.Id == userId))
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userId);

            task.Users.Add(user);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new Response(true)
            {
                Result = new CompletTaskResponse
                {
                    IsCompleted = false,
                    Path = task.Path
                }
            });
        }

        return Ok(new Response(true)
        {
            Result = new CompletTaskResponse
            {
                IsCompleted = true,
                Path = task.Path
            }
        });
    }
    [HttpPost("[Action]/{id}")]
    public async Task<IActionResult> CompletChallenge(string id)
    {
        var userId = User.FindFirstValue(ClaimTypes.Sid);

        var tasks = _unitOfWork.TaskRepository.Queryable(
            predicate: x => x.ChallengeId == id,
            include: i => i.Include(x => x.Users).Include(x => x.Challenge)
        );

        foreach (var task in tasks)
            if (!task.Users.Any(x => x.Id == userId)) return BadRequest(
                new Response(false) { Result = MessageConstant.ExceptionNotAccess });

        var user = await _unitOfWork.UserRepository.GetAsync(
            predicate: x => x.Id == userId
        );
        var challenge = await _unitOfWork.ChallengeRepository.GetAsync(
            predicate: x => x.Id == id,
            include: i => i.Include(x => x.Users)
        );

        if (!challenge.Users.Any(x => x == user))
        {
            challenge.Users.Add(user);

            user.BalanceCoin += challenge.Bonus;

            var leagues = await _unitOfWork.LeagueRepository.Queryable(
                orderBy: o => o.OrderByDescending(x => x.AvailableCoin)).ToListAsync();

            user.League = leagues.FirstOrDefault(
                x => x.AvailableCoin < user.BalanceCoin);

            await _unitOfWork.SaveChangesAsync();
        }

        var map = _unitOfWork.Mapper.Map<ChallengeModel>(challenge);

        map.IsCompleted = challenge.Users.Any(x => x.Id == userId);

        return Ok(new Response(true)
        {
            Result = new CompletChallengeResponse
            {
                User = _unitOfWork.Mapper.Map<UserModel>(user),
                Challenge = map
            }
        });
    }
}
public class CompletTaskResponse
{
    public string Path { get; set; }
    public bool IsCompleted { get; set; }
}
public class CompletChallengeResponse
{
    public UserModel User { get; set; }
    public ChallengeModel Challenge { get; set; }
}