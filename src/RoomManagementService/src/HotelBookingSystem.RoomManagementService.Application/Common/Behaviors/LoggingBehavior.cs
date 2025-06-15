using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelBookingSystem.RoomManagementService.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var requestGuid = Guid.NewGuid().ToString();

            var requestNameWithGuid = $"{requestName} [{requestGuid}]";

            _logger.LogInformation("[START] {RequestNameWithGuid}", requestNameWithGuid);

            TResponse response;

            try
            {
                response = await next();
            }
            finally
            {
                _logger.LogInformation("[END] {RequestNameWithGuid}", requestNameWithGuid);
            }

            return response;
        }
    }
}