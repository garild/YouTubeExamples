using Microsoft.AspNetCore.Mvc;

public static class ProblemDetailsExtension
{
    extension(Exception ex)
    {
        public bool IsBadRequest => ex is ArgumentException or InvalidOperationException or FormatException;
        public ProblemDetails ToProblem()
        {
            var problem = new ProblemDetails
            {
                Detail = "Internal Error",
                Status = 500,
                Instance = "/api/order"
            };

            return problem;
        }
    }
}


public static class CollectionExteions
{
    extension(List<int> data)
    {
        public bool IsEmpty => data is not null && data.Count > 0;
    }
}