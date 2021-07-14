using Newtonsoft.Json;

namespace Api.Domain.Models
{
    public class BaseResponse
    {
        [JsonProperty("status")]
        public bool Status { get; set; } = true;
    }

    public class ListBaseResponse : BaseResponse
    {
        [JsonProperty("paginator")]
        public Paginator Paginator { get; set; }
    }

    public class FailResponse : BaseResponse
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }
    }
}