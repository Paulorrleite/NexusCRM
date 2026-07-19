using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexusCRM.Application.Plans;
using NexusCRM.Application.Plans.DeletePlan;
using NexusCRM.Application.Plans.EditPlan;
using NexusCRM.Application.Plans.ListPlans;
using NexusCRM.Application.Plans.RegisterPlan;
using NexusCRM.Domain;
using NexusCRM.Domain.Organizations;

namespace NexusCRM.API.Features.Plans;

[ApiController]
[Route("api/plans")]
public sealed class PlanController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<PlanListItem>>> ListAsync(ListPlansQuery query)
    {
        var result = await sender.Send(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<RegisterPlanResult>> RegisterAsync([FromBody] RegisterPlanCommand command)
    {
        try
        {
            var result = await sender.Send(command);

            return Created($"/api/plans/{result.PlanId}", result);
        }
        catch (DomainException exception)
        {
            return BadRequestProblem(exception.Message);
        }
    }

    [HttpPut("{planId:guid}")]
    public async Task<IActionResult> EditAsync([FromBody] EditPlanCommand command)
    {
        try
        {
            var result = await sender.Send(command);

            return result.Updated ? NoContent() : Conflict(result);
        }
        catch (DomainException exception)
        {
            return DomainProblem(exception);
        }
    }

    [HttpDelete("{planId:guid}")]
    public async Task<IActionResult> DeleteAsync(DeletePlanCommand command)
    {
        try
        {
            var result = await sender.Send(command);

            return result.Deleted ? NoContent() : Conflict(result);
        }
        catch (DomainException exception)
        {
            return DomainProblem(exception);
        }
    }

    private ActionResult DomainProblem(DomainException exception)
    {
        if (exception.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
        {
            return NotFoundProblem(exception.Message);
        }

        return BadRequestProblem(exception.Message);
    }

    private BadRequestObjectResult BadRequestProblem(string detail)
    {
        return BadRequest(new ProblemDetails
        {
            Title = "Invalid request.",
            Detail = detail,
            Status = StatusCodes.Status400BadRequest
        });
    }

    private NotFoundObjectResult NotFoundProblem(string detail)
    {
        return NotFound(new ProblemDetails
        {
            Title = "Resource not found.",
            Detail = detail,
            Status = StatusCodes.Status404NotFound
        });
    }
}
