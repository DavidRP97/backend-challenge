using Domain.Entities;
using Domain.Records;
using Microsoft.AspNetCore.Mvc;
using Services.Service.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace WebApi.Controllers
{
    [Route("/entregadores")]
    [ApiController]
    public class DeliveryMenController(IDeliveryMenService deliveryMenService) : ControllerBase
    {
        private readonly IDeliveryMenService _deliveryMenService = deliveryMenService;

        [HttpPost]
        [SwaggerOperation(Summary = "Cadastrar entregador", Tags = new[] { "entregadores" })]
        public async Task<IActionResult> PostNewMotorcycleAsync([FromBody] DeliveryMen model)
        {
            var result = await _deliveryMenService.InsertNewAsync(model);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                201 => StatusCode(StatusCodes.Status201Created, result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }

        [HttpPost("{id}/cnh")]
        [SwaggerOperation(Summary = "Enviar foto da CNH", Tags = new[] { "entregadores" })]
        public async Task<IActionResult> UpdateCnhImageMotorcycleAsync(string id, [FromBody] UpdateCnhImage model)
        {
            var result = await _deliveryMenService.UpdateCnhImageAsync(id, model);

            return result.StatusCode switch
            {
                500 => StatusCode(StatusCodes.Status500InternalServerError, result),
                200 => Ok(result),
                201 => StatusCode(StatusCodes.Status201Created, result),
                400 => BadRequest(result),
                _ => BadRequest(result)
            };
        }
    }
}
