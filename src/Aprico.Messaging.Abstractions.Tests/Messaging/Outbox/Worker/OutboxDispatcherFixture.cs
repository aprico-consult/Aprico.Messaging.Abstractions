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
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Aprico.AutoFixture.Xunit2;
using Aprico.Moq.Extensions;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Azure.Messaging.ServiceBus;
using Moq;
using Moq.Protected;

namespace Aprico.Messaging.Outbox.Worker;

public class OutboxDispatcherFixture
{
	[Theory]
	[AutoData<AutoMoqCustomization>]
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	[SuppressMessage("Reliability", "CA2012:Use ValueTasks correctly")]
	public async Task DispatchAsyncDispatchesMessagesAtomically(
		[Frozen] IOutboxReader<ServiceBusMessage> outboxReader,
		OutboxDispatcher<ServiceBusMessage> sut,
		[Frozen] DbTransaction dbTransaction,
		DbConnection dbConnection,
		string subject1,
		string subject2)
	{
		outboxReader.AsMock()
			.SetupSequence(static reader => reader.DequeueAsync(It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(
				(Subject: subject1, Messages: [
					new ServiceBusMessage($"{Guid.NewGuid()}")
				]))
			.ReturnsAsync(
				(Subject: subject2, Messages: [
					new ServiceBusMessage($"{Guid.NewGuid()}")
				]))
			.ReturnsAsync(outboxReader.Empty);
		dbConnection.AsMock()
			.Protected()
			.Setup<DbTransaction>("BeginDbTransaction", ItExpr.IsAny<IsolationLevel>())
			.Returns(dbTransaction);

		await sut.DispatchAsync(dbConnection);

		dbConnection.AsMock()
			.Protected()
			.Verify("BeginDbTransaction", Times.Exactly(callCount: 3), ItExpr.IsAny<IsolationLevel>());
		dbTransaction.AsMock()
			.Verify(static transaction => transaction.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(callCount: 2));
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	public async Task DispatchAsyncDrainsOutbox(
		[Frozen] IOutboxReader<ServiceBusMessage> outboxReader,
		OutboxDispatcher<ServiceBusMessage> sut,
		DbConnection dbConnection,
		string subject1,
		string subject2)
	{
		outboxReader.AsMock()
			.SetupSequence(static reader => reader.DequeueAsync(It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync(
				(Subject: subject1, Messages: [
					new ServiceBusMessage($"{Guid.NewGuid()}"),
					new ServiceBusMessage($"{Guid.NewGuid()}")
				]))
			.ReturnsAsync(
				(Subject: subject2, Messages: [
					new ServiceBusMessage($"{Guid.NewGuid()}"),
					new ServiceBusMessage($"{Guid.NewGuid()}")
				]))
			.ReturnsAsync(outboxReader.Empty);

		await sut.DispatchAsync(dbConnection);

		outboxReader.AsMock()
			.Verify(static reader => reader.DequeueAsync(It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()), Times.Exactly(callCount: 3));
		outboxReader.AsMock()
			.VerifyNoOtherCalls();
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	[SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
	public async Task DispatchAsyncPublishesMessages(
		[Frozen] IOutboxReader<ServiceBusMessage> outboxReader,
		[Frozen] IBatchMessagePublisher<ServiceBusMessage> batchMessagePublisher,
		OutboxDispatcher<ServiceBusMessage> sut,
		DbConnection dbConnection,
		string subject1,
		string subject2)
	{
		IEnumerable<ServiceBusMessage> batch1 = [
			new($"{Guid.NewGuid()}"),
			new($"{Guid.NewGuid()}")
		];
		IEnumerable<ServiceBusMessage> batch2 = [
			new($"{Guid.NewGuid()}"),
			new($"{Guid.NewGuid()}")
		];
		outboxReader.AsMock()
			.SetupSequence(static reader => reader.DequeueAsync(It.IsAny<DbTransaction>(), It.IsAny<CancellationToken>()))
			.ReturnsAsync((Subject: subject1, Messages: batch1))
			.ReturnsAsync((Subject: subject2, Messages: batch2))
			.ReturnsAsync(outboxReader.Empty);

		await sut.DispatchAsync(dbConnection);

		batchMessagePublisher.AsMock()
			.Verify(publisher => publisher.PublishAsync(subject1, batch1, It.IsAny<CancellationToken>()), Times.Once);
		batchMessagePublisher.AsMock()
			.Verify(publisher => publisher.PublishAsync(subject2, batch2, It.IsAny<CancellationToken>()), Times.Once);
		batchMessagePublisher.AsMock()
			.VerifyNoOtherCalls();
	}

	[Theory]
	[AutoData<AutoMoqCustomization>]
	[SuppressMessage("ReSharper", "NullableWarningSuppressionIsUsed")]
	public async Task DispatchAsyncThrowsIfConnectionIsNull(OutboxDispatcher<ServiceBusMessage> sut)
	{
		await Invoking(async () => await sut.DispatchAsync(null!))
			.Should()
			.ThrowAsync<ArgumentNullException>();
	}
}
