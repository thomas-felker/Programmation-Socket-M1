using System;
using System.Collections.Generic;
using System.Text;
using ClassClient;

namespace MainProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            Client client = new Client();
            client.ExecuteClient();
        }
    }
}
