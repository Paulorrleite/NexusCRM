using MediatR;
using Microsoft.AspNetCore.Mvc;
using NexusCRM.Application.Organizations;
using NexusCRM.Application.Organizations.EditOrganization;
using NexusCRM.Application.Organizations.InactivateOrganization;
using NexusCRM.Application.Organizations.ListOrganizations;
using NexusCRM.Application.Organizations.RegisterOrganization;
using NexusCRM.Application.Organizations.SearchOrganizations;
using NexusCRM.Domain;

namespace NexusCRM.API.Features.Organizations;

[ApiController]
[Route("api/organizations")]
public sealed class OrganizationController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<OrganizationListItem>>> ListAsync(
        [FromQuery] ListOrganizationsQuery query)
    {
        var result = await sender.Send(query);

        return Ok(result);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyCollection<OrganizationListItem>>> SearchAsync(
        [FromQuery] SearchOrganizationsQuery query)
    {
        var result = await sender.Send(query);

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<RegisterOrganizationResult>> RegisterAsync(
        [FromBody] RegisterOrganizationCommand command)
    {
        try
        {
            var result = await sender.Send(command);

            return Created($"/api/organizations/{result.OrganizationId}", result);
        }
        catch (DomainException exception)
        {
            return BadRequestProblem(exception.Message);
        }
    }

    [HttpPut("{organizationId:guid}")]
    public async Task<IActionResult> EditAsync(
        Guid organizationId,
        [FromBody] EditOrganizationRequest request)
    {
        try
        {
            var command = new EditOrganizationCommand(
                organizationId,
                request.Name,
                request.Status);

            _ = await sender.Send(command);

            return NoContent();
        }
        catch (DomainException exception)
        {
            return DomainProblem(exception);
        }
    }

    [HttpDelete("{organizationId:guid}")]
    public async Task<IActionResult> InactivateAsync(Guid organizationId)
    {
        try
        {
            var command = new InactivateOrganizationCommand(organizationId);

            _ = await sender.Send(command);

            return NoContent();
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

public sealed record EditOrganizationRequest(
    string Name,
    NexusCRM.Domain.Organizations.OrganizationStatus Status);
