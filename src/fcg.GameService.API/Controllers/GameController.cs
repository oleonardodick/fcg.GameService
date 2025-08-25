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

        public GameController(
            IGameUseCase gameUseCase,
            IValidator<CreateGameDTO> validatorCreate,
            IValidator<UpdateGameDTO> validatorUpdate,
            IValidator<UpdateTagsDTO> validatorTagsUpdate)
        {
            _gameUseCase = gameUseCase;
            _validatorCreate = validatorCreate;
            _validatorUpdate = validatorUpdate;
            _validatorTagsUpdate = validatorTagsUpdate;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _gameUseCase.GetAllAsync());

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
                
            var game = await _gameUseCase.GetByIdAsync(id);

            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateGameDTO game)
        {
            var errors = ValidationHelper.Validate(_validatorCreate, game);
            if (errors.Count > 0)
                throw new AppValidationException(errors);

            var createdGame = await _gameUseCase.CreateAsync(game);
            return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
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

            var errors = ValidationHelper.Validate(_validatorUpdate, game);

            if (errors.Count > 0)
                throw new AppValidationException(errors);

            await _gameUseCase.UpdateAsync(id, game);

            return NoContent();
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

            var errors = ValidationHelper.Validate(_validatorTagsUpdate, tags);

            if (errors.Count > 0)
                throw new AppValidationException(errors);

            await _gameUseCase.UpdateTagsAsync(id, tags);

            return NoContent();
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

            await _gameUseCase.DeleteAsync(id);

            return NoContent();
        }
    }
}
