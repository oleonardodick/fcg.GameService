using fcg.GameService.Application.Helpers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
using fcg.GameService.Presentation.DTOs.GameLibrary.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class GameLibraryController : ControllerBase
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
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                List<ErrorDetails> error = [
                    new ErrorDetails {
                        Property = nameof(id),
                        Errors = ["O ID deve ser informado"]
                    }
                ];
                
                throw new AppValidationException(
                    error
                );
            }

            var gameLibrary = await _gameLibraryUseCase.GetByIdAsync(id);

            return Ok(gameLibrary);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                List<ErrorDetails> error = [
                    new ErrorDetails {
                        Property = nameof(userId),
                        Errors = ["O ID do usuário deve ser informado"]
                    }
                ];
                
                throw new AppValidationException(
                    error
                );
            }
                
            var gameLibrary = await _gameLibraryUseCase.GetByUserIdAsync(userId);

            return Ok(gameLibrary);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            var errors = ValidationHelper.Validate(_validatorCreate, gameLibrary);

            if (errors.Count > 0)
                throw new AppValidationException(errors);
                
            var createdGameLibrary = await _gameLibraryUseCase.CreateAsync(gameLibrary);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdGameLibrary.Id },
                createdGameLibrary
            );
        }

        [HttpPost("{libraryId}/addGame")]
        public async Task<IActionResult> AddGame(string libraryId, [FromBody] AddGameToLibraryDTO game)
        {
            if (string.IsNullOrWhiteSpace(libraryId))
            {
                List<ErrorDetails> error = [
                    new ErrorDetails {
                        Property = nameof(libraryId),
                        Errors = ["O ID da biblioteca deve ser informado"]
                    }
                ];
                
                throw new AppValidationException(
                    error
                );
            }

            if (await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
            {
                List<ErrorDetails> error = [
                    new ErrorDetails {
                        Property = nameof(game.Id),
                        Errors = ["Jogo já cadastrado."]
                    }
                ];
                
                throw new AppValidationException(
                    error
                );
            }

            var errors = ValidationHelper.Validate(_validatorAddGame, game);

            if (errors.Count > 0)
                throw new AppValidationException(errors);

            await _gameLibraryUseCase.AddGameToLibraryAsync(libraryId, game);

            return NoContent();
        }

        [HttpDelete("{libraryId}/removeGame")]
        public async Task<IActionResult> RemoveGame(string libraryId, [FromBody] RemoveGameFromLibraryDTO game)
        {
            if (string.IsNullOrWhiteSpace(libraryId))
            {
                List<ErrorDetails> error = [
                    new ErrorDetails {
                        Property = nameof(libraryId),
                        Errors = ["O ID da biblioteca deve ser informado"]
                    }
                ];
                
                throw new AppValidationException(
                    error
                );
            }

            var errors = ValidationHelper.Validate(_validatorRemoveGame, game);

            if (errors.Count > 0)
                throw new AppValidationException(errors);

            if (!await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                    return NotFound("Jogo não cadastrado");

            var success = await _gameLibraryUseCase.RemoveGameFromLibraryAsync(libraryId, game);

            return NoContent();
        }
    }
}
