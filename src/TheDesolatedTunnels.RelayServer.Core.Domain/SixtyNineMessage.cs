// ***********************************************************************
// Assembly         : TheDesolatedTunnels.RelayServer.Core.Domain
// Author           : Tom Esendam (ts.esendam@student.han.nl)
// Created          : 12-14-2020
//
// Last Modified By : Tom Esendam (ts.esendam@student.han.nl)
// Last Modified On : 12-15-2020
// ***********************************************************************
// <copyright file="SixtyNineMessage.cs" company="TheDesolatedTunnels.RelayServer.Core.Domain">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;

namespace TheDesolatedTunnels.RelayServer.Core.Domain
{
    public abstract record SixtyNineMessage(string? Source, SixtyNineMessageType PayloadType);

    public abstract record SixtyNineSendibleMessage(string Destination, SixtyNineMessageType PayloadType,
        Memory<byte>? Payload = default, string? Source = null) : SixtyNineMessage(Source,
        PayloadType);

    public record InitMessage(string? Source) : SixtyNineMessage(Source,
        SixtyNineMessageType.Init);

    public record InitResponseMessage(string Destination) : SixtyNineSendibleMessage(Destination,
        SixtyNineMessageType.Init);

    public record PayloadMessage(string Source, string Destination, Memory<byte>? Payload) : SixtyNineSendibleMessage(
        Destination, SixtyNineMessageType.Payload, Payload, Source);

    public record ErrorMessage
        (string Destination, Memory<byte>? Payload, string? Source = null) : SixtyNineSendibleMessage(Destination,
            SixtyNineMessageType.Error, Payload, Source);

    public record CloseMessage() : SixtyNineMessage(null, SixtyNineMessageType.Init);
}