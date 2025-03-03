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

/// <summary>Defines a contract for serializing payload objects into messages with optional metadata.</summary>
/// <typeparam name="TMessage">The type of the serialized message, which must be a non-null type.</typeparam>
/// <remarks>
/// This interface provides a standardized mechanism for transforming payloads into messaging broker-specific messages,
/// with support for enriching messages through optional contextual metadata.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IMessageSerializer<out TMessage>
	where TMessage : notnull
{
	/// <summary>Serializes the given body (payload) into a message with optional metadata.</summary>
	/// <typeparam name="TBody">The type of the body to be serialized, which must be a non-null type.</typeparam>
	/// <param name="body">The payload to be serialized into a message.</param>
	/// <param name="messageId">Optional unique identifier for the message.</param>
	/// <param name="correlationId">Optional identifier used to correlate related messages.</param>
	/// <param name="sessionId">Optional identifier for grouping related messages in a session.</param>
	/// <param name="businessId">Optional business-specific identifier.</param>
	/// <param name="timestamp">Optional timestamp indicating when the message was created.</param>
	/// <param name="scheduledEnqueueTime">Optional time when the message should be enqueued.</param>
	/// <returns>A serialized message of type <typeparamref name="TMessage"/>.</returns>
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
