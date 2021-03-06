using System;
using System.Threading.Tasks;
using Market.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Infrastructure.Dispatchers
{
    public class InMemoryDispatcher : IDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public InMemoryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public Task SendAsync<T>(T command) where T : ICommand
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<T>>();
                return handler.HandleAsync(command);
            }
        }

        public Task<T> QueryAsync<T>(IQuery<T> query)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handlerType = typeof(IQueryHandler<,>).MakeGenericType(query.GetType(), typeof(T));
                dynamic handler = scope.ServiceProvider.GetRequiredService(handlerType);
                return handler.HandleAsync((dynamic) query);
            }
        }
    }
}