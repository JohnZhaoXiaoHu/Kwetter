﻿using Kwetter.Services.Common.Application.Configurations;
using Kwetter.Services.Common.Application.Eventing;
using Kwetter.Services.Common.Application.Eventing.Bus;
using Kwetter.Services.Common.Domain.Events;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Kwetter.Services.Common.Infrastructure.Eventing.Bus
{
    /// <summary>
    /// Represents the <see cref="EventBus"/> class.
    /// </summary>
    public sealed class EventBus : IEventBus
    {
        private readonly ObjectPool<IModel> _channelPool;
        private readonly ServiceConfiguration _serviceConfiguration;
        private readonly IEventSerializer _eventSerializer;
        private const string KwetterExchange = "KwetterExchange";
        private const string DeadLetterExchange = "DeadLetterExchange";
        private const string DeadLetterQueue = "DeadLetterQueue";

        /// <summary>
        /// Initializes a new instance of the <see cref="EventBus"/> class.
        /// </summary>
        /// <param name="serviceConfigurationOptions">The service configuration options.</param>
        /// <param name="objectPolicy">The object pooling policy.</param>
        /// <param name="eventSerializer">The event serializer.</param>
        public EventBus(
            IOptions<ServiceConfiguration> serviceConfigurationOptions,
            IPooledObjectPolicy<IModel> objectPolicy, 
            IEventSerializer eventSerializer)
        {
            _channelPool = new DefaultObjectPool<IModel>(objectPolicy ?? throw new ArgumentNullException(nameof(objectPolicy)), Environment.ProcessorCount * 2);
            _serviceConfiguration = serviceConfigurationOptions.Value ?? throw new ArgumentNullException(nameof(serviceConfigurationOptions));
            _eventSerializer = eventSerializer ?? throw new ArgumentNullException(nameof(eventSerializer));

            // Declares the dead letter exchange and queue, for when events fail to successfully get processed.
            IModel channel = _channelPool.Get();
            channel.ExchangeDeclare(DeadLetterExchange, ExchangeType.Fanout, durable: true);
            channel.ExchangeDeclare(KwetterExchange, ExchangeType.Direct, durable: true);
            channel.QueueDeclare(DeadLetterQueue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            channel.QueueBind(DeadLetterQueue, DeadLetterExchange, "");
            _channelPool.Return(channel);
        }

        /// <inheritdoc cref="IEventPublisher.Publish{TEvent}(TEvent,string)"/>
        public void Publish<TEvent>(TEvent @event, string queueName) where TEvent : Event
        {
            if (@event is null)
                throw new ArgumentNullException(nameof(@event));
            string actualQueueName = $"{_serviceConfiguration.ShortTitle}.{queueName}";
            IModel channel = GetChannel(actualQueueName);
            // NOTE: type casted to object to ensure no type issues when serializing.
            ReadOnlyMemory<byte> message = _eventSerializer.Serialize<object>(@event);
            IBasicProperties basicProperties = channel.CreateBasicProperties();
            basicProperties.DeliveryMode = 2;
            channel.BasicPublish(KwetterExchange, actualQueueName, basicProperties: basicProperties, body: message);
            _channelPool.Return(channel);
        }

        /// <inheritdoc cref="IEventBus.Subscribe{TEvent,TEventHandler}(string,TEventHandler)"/>
        public void Subscribe<TEvent, TEventHandler>(string queueName, TEventHandler eventHandler) 
            where TEvent : Event 
            where TEventHandler : IEventHandler<TEvent>
        {
            if (eventHandler is null)
                throw new ArgumentNullException(nameof(eventHandler));
            IModel channel = GetChannel(queueName);
            AsyncEventingBasicConsumer eventingBasicConsumer = new(channel);
            eventingBasicConsumer.Received += async (sender, eventArgs) =>
            {
                try
                {
                    TEvent @event = _eventSerializer.Deserialize<TEvent>(eventArgs.Body);
                    await eventHandler.HandleAsync(@event, CancellationToken.None);
                    channel.BasicAck(eventArgs.DeliveryTag, false);
                }
                catch (Exception)
                {
                    channel.BasicNack(eventArgs.DeliveryTag, false, false);
                }
            };
            string tag = channel.BasicConsume(queueName, false, eventingBasicConsumer);
            eventHandler.UnsubscribeEventHandler += (sender, eventArgs) =>
            {
                channel.BasicCancel(tag);
            };
            _channelPool.Return(channel);
        }

        /// <inheritdoc cref="IEventBus.Unsubscribe{TEvent,TEventHandler}(TEventHandler)"/>
        public void Unsubscribe<TEvent, TEventHandler>(TEventHandler eventHandler)
            where TEvent : Event
            where TEventHandler : IEventHandler<TEvent>
        {
            eventHandler.Unsubscribe();
        }

        private IModel GetChannel(string queueName)
        {
            IModel channel = _channelPool.Get();
            Dictionary<string, object> arguments = new()
            {
                {"x-dead-letter-exchange", DeadLetterExchange}
            };
            channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
            channel.QueueBind(queueName, KwetterExchange, queueName);
            return channel;
        }
    }
}
