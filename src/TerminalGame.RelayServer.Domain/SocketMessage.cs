using System.Text.Json.Serialization;

namespace TerminalGame.RelayServer.Domain
{
    public record SocketMessage
    {
        [JsonPropertyName("destination")]
        public string? Destination { get; init; }

        [JsonPropertyName("source")]
        public string? Source { get; init; }

        [JsonPropertyName("payload")]
        public string? Payload { get; set; }

        [JsonPropertyName("payloadType")]
        public SocketPayloadType PayloadType { get; init; }
    }
}