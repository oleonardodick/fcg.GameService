using fcg.GameService.API.DTOs;
using fcg.GameService.API.DTOs.Requests;
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
    public class GameController : BaseApiController
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
        [ProducesResponseType(typeof(IList<ResponseGameDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<ResponseGameDTO>>> GetAll()
        {
            var games = await _gameUseCase.GetAllAsync();
            return Success(games);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ResponseGameDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResponseGameDTO>> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(id),
                    Errors = ["O Id deve ser informado."]
                });

            var game = await _gameUseCase.GetByIdAsync(id);

            return game is null ? NotFound("Jogo não encontrado.") : Success(game);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ResponseGameDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Create([FromBody] CreateGameDTO game)
        {
            var validation = ValidationHelper.Validate(_validatorCreate, game);
            if (validation.Count > 0)
                return BadRequest(validation);

            var createdGame = await _gameUseCase.CreateAsync(game);
            return CreatedAtAction(nameof(GetById), new { id = createdGame.Id }, createdGame);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Update(string id, [FromBody] UpdateGameDTO game)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(id),
                    Errors = ["O Id deve ser informado."]
                });

            var validation = ValidationHelper.Validate(_validatorUpdate, game);

            if (validation.Count > 0)
                return BadRequest(validation);

            var success = await _gameUseCase.UpdateAsync(id, game);

            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }

        [HttpPatch("/tags/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateTags(string id, [FromBody] UpdateTagsDTO tags)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(id),
                    Errors = ["O Id deve ser informado."]
                });

            var validation = ValidationHelper.Validate(_validatorTagsUpdate, tags);

            if (validation.Count > 0)
                return BadRequest(validation);

            var success = await _gameUseCase.UpdateTagsAsync(id, tags);
            
            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(NotFoundProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(BusinesRuleProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorResponseDTO
                {
                    Property = nameof(id),
                    Errors = ["O Id deve ser informado."]
                });

            var success = await _gameUseCase.DeleteAsync(id);

            return success ? NoContent() : NotFound("Jogo não encontrado.");
        }
    }
}
