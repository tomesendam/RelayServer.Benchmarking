using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using TerminalGame.RelayServer.Lib;

namespace TerminalGame.RelayServer.Benchmark
{
    [MemoryDiagnoser]
    public class SpanVsString
    {
        public SpanVsString()
        {
            MessageHandler = new MessageHandler();
            SpanMessageHandler = new SpanMessageHandler();
            MessageHandlerWithoutYield = new MessageHandlerWithoutYield();
            _messageToDecode = Encoding.UTF8.GetBytes(
                "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
                + "{[35][{\"payloadType\":\"INIT\",\"source\":\"1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload0\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload1\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload2\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload3\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload4\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload5\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload6\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload7\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload8\"}]}{[77][{\"payloadType\":\"MESSAGE\",\"destination\":\"0\",\"source\":\"1\",\"payload\":\"Payload9\"}]}"
            );
        }

        private SpanMessageHandler SpanMessageHandler { get; }

        private MessageHandler MessageHandler { get; }
        
        private MessageHandlerWithoutYield MessageHandlerWithoutYield { get; }
        
        private readonly byte[] _messageToDecode;
        
        private readonly Consumer _consumer = new();
        
        [Benchmark]
        public void Span()
        {
            SpanMessageHandler.DecodeBuffer(_messageToDecode, 0, 9823).ToList();
        }

        [Benchmark]
        public void String()
        {
            MessageHandler.DecodeBuffer(_messageToDecode, 0, 9823).ToList();
        }
        
        // [Benchmark]
        // public void StringWithoutYield()
        // {
        //     MessageHandlerWithoutYield.DecodeBuffer(_messageToDecode, 0, 893).ToList();
        // }
    }


    internal class Program
    {
        public static void Main(string[] args) //=> BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
        {
            var summary = BenchmarkRunner.Run(typeof(Program).Assembly);
        }
    }
}