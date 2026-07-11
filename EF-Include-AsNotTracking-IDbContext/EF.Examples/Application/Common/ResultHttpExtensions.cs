using Microsoft.AspNetCore.Http;

namespace EF.Examples.Application.Common;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result) =>
        result switch
        {
            { IsSuccess: true, HasData: true } => Results.Ok(result.Value),
            { IsSuccess: true, HasData: false } => Results.NotFound(),
            _ => Results.BadRequest(result.Error)
        };

    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? Results.NoContent() : Results.BadRequest(result.Error);
}
