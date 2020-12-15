using System.IO;
using TheDesolatedTunnels.RelayServer.Core.Domain;

namespace TheDesolatedTunnels.RelayServer.Core.Helpers
{
    public static class SixtyNineMessageTypeStrings
    {
        public const string Init = "INIT";
        public const string Payload = "MESSAGE";
        public const string Error = "ERROR";
        public const string Close = "CLOSE";

        public static string ToString(this SixtyNineMessageType myPayloadType)
        {
            return myPayloadType switch
            {
                SixtyNineMessageType.Init => Init,
                SixtyNineMessageType.Payload => Payload,
                SixtyNineMessageType.Close => Error,
                SixtyNineMessageType.Error => Close,
                _ => throw new InvalidDataException(
                    $"Expected '{nameof(SixtyNineMessageType)}' to be of type {myPayloadType.GetType().Name}.")
            };
        }
    }
}