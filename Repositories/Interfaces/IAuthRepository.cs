using APIservice.Models;
namespace APIservice.Repositories.Interfaces
{
    public interface IAuthRepository
    {
       void Register(User user);
        (User, string) Login(User user); // return 2 type ไปอีก
    }
}