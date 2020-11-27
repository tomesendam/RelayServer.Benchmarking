namespace TerminalGame.RelayServer.WithBedrock
{
    public enum MyPayloadType
    {
        Init,
        Payload
    }

    public abstract record MyRequestMessage(string Source, MyPayloadType PayloadType);

    public record InitMessage(string Source) : MyRequestMessage(Source, MyPayloadType.Init);
    public record PayloadMessage(string Source, string Destination, string Payload) : MyRequestMessage(Source, MyPayloadType.Payload);

}
