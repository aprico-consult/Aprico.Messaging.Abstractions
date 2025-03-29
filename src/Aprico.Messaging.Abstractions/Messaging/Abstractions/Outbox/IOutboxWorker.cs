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

/// <summary>Background worker abstraction responsible for dequeuing and dispatching messages stored in the transactional outbox.</summary>
/// <typeparam name="TMessage">The type of messages to be processed from the outbox.</typeparam>
/// <remarks>
/// As part of the transactional outbox pattern, this component retrieves messages that were enqueued during a database
/// transaction and reliably dispatches them to their intended destinations.
/// </remarks>
/// <seealso cref="IOutboxClient{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IOutboxWorker<TMessage>
{
	/// <summary>Dequeues up to the specified number of messages from the outbox as part of the given database transaction.</summary>
	/// <param name="transaction">The database transaction that the dequeue operation will participate in.</param>
	/// <param name="messageCount">The maximum number of messages to retrieve.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous dequeue operation, yielding a collection of messages.</returns>
	/// <remarks>
	/// The implementation should ensure that dequeued messages are removed from the outbox and are not retrievable in
	/// subsequent dequeue operations.
	/// </remarks>
	// Task<IEnumerable<TMessage>> DequeueAsync(DbTransaction transaction, int messageCount, CancellationToken cancellationToken = default);
	Task<(string destinationAggregate, IEnumerable<TMessage> messages)> DequeueAsync(DbTransaction transaction, int messageCount, CancellationToken cancellationToken = default);
}
