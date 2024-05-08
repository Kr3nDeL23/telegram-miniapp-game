using Microsoft.AspNetCore.Mvc;

using Telegram.Bot.Types;

using Presentation.Telegram;
using Telegram.Bot;

namespace Presentation.Controllers;

[ApiController, Route("[Controller]")]
public class TelegramController : ControllerBase
{
    private readonly UpdateHandler _handler;
    private readonly ITelegramBotClient _client;

    public TelegramController(ITelegramBotClient client, UpdateHandler handler)
    {
        _client = client;
        _handler = handler;
    }
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        await _handler.HandleUpdateAsync(
            _: _client,
            update: update,
            cancellationToken: CancellationToken.None);
        return Ok();
    }
}
