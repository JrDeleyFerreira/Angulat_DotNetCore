using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProEventos.Api.Extensions;
using ProEventos.Application.Dtos;

namespace ProEventos.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class RedesSociaisController : ControllerBase
{
    private readonly IRedeSocialService _redeSocialService;
    private readonly IEventoService _eventoService;
    private readonly IPalestranteService _palestranteService;

    public RedesSociaisController(IRedeSocialService RedeSocialService,
                                  IEventoService eventoService,
                                  IPalestranteService palestranteService)
    {
        _palestranteService = palestranteService;
        _redeSocialService = RedeSocialService;
        _eventoService = eventoService;
    }

    [HttpGet("evento/{eventoId}")]
    public async Task<IActionResult> GetByEvento(int eventoId)
    {
        try
        {
            if (!await AutorEvento(eventoId))
                return Unauthorized();

            var redeSocial = await _redeSocialService.GetAllByEventoIdAsync(eventoId);
            return redeSocial is null
                ? NoContent()
                : Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar Rede Social por Evento. Erro: {ex.Message}");
        }
    }

    [HttpGet("palestrante")]
    public async Task<IActionResult> GetByPalestrante()
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId());
            if (palestrante == null) return Unauthorized();

            var redeSocial = await _redeSocialService.GetAllByPalestranteIdAsync(palestrante.Id);
            return redeSocial is null
                ? NoContent()
                : Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar recuperar Rede Social por Palestrante. Erro: {ex.Message}");
        }
    }

    [HttpPut("evento/{eventoId}")]
    public async Task<IActionResult> SaveByEvento(int eventoId, RedeSocialDto[] models)
    {
        try
        {
            if (!await AutorEvento(eventoId))
                return Unauthorized();

            var redeSocial = await _redeSocialService.SaveByEvento(eventoId, models);
            return redeSocial is null
                ? NoContent()
                : Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar salvar Rede Social por Evento. Erro: {ex.Message}");
        }
    }

    [HttpPut("palestrante")]
    public async Task<IActionResult> SaveByPalestrante(RedeSocialDto[] models)
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId());
            if (palestrante == null) return Unauthorized();

            var redeSocial = await _redeSocialService.SaveByPalestrante(palestrante.Id, models);
            return redeSocial is null
                ? NoContent()
                : Ok(redeSocial);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar salvar Rede Social por Palestrante. Erro: {ex.Message}");
        }
    }

    [HttpDelete("evento/{eventoId}/{redeSocialId}")]
    public async Task<IActionResult> DeleteByEvento(int eventoId, int redeSocialId)
    {
        try
        {
            if (!await AutorEvento(eventoId))
                return Unauthorized();

            var redeSocial = await _redeSocialService.GetRedeSocialEventoByIdsAsync(
                eventoId, redeSocialId);

            return redeSocial is null
                ? NoContent()
                : await _redeSocialService.DeleteByEvento(eventoId, redeSocialId)
                   ? Ok(new { message = "Rede Social Deletada" })
                   : throw new Exception("Ocorreu um problem não específico ao tentar deletar Rede Social por Evento.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar Rede Social por Evento. Erro: {ex.Message}");
        }
    }

    [HttpDelete("palestrante/{redeSocialId}")]
    public async Task<IActionResult> DeleteByPalestrante(int redeSocialId)
    {
        try
        {
            var palestrante = await _palestranteService.GetPalestranteByUserIdAsync(User.GetUserId());
            if (palestrante == null) return Unauthorized();

            var redeSocial = await _redeSocialService.GetRedeSocialPalestranteByIdsAsync(
                palestrante.Id, redeSocialId);

            return redeSocial is null
                ? NoContent()
                : await _redeSocialService.DeleteByPalestrante(palestrante.Id, redeSocialId)
                   ? Ok(new { message = "Rede Social Deletada" })
                   : throw new Exception("Ocorreu um problem não específico ao tentar deletar Rede Social por Palestrante.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                $"Erro ao tentar deletar Rede Social por Palestrante. Erro: {ex.Message}");
        }
    }

    [NonAction]
    private async Task<bool> AutorEvento(int eventoId)
    {
        var evento = await _eventoService.GetEventoByIdAsync(User.GetUserId(), eventoId, false);
        return evento is not null;
    }
}