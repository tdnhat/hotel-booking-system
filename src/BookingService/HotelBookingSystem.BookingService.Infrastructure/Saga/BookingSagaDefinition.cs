using HotelBookingSystem.BookingService.Domain.Entities;
using MassTransit;

namespace HotelBookingSystem.BookingService.Infrastructure.Saga
{
    public class BookingSagaDefinition : SagaDefinition<BookingState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, 
                                             ISagaConfigurator<BookingState> sagaConfigurator, 
                                             IRegistrationContext context)
        {
            // Configure retry policy - if something fails, retry up to 3 times
            endpointConfigurator.UseMessageRetry(r => r.Intervals(500, 1000, 2000));
            
            // Configure concurrency limit - process max 10 saga instances at once
            endpointConfigurator.ConcurrentMessageLimit = 10;
        }
    }
}