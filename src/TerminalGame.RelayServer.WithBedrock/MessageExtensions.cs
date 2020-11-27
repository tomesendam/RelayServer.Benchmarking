using System;
using System.Text;

namespace TerminalGame.RelayServer.WithBedrock
{
    public static class MyRequestMessageExtensions
    {
        public static int GetMessageLength(this MyRequestMessage message) =>
            message switch
            {
                InitMessage msg => msg.GetMessageLength(),
                PayloadMessage msg => msg.GetMessageLength(),
                _ => throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}")
            };

        public const string EmptyInitJsonMessage = "{\"payloadType\":\"INIT\",\"source\":\"\"}";
        public static int GetMessageLength(this InitMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);

            return EmptyInitJsonMessage.Length + sourceLength;
        }

        public const string EmptyPayloadJsonMessage = "{\"payloadType\":\"MESSAGE\",\"destination\":\"\",\"source\":\"\",\"payload\":\"\"}";
        public static int GetMessageLength(this PayloadMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source);
            var destinationLength = Encoding.UTF8.GetByteCount(message.Destination);
            var payloadLength = Encoding.UTF8.GetByteCount(message.Payload);

            return EmptyPayloadJsonMessage.Length + sourceLength + destinationLength + payloadLength;
        }
    }
}
