using fcg.GameService.API.DTOs;
using fcg.GameService.API.DTOs.GameLibrary;
using fcg.GameService.API.DTOs.GameLibrary.Requests;
using fcg.GameService.API.DTOs.Responses;
using fcg.GameService.API.Helpers;
using fcg.GameService.API.ProblemsDefinitions;
using fcg.GameService.API.UseCases.Interfaces;
using FluentValidation;
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
        private readonly IValidator<CreateGameLibraryDTO> _validatorCreate;
        private readonly IValidator<AddGameToLibraryDTO> _validatorAddGame;
        private readonly IValidator<RemoveGameFromLibraryDTO> _validatorRemoveGame;

        public GameLibraryController(
            IGameLibraryUseCase gameLibraryUseCase,
            IValidator<CreateGameLibraryDTO> validatorCreate,
            IValidator<AddGameToLibraryDTO> validatorAddGame,
            IValidator<RemoveGameFromLibraryDTO> validatorRemoveGame)
        {
            _gameLibraryUseCase = gameLibraryUseCase;
            _validatorCreate = validatorCreate;
            _validatorAddGame = validatorAddGame;
            _validatorRemoveGame = validatorRemoveGame;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseGameLibraryDTO>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(id),
                    Errors = ["O Id deve ser informado."]
                });

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
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(userId),
                    Errors = ["O Id do usuário deve ser informado."]
                });
                
            var gameLibrary = await _gameLibraryUseCase.GetByUserIdAsync(userId);

            return gameLibrary is null ? NotFound("Biblioteca de jogos não encontrada") : Success(gameLibrary);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameLibraryDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            var validation = ValidationHelper.Validate(_validatorCreate, gameLibrary);

            if (validation.Count > 0)
                return BadRequest(validation);
                
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
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(libraryId),
                    Errors = ["O Id da biblioteca deve ser informada."]
                });

            if (await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(game.Id),
                    Errors = ["Jogo já cadastrado."]
                });

            var validation = ValidationHelper.Validate(_validatorAddGame, game);

            if (validation.Count > 0)
                return BadRequest(validation);

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
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(libraryId),
                    Errors = ["O Id da biblioteca deve ser informado."]
                });

            var validation = ValidationHelper.Validate(_validatorRemoveGame, game);

            if (validation.Count > 0)
                return BadRequest(validation);

            if (!await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                    return NotFound("Jogo não cadastrado");

            var success = await _gameLibraryUseCase.RemoveGameFromLibraryAsync(libraryId, game);

            return success ? NoContent() : NotFound("Biblioteca de jogos não encontrada");
        }
    }
}
