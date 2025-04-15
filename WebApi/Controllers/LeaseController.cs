using Domain.Entities;
using Domain.Records;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [Route("api/locacao")]
    [ApiController]
    public class LeaseController(ILeaseService leaseService) : ControllerBase
    {
        private readonly ILeaseService _leaseService = leaseService;

        [HttpPost]
        [SwaggerOperation(Summary = "Alugar uma moto", Tags = new[] { "locação" })]
        public async Task<IActionResult> RentMotorcycleAsync([FromBody] MotorcycleRent model)
        {
            var result = await _leaseService.RentMotorcycleAsync(model);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                201 => StatusCode(StatusCodes.Status201Created, result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Consultar locação por id", Tags = new[] { "locação" })]
        public async Task<IActionResult> GetMotorcycleRentByIdAsync(string id)
        {
            var result = await _leaseService.GetLeaseByIdAsync(id);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result.Data),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }

        [HttpPut("{id}/devolucao")]
        [SwaggerOperation(Summary = "Modificar a placa de uma moto", Tags = new[] { "locação" })]
        public async Task<IActionResult> UpdateMotorcycleAsync(string id, [FromBody] SetReturnDate model)
        {
            var result = await _leaseService.MotorcycleSetReturnDateAsync(model, id);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
    }
}
