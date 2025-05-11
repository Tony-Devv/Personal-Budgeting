using Model.Entities;

namespace View;

public interface IView
{
    Task Show();

    Task Initialize(User obj);
}