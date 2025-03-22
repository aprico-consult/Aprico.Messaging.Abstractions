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

namespace Aprico.Messaging.Abstractions;

/// <summary>Represents an abstraction for a transactional message outbox pattern implementation.</summary>
/// <typeparam name="TMessage">The type of messages to be stored in the outbox.</typeparam>
/// <remarks>
/// The Outbox pattern is a mechanism for ensuring reliable message publishing within a transactional context. It allows
/// messages to be enqueued and dequeued within a database transaction, providing atomicity and reliability.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IOutbox<TMessage>
{
	/// <summary>Enqueues a single message to the outbox within the specified database transaction.</summary>
	/// <param name="transaction">The database transaction in which the message will be enqueued.</param>
	/// <param name="entityName">The name of the entity associated with the message.</param>
	/// <param name="message">The message to be enqueued.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous enqueue operation.</returns>
	Task EnqueueAsync(DbTransaction transaction, string entityName, TMessage message, CancellationToken cancellationToken = default);

	/// <summary>Enqueues multiple messages to the outbox within the specified database transaction.</summary>
	/// <param name="transaction">The database transaction in which the messages will be enqueued.</param>
	/// <param name="entityName">The name of the entity associated with the messages.</param>
	/// <param name="messages">A collection of messages to be enqueued.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task representing the asynchronous enqueue operation.</returns>
	Task EnqueueAsync(DbTransaction transaction, string entityName, IEnumerable<TMessage> messages, CancellationToken cancellationToken = default);

	/// <summary>Dequeues a specified number of messages from the outbox within the given database transaction.</summary>
	/// <param name="transaction">The database transaction in which the messages will be dequeued.</param>
	/// <param name="messageCount">The maximum number of messages to retrieve.</param>
	/// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
	/// <returns>A task that represents the asynchronous dequeue operation, yielding a collection of messages.</returns>
	/// <remarks>
	/// The implementation should ensure that dequeued messages are removed from the outbox and are not retrievable in
	/// subsequent dequeue operations.
	/// </remarks>
	Task<IEnumerable<TMessage>> DequeueAsync(DbTransaction transaction, int messageCount, CancellationToken cancellationToken = default);
}
