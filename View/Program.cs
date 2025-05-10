namespace View;

class Program
{
    static async Task Main(string[] args)
    {
        IView AuthView = new Authenticator();
        
        
        await AuthView.Initialize(null); // null because there is no user for now

        await AuthView.Show();
    }
}