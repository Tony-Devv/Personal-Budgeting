public class IncomeController
{
	public bool TryAddIncome(Income income) { return true; }
	public bool TryDeleteIncome(Income income) { return true; }
	public bool TrySearchIncome(string name, int userId) { return true; }
	public bool TryGetIncomeById(int id) { return true; }
}
