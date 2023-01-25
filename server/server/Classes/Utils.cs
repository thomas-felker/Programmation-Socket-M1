using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace server.Classes
{
    class Utils
    {
        public static int sendMessageToClient(Socket clientSocket, string data, string typeMsg)
        {
            try
            {
                clientSocket.Send(Encoding.ASCII.GetBytes(Constantes.serveur + typeMsg + data + "~"));
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        public static string receiveMessageFromClient(Socket clientSocket, byte[] bytes)
        {
            try
            {
                int nbBytes = clientSocket.Receive(bytes);
                string[] message = Encoding.ASCII.GetString(bytes, 0, nbBytes).Split("$");
                return message[1].Split('@')[1].Split('~')[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return "-1";
            }
        }
    }
}
