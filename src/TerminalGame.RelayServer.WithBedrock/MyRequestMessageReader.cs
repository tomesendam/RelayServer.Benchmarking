using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using System;
using System.Buffers;
using System.IO;
using System.Text.Json;

namespace TerminalGame.RelayServer.WithBedrock
{
    public class MyRequestMessageReader : IMessageReader<MyRequestMessage>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed, ref SequencePosition examined, out MyRequestMessage message)
        {
            var sequenceReader = new SequenceReader<byte>(input);
            if (!sequenceReader.TryReadBigEndian(out int length) || input.Length < length)
            {
                message = default!;
                return false;
            }

            bool completed = false;
            string payloadType = default!;
            string source = default!;
            string destination = default!;
            string payload = default!;

            var rawPayload = input.Slice(sequenceReader.Position, length);
            var reader = new Utf8JsonReader(rawPayload, isFinalBlock: true, state: default);
            reader.CheckRead();

            // We're always parsing a JSON object
            reader.EnsureObjectStart();

            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(MyRequestMessageWriter.PayloadTypePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payloadType = reader.ReadAsString(MyRequestMessageWriter.PayloadTypePropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestMessageWriter.PayloadTypePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestMessageWriter.SourcePropertyNameBytes.EncodedUtf8Bytes))
                        {
                            source = reader.ReadAsString(MyRequestMessageWriter.SourcePropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestMessageWriter.SourcePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestMessageWriter.DestinationPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            destination = reader.ReadAsString(MyRequestMessageWriter.DestinationPropertyName)
                                            ?? throw new InvalidDataException($"Expected '{MyRequestMessageWriter.DestinationPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(MyRequestMessageWriter.PayloadPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payload = reader.ReadAsString(MyRequestMessageWriter.PayloadPropertyName)
                                        ?? throw new InvalidDataException($"Expected '{MyRequestMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else
                        {
                            reader.CheckRead();
                            reader.Skip();
                        }
                        break;
                    case JsonTokenType.EndObject:
                        completed = true;
                        break;
                }
            }
            while (!completed && reader.CheckRead());

            consumed = rawPayload.End;
            examined = consumed;

            message = payloadType switch
            {
                "INIT" => new InitMessage(source),
                "MESSAGE" => new PayloadMessage(source, destination, payload),
                _ => throw new InvalidDataException($"Expected '{MyRequestMessageWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.")
            };
            return message != null;
        }
    }
}