#region Copyright & License

// Copyright © 2024 - 2025 Aprico Consultants
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aprico.Messaging.Abstractions;

/// <summary>
/// Defines a contract for assembling messages of a specific type <typeparamref name="TMessage"/> with a payload body and
/// optional metadata.
/// </summary>
/// <typeparam name="TMessage">The non-null type of the serialized message.</typeparam>
/// <remarks>
/// This interface provides a standardized mechanism for assembling messages of a specific messaging broker type
/// <typeparamref name="TMessage"/>, with a given payload and optional contextual metadata.
/// </remarks>
/// <seealso cref="IMessageDisassembler{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IMessageAssembler<out TMessage>
	where TMessage : notnull
{
	/// <summary>Assembles a <typeparamref name="TMessage"/> message with a payload <paramref name="body"/> and optional metadata.</summary>
	/// <typeparam name="TBody">The non-null type of the body to be serialized as the <typeparamref name="TMessage"/> payload.</typeparam>
	/// <param name="body">The payload to be serialized into the assembled <typeparamref name="TMessage"/> message.</param>
	/// <param name="messageId">Optional unique identifier for the message.</param>
	/// <param name="correlationId">Optional identifier to correlate related messages.</param>
	/// <param name="sessionId">Optional identifier for grouping related messages within a session.</param>
	/// <param name="businessId">Optional business-specific identifier.</param>
	/// <param name="timestamp">Optional timestamp indicating the message creation time.</param>
	/// <param name="scheduledEnqueueTime">Optional time for message enqueuing.</param>
	/// <returns>An assembled message of type <typeparamref name="TMessage"/>.</returns>
	TMessage Serialize<TBody>(
		TBody body,
		string? messageId = null,
		string? correlationId = null,
		string? sessionId = null,
		string? businessId = null,
		DateTimeOffset? timestamp = null,
		DateTimeOffset? scheduledEnqueueTime = null)
		where TBody : notnull;
}
