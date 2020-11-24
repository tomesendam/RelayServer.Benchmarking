using System.Text.Json.Serialization;

namespace TerminalGame.RelayServer.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum SocketPayloadType
    {
        OPEN,
        CLOSE,
        INIT,
        ERROR,
        MESSAGE
    }
}