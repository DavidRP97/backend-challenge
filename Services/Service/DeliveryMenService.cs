using Data.Enums;
using Data.Mongo;
using Domain.Entities;
using Domain.Records;
using Domain.Responses;
using Serilog;
using Services.Service.Interfaces;

namespace Services.Service
{
    public class DeliveryMenService(ILogger logger, IMongoConnection mongoConnection) : IDeliveryMenService
    {
        private readonly ILogger _logger = logger;
        private readonly IMongoConnection _mongoConnection = mongoConnection;
        public async Task<Response> InsertNewAsync(DeliveryMen model)
        {
            try
            {
                var checkFields = await _mongoConnection.GetWithTwoFiltersAsync<DeliveryMen>(MongoCollections.DeliveryMen, model.CnhNumber, model.Cnpj);

                if (checkFields.Count != 0)
                {
                    var deliveryMan = checkFields.FirstOrDefault() ?? new();

                    return deliveryMan.Cnpj.Equals(model.Cnpj) && deliveryMan.CnhNumber.Equals(model.CnhNumber) ?
                        CustomResponses.BadRequest("Cnpj e número de CNH informados já cadastrados!") :
                        deliveryMan.Cnpj.Equals(model.Cnpj) ?
                        CustomResponses.BadRequest("Cnpj informado já cadastrado") :
                        CustomResponses.BadRequest("Número de CNH informado já cadastrado");
                }

                var resultImage = SaveCnhImage(model.CnhImage, model.CnhNumber);

                if (!resultImage.Success)
                    return resultImage;

                await _mongoConnection.SetDocumentAsync(model, MongoCollections.DeliveryMen);

                return CustomResponses.Ok("");
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                return CustomResponses.InternalServerError();
            }
        }
        public async Task<Response> UpdateCnhImageAsync(string id, UpdateCnhImage model)
        {
            var deliveryMan = await _mongoConnection.GetDocumentByFilterAsync<DeliveryMen>(MongoCollections.DeliveryMen, id, "Identifier");

            if (deliveryMan is null || deliveryMan.Count == 0) return CustomResponses.BadRequest("Identificador fornecido não encontrado");

            var resultImage = SaveCnhImage(model.CnhImage, deliveryMan?.FirstOrDefault()?.CnhNumber ?? "");

            if (!resultImage.Success)
                return resultImage;

            return CustomResponses.Ok("Imagem atualizada com sucesso!");
        }

        private enum ImageFormat
        {
            Unknown,
            Png,
            Bmp
        }

        private static ImageFormat GetImageFormat(byte[] imageBytes)
        {
            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return ImageFormat.Bmp;

            if (imageBytes.Length >= 8)
            {
                byte[] pngSignature = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
                bool isPng = true;

                for (int i = 0; i < pngSignature.Length; i++)
                {
                    if (imageBytes[i] != pngSignature[i])
                    {
                        isPng = false;
                        break;
                    }
                }

                if (isPng)
                    return ImageFormat.Png;
            }

            return ImageFormat.Unknown;
        }
        private static Response SaveCnhImage(string base64Image, string cnhNumber)
        {
            if (base64Image.Contains(','))
                base64Image = base64Image.Split(',')[1];

            byte[] imageBytes = Convert.FromBase64String(base64Image);

            var imageFormat = GetImageFormat(imageBytes);

            if (imageFormat == ImageFormat.Unknown) return CustomResponses.BadRequest("Formato de imagem inválido");

            string fileExtension = imageFormat == ImageFormat.Png ? ".png" : ".bmp";

            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "cnh_images");

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            string fileName = $"{cnhNumber}{fileExtension}";

            string fullPath = Path.Combine(directoryPath, fileName);

            if (File.Exists(fullPath))
                File.Delete(fullPath);

            Directory.CreateDirectory(directoryPath);

            File.WriteAllBytes(fullPath, imageBytes);

            return CustomResponses.Ok("");
        }
    }
}
