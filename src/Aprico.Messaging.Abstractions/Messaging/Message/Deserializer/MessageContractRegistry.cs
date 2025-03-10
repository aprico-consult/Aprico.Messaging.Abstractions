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
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Aprico.Messaging.Message.Deserializer;

/// <summary>Maintains a collection of message contract types authorized for deserialization in a targeted messaging endpoint.</summary>
/// <remarks>
/// <para>
/// Enables precise control over message contract type registration, limiting deserialization to supported types within a
/// specific endpoint.
/// </para>
/// <para>
/// Provides a thread-safe runtime system for identifying and retrieving message contract types via computed string-based
/// identifiers.
/// </para>
/// <para>
/// Supports flexible contract type registration through:
/// <list type="bullet">
/// <item>Granular single-type registration</item>
/// <item>Bulk assembly-level type discovery and registration</item> <item>Duplicate registration prevention</item>
/// </list>
/// </para>
/// </remarks>
/// <param name="getContractIdentifierDelegate">
/// A delegate function that converts a <see cref="Type"/> to its corresponding
/// string-based identifier. This delegate is used to generate unique identifiers for message contract types during registration.
/// </param>
/// <threadsafety>
/// Ensures thread-safe concurrent read and write operations through the use of
/// <see cref="ConcurrentDictionary{TKey,TValue}"/>, preventing race conditions during type registration.
/// </threadsafety>
[SuppressMessage("ReSharper", "MemberCanBeInternal", Justification = "Public API.")]
public class MessageContractRegistry(Func<Type, string?> getContractIdentifierDelegate)
{
	private Func<Type, string?> GetContractIdentifierDelegate { get; } = getContractIdentifierDelegate ?? throw new ArgumentNullException(nameof(getContractIdentifierDelegate));

	/// <summary>Determines whether a contract <typeparamref name="T"/> is registered.</summary>
	/// <typeparam name="T">The message contract type to check for registration.</typeparam>
	/// <returns><see langword="true"/> if the contract <typeparamref name="T"/> is registered; otherwise, <see langword="false"/>.</returns>
	public bool IsContractRegistered<T>()
		where T : notnull
	{
		return IsContractRegistered(typeof(T));
	}

	/// <summary>Determines whether a contract <see cref="Type"/> is registered.</summary>
	/// <param name="type">message contract type to check for registration.</param>
	/// <returns><see langword="true"/> if the contract <paramref name="type"/> is registered; otherwise, <see langword="false"/>.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="type"/> is null.</exception>
	public bool IsContractRegistered(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);
		return IsContractRegistered(GetContractIdentifierDelegate.Invoke(type));
	}

	/// <summary>Determines whether a contract type is registered using its string-based identifier.</summary>
	/// <param name="contractIdentifier">The string-based identifier of the contract type to check.</param>
	/// <returns>
	/// <see langword="true"/> if a contract type with the specified <paramref name="contractIdentifier"/> is registered;
	/// otherwise, <see langword="false"/>.
	/// </returns>
	public bool IsContractRegistered(string? contractIdentifier)
	{
		return contractIdentifier is not null && _registry.ContainsKey(contractIdentifier);
	}

	/// <summary>Retrieves the registered contract <see cref="Type"/> for a given <paramref name="contractIdentifier"/>.</summary>
	/// <param name="contractIdentifier">The string-based identifier of the contract type to retrieve.</param>
	/// <returns>The <see cref="Type"/> registered for the specified <paramref name="contractIdentifier"/>.</returns>
	/// <exception cref="InvalidOperationException">
	/// Thrown if no contract <see cref="Type"/> has been registered for the specified
	/// <paramref name="contractIdentifier"/>.
	/// </exception>
	/// <exception cref="ArgumentNullException">Thrown if the provided <paramref name="contractIdentifier"/> is null.</exception>
	public Type GetRegisteredContract(string contractIdentifier)
	{
		return TryGetRegisteredContract(contractIdentifier, out var type)
			? type
			: throw new InvalidOperationException($"No contract type has been registered for message contract identifier '{contractIdentifier}'.");
	}

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
	public bool TryGetRegisteredContract(string contractIdentifier, [NotNullWhen(returnValue: true)] out Type? type)
	{
		return _registry.TryGetValue(contractIdentifier, out type);
	}

	/// <summary>Registers a specific message contract <typeparamref name="T"/>.</summary>
	/// <typeparam name="T">The contract type <typeparamref name="T"/> to register.</typeparam>
	/// <returns>
	/// The current <see cref="MessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	public MessageContractRegistry RegisterContract<T>()
		where T : notnull
	{
		return RegisterContract(typeof(T));
	}

	/// <summary>Registers a message contract <paramref name="type"/>.</summary>
	/// <param name="type">The message contract type to be registered.</param>
	/// <returns>
	/// The current <see cref="MessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	public MessageContractRegistry RegisterContract(Type type)
	{
		ArgumentNullException.ThrowIfNull(type);
		RegisterContractType(type);
		return this;
	}

	/// <summary>
	/// Automatically registers all message contract types from the assembly containing the specified type
	/// <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">A representative type from the assembly containing the message contract types.</typeparam>
	/// <returns>
	/// The current <see cref="MessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	/// <remarks>
	/// Only <see cref="Assembly.ExportedTypes"/> types with a non-<see langword="null"/> and non-<see cref="string.Empty"/>
	/// contract identifier will be registered.
	/// </remarks>
	[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global", Justification = "Fluent API.")]
	public MessageContractRegistry RegisterContractAssembly<T>()
		where T : notnull
	{
		return RegisterContractAssembly(typeof(T).Assembly);
	}

	/// <summary>Automatically registers all message contract types from the specified <paramref name="assembly"/>.</summary>
	/// <param name="assembly">The assembly containing the message contract types.</param>
	/// <returns>
	/// The current <see cref="MessageContractRegistry"/> instance, enabling fluent method chaining for contract type
	/// registrations.
	/// </returns>
	/// <remarks>
	/// Only <see cref="Assembly.ExportedTypes"/> types with a non-<see langword="null"/> and non-<see cref="string.Empty"/>
	/// contract identifier will be registered.
	/// </remarks>
	[SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API.")]
	public MessageContractRegistry RegisterContractAssembly(Assembly assembly)
	{
		ArgumentNullException.ThrowIfNull(assembly);
		foreach (var type in assembly.ExportedTypes.Where(t => !string.IsNullOrWhiteSpace(GetContractIdentifierDelegate(t)))) RegisterContractType(type);
		return this;
	}

	private void RegisterContractType(Type type)
	{
		var contractIdentifier = GetContractIdentifierDelegate.Invoke(type);
		if (string.IsNullOrWhiteSpace(contractIdentifier)) return;
		if (_registry.TryAdd(contractIdentifier, type)) return;
		var registeredType = _registry[contractIdentifier];
		if (registeredType == type) return;
		throw new InvalidOperationException(
			$"The message contract identifier '{contractIdentifier}' has already been registered by the contract type '{registeredType.FullName}' and cannot be registered again by the contract type '{type.FullName}'.");
	}

	private readonly ConcurrentDictionary<string, Type> _registry = [];
}
