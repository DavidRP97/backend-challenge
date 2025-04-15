using MediatR;
using MongoDB.Bson;

namespace Services.Events
{
    public class RegisteredMotorcycleEvent(ObjectId id) : INotification
    {
        public ObjectId Id { get; set; } = id;
    }
}
