using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace client.Classes.Modes_de_jeu
{
    class PvPServeur : ModeDeJeu
    {

        #region Constructeur 

        public PvPServeur(int Joueur) { }

        #endregion

        #region Méthodes publiques

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
            reinitPartie();
            // Message d'intro
            Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
            Thread.Sleep(250);

            Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
            Thread.Sleep(250);

            // Déroulement de la partie
            jouerClassique(data, bytes, nbBytes, socketSender);

            // Réception des affichage des résultats
            Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

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
    }
}
