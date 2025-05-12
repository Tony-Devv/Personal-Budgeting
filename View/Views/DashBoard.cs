using System;
using Model.Entities;

namespace View.Views
{
    public class DashBoard : IView
    {
        private IView _budgetView;
        private IView _incomeView;
        private IView _expenseView;
        private IView _editDetailsView;
        private User _user;

        public async Task Initialize(User obj)
        {
            _user = obj;

            _budgetView = new BudgetView();
            _incomeView = new IncomeView();
            _expenseView = new ExpenseView();
            _editDetailsView = new EditDetailsView();

            await _budgetView.Initialize(obj);
            await _incomeView.Initialize(obj);
            await _expenseView.Initialize(obj);
            await _editDetailsView.Initialize(obj);
        }

        public async Task Show()
        {
            while (true)
            {
                Console.Clear();
                await ShowSummary();
                Console.WriteLine("=== Dashboard ===");
                Console.WriteLine("1. Budget Dashboard");
                Console.WriteLine("2. Expense Dashboard");
                Console.WriteLine("3. Incomes Dashboard");
                Console.WriteLine("4. Edit Details");
                Console.WriteLine("5. Logout");
                Console.Write("Choose an option (1-5): ");

                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await _budgetView.Show();
                        break;
                    case "2":
                        await _expenseView.Show();
                        break;
                    case "3":
                        await _incomeView.Show();
                        break;
                    case "4":
                        await _editDetailsView.Show();
                        break;
                    case "5":
                        Console.WriteLine("Logging out...");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press any key to try again...");
                        Console.ReadKey();
                        break;
                }
            }
        }



        public async Task ShowSummary()
        {
            var tempUserController = new UserController();

            var result_i = await tempUserController.TryGetTotalUserIncomes(_user.Id);
            var result_E = await tempUserController.TryGetTotalUserSpentExpenses(_user.Id);


            if (result_i.Success && result_E.Success)
            {
                Console.WriteLine($"Welcome {_user.UserName} :D \n" +
                                  $"Here is a quick Summary for you \n" +
                                  $"Total Income: {result_i.Total} \n" +
                                  $"Total Money Spent on Expenses: {result_E.Total} \n" +
                                  $"================================================\n");            
            }
            else
            {
                Console.WriteLine("Failed to load Summary");
            }
        }
    }
}
