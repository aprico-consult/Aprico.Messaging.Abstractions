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

using System.Diagnostics.CodeAnalysis;
using Aprico.Messaging.Message.Deserializer;

namespace Aprico.Messaging.Abstractions;

/// <summary>
/// Defines a contract for disassembling messages of a specific type <typeparamref name="TMessage"/> returning the
/// deserialized payload body.
/// </summary>
/// <typeparam name="TMessage">The non-null type of the serialized message.</typeparam>
/// <remarks>
/// This interface provides a standardized mechanism for disassembling messages from a specific messaging broker type
/// <typeparamref name="TMessage"/> and deserializing their payloads.
/// </remarks>
/// <seealso cref="IMessageAssembler{TMessage}"/>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IMessageDisassembler<in TMessage>
	where TMessage : notnull
{
	/// <summary>Deserializes the payload body from the given <typeparamref name="TMessage"/> message.</summary>
	/// <param name="message">
	/// The <typeparamref name="TMessage"/> message to be disassembled and whose payload requires
	/// deserialization.
	/// </param>
	/// <returns>The deserialized message payload object.</returns>
	object DeserializeBody(TMessage message);

	/// <summary>
	/// Deserializes the payload body from the given <typeparamref name="TMessage"/> message using the specified message
	/// contract registry.
	/// </summary>
	/// <param name="message">
	/// The <typeparamref name="TMessage"/> message to be disassembled and whose payload requires
	/// deserialization.
	/// </param>
	/// <param name="messageContractRegistry">The <see cref="IMessageContractRegistry"/> containing message contract information.</param>
	/// <returns>The deserialized message payload object.</returns>
	object DeserializeBody(TMessage message, IMessageContractRegistry messageContractRegistry);
}
