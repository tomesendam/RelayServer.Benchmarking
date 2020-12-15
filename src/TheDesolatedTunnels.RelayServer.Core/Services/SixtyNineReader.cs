using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using Bedrock.Framework.Protocols;
using Microsoft.AspNetCore.Internal;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Helpers;

namespace TheDesolatedTunnels.RelayServer.Core.Services
{
    public class SixtyNineReader : IMessageReader<SixtyNineMessage>
    {
        public bool TryParseMessage(in ReadOnlySequence<byte> input, ref SequencePosition consumed,
            ref SequencePosition examined, out SixtyNineMessage message)
        {
            var sequenceReader = new SequenceReader<byte>(input);
            if (!sequenceReader.TryReadBigEndian(out int length) || input.Length < length)
            {
                message = default!;
                return false;
            }

            var completed = false;
            string payloadType = default!;
            string source = default!;
            string destination = default!;
            string payload = default!;

            var rawPayload = input.Slice(sequenceReader.Position, length);
            //This reader is taken from Microsoft.AspNetCore.Internal
            var reader = new Utf8JsonReader(rawPayload, true, default);
            reader.CheckRead();

            reader.EnsureObjectStart();

            do
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.PropertyName:
                        if (reader.ValueTextEquals(SixtyNinePropertyNames.PayloadTypePropertyNameBytes
                            .EncodedUtf8Bytes))
                        {
                            payloadType = reader.ReadAsString(SixtyNinePropertyNames.PayloadTypePropertyName)
                                          ?? throw new InvalidDataException(
                                              $"Expected '{SixtyNinePropertyNames.PayloadTypePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(SixtyNineWriter.SourcePropertyNameBytes.EncodedUtf8Bytes)
                        )
                        {
                            source = reader.ReadAsString(SixtyNineWriter.SourcePropertyName)
                                     ?? throw new InvalidDataException(
                                         $"Expected '{SixtyNineWriter.SourcePropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(SixtyNineWriter.DestinationPropertyNameBytes
                            .EncodedUtf8Bytes))
                        {
                            destination = reader.ReadAsString(SixtyNineWriter.DestinationPropertyName)
                                          ?? throw new InvalidDataException(
                                              $"Expected '{SixtyNineWriter.DestinationPropertyName}' to be of type {JsonTokenType.String}.");
                        }
                        else if (reader.ValueTextEquals(
                            SixtyNineWriter.PayloadPropertyNameBytes.EncodedUtf8Bytes))
                        {
                            payload = reader.ReadAsString(SixtyNineWriter.PayloadPropertyName)
                                      ?? throw new InvalidDataException(
                                          $"Expected '{SixtyNineWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.");
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
            } while (!completed && reader.CheckRead());

            consumed = rawPayload.End;
            examined = consumed;

            message = payloadType switch
            {
                "INIT" => new InitMessage(source),
                "MESSAGE" => new PayloadMessage(source, destination, payload),
                "ERROR" => new ErrorMessage(destination, payload, source),
                "CLOSE" => new CloseMessage(),
                _ => throw new InvalidDataException(
                    $"Expected '{SixtyNineWriter.PayloadPropertyName}' to be of type {JsonTokenType.String}.")
            };
            return message != null;
        }
    }
}