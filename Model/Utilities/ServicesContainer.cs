using System.Security.Cryptography;
using Model.Handlers;
using Model.Interfaces;
using Model.Repositories;

namespace Model.Utilities;

public class ServicesContainer
{
    private delegate object ServiceCreationDelegate();
    
    private Dictionary<Type, ServiceCreationDelegate> _services = new Dictionary<Type, ServiceCreationDelegate>();
    private static Lazy<ServicesContainer> instance => new(() => new ServicesContainer());

    public static ServicesContainer Instance => instance.Value;

    private ServicesContainer()
    {
        _services.Add(typeof(IUserRepository),() => new UserRepository());    
        _services.Add(typeof(IPasswordHasher),() => new Sha512PasswordHasher());    
        _services.Add(typeof(IIncomeRepository),() => new IncomeRepository());    
        _services.Add(typeof(IExpenseRepository),() => new ExpenseRepository());    
        _services.Add(typeof(IBudgetRepository),() => new BudgetRepository());    
        _services.Add(typeof(UserHandler),() => new UserHandler());
        _services.Add(typeof(IncomeHandler), () => new IncomeHandler());
        _services.Add(typeof(BudgetHandler), () => new BudgetHandler());
        _services.Add(typeof(ExpenseHandler), () => new ExpenseHandler());
    }

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

