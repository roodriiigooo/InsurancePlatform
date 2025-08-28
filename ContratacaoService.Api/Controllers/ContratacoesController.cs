using ContratacaoService.Application.Contratacoes.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContratacaoService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContratacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ContratacoesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContratacaoDto>>> ListarContratacoes()
    {
        var query = new ListarContratacoesQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }
}