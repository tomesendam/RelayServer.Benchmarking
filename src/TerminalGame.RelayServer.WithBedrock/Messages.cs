using System.IO;

namespace TerminalGame.RelayServer.WithBedrock
{

    public static class MyPayloadTypeStrings
    {
        public const string Init = "INIT";
        public const string Payload = "MESSAGE";

        public static string ToString(this MyPayloadType myPayloadType) =>
            myPayloadType switch
            {
                MyPayloadType.Init => Init,
                MyPayloadType.Payload => Payload,
                _ => throw new InvalidDataException($"Expected '{nameof(MyPayloadType)}' to be of type {myPayloadType.GetType().Name}.")
            };
    }

    public enum MyPayloadType
    {
        Init,
        Payload
    }

    public abstract record MyRequestMessage(string Source, MyPayloadType PayloadType);

    public record InitMessage(string Source) : MyRequestMessage(Source, MyPayloadType.Init);
    public record PayloadMessage(string Source, string Destination, string Payload) : MyRequestMessage(Source, MyPayloadType.Payload);
}
