using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace client.Classes.Modes_de_jeu
{
    class Classique : ModeDeJeu
    {
        #region Données membres

        bool Chrono { get; set; }

        #endregion

        #region Constructeur

        public Classique (bool withChrono = false)
        {
			Chrono = withChrono;
        }

        #endregion

        #region Méthodes

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
			reinitPartie();

			// Message d'intro
			Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));

			if (Chrono)
            {
				string msgChrono = Utils.receiveMessageFromServeur(socketSender, bytes);
				// Envoie de la valeur du chrono désirée
				do
				{
					Console.WriteLine(msgChrono);
					data = Console.ReadLine();
				} while (!Utils.checkValueChrono(data));
				Utils.sendMessageToServeur(socketSender, data.ToLower(), Constantes.typeMsgMessage);
			}

			jouerClassique(data, bytes, nbBytes, socketSender);

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
