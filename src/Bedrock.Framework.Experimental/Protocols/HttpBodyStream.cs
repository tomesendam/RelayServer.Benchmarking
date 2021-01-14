﻿using System;
using System.Buffers;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bedrock.Framework.Protocols
{
    internal class HttpBodyStream : Stream
    {
        private ProtocolReader _reader;
        private IMessageReader<ReadOnlySequence<byte>> _bodyReader;

        public HttpBodyStream(ProtocolReader reader, IMessageReader<ReadOnlySequence<byte>> bodyReader)
        {
            _reader = reader;
            _bodyReader = bodyReader;
        }

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new System.NotImplementedException();

        public override long Position { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            var result = await _reader.ReadAsync(_bodyReader, maximumMessageSize: buffer.Length, cancellationToken).ConfigureAwait(false);
            var data = result.Message;

            // Connection died
            if (result.IsCompleted)
            {
                return 0;
            }

            if (data.Length > 0)
            {
                data.CopyTo(buffer.Span);
            }

            _reader.Advance();

            return (int)data.Length;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}