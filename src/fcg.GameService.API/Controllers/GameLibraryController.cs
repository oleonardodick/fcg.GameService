using System.Diagnostics;
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
        private readonly IAppLogger<GameLibraryController> _logger;

        public GameLibraryController(
            IGameLibraryUseCase gameLibraryUseCase,
            IValidator<CreateGameLibraryDTO> validatorCreate,
            IValidator<AddGameToLibraryDTO> validatorAddGame,
            IValidator<RemoveGameFromLibraryDTO> validatorRemoveGame,
            IAppLogger<GameLibraryController> logger)
        {
            _gameLibraryUseCase = gameLibraryUseCase;
            _validatorCreate = validatorCreate;
            _validatorAddGame = validatorAddGame;
            _validatorRemoveGame = validatorRemoveGame;
            _logger = logger;
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

            _logger.LogInformation("Iniciando a busca da biblioteca com o ID {id}", id);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var gameLibrary = await _gameLibraryUseCase.GetByIdAsync(id);
                return Ok(gameLibrary);
            }
            finally
            {   
                stopwatch.Stop();
                _logger.LogInformation("Finalizou a busca da biblioteca com o ID {id} após {duration}ms", id, stopwatch.ElapsedMilliseconds);
            }
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

            _logger.LogInformation("Iniciando a busca da biblioteca do usuário com o ID {id}", userId);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var gameLibrary = await _gameLibraryUseCase.GetByUserIdAsync(userId);
                return Ok(gameLibrary);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizou a busca da biblioteca do usuário com o ID {id} após {duration}ms",
                    userId,
                    stopwatch.ElapsedMilliseconds
                );
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameLibraryDTO gameLibrary)
        {
            _logger.LogInformation("Iniciando a criação da biblioteca do usuário {id}", gameLibrary.UserId);
            var stopwatch = Stopwatch.StartNew();
            try
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
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Criação da biblioteca do usuário {id} finalizada após {duration}ms", gameLibrary.UserId, stopwatch.ElapsedMilliseconds);
            }
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
            
            _logger.LogInformation("Iniciando a busca se o jogo já existe na biblioteca de Id {id}.", libraryId);
            var stopwatch = Stopwatch.StartNew();
            try
            {
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
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizou a busca se o jogo já existe na biblioteca de ID {id} após {duration}ms", libraryId, stopwatch.ElapsedMilliseconds);
            }
            

            _logger.LogInformation("Iniciando a inclusão de jogo na biblioteca de Id {id}.", libraryId);
            stopwatch.Reset();
            stopwatch.Start();
            try
            {
                var errors = ValidationHelper.Validate(_validatorAddGame, game);
                if (errors.Count > 0)
                    throw new AppValidationException(errors);
                await _gameLibraryUseCase.AddGameToLibraryAsync(libraryId, game);
                return NoContent();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizou a inclusão de jogo na biblioteca de Id {id} após {duration}ms.", libraryId, stopwatch.ElapsedMilliseconds);
            }
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
                
            _logger.LogInformation("Iniciando a busca se o jogo de ID {id} existe na biblioteca de id {libraryId}.", game.Id, libraryId);
            var stopwatch = Stopwatch.StartNew();
            if (!await _gameLibraryUseCase.ExistsGameOnLibraryAsync(libraryId, game.Id))
                return NotFound("Jogo não cadastrado");
            stopwatch.Stop();
            _logger.LogInformation("Finalizou a busca se o jogo de ID {id} existe na biblioteca de id {libraryId} após {duration}ms.",
                game.Id,
                libraryId,
                stopwatch.ElapsedMilliseconds
            );

            _logger.LogInformation("Iniciando a exclusão do jogo de ID {id} na biblioteca de id {libraryId}.", game.Id, libraryId);
            stopwatch.Reset();
            stopwatch.Start();
            await _gameLibraryUseCase.RemoveGameFromLibraryAsync(libraryId, game);
            stopwatch.Stop();
            _logger.LogInformation("Finalizou a exclusão do jogo de ID {id} na biblioteca de id {libraryId} após {duration}ms.",
                game.Id,
                libraryId,
                stopwatch.ElapsedMilliseconds
            );

            return NoContent();
        }
    }
}
