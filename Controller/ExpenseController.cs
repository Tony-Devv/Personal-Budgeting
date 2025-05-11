using Model.Entities;
using Model.Handlers;
using Model.Utilities;
using Controller.Validators;
using FluentValidation;

public class ExpenseController
{
    public enum ExpenseInputValidationRules
    {
        Add,
        Update,
        Delete,
        SetReminders,
    }
    
    private readonly ExpenseHandler _expenseHandler = ServicesContainer.Instance.GetService<ExpenseHandler>();
    private readonly ExpenseInputValidator _validator = new ExpenseInputValidator();
    
    public async Task<(bool Success, int ExpenseId, List<string> Errors)> TryAddExpense(Expense expense)
    {/*
        var validationResult = await _validator.ValidateAsync(expense, options => 
        {
            options.IncludeRuleSets(ExpenseInputValidationRules.Add.ToString());
        });
        
        if (!validationResult.IsValid)
        {
            return (false, -1, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }
        */
        
        int result = await _expenseHandler.AddNewExpense(expense);
        return (result != -1, result, new List<string>());
    }

    public async Task<(bool Success, Expense? newExpense, List<string> Errors)> TryUpdateExpense(Expense newValues)
    {/*
        var validationResult = await _validator.ValidateAsync(newValues, options => 
        {
            options.IncludeRuleSets(ExpenseInputValidationRules.Update.ToString());
        });
        
        if (!validationResult.IsValid)
        {
            return (false, null, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }
        */
        
        var updatedExpense = await _expenseHandler.UpdateExpense(newValues);
        return (updatedExpense != null, updatedExpense, new List<string>());
    }

    public async Task<(bool Success, List<string> Errors)> TryDeleteExpense(Expense expense)
    {/*
        var validationResult = await _validator.ValidateAsync(expense, options => 
        {
            options.IncludeRuleSets(ExpenseInputValidationRules.Delete.ToString());
        });
        
        if (!validationResult.IsValid)
        {
            return (false, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }
        */
        
        int result = await _expenseHandler.DeleteExpense(expense);
        return (result != -1, new List<string>());
    }

    public async Task<(bool Success, Expense? expense, List<string> Errors)> TrySearchExpense(string expenseName, int userId)
    {/*
        var validationResult = _validator.ValidateForSearch(userId, expenseName);
        
        if (!validationResult.Success)
        {
            return (false, null, validationResult.errors);
        }
        */
        
        var expense = await _expenseHandler.SearchExpenseByName(userId, expenseName);
        return (expense != null, expense, new List<string>());
    }

    public async Task<(bool Success, List<string> Errors)> TrySetExpensesReminders(List<Expense> expenses, DateTime reminderTime)
    {
        /*
        var errors = new List<string>();
        
        foreach (var expense in expenses)
        {
            var validationResult = await _validator.ValidateAsync(expense, options => 
            {
                options.IncludeRuleSets(ExpenseInputValidationRules.SetReminders.ToString());
            });
            
            if (!validationResult.IsValid)
            {
                errors.AddRange(validationResult.Errors.Select(e => e.ErrorMessage));
            }
        }
        
        if (errors.Any())
        {
            return (false, errors);
        }
        */
        
        bool result = await _expenseHandler.SetExpensesWithReminder(expenses, reminderTime);
        return (result, new List<string>());
    }

    public async Task<(bool Success, List<Expense> expenses, List<string> Errors)> TryGetExpensesWithReminder(int userId)
    {/*
        var intValidator = new InlineValidator<int>();
        intValidator.RuleFor(i => i).GreaterThan(0).WithMessage("UserId can't be 0 or negative");
        
        var validationResult = await intValidator.ValidateAsync(userId);
        if (!validationResult.IsValid)
        {
            return (false, new List<Expense>(), validationResult.Errors.Select(e => e.ErrorMessage).ToList());
        }
        */
        
        var expenses = await _expenseHandler.GetExpensesThatHasReminders(userId);
        return (expenses.Count != 0, expenses, new List<string>());
    }
    
}