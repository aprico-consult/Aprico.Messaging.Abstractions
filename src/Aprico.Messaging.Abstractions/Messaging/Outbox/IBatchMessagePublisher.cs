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

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Aprico.Messaging.Outbox.Worker;

namespace Aprico.Messaging.Outbox;

/// <summary>Defines an abstraction for publishing a batch of messages to an external messaging infrastructure.</summary>
/// <typeparam name="TMessage">The type of message to be published.</typeparam>
/// <remarks>
/// <para>
/// This interface is designed to support the Outbox pattern by enabling the publication of messages that have been dequeued
/// from a transactional outbox store.
/// </para>
/// <para>
/// It is typically used in conjunction with the <see cref="OutboxDispatcher{TMessage}"/>, which orchestrates the retrieval
/// and dispatch of messages. The interface abstracts over brokered messaging systems such as Azure Service Bus, RabbitMQ, and
/// others, allowing implementations to integrate with different platforms.
/// </para>
/// </remarks>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IBatchMessagePublisher<in TMessage>
{
	/// <summary>
	/// Publishes a collection of messages, using the specified subject to determine the appropriate logical destination
	/// within the messaging infrastructure.
	/// </summary>
	/// <param name="subject">
	/// A logical identifier used to route the messages — typically mapped to a topic, channel, or queue.
	/// Messages that share the same subject are generally published to the same destination.
	/// </param>
	/// <param name="messages">The collection of messages to publish.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
	/// <returns>A task representing the asynchronous publish operation.</returns>
	Task PublishAsync(string subject, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
}
