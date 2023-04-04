using golden.raspberry.awards.api.Application.Services;
using golden.raspberry.awards.api.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace golden.raspberry.awards.api.Controllers.v1
{
    [ApiController]
    [Route("v{version:apiVersion}/[controller]")]
    public class ProducerAwardsController : ControllerBase
    {
        private readonly ILogger<MovieAwardsController> _logger;
        public readonly IProducerAwardService _producerAwardService;

        public ProducerAwardsController(ILogger<MovieAwardsController> logger,
            IProducerAwardService producerAwardService)
        {
            _logger = logger;
            _producerAwardService = producerAwardService;
        }


        /// <summary>
        /// Retorna duas listas com o(s) produtor(es) com maior e menor intervalo entre dois prêmios consecutivos.
        /// </summary>
        /// <response code="500">
        ///     ## Exception type unknown:
        ///     - **500:** Ocorreu um erro inesperado no servidor. Verifique a mensagem de erro.
        /// </response>
        [HttpGet]
        [ProducesResponseType(typeof(ProducerAwardResult), 200)]
        public IActionResult Get()
        {
            return Execute(() => _producerAwardService.GetProducerAwardsResults());
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
