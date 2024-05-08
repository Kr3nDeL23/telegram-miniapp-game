using AutoMapper;
using Presentation.Common.Domain.Contexts;
using Presentation.Common.Repositories;

namespace Presentation.Common;
public class UnitOfWork
{
    private IMapper _mapper;
    private LevelRepository _levelRepository;
    private UserRepository _userRepository;
    private SkinRepository _skinRepository;
    private SquadRepository _squadRepository;
    private TaskRepository _taskRepository;
    private ChallengeRepository _challengeRepository;
    private LeagueRepository _leagueRepository;
    private readonly ApplicationDBContext _context;
    public UnitOfWork(IMapper mapper, ApplicationDBContext context)
    {
        _mapper = mapper;
        _context = context;
    }
    public UserRepository UserRepository
    {
        get
        {
            if (_userRepository == null)
                _userRepository = new UserRepository(_context);
            return _userRepository;
        }
    }
    public SquadRepository SquadRepository
    {
        get
        {
            if (_squadRepository == null)
                _squadRepository = new SquadRepository(_context);
            return _squadRepository;
        }
    }
    public ChallengeRepository ChallengeRepository
    {
        get
        {
            if (_challengeRepository == null)
                _challengeRepository = new ChallengeRepository(_context);
            return _challengeRepository;
        }
    }
    public TaskRepository TaskRepository
    {
        get
        {
            if (_taskRepository == null)
                _taskRepository = new TaskRepository(_context);
            return _taskRepository;
        }
    }
    public LevelRepository LevelRepository
    {
        get
        {
            if (_levelRepository == null)
                _levelRepository = new LevelRepository(_context);
            return _levelRepository;
        }
    }
    public SkinRepository SkinRepository
    {
        get
        {
            if (_skinRepository == null)
                _skinRepository = new SkinRepository(_context);
            return _skinRepository;
        }
    }

    public LeagueRepository LeagueRepository
    {
        get
        {
            if (_leagueRepository == null)
                _leagueRepository = new LeagueRepository(_context);
            return _leagueRepository;
        }
    }
    public IMapper Mapper
    {
        get
        {
            return _mapper;
        }
    }
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
