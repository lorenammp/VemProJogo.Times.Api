using Microsoft.AspNetCore.Mvc;
using VemProJogo.Times.Application.Exceptions;

namespace VemProJogo.Times.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title) = MapException(exception);

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro nao tratado durante o processamento da requisicao.");
        }
        else
        {
            _logger.LogWarning(exception, "Falha de negocio durante o processamento da requisicao.");
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Type = $"https://httpstatuses.com/{statusCode}",
            Title = title,
            Detail = exception.Message,
            Status = statusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions["traceId"] = context.TraceIdentifier;

        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int statusCode, string title) MapException(Exception exception) =>
        exception switch
        {
            ResourceNotFoundException or KeyNotFoundException
                => (StatusCodes.Status404NotFound, "Recurso nao encontrado"),
            BusinessValidationException or ArgumentException
                => (StatusCodes.Status400BadRequest, "Erro de validacao"),
            ConflictException or InvalidOperationException
                => (StatusCodes.Status409Conflict, "Conflito de negocio"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };
}
