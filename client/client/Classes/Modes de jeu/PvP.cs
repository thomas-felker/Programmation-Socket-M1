using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace client.Classes.Modes_de_jeu
{
    class PvP : ModeDeJeu
    {

        #region Données membres

        public string essaiChercheur { get; set; }

        #endregion

        #region Constructeur

        public PvP() { }

        #endregion

        #region Méthodes publiques

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
            reinitPartie();

            // Message d'intro
            Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

            // Message d'attribution des rôles
            data = Utils.receiveMessageFromServeur(socketSender, bytes);
            Console.WriteLine(data);

            // Lancement de la partie en fonction du rôle
            if (data.Contains("decideur"))
                jouerDecideur(data, bytes, nbBytes, socketSender);
            else
                jouerChercheur(data, bytes, nbBytes, socketSender);

            // Demande au joueur s'il veut rejouer
            askPlayAgain(socketSender);

            // Envoie de la réponse au serveur et réception du traitement
            Utils.sendMessageToServeur(socketSender, rejouerJ1, Constantes.typeMsgRejouer);
            rejouerJ1 = Utils.receiveMessageFromServeur(socketSender, bytes);

            return rejouerJ1;
        }

        public override void reinitPartie()
        {
            rejouerJ1 = "N";
            victoireJ1 = false;
            gameOverJ1 = false;
        }

        #endregion

        #region Méthodes privées

        private void jouerChercheur(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
            jouerClassique(data, bytes, nbBytes, socketSender);
        }

        private void jouerDecideur(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
            /** Envoi du mot à deviner au serveur pour obfuscation */
            do
            {
                essaiChercheur = Console.ReadLine();
            } while (!Utils.checkWordToSend(essaiChercheur));
            Utils.sendMessageToServeur(socketSender, essaiChercheur, Constantes.typeMsgReponse);

            while ((victoireJ1 == false) && (gameOverJ1 == false))
            {
                Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

                data = Utils.receiveMessageFromServeur(socketSender, bytes);
                Console.WriteLine(data);

                if (data.ToLower().Contains("victoire"))
                    victoireJ1 = true;

                if (data.ToLower().Contains("defaite"))
                    gameOverJ1 = true;
            }
        }

        #endregion
    }
}
