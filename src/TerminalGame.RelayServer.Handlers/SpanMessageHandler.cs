using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using TerminalGame.RelayServer.Domain;

namespace TerminalGame.RelayServer.Handlers
{
    public class SpanMessageHandler
    {
        private bool MessageIncomplete { get; set; }

        private byte[]? AccumulateBuffer { get; set; }
        
        
        private int Consumed { get; set; }
            
        
        public IEnumerable<SocketMessage?> DecodeBuffer(byte[] buffer, long offset, long size)
        {
             
            //AccumulateMessage += Encoding.UTF8.GetString(buffer, (int) offset, (int) size);

            if (MessageIncomplete && AccumulateBuffer is not null)
            {
                AccumulateBuffer = CombineBuffers(AccumulateBuffer, buffer);
            }

            while (Consumed != size && !MessageIncomplete)
            {
                var span = Encoding.UTF8.GetString(buffer, (int) offset, (int) size).AsSpan();
                //var span = new ReadOnlySpan<char>();
                //var ff = Encoding.UTF8.GetChars(buffer, span);
                //var span = AccumulateMessage.AsSpan();
                var slicedSpan = span[Consumed..];
                var sizeStartIndex = slicedSpan.IndexOf("{[", StringComparison.Ordinal);

                if (sizeStartIndex != -1)
                {
                    var sizeEndIndex = slicedSpan[sizeStartIndex..].IndexOf("][", StringComparison.Ordinal);
                    var messageEnd = sizeEndIndex + 2 +
                                     int.Parse(slicedSpan[(sizeStartIndex + 2)..sizeEndIndex]);

                    if (span.Length >= messageEnd + 2)
                    {
                        var messageSpan = slicedSpan[(sizeEndIndex + 2)..messageEnd];

                        var message = DeserializeSocketMessage(messageSpan);

                        Consumed += messageEnd + 2;
                        MessageIncomplete = false;
                        yield return message;
                    }
                    else
                    {
                        AccumulateBuffer ??= buffer;
                        MessageIncomplete = true;
                    }
                }
                else
                {
                    AccumulateBuffer ??= buffer;
                    MessageIncomplete = true;
                }
            }

            if (Consumed == size && !MessageIncomplete)
            {
                Consumed = 0;
                AccumulateBuffer = null;
            }
        }

        private static SocketMessage? DeserializeSocketMessage(ReadOnlySpan<char> messageReadOnlySpan)
        {
            SocketMessage? message;
            try
            {
                var byteCount = Encoding.UTF8.GetByteCount(messageReadOnlySpan);
                Span<byte> buffer = stackalloc byte[byteCount];
                Encoding.UTF8.GetBytes(messageReadOnlySpan, buffer);
                message = JsonSerializer.Deserialize<SocketMessage>(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                message = new SocketMessage
                {
                    Destination = null,
                    Payload = "Error deserializing message",
                    Source = null,
                    PayloadType = SocketPayloadType.ERROR
                };
            }

            return message;
        }
        
        private static byte[] CombineBuffers(byte[] first, byte[] second)
        {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }
    }
}