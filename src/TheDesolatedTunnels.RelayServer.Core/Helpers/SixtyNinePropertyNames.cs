using System.Text.Json;

namespace TheDesolatedTunnels.RelayServer.Core.Helpers
{
    public static class SixtyNinePropertyNames
    {
        public const string PayloadTypePropertyName = "payloadType";

        public static readonly JsonEncodedText PayloadTypePropertyNameBytes =
            JsonEncodedText.Encode(PayloadTypePropertyName);

        public static readonly JsonEncodedText PayloadTypeInitPropertyValue =
            JsonEncodedText.Encode(SixtyNineMessageTypeStrings.Init);

        public static readonly JsonEncodedText PayloadTypePayloadPropertyValue =
            JsonEncodedText.Encode(SixtyNineMessageTypeStrings.Payload);

        public static readonly JsonEncodedText PayloadTypeErrorPropertyValue =
            JsonEncodedText.Encode(SixtyNineMessageTypeStrings.Error);

        public static readonly JsonEncodedText PayloadTypeClosePropertyValue =
            JsonEncodedText.Encode(SixtyNineMessageTypeStrings.Close);
    }
}