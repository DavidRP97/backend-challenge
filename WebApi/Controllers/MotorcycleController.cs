using Domain.Entities;
using Domain.Records;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;
using System;

namespace WebApi.Controllers
{
    [Route("/motos")]
    [ApiController]
    public class MotorcycleController(IMotorcycleService motorcycleService) : ControllerBase
    {
        private readonly IMotorcycleService _motorcycleService = motorcycleService;

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastrar uma nova moto", Tags = new[] { "motos" })]
        public async Task<IActionResult> PostNewMotorcycleAsync([FromBody] Motorcycle model)
        {
            var result = await _motorcycleService.InsertAsync(model);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                201 => StatusCode(StatusCodes.Status201Created, result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Consultar motos existentes", Tags = new[] { "motos" })]
        public async Task<IActionResult> GetMotorcycleAsync([FromQuery] string? plate)
        {
            var result = await _motorcycleService.GetAllAsync(plate);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result.Data),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Remover uma moto", Tags = new[] { "motos" })]
        public async Task<IActionResult> DeleteMotorcycleAsync(string id)
        {
            var result = await _motorcycleService.DeleteAsync(id);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Consultar motos existentes por id", Tags = new[] { "motos" })]
        public async Task<IActionResult> GetMotorcycleByIdAsync(string id)
        {
            var result = await _motorcycleService.GetByIdAsync(id);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result.Data),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
        [HttpPut("{id}/placa")]
        [SwaggerOperation(Summary = "Modificar a placa de uma moto", Tags = new[] { "motos" })]
        public async Task<IActionResult> UpdateMotorcycleAsync(string id, [FromBody] UpdateMotorcyclePlate model)
        {
            var result = await _motorcycleService.UpdatePlateAsync(model, id);

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
