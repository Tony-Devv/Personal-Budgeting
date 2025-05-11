using System.Security.Cryptography;
using Model.Handlers;
using Model.Interfaces;
using Model.Repositories;

namespace Model.Utilities;

/// <summary>
/// Provides a centralized service locator pattern implementation for dependency management.
/// </summary>
/// <remarks>
/// This class serves as an alternative to traditional dependency injection frameworks, 
/// implementing the dependency inversion principle by decoupling service instantiation 
/// from service usage. It acts as a lightweight IoC container that registers and 
/// provides access to various application services.
/// 
/// The container is implemented as a thread-safe singleton, ensuring only one instance 
/// exists throughout the application lifecycle. Services are lazily instantiated when 
/// first requested.
/// </remarks>
public class ServicesContainer
{
    /// <summary>
    /// Delegate type for service factory methods.
    /// </summary>
    /// <remarks>
    /// This delegate is used to encapsulate service creation logic,
    /// allowing services to be instantiated only when needed.
    /// </remarks>
    private delegate object ServiceCreationDelegate();
    
    /// <summary>
    /// Dictionary containing service type mappings to their creation delegates.
    /// </summary>
    /// <remarks>
    /// Maps service types (interfaces or concrete types) to factory methods
    /// that create new instances of those services.
    /// </remarks>
    private Dictionary<Type, ServiceCreationDelegate> _services = new Dictionary<Type, ServiceCreationDelegate>();
    
    /// <summary>
    /// Thread-safe lazy-initialized singleton instance.
    /// </summary>
    /// <remarks>
    /// Uses Lazy<T> for thread-safe initialization that occurs only when first accessed.
    /// This ensures the service container is only initialized once across all threads.
    /// </remarks>
    private static Lazy<ServicesContainer> instance => new(() => new ServicesContainer());

    /// <summary>
    /// Gets the singleton instance of the services container.
    /// </summary>
    /// <value>The single instance of ServicesContainer that exists in the application.</value>
    public static ServicesContainer Instance => instance.Value;

    /// <summary>
    /// Private constructor that registers all application services.
    /// </summary>
    /// <remarks>
    /// This constructor is private to enforce the singleton pattern.
    /// All service registrations are defined here, mapping interfaces to their implementations
    /// and registering concrete service types.
    /// </remarks>
    private ServicesContainer()
    {
        // Repository registrations
        _services.Add(typeof(IUserRepository), () => new UserRepository());    
        _services.Add(typeof(IPasswordHasher), () => new Sha512PasswordHasher());    
        _services.Add(typeof(IIncomeRepository), () => new IncomeRepository());    
        _services.Add(typeof(IExpenseRepository), () => new ExpenseRepository());    
        _services.Add(typeof(IBudgetRepository), () => new BudgetRepository());
        
        // Handler registrations
        _services.Add(typeof(UserHandler), () => new UserHandler());
        _services.Add(typeof(IncomeHandler), () => new IncomeHandler());
        _services.Add(typeof(BudgetHandler), () => new BudgetHandler());
        _services.Add(typeof(ExpenseHandler), () => new ExpenseHandler());
    }

    /// <summary>
    /// Retrieves a service instance of the specified type.
    /// </summary>
    /// <typeparam name="TService">The type of service to retrieve.</typeparam>
    /// <returns>A new instance of the requested service.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the requested service type is not registered in the container.
    /// </exception>
    /// <remarks>
    /// This method creates a new instance of the requested service each time it's called.
    /// If the service is not registered, an exception is thrown with a clear error message.
    /// 
    /// Note that this implementation doesn't handle dependency relationships between services;
    /// each service is instantiated independently. For services that require other services,
    /// those dependencies would need to be resolved manually within the service constructors
    /// by calling GetService again.
    /// </remarks>
    public TService GetService<TService>() where TService : class
    {
        if (_services.TryGetValue(typeof(TService), out var factory))
        {
            var service = (TService)factory();
            
            return service;
        }

        throw new InvalidOperationException($"Service of type {typeof(TService)} is not registered.");    
    }
}