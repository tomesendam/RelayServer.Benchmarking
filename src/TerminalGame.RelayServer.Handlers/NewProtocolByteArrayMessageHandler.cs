using System;
using System.Collections.Generic;
using System.Text.Json;
using TerminalGame.RelayServer.Domain;

namespace TerminalGame.RelayServer.Handlers
{
    public class NewProtocolByteArrayMessageHandler
    {
        private bool MessageIncomplete { get; set; }

        private byte[] AccumulateMessage { get; set; } = Array.Empty<byte>();


        public IEnumerable<SocketMessage?> DecodeBuffer(byte[] buffer, long offset, long size)
        {
            if (AccumulateMessage.Length == 0)
            {
                AccumulateMessage = buffer[..Convert.ToInt32(size)];
            }
            else
            {
                var accumulateMessage = new byte[AccumulateMessage.Length + size];
                Buffer.BlockCopy(AccumulateMessage, 0, accumulateMessage, 0, AccumulateMessage.Length);
                Buffer.BlockCopy(buffer, 0, accumulateMessage, AccumulateMessage.Length, Convert.ToInt32(size));
                AccumulateMessage = accumulateMessage;
            }

            while (AccumulateMessage.Length >= 4 && !MessageIncomplete)
            {
                var ff = AccumulateMessage[..4];
                if (BitConverter.IsLittleEndian) Array.Reverse(ff);

                var messageSize = BitConverter.ToInt32(ff, 0);

                if (messageSize <= AccumulateMessage.Length - 4)
                {
                    var messageString = AccumulateMessage[4..(messageSize + 4)];

                    var message = DeserializeSocketMessage(messageString);

                    AccumulateMessage = AccumulateMessage[(messageSize + 4)..];
                    MessageIncomplete = false;
                    yield return message;
                }
                else
                {
                    MessageIncomplete = true;
                    yield break;
                }
            }
        }

        private static SocketMessage DeserializeSocketMessage(byte[] messageString)
        {
            SocketMessage? message;
            try
            {
                message = JsonSerializer.Deserialize<SocketMessage>(messageString);
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
    }
}