using System.Diagnostics;
using fcg.GameService.Application.Helpers;
using fcg.GameService.Application.Interfaces;
using fcg.GameService.Domain.Exceptions;
using fcg.GameService.Domain.Models;
using fcg.GameService.Presentation.DTOs.Game.Requests;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace fcg.GameService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class GameController : ControllerBase
    {
        private readonly IGameUseCase _gameUseCase;
        private readonly IValidator<CreateGameDTO> _validatorCreate;
        private readonly IValidator<UpdateGameDTO> _validatorUpdate;
        private readonly IValidator<UpdateTagsDTO> _validatorTagsUpdate;
        private readonly IAppLogger<GameController> _logger;

        public GameController(
            IGameUseCase gameUseCase,
            IValidator<CreateGameDTO> validatorCreate,
            IValidator<UpdateGameDTO> validatorUpdate,
            IValidator<UpdateTagsDTO> validatorTagsUpdate,
            IAppLogger<GameController> logger)
        {
            _gameUseCase = gameUseCase;
            _validatorCreate = validatorCreate;
            _validatorUpdate = validatorUpdate;
            _validatorTagsUpdate = validatorTagsUpdate;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Iniciando busca da lista de jogos");
            var stopwatch = Stopwatch.StartNew();
            var result = await _gameUseCase.GetAllAsync();
            stopwatch.Stop();
            _logger.LogInformation("Finalizou a busca da lista de jogos após {duration}ms", stopwatch.ElapsedMilliseconds);
            return Ok(result);
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
            _logger.LogInformation("Iniciando a busca do jogo com o ID {id}", id);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var game = await _gameUseCase.GetByIdAsync(id);
                return Ok(game);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizou a busca do jogo com o ID {id} após {duration}ms", id, stopwatch.ElapsedMilliseconds);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameDTO game)
        {
            _logger.LogInformation("Iniciando a criação do jogo {name}", game.Name);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var errors = ValidationHelper.Validate(_validatorCreate, game);
                if (errors.Count > 0)
                    throw new AppValidationException(errors);

                var createdGame = await _gameUseCase.CreateAsync(game);
                return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Criação do jogo {name} finalizada após {duration}ms", game.Name, stopwatch.ElapsedMilliseconds);
            }
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateGameDTO game)
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

            _logger.LogInformation("Iniciando atualização do jogo com id {id}", id);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var errors = ValidationHelper.Validate(_validatorUpdate, game);
                if (errors.Count > 0)
                    throw new AppValidationException(errors);
                await _gameUseCase.UpdateAsync(id, game);
                return NoContent();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizando atualização do jogo com id {id} após {duration}ms", id, stopwatch.ElapsedMilliseconds);
            }            
        }

        [HttpPatch("tags/{id}")]
        public async Task<IActionResult> UpdateTags(string id, [FromBody] UpdateTagsDTO tags)
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

            _logger.LogInformation("Iniciando a atualização das tags do jogo com id {id}", id);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var errors = ValidationHelper.Validate(_validatorTagsUpdate, tags);
                if (errors.Count > 0)
                    throw new AppValidationException(errors);
                await _gameUseCase.UpdateTagsAsync(id, tags);
                return NoContent();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizando atualização das tags do jogo com id {id} após {duration}ms", id, stopwatch.ElapsedMilliseconds);
            }            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
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

            _logger.LogInformation("Iniciando a exclusão do jogo com id {id}", id);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _gameUseCase.DeleteAsync(id);
                return NoContent();
            }
            finally
            {
                stopwatch.Stop();
                _logger.LogInformation("Finalizando exclusão do jogo com id {id} após {duration}ms", id, stopwatch.ElapsedMilliseconds);
            }
        }
    }
}
