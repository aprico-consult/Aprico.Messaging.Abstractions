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
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aprico.Messaging.Outbox.Worker;

/// <summary>Coordinates the dispatch of messages from a transactional outbox to an external message broker.</summary>
/// <typeparam name="TMessage">The type of messages to be dispatched.</typeparam>
/// <remarks>
/// <para>
/// This class implements the orchestration logic of the transactional outbox pattern. It retrieves messages from an outbox
/// store and publishes them to an external messaging infrastructure via a message publisher.
/// </para>
/// <para>
/// It uses a provided database connection to repeatedly create transactions and flush the outbox until no messages remain.
/// Each batch of messages is processed within its own transaction to ensure consistency and resilience.
/// </para>
/// <para>
/// Designed for use in infrastructure or background services, this component ensures reliable and consistent message
/// delivery by coupling the dequeue and publish operations within transactional boundaries.
/// </para>
/// </remarks>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
public class OutboxDispatcher<TMessage>
{
	public OutboxDispatcher(IOutboxReader<TMessage> outboxReader, IMessagePublisher<TMessage> messagePublisher)
	{
		ArgumentNullException.ThrowIfNull(outboxReader);
		ArgumentNullException.ThrowIfNull(messagePublisher);
		_outboxReader = outboxReader;
		_messagePublisher = messagePublisher;
	}

	/// <summary>
	/// Dispatches all messages currently in the outbox using the provided database connection, creating a new transaction for
	/// each batch until the outbox is fully drained.
	/// </summary>
	/// <param name="connection">The database connection used to initiate transactions for dequeuing and publishing messages.</param>
	/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
	/// <returns>A task representing the asynchronous dispatch operation.</returns>
	/// <remarks>
	/// <para>
	/// This method dequeues messages from the outbox store and publishes them to the external messaging infrastructure within
	/// individual transactions. The outbox is flushed in multiple batches, each processed in its own transaction. A transaction is
	/// committed only after successful publishing, ensuring consistency between local state and the message broker.
	/// </para>
	/// <para>Intended to be called by background workers or scheduled jobs responsible for processing the outbox periodically.</para>
	/// </remarks>
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "Any() only enumerates one element.")]
	public async Task DispatchAsync(DbConnection connection, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(connection);
		while (true)
		{
			await using var transaction = await connection.BeginTransactionAsync(cancellationToken);
			var (subject, messages) = await _outboxReader.DequeueAsync(transaction, cancellationToken);
			if (messages.Any() == false) break;
			await _messagePublisher.PublishAsync(subject, messages, cancellationToken);
			await transaction.CommitAsync(cancellationToken);
		}
	}

	private readonly IMessagePublisher<TMessage> _messagePublisher;
	private readonly IOutboxReader<TMessage> _outboxReader;
}
