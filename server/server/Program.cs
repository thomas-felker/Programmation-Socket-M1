using System;
using System.Collections.Generic;
using System.Text;
using ClassServer;

namespace MainProgram
{
    class Program
    {
        public static void Main(string[] args)
        {
            Serveur serveur = new Serveur();
            serveur.ExecuteServer();
        }
    }
}
