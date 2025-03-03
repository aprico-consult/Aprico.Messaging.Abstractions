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

namespace Aprico.Messaging.Abstractions;

/// <summary>Defines a contract for deserializing messages into their underlying payload objects.</summary>
/// <typeparam name="TMessage">The type of the message to be deserialized, which must be a non-null type.</typeparam>
/// <remarks>
/// This interface provides a standardized mechanism to extracting and deserializing message payloads from messaging
/// broker-specific messages.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Public API.")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Public API.")]
public interface IMessageDeserializer<in TMessage>
	where TMessage : notnull
{
	/// <summary>Extracts and returns the body (payload) from the given message.</summary>
	/// <param name="message">The message whose payload has to be deserialized.</param>
	/// <returns>The payload of the message as an object.</returns>
	object DeserializeBody(TMessage message);
}
