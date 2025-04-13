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

namespace Aprico.Messaging.Outbox;

/// <summary>Defines an abstraction for publishing messages to an external messaging infrastructure.</summary>
/// <typeparam name="TMessage">The type of message to be published.</typeparam>
/// <remarks>
/// <para>
/// This interface establishes a contract for sending messages to external systems. Implementations may target specific
/// messaging platforms such as Azure Service Bus, RabbitMQ, or others.
/// </para>
/// <para>
/// Messages sharing a common subject are typically routed to the same logical destination — such as a topic, queue, or
/// channel — within the messaging infrastructure.
/// </para>
/// </remarks>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IMessagePublisher<in TMessage>
{
	/// <summary>
	/// Publishes a collection of messages, using the specified subject to determine the appropriate logical destination
	/// within the messaging infrastructure.
	/// </summary>
	/// <param name="subject">
	/// A logical identifier used to route the messages — typically mapped to a topic, channel, or queue.
	/// Messages that share the same subject are generally published to the same destination.
	/// </param>
	/// <param name="message">The collection of messages to publish.</param>
	/// <param name="cancellationToken">A token to observe while waiting for the operation to complete.</param>
	/// <returns>A task representing the asynchronous publish operation.</returns>
	Task PublishAsync(string subject, IEnumerable<TMessage> message, CancellationToken cancellationToken = default);
}
