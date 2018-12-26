using System;
using System.Collections.Generic;
using System.IO.Pipes;

namespace ChessDAL.Models
{
    public partial class User
    {
        public int id { get; private set; }
        public string username { get; set; }
        public string password { get; set; }
        public NamedPipeServerStream userServer { get; set; }

        public User() { }

        public User(int id, string username, string password)
        {
            this.id = id;
            this.username = username;
            this.password = password;
        }

        public User(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
    }
}
