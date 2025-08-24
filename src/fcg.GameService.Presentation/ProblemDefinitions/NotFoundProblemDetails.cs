using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.Presentation.ProblemDefinitions;

public class NotFoundProblemDetails : ProblemDetails
{
    public NotFoundProblemDetails(string detail)
    {
        Title = "Recurso não encontrado";
        Type = "https://httpstatuses.com/404";
        Status = StatusCodes.Status404NotFound;
        Detail = detail;
    }
}
