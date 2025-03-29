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
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Aprico.Messaging.Abstractions.Outbox;

/// <summary>Abstraction for a transactional outbox client, allowing messages to be enqueued as part of the ambient transaction.</summary>
/// <typeparam name="TMessage">The type of messages to be stored in the outbox.</typeparam>
/// <remarks>
/// The transactional outbox pattern ensures atomic and reliable message delivery by storing messages within the same
/// transaction as other domain changes. Messages are later dequeued and dispatched by a background worker.
/// </remarks>
/// <seealso cref="IOutboxWorker{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IOutboxClient<in TMessage>
{
	/// <summary>Enqueues a single message into the outbox as part of the specified database transaction.</summary>
	/// <param name="transaction">The database transaction that the enqueue operation will participate in.</param>
	/// <param name="destinationAggregate">The name of the destination aggregate the message is intended for.</param>
	/// <param name="message">The message to enqueue.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
	/// <returns>A task representing the asynchronous enqueue operation.</returns>
	Task EnqueueAsync(DbTransaction transaction, string destinationAggregate, TMessage message, CancellationToken cancellationToken = default);

	/// <summary>Enqueues multiple messages into the outbox as part of the specified database transaction.</summary>
	/// <param name="transaction">The database transaction that the enqueue operation will participate in.</param>
	/// <param name="destinationAggregate">The name of the destination aggregate the message is intended for.</param>
	/// <param name="messages">The collection of messages to be enqueued.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous enqueue operation.</returns>
	Task EnqueueAsync(DbTransaction transaction, string destinationAggregate, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
}
