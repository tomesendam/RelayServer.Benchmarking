using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Helpers;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    public class SixtyNineWriter : IMessageWriter<SixtyNineSendibleMessage>
    {
        public const string SourcePropertyName = "source";
        public const string DestinationPropertyName = "destination";
        public const string PayloadPropertyName = "payload";
        
        public static readonly JsonEncodedText SourcePropertyNameBytes = JsonEncodedText.Encode(SourcePropertyName);
        public static readonly JsonEncodedText DestinationPropertyNameBytes = JsonEncodedText.Encode(DestinationPropertyName);
        public static readonly JsonEncodedText PayloadPropertyNameBytes = JsonEncodedText.Encode(PayloadPropertyName);

        public void WriteMessage(SixtyNineSendibleMessage message, IBufferWriter<byte> stream)
        {
            WriteHeaders(message, stream);
            WriteContent(message, stream);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]

        private static void WriteHeaders(SixtyNineSendibleMessage message, IBufferWriter<byte> stream)
        {
            var size = message.GetMessageLength();

            var sizeSpan = stream.GetSpan(4);
            BinaryPrimitives.WriteInt32BigEndian(sizeSpan, size);
            stream.Advance(4);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WriteContent(SixtyNineSendibleMessage message, IBufferWriter<byte> stream)
        {
            var reusableWriter = ReusableUtf8JsonWriter.Get(stream);

            try
            {
                var writer = reusableWriter.GetJsonWriter();

                writer.WriteStartObject();

                switch (message)
                {
                    case ErrorMessage m:
                        WriteErrorMessage(m, writer);
                        break;
                    case InitResponseMessage m:
                        WriteInitResponseMessage(m, writer);
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

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WriteErrorMessage(ErrorMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            if (message.Source is not null) WriteSource(message, writer);
            WriteDestination(message.Destination, writer);
            if (message.Payload != null) WritePayload(message.Payload, writer);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WriteInitResponseMessage(InitResponseMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteDestination(message.Destination, writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WritePayLoadMessage(PayloadMessage message, Utf8JsonWriter writer)
        {
            WritePayloadType(message, writer);
            WriteSource(message, writer);
            WriteDestination(message.Destination, writer);
            WritePayload(message.Payload!, writer);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WritePayloadType(SixtyNineSendibleMessage message, Utf8JsonWriter writer)
        {
            var payloadType = message.PayloadType switch
            {
                SixtyNineMessageType.Init => SixtyNinePropertyNames.PayloadTypeInitPropertyValue,
                SixtyNineMessageType.Payload => SixtyNinePropertyNames.PayloadTypePayloadPropertyValue,
                SixtyNineMessageType.Close => SixtyNinePropertyNames.PayloadTypeClosePropertyValue,
                SixtyNineMessageType.Error => SixtyNinePropertyNames.PayloadTypeErrorPropertyValue,
                _ => throw new InvalidOperationException($"Unsupported message type: {message.PayloadType}")
            };

            writer.WriteString(SixtyNinePropertyNames.PayloadTypePropertyNameBytes, payloadType);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WriteSource(SixtyNineSendibleMessage message, Utf8JsonWriter writer)
        {
            if (message.Source == null) throw new ArgumentNullException(nameof(message));

            writer.WriteString(SourcePropertyNameBytes, message.Source);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WriteDestination(string destination, Utf8JsonWriter writer)
        {
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            writer.WriteString(DestinationPropertyNameBytes, destination);
        }
        [MethodImpl(MethodImplOptions.AggressiveOptimization)]
        private static void WritePayload(string payload, Utf8JsonWriter writer)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));
            writer.WriteString(PayloadPropertyNameBytes, payload);
        }
    }
}