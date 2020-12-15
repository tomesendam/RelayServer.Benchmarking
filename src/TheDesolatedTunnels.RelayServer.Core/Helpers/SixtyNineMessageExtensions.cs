using System;
using System.Text;
using TheDesolatedTunnels.RelayServer.Core.Domain;

namespace TheDesolatedTunnels.RelayServer.Core.Helpers
{
    public static class SixtyNineMessageExtensions
    {
        public const string EmptyInitJsonMessage = "{\"payloadType\":\"INIT\",\"destination\":\"\"}";

        public const string EmptyPayloadJsonMessage =
            "{\"payloadType\":\"MESSAGE\",\"destination\":\"\",\"source\":\"\",\"payload\":\"\"}";

        public static int GetMessageLength(this SixtyNineSendibleMessage message)
        {
            return message switch
            {
                ErrorMessage msg => msg.GetMessageLength(),
                InitResponseMessage msg => msg.GetMessageLength(),
                PayloadMessage msg => msg.GetMessageLength(),
                _ => throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}")
            };
        }

        public static int GetMessageLength(this InitResponseMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Destination);

            return EmptyInitJsonMessage.Length + sourceLength;
        }

        public static int GetMessageLength(this PayloadMessage message)
        {
            var sourceLength = Encoding.UTF8.GetByteCount(message.Source!);
            var destinationLength = Encoding.UTF8.GetByteCount(message.Destination);
            var payloadLength = Encoding.UTF8.GetByteCount(message.Payload!);

            return EmptyPayloadJsonMessage.Length + sourceLength + destinationLength + payloadLength;
        }
        
        public static int GetMessageLength(this ErrorMessage message)
        {
            int sourceLength = default;
            if (message.Source != null)
            {
                sourceLength = Encoding.UTF8.GetByteCount(message.Source);
            }
           
            var destinationLength = Encoding.UTF8.GetByteCount(message.Destination);
            var payloadLength = Encoding.UTF8.GetByteCount(message.Payload!);

            return EmptyPayloadJsonMessage.Length + sourceLength + destinationLength + payloadLength;
        }
    }
}