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

namespace Aprico.Messaging;

/// <summary>Abstraction for a transactional outbox client, allowing messages to be enqueued as part of a database transaction.</summary>
/// <typeparam name="TMessage">The type of messages to be persisted to the outbox store.</typeparam>
/// <remarks>
/// This interface is intended for use by application-layer code to persist outgoing messages as part of the same database
/// transaction that modifies domain state. According to the transactional outbox pattern, messages are later retrieved from the
/// outbox and delivered asynchronously by a background delivery process.
/// </remarks>
/// <seealso cref="IOutboxStore{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IOutbox<in TMessage>
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
	/// <param name="destinationAggregate">The name of the destination aggregate the messages are intended for.</param>
	/// <param name="messages">The collection of messages to enqueue.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous enqueue operation.</returns>
	Task EnqueueAsync(DbTransaction transaction, string destinationAggregate, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);
}
