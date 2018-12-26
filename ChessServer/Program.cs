using ChessDAL;
using ChessDAL.Models;
using ChessDAL.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessServer
{
    class Program
    {
        private static List<User> usersOnline = new List<User>();
        private static IUserService userService;

        static void Main(string[] args)
        {
            userService = new UserService();
            DetectNewUser().Wait();
        }

        // Repeating function that searches for any clients connecting
        private static async Task DetectNewUser()
        {
            NamedPipeServerStream usersPipeLogin = new NamedPipeServerStream
                ("userLogin", PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);

                // Waits here for a connection
            while (!usersPipeLogin.IsConnected) await usersPipeLogin.WaitForConnectionAsync();

            // Reads a new user as a json string from the client connected
            User newUser = JsonConvert.DeserializeObject<User>(ReadString(usersPipeLogin));

            // Find if user exists
            newUser = userService.FindUser(newUser.username, newUser.password);
            if (newUser != null)
            {
                Console.WriteLine($"{newUser.username} has been found. Attempting to Log-in");

                // If user is not alread logged in
                if (usersOnline.Find(user => user.username == newUser.username) == null)
                {
                    newUser.userServer = new NamedPipeServerStream
                        (newUser.username, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                    newUser.userServer.WaitForConnection();

                    usersOnline.Add(newUser);
                    Console.WriteLine($"User: {newUser.username} has logged in");
                }
                else if (usersOnline.Find(user => user.username == newUser.username) != null)
                {
                    Console.WriteLine($"{newUser.username} is already logged in");
                }


                // Repeat
                usersPipeLogin.Close();
                DetectNewUser().Wait();
            }
        }
        
        // Reads from a stream and returns a string value
        private static string ReadString(PipeStream readFromStream)
        {
            StringBuilder messageBuilder = new StringBuilder();
            string messageChunk = string.Empty;
            byte[] messageBuffer = new byte[5];

            do
            {
                readFromStream.Read(messageBuffer, 0, messageBuffer.Length);
                messageChunk = Encoding.UTF8.GetString(messageBuffer);
                messageBuilder.Append(messageChunk);
                messageBuffer = new byte[messageBuffer.Length];
            }
            while (!readFromStream.IsMessageComplete);

            return messageBuilder.ToString();
        }
    }
}
