using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Commands;
using Ordering.Application.Mapper;

namespace Ordering.API.EventBusConsumer
{
    public class BasketOrderingConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BasketOrderingConsumer> _logger;

        public BasketOrderingConsumer(IMediator mediator, ILogger<BasketOrderingConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = OrderingMapper.Mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await _mediator.Send(command);
            _logger.LogInformation("BasketCheckoutEvent consumed successfully. Created Order Id : {result}", result);
        }
    }

    public class QueueBasketCheckoutConsumerDefinition : ConsumerDefinition<BasketOrderingConsumer>
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<BasketOrderingConsumer> consumerConfigurator,
            IRegistrationContext registrationContext)
        {
            consumerConfigurator.UseMessageRetry(retry => retry.Interval(3, TimeSpan.FromSeconds(5)));
            base.ConfigureConsumer(endpointConfigurator, consumerConfigurator, registrationContext);
        }
    }
}
