using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Filters;

using Presentation.Common.Domain.Configurations;

namespace Presentation.Telegram;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ValidateTelegramBotAttribute : TypeFilterAttribute
{
    public ValidateTelegramBotAttribute()
        : base(typeof(ValidateTelegramBotFilter))
    {
    }

    private class ValidateTelegramBotFilter : IActionFilter
    {
        private readonly string _secret;

        public ValidateTelegramBotFilter(IOptions<TelegramConfiguration> options)
        {
            _secret = options.Value.Secret;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!IsValidRequest(context.HttpContext.Request))
            {
                context.Result = new ObjectResult("\"X-Telegram-Bot-Api-Secret-Token\" is invalid")
                {
                    StatusCode = 403
                };
            }
        }

        private bool IsValidRequest(HttpRequest request)
        {

            if (!request.Headers.TryGetValue("X-Telegram-Bot-Api-Secret-Token", out var secretTokenHeader)) 
                return false;

            return string.Equals(secretTokenHeader, _secret, StringComparison.Ordinal);
        }
    }
}