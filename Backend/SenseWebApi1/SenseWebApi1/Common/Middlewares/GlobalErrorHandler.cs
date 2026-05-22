using System.Text.Json;
using SC.Internship.Common.Exceptions;
using SC.Internship.Common.ScResult;
using SenseWebApi1.domain.Exceptions;

namespace SenseWebApi1.Common.Middlewares;

public class GlobalErrorHandler
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalErrorHandler> _logger;

    public GlobalErrorHandler(RequestDelegate next, ILogger<GlobalErrorHandler> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch(ArgumentException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch(ExceptionsAdapter ex)
        {
            await HandleExceptionAsync(httpContext, ex.Exceptions);
                
        }
        catch(ScException ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }

    }
    private  async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var response = context.Response;
        context.Response.StatusCode = 400;
        // ReSharper disable once SuggestVarOrType_BuiltInTypes
        string result = exception.Message;
        _logger.LogError(exception.Message);
        var jsonError = JsonSerializer.Serialize(new ScResult { Error = new ScError { Message = result } });
        await response.WriteAsync(jsonError);
            
    }
    private async Task HandleExceptionAsync(HttpContext context, Dictionary<string, List<string>> errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = 400;
        var jsonError = JsonSerializer.Serialize(new ScResult { Error = new ScError { ModelState = errors } });
        _logger.LogError(jsonError);
        await context.Response.WriteAsync(jsonError);
    }
}