using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace client.Classes
{
    class Utils
    {
        public static int sendMessageToServeur(Socket socketSender, string data, string typeMsg)
        {
            try
            {
                socketSender.Send(Encoding.ASCII.GetBytes(Constantes.client + typeMsg + data + "~"));
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return -1;
            }
        }

        public static string receiveMessageFromServeur(Socket socketSender, byte[] bytes)
        {
            try
            {
                int nbBytes = socketSender.Receive(bytes);
                string[] message = Encoding.ASCII.GetString(bytes, 0, nbBytes).Split("$");
                return message[1].Split('@')[1].Split('~')[0];
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return "-1";
            }
        }

        public static Boolean checkStringMDJChoice(string data)
        {
            if (int.TryParse(data, out int tmp))
            {
                List<int> tmpList = new List<int>() { 0, 1, 2, 3, 4, 5 };
                return tmpList.Contains(tmp) && data.Length > 0;
            }
            return false;
        }

        public static Boolean checkStringTry(string data)
        {
            Boolean res = true;
            for (int i = 0; i < data.Length; i++)
            {
                if (!Char.IsLetter(data[i])) res = false;
            }
            return res && data.Length > 0;
        }

        public static Boolean checkStringPlayAgain(string data)
        {
            string tmp = data.ToUpper();
            return tmp.Length > 0 && (tmp.Contains("O") || tmp.Contains("N"));
        }

        public static Boolean checkWordToSend(string data)
        {
            Boolean res = true;
            for (int i = 0; i < data.Length; i++)
            {
                if (!Char.IsLetter(data[i])) res = false;
            }
            return res && data.Length > 2;
        }

        public static Boolean checkValueChrono(string data)
        {
            Boolean res = true;
            for (int i = 0; i < data.Length; i++)
            {
                if (!Char.IsDigit(data[i])) res = false;
            }
            return res && data.Length > 0;
        }

        public static string[] decryptMsg(string data)
        {
            return data.Split('\\');
        }
    }
}
;