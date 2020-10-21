using Api.Domain.Models;

namespace Api.Web.Models
{
    public class CollectionEventReceived
    {
        public string Type { get; set; }
        public string Collection { get; set; }
        public string Id { get; set; }
        public string StationId { get; set; }
        public BaseEntity Model { get; set; }

        public void Deconstruct(out string type, out string collection, out string id, out string stationId, out BaseEntity model)
        {
            type = Type;
            collection = Collection;
            id = Id;
            stationId = StationId;
            model = Model;
        }
    }
}