using MediatR;
using Microsoft.AspNetCore.Mvc;
using PropostaService.Application.Propostas.Commands;
using PropostaService.Application.Propostas.Queries;

namespace PropostaService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropostasController : ControllerBase
{
    private readonly IMediator _mediator;

    #region DTO
    public class RecusarPropostaRequest
    {
        public string Motivo { get; set; }
    }

    public class EditarPropostaRequest
    {
        public string NomeCliente { get; set; }
        public decimal Valor { get; set; }
    }
    #endregion

    public PropostasController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> ListarPropostas()
    {
        var query = new ListarPropostasQuery();
        var resultado = await _mediator.Send(query);
        return Ok(resultado);
    }

    [HttpPost]
    public async Task<IActionResult> CriarProposta([FromBody] CriarPropostaCommand command)
    {
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(CriarProposta), new { id = result.Value }, result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> ObterPropostaPorId(Guid id)
    {
        var query = new ObterPropostaPorIdQuery { Id = id };
        var proposta = await _mediator.Send(query);

        return proposta is not null ? Ok(proposta) : NotFound();
    }

    [HttpPut("{id}/aprovar")]
    public async Task<IActionResult> AprovarProposta(Guid id)
    {
        var command = new AprovarPropostaCommand { PropostaId = id };
        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Proposta.NaoEncontrada")
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> EditarProposta(Guid id, [FromBody] EditarPropostaRequest request)
    {
        var command = new EditarPropostaCommand
        {
            PropostaId = id,
            NomeCliente = request.NomeCliente,
            Valor = request.Valor
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Proposta.NaoEncontrada")
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }

    [HttpPut("{id}/recusar")]
    public async Task<IActionResult> RecusarProposta(Guid id, [FromBody] RecusarPropostaRequest request)
    {
        var command = new RecusarPropostaCommand
        {
            PropostaId = id,
            Motivo = request.Motivo
        };

        var result = await _mediator.Send(command);

        if (result.IsFailure)
        {
            if (result.Error.Code == "Proposta.NaoEncontrada")
            {
                return NotFound(result.Error);
            }
            return BadRequest(result.Error);
        }

        return NoContent();
    }
}