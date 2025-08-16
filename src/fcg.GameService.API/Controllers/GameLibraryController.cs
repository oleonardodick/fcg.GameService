using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.ProblemsDefinitions;
using fcg.GameService.API.UseCases.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class GameLibraryController : BaseApiController
    {
        private readonly IGameLibraryUseCase _gameLibraryUseCase;

        public GameLibraryController(IGameLibraryUseCase gameLibraryUseCase)
        {
            _gameLibraryUseCase = gameLibraryUseCase;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseGameLibraryDTO>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(nameof(id), "O ID deve ser informado.");

            var gameLibrary = await _gameLibraryUseCase.GetByIdAsync(id);

            return gameLibrary is null ? NotFound("Biblioteca de jogos não encontrada") : Success(gameLibrary);
        }

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseGameLibraryDTO>> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest(nameof(userId), "O id do usuário deve ser informado.");
                
            var gameLibrary = await _gameLibraryUseCase.GetByUserIdAsync(userId);

            return gameLibrary is null ? NotFound("Biblioteca de jogos não encontrada") : Success(gameLibrary);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            var createdGameLibrary = await _gameLibraryUseCase.CreateAsync(gameLibrary);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdGameLibrary.Id },
                createdGameLibrary
            );
        }

        [HttpPost("{libraryId}/addGame")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddGame(string libraryId, [FromBody] AddGameToLibraryDTO game)
        {
            if (string.IsNullOrWhiteSpace(libraryId))
                return BadRequest(nameof(libraryId), "O id da biblioteca deve ser informado.");

            if (await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                return BadRequest(nameof(game.Id), "Jogo já cadastrado");

            var success = await _gameLibraryUseCase.AddGameToLibraryAsync(libraryId, game);

            return success ? NoContent() : NotFound("Biblioteca de jogos não encontrada");
        }

        [HttpDelete("{libraryId}/removeGame")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveGame(string libraryId, [FromBody] RemoveGameFromLibraryDTO game)
        {
            if (string.IsNullOrWhiteSpace(libraryId))
                return BadRequest(nameof(libraryId), "O id da biblioteca deve ser informado.");

            if (!await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                return NotFound("Jogo não cadastrado");

            var success = await _gameLibraryUseCase.RemoveGameFromLibraryAsync(libraryId, game);

            return success ? NoContent() : NotFound("Biblioteca de jogos não encontrada");
        }
    }
}
