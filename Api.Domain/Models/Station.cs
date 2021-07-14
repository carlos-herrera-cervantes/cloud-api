using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class Station : BaseEntity
    {
        [BsonElement("name")]
        [JsonProperty("name")]
        [Required(ErrorMessage = "StationNameRequired")]
        public string Name { get; set; }

        [BsonElement("email")]
        [JsonProperty("email")]
        [Required(ErrorMessage = "EmailRequired")]
        [DataType(DataType.EmailAddress, ErrorMessage = "InvalidEmail")]
        public string Email { get; set; }

        [BsonElement("stationKey")]
        [JsonProperty("stationKey")]
        [Required(ErrorMessage = "StationKeyRequired")]
        public string StationKey { get; set; }

        [BsonElement("active")]
        [JsonProperty("active")]
        public bool Active { get; set; } = true;

        [BsonElement("street")]
        [JsonProperty("street")]
        public string Street { get; set; }

        [BsonElement("outside")]
        [JsonProperty("outside")]
        public string Outside { get; set; }

        [BsonElement("zipCode")]
        [JsonProperty("zipCode")]
        public string ZipCode { get; set; }

        [BsonElement("state")]
        [JsonProperty("state")]
        [Required(ErrorMessage = "StationStateRequired")]
        public string State { get; set; }

        [BsonElement("municipality")]
        [JsonProperty("municipality")]
        [Required(ErrorMessage = "StationMunicipality")]
        public string Municipality { get; set; }
    }

    public class SingleStationResponse : BaseResponse
    {
        [JsonProperty("data")]
        public Station Data { get; set; }
    }

    public class ListStationResponse : ListBaseResponse
    {
        [JsonProperty("data")]
        public List<Station> Data { get; set; }
    }
}
