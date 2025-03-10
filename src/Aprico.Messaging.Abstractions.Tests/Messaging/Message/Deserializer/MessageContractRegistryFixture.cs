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
using Aprico.Messaging.Message.Dummies;
using AutoFixture.Xunit2;
using FluentAssertions.Execution;

namespace Aprico.Messaging.Message.Deserializer;

public abstract class MessageContractRegistryFixture
{
	#region Nested Type: GetRegisteredContract

	public class GetRegisteredContract : MessageContractRegistryFixture
	{
		[Theory]
		[AutoData]
		public void FailsForUnregisteredContractType(string contractIdentifier)
		{
			var sut = new MessageContractRegistry(_ => contractIdentifier);
			Invoking(() => sut.GetRegisteredContract(contractIdentifier))
				.Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"No contract type has been registered for message contract identifier '{contractIdentifier}'.");
		}

		[Theory]
		[AutoData]
		public void SucceedsForRegisteredContractType(string contractIdentifier)
		{
			new MessageContractRegistry(_ => contractIdentifier).RegisterContract<ContractOne>()
				.GetRegisteredContract(contractIdentifier)
				.Should()
				.Be<ContractOne>();
		}
	}

	#endregion

	#region Nested Type: RegisterContract

	public class RegisterContract : MessageContractRegistryFixture
	{
		[Theory]
		[AutoData]
		public void CannotRegisterTwoContractTypesWithSameName(string contractIdentifier)
		{
			var sut = new MessageContractRegistry(_ => contractIdentifier).RegisterContract<ContractOne>();
			Invoking(sut.RegisterContract<ContractTwo>)
				.Should()
				.Throw<InvalidOperationException>()
				.WithMessage(
					$"The message contract identifier '{contractIdentifier}' " + $"has already been registered by the contract type '{typeof(ContractOne).FullName}' "
					+ $"and cannot be registered again by the contract type '{typeof(ContractTwo).FullName}'.");
		}

		[Theory]
		[AutoData]
		public void CanRegisterContractTypeTwice(string contractIdentifier)
		{
			var sut = new MessageContractRegistry(_ => contractIdentifier).RegisterContract<ContractOne>();
			Invoking(sut.RegisterContract<ContractOne>)
				.Should()
				.NotThrow();
		}

		[Theory]
		[AutoData]
		public void CanRegisterContractTypeWhoseNameIsNotNullOrEmpty(string contractIdentifier)
		{
			using var _ = new AssertionScope();

			var sut = new MessageContractRegistry(_ => contractIdentifier).RegisterContract<ContractOne>();
			sut.IsContractRegistered<ContractOne>()
				.Should()
				.BeTrue();
			sut.IsContractRegistered(contractIdentifier)
				.Should()
				.BeTrue();
		}

		[Fact]
		public void WillNotRegisterContractTypeWhoseNameIsEmpty()
		{
			using var _ = new AssertionScope();

			var sut = new MessageContractRegistry(static _ => string.Empty).RegisterContract(typeof(ContractOne));
			sut.IsContractRegistered(string.Empty)
				.Should()
				.BeFalse();
			sut.IsContractRegistered(typeof(ContractOne))
				.Should()
				.BeFalse();
		}

		[Fact]
		public void WillNotRegisterContractTypeWhoseNameIsNull()
		{
			using var _ = new AssertionScope();

			var sut = new MessageContractRegistry(static _ => null).RegisterContract(typeof(ContractOne));
			sut.IsContractRegistered(string.Empty)
				.Should()
				.BeFalse();
			sut.IsContractRegistered(typeof(ContractOne))
				.Should()
				.BeFalse();
		}
	}

	#endregion

	#region Nested Type: RegisterContractAssembly

	public class RegisterContractAssembly : MessageContractRegistryFixture
	{
		[Theory]
		[AutoData]
		public void CanRegisterContractTypesForAssembly(string contractIdentifierOne, string contractIdentifierTwo)
		{
			using var _ = new AssertionScope();

			var sut = new MessageContractRegistry(type => type == typeof(ContractOne) ? contractIdentifierOne : type == typeof(ContractTwo) ? contractIdentifierTwo : null)
				.RegisterContractAssembly<ContractOne>();
			sut.IsContractRegistered<ContractOne>()
				.Should()
				.BeTrue();
			sut.IsContractRegistered(contractIdentifierOne)
				.Should()
				.BeTrue();
			sut.IsContractRegistered<ContractTwo>()
				.Should()
				.BeTrue();
			sut.IsContractRegistered(contractIdentifierTwo)
				.Should()
				.BeTrue();
		}
	}

	#endregion

	#region Nested Type: TryGetRegisteredContract

	public class TryGetRegisteredContract : MessageContractRegistryFixture
	{
		[Theory]
		[AutoData]
		public void ReturnsNullWhenContractNotRegistered(string contractIdentifier)
		{
			new MessageContractRegistry(_ => contractIdentifier).TryGetRegisteredContract(contractIdentifier, out var type)
				.Should()
				.BeFalse();
			type.Should()
				.BeNull();
		}

		[Theory]
		[AutoData]
		public void ReturnsTypeWhenContractRegistered(string contractIdentifier)
		{
			new MessageContractRegistry(_ => contractIdentifier).RegisterContract<ContractOne>()
				.TryGetRegisteredContract(contractIdentifier, out var type)
				.Should()
				.BeTrue();
			type.Should()
				.Be<ContractOne>();
		}
	}

	#endregion
}
