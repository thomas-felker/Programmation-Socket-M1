using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace client.Classes.Modes_de_jeu
{
    class Inverse : ModeDeJeu
    {
        #region Méthodes

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
            reinitPartie();

            // Message d'intro
            Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

            // Envoie du mot au serveur
            do
            {
                data = Console.ReadLine();
            } while (!Utils.checkWordToSend(data));
            Utils.sendMessageToServeur(socketSender, data, Constantes.typeMsgMessage);

            while (!victoireJ1 && !gameOverJ1)
            {
                Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

                data = Utils.receiveMessageFromServeur(socketSender, bytes);
                Console.WriteLine(data);

                if (data.ToLower().Contains("victoire"))
                    victoireJ1 = true;

                if (data.ToLower().Contains("defaite"))
                    gameOverJ1 = true;
            }

            askPlayAgain(socketSender);

            Utils.sendMessageToServeur(socketSender, rejouerJ1, Constantes.typeMsgRejouer);

            return rejouerJ1;
        }

        public override void reinitPartie()
        {
            rejouerJ1 = "N";
            victoireJ1 = false;
            gameOverJ1 = false;
        }

        #endregion
    }
}
