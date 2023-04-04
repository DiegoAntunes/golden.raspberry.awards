using golden.raspberry.awards.api.Application.Services;
using golden.raspberry.awards.api.Application.Validators;
using golden.raspberry.awards.api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace golden.raspberry.awards.api.Controllers.v1
{
    [ApiController]
    [Route("api/{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class MovieAwardsController : ControllerBase
    {
        private readonly ILogger<MovieAwardsController> _logger;
        public readonly IBaseService<MovieAwardNomination> _baseService;

        public MovieAwardsController(ILogger<MovieAwardsController> logger,
            IBaseService<MovieAwardNomination> baseService)
        {
            _logger = logger;
            _baseService = baseService;
        }

        /// <summary>
        /// Retorna a lista de indicados e vencedores da categoria Pior Filme do Golden Raspberry Awards.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpGet]
        public IActionResult Get()
        {
            return Execute(() => _baseService.Get());
        }

        /// <summary>
        /// Retorna o indicado da categoria Pior Filme do Golden Raspberry Awards através do seu identificador.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            if (id == 0)
                return NotFound();

            return Execute(() => _baseService.GetById(id));
        }

        /// <summary>
        /// Cria um indicado da categoria Pior Filme do Golden Raspberry Awards.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpPost]
        public IActionResult Post([FromBody] MovieAwardNomination movieAward)
        {
            if (movieAward == null)
                return NotFound();

            try
            {
                return CreatedAtAction(nameof(Post), _baseService.Add<MovieAwardValidator>(movieAward).Id, movieAward.Id);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza um indicado da categoria Pior Filme do Golden Raspberry Awards.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpPut]
        public IActionResult Put([FromBody] MovieAwardNomination movieAward)
        {
            if (movieAward == null)
                return NotFound();

            return Execute(() => _baseService.Update<MovieAwardValidator>(movieAward));
        }

        /// <summary>
        /// Deleta um indicado da categoria Pior Filme do Golden Raspberry Awards através do seu identificador.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id == 0)
                return NotFound();

            Execute(() => { _baseService.Delete(id); return true; });

            return new NoContentResult();
        }

        private IActionResult Execute(Func<object> func)
        {
            try
            {
                var result = func();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { ErrorMessage = ex.Message });
            }
        }
    }
}