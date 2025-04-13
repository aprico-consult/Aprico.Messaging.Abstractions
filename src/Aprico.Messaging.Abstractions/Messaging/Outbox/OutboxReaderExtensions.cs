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

namespace Aprico.Messaging.Outbox;

[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
public static class OutboxReaderExtensions
{
	/// <summary>
	/// Provides a convenient, immutable empty result matching the return type of
	/// <see cref="IOutboxReader{TMessage}.DequeueAsync"/>.
	/// </summary>
	/// <value>A tuple containing an empty string as the subject and an empty collection of messages.</value>
	/// <remarks>
	/// <para>
	/// This default implementation provides a standardized, semantically clear approach to representing a "no messages" state.
	/// Using an empty tuple instead of <see langword="null"/> (which is not supported for value tuples) simplifies the interface
	/// design and eliminates the need for manual tuple creation in each implementation.
	/// </para>
	/// <para>
	/// Besides, trying to support <see langword="null"/> results would have introduced unnecessary complexity, requiring
	/// extensive null-checking and potentially increasing the risk of null-reference exceptions.
	/// </para>
	/// </remarks>
	/// <example>
	/// Instead of manually creating an empty result
	/// <code><![CDATA[
	/// return (string.Empty, Enumerable.Empty<TMessage>());
	/// ]]></code> you can simply use
	/// <code><![CDATA[
	/// return this.Empty();
	/// ]]></code>
	/// </example>
	public static (string Subject, IEnumerable<TMessage> Messages) Empty<TMessage>(this IOutboxReader<TMessage> _)
	{
		return (string.Empty, []);
	}
}
