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
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Aprico.Messaging.Message.Deserializer;

/// <summary>
/// Defines a contract for registering and managing message contract types authorized for deserialization in a targeted
/// messaging endpoint.
/// </summary>
/// <remarks>
/// This interface provides methods for registering, tracking, and retrieving message contract types used in messaging or
/// communication scenarios.
/// </remarks>
[SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Public API.")]
public interface IMessageContractRegistry
{
	/// <summary>Determines whether a contract <typeparamref name="T"/> is registered.</summary>
	/// <typeparam name="T">The message contract type to check for registration.</typeparam>
	/// <returns><see langword="true"/> if the contract <typeparamref name="T"/> is registered; otherwise, <see langword="false"/>.</returns>
	bool IsContractRegistered<T>()
		where T : notnull;

	/// <summary>Determines whether a contract <see cref="Type"/> is registered.</summary>
	/// <param name="type">message contract type to check for registration.</param>
	/// <returns><see langword="true"/> if the contract <paramref name="type"/> is registered; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="type"/> is null.</exception>
	bool IsContractRegistered(Type type);

	/// <summary>Determines whether a contract type is registered using its string-based identifier.</summary>
	/// <param name="contractIdentifier">The string-based identifier of the contract type to check.</param>
	/// <returns>
	/// <see langword="true"/> if a contract type with the specified <paramref name="contractIdentifier"/> is registered;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	bool IsContractRegistered(string? contractIdentifier);

	/// <summary>Retrieves the registered contract <see cref="Type"/> for a given <paramref name="contractIdentifier"/>.</summary>
	/// <param name="contractIdentifier">The string-based identifier of the contract type to retrieve.</param>
	/// <returns>The <see cref="Type"/> registered for the specified <paramref name="contractIdentifier"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if no contract <see cref="Type"/> has been registered for the specified
	/// <paramref name="contractIdentifier"/>.
	/// </exception>
	/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="contractIdentifier"/> is null.</exception>
	Type GetRegisteredContract(string contractIdentifier);

	/// <summary>
	/// Attempts to retrieve the registered contract <see cref="Type"/> associated with the specified
	/// <paramref name="contractIdentifier"/>.
	/// </summary>
	/// <param name="contractIdentifier">The string-based identifier of the contract type to retrieve.</param>
	/// <param name="type">
	/// When this method returns, contains the registered contract <see cref="Type"/> if found, or
	/// <see langword="null"/> if no matching contract is registered.
	/// </param>
	/// <returns>
	/// <see langword="true"/> if a contract type is found for the specified <paramref name="contractIdentifier"/>; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	/// <remarks>
	/// This method provides a safe way to retrieve a registered contract type without throwing an exception if the contract
	/// identifier is not found in the registry.
	/// </remarks>
	bool TryGetRegisteredContract(string contractIdentifier, [NotNullWhen(returnValue: true)] out Type? type);

	/// <summary>Registers a specific message contract <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The contract type <typeparamref name="T"/> to register.</typeparam>
	/// <returns>
	/// The current <see cref="IMessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	IMessageContractRegistry RegisterContract<T>()
		where T : notnull;

	/// <summary>Registers a message contract <paramref name="type"/>.</summary>
	/// <param name="type">The message contract type to be registered.</param>
	/// <returns>
	/// The current <see cref="IMessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	IMessageContractRegistry RegisterContract(Type type);

	/// <summary>
	/// Automatically registers all message contract types from the assembly containing the specified type
	/// <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">A representative type from the assembly containing the message contract types.</typeparam>
	/// <returns>
	/// The current <see cref="IMessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	/// <remarks>
	/// Only <see cref="Assembly.ExportedTypes"/> types with a non-<see langword="null"/> and non-<see cref="string.Empty"/>
	/// contract identifier will be registered.
	/// </remarks>
	IMessageContractRegistry RegisterContractAssembly<T>()
		where T : notnull;

	/// <summary>Automatically registers all message contract types from the specified <paramref name="assembly"/>.</summary>
	/// <param name="assembly">The assembly containing the message contract types.</param>
	/// <returns>
	/// The current <see cref="IMessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	/// <remarks>
	/// Only <see cref="Assembly.ExportedTypes"/> types with a non-<see langword="null"/> and non-<see cref="string.Empty"/>
	/// contract identifier will be registered.
	/// </remarks>
	IMessageContractRegistry RegisterContractAssembly(Assembly assembly);
}
