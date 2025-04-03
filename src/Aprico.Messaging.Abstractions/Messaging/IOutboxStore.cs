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

/// <summary>Abstraction for accessing messages stored in the transactional outbox for delivery processing.</summary>
/// <typeparam name="TMessage">The type of messages to be delivered from the outbox.</typeparam>
/// <remarks>
/// As part of the transactional outbox pattern, this interface defines an abstraction for retrieving previously enqueued
/// messages from the outbox message store. It is intended to be used by background delivery components responsible for dispatching
/// these messages via a messaging broker.
/// </remarks>
/// <seealso cref="IOutbox{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IOutboxStore<TMessage>
{
	/// <summary>Dequeues up to the specified number of messages from the outbox as part of the given database transaction.</summary>
	/// <param name="transaction">The database transaction that the dequeue operation will participate in.</param>
	/// <param name="messageCount">The maximum number of messages to retrieve.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the asynchronous operation.</param>
	/// <returns>
	/// A task representing the asynchronous dequeue operation, yielding a tuple with a subject, or topic, and a collection
	/// of messages pertaining to that subject or topic.
	/// </returns>
	/// <remarks>
	/// The implementation should ensure that dequeued messages are removed from the outbox and are not retrievable in
	/// subsequent dequeue operations.
	/// </remarks>
	Task<(string Subject, IEnumerable<TMessage> Messages)> DequeueAsync(DbTransaction transaction, int messageCount, CancellationToken cancellationToken = default);
}
