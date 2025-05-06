using Model.Entities;
using Model.Repositories;

namespace FastManualTest;

class Program
{
    static async Task Main(string[] args)
    {
        User user = new User()
        {
            UserName = "Hamid",
            Email = "test@gmail.com",
            PhoneNumber = "01220342",
            Password = "idk1234"
        };

        UserRepository userRepository = new UserRepository();

        int result = await userRepository.Add(user);
        Console.WriteLine(result);

        User? r_user = await userRepository.GetById(result);

        Console.WriteLine(r_user?.ToString());

        User? rr_user = await userRepository.RetrieveUserByEmail(user.Email);
        
        Console.WriteLine(rr_user?.ToString());

        user.UserName = "MOL";

        int r_result = await  userRepository.Update(user);
        
        Console.WriteLine(user.ToString());

        int id = user.Id;
        Console.WriteLine(await userRepository.CheckExist(id));
    }
}