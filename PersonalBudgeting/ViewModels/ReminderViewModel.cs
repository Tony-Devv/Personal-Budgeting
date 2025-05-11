using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PersonalBudgeting.ViewModels;

public partial class ReminderViewModel : ObservableObject
{
    [ObservableProperty]
    private string _billName = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private DateTime _dueDate = DateTime.Now;

    [ObservableProperty]
    private string _status = "Pending";
}
