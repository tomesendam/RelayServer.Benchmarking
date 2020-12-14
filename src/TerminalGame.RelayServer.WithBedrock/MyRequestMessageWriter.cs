using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Text.Json;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyRequestMessageWriter : IMessageWriter<MyRequestMessage>
    {
        public void WriteMessage(MyRequestMessage message, IBufferWriter<byte> stream)
        {
            WriteHeaders(message, stream);
            WriteContent(message, stream);
        }

        private static void WriteHeaders(MyRequestMessage message, IBufferWriter<byte> stream)
        {
            // HACK : Send an arbitrary number right now because all message have that size during test
            var size = message is InitMessage ? 35 : 77;
            // HACK

            var sizeSpan = stream.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(sizeSpan, size);
            stream.Write(sizeSpan[..4]);
        }

        private static void WriteContent(MyRequestMessage message, IBufferWriter<byte> stream)
        {
            var reusableWriter = ReusableUtf8JsonWriter.Get(stream);

            try
            {
                var writer = reusableWriter.GetJsonWriter();

                writer.WriteStartObject();

                switch (message)
                {
                    case InitMessage m:
                        WriteInitMessage(m, writer);
                        break;
                    case PayloadMessage m:
                        WritePayLoadMessage(m, writer);
                        break;
                    default:
                        throw new InvalidOperationException($"Unsupported message type: {message.GetType().FullName}");
                }

                writer.WriteEndObject();
                writer.Flush();
                Debug.Assert(writer.CurrentDepth == 0);
            }
            finally
            {
                ReusableUtf8JsonWriter.Return(reusableWriter);
            }
        }

        private static void WriteInitMessage(InitMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteSource(message, writer);
        }

        private static void WritePayLoadMessage(PayloadMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteSource(message, writer);
            WriteDestination(message, writer);
            WritePayload(message, writer);
        }

        public const string PayloadTypePropertyName = "payloadType";
        public static readonly JsonEncodedText PayloadTypePropertyNameBytes = JsonEncodedText.Encode(PayloadTypePropertyName);
        public static readonly JsonEncodedText PayloadTypeInitPropertyValue = JsonEncodedText.Encode("INIT");
        public static readonly JsonEncodedText PayloadTypePayloadPropertyValue = JsonEncodedText.Encode("MESSAGE");
        private static void WritePayloadType(MyRequestMessage message, Utf8JsonWriter writer)
        {
            var payloadType = message.PayloadType switch
            {
                MyPayloadType.Init => PayloadTypeInitPropertyValue,
                MyPayloadType.Payload => PayloadTypePayloadPropertyValue,
                _ => throw new InvalidOperationException($"Unsupported message type: {message.PayloadType}")
            };

            writer.WriteString(PayloadTypePropertyNameBytes, payloadType);
        }

        public const string SourcePropertyName = "source";
        public static readonly JsonEncodedText SourcePropertyNameBytes = JsonEncodedText.Encode(SourcePropertyName);
        private static void WriteSource(MyRequestMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(SourcePropertyNameBytes, message.Source);
        }

        public const string DestinationPropertyName = "destination";
        public static readonly JsonEncodedText DestinationPropertyNameBytes = JsonEncodedText.Encode(DestinationPropertyName);
        private static void WriteDestination(PayloadMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(DestinationPropertyNameBytes, message.Destination);
        }

        public const string PayloadPropertyName = "payload";
        public static readonly JsonEncodedText PayloadPropertyNameBytes = JsonEncodedText.Encode(PayloadPropertyName);
        private static void WritePayload(PayloadMessage message, Utf8JsonWriter writer)
        {
            writer.WriteString(PayloadPropertyNameBytes, message.Payload);
        }
    }
}
