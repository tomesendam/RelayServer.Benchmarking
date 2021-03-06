using System;
using System.Buffers;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoMoq;
using Microsoft.AspNetCore.Connections;
using Moq;
using TheDesolatedTunnels.RelayServer.Core.ConnectionHandlers;
using TheDesolatedTunnels.RelayServer.Core.Domain;
using TheDesolatedTunnels.RelayServer.Core.Interfaces.Services;
using TheDesolatedTunnels.RelayServer.Core.Services;
using Xunit;

namespace TheDesolatedTunnels.RelayServer.Core.Tests.ConnectionHandlers
{
    public class SixtyNineProtocolHandlerTests
    {
        [Fact(Skip =
            "next to impossible to test. due to the dependency of a connection which cancellation token is used in a while loop.")]
        public async Task OnConnectedAsync_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var mocker = new AutoMoqer();
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var sixtyNineProtocolHandler = mocker.Create<SixtyNineProtocolHandler>();
            ConnectionContext connection = fixture.Build<DefaultConnectionContext>()
                .With(x => x.ConnectionClosed, new CancellationToken(false)).WithAutoProperties().Create();

            SixtyNineMessage expected = new InitMessage("megakek");

            mocker.GetMock<SixtyNineReader>().Setup(x => x.TryParseMessage(It.IsAny<ReadOnlySequence<byte>>(),
                ref It.Ref<SequencePosition>.IsAny, ref It.Ref<SequencePosition>.IsAny, out expected));


            // Act
            await sixtyNineProtocolHandler.OnConnectedAsync(
                connection);

            Thread.Sleep(100);
            connection.ConnectionClosed = new CancellationToken(true);
            // Assert

            mocker.GetMock<IMessageHandler>()
                .Verify(x => x.HandleMessage(It.IsAny<ConnectionContext>(), It.IsAny<InitMessage>()), Times.Once);
        }
    }
}