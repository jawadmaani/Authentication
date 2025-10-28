using Authentication.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Report_System_Backend.middleware;

[ApiController]
public class GlobalExceptionHandler : ControllerBase
{
    [Route("/error")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult HandleError()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();

        if (context?.Error is EmptyDataBaseFromUsers emptyDbEx)
            return NotFound(new { message = emptyDbEx.Message });

        if (context?.Error is InvalidCredentialsException)
            return Unauthorized(new { message = context.Error.Message });

        if (context?.Error is UserAlreadyExistsException)
            return Conflict(new { message = context.Error.Message });

        return Problem(detail: context?.Error.Message, statusCode: 500);
    }
}