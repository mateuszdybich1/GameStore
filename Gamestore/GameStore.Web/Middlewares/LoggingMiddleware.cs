using System.Diagnostics;
using System.Net;
using System.Text;

namespace GameStore.Web.Middlewares;

public class LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, string infoLogsPath, string errorLogsPath)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<LoggingMiddleware> _logger = logger;
    private readonly string _infoLogsPath = infoLogsPath;
    private readonly string _errorLogsPath = errorLogsPath;

    private string _logContent = string.Empty;
    private Stopwatch _stopwatch;

    public async Task InvokeAsync(HttpContext context)
    {
        _stopwatch = Stopwatch.StartNew();

        var requestBody = await ReadBodyAsync(context.Request.Body);

        _logContent += $"\nRequest Details - IP Address: {context.Connection.RemoteIpAddress}\n";
        _logContent += $"Method: {context.Request.Method}\n";
        _logContent += $"Path: {context.Request.Path}\n";
        _logContent += $"QueryString: {context.Request.QueryString}\n";
        _logContent += $"Body: {requestBody}\n";

        using var bufferedRequest = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
        context.Request.Body = bufferedRequest;

        using var responseBody = new MemoryStream();
        var originalBodyStream = context.Response.Body;
        context.Response.Body = responseBody;

        responseBody.Seek(0, SeekOrigin.Begin);
        var responseBodyText = await ReadBodyAsync(responseBody);

        _logContent += $"Response Details - StatusCode: {context.Response.StatusCode}\n";
        _logContent += $"Body: {responseBodyText}\n";

        try
        {
            await _next(context);
            _stopwatch.Stop();
            _logContent += $"Elapsed Time: {_stopwatch.ElapsedMilliseconds} ms";
            _logContent += "---------------\n";

            _logger.LogInformation(_logContent);

            try
            {
                File.AppendAllText(_infoLogsPath, $"{DateTime.UtcNow}: {_logContent}", Encoding.UTF8);
            }
            catch (Exception fileEx)
            {
                _logger.LogError($"Error writing to file: {fileEx.Message}");
            }
        }
        catch (Exception ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            context.Response.ContentType = "text/plain";

            await context.Response.WriteAsync(ex.Message);

            await LogExceptionDetails(ex);
        }

        responseBody.Seek(0, SeekOrigin.Begin);
        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static async Task<string> ReadBodyAsync(Stream body)
    {
        using var reader = new StreamReader(body, Encoding.UTF8, true, 1024, true);
        return await reader.ReadToEndAsync();
    }

    private Task LogExceptionDetails(Exception ex)
    {
        _logContent += $"Exception Type: {ex.GetType().Name}, Message: {ex.Message}\n";

        if (ex.InnerException != null)
        {
            _logContent += $"Inner Exception Type: {ex.InnerException.GetType().Name}, Message: {ex.InnerException.Message}\n";
        }

        _logContent += $"Exception Details: {ex}\n";
        _logContent += $"Stack Trace: {ex.StackTrace}\n";

        _logContent += $"Elapsed Time: {_stopwatch.ElapsedMilliseconds} ms\n";
        _logContent += "---------------\n";

        _logger.LogError(_logContent);

        try
        {
            File.AppendAllText(_errorLogsPath, $"{DateTime.UtcNow}: {_logContent}", Encoding.UTF8);
        }
        catch (Exception fileEx)
        {
            _logger.LogError($"Error writing to file: {fileEx.Message}");
        }

        return Task.CompletedTask;
    }
}
