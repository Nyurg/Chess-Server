using ChessDAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessDAL.Services
{
    public interface IUserService
    {
        List<User> RetrieveUsers();

        User FindUser(string username, string password);
    }
}
