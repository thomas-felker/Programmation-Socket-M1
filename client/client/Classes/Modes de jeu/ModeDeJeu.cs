using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace client.Classes.Modes_de_jeu
{
	abstract class ModeDeJeu
	{
		#region Données membres

		// Données en rapport avec le serveur
		public string data { get; set; }

		// Données en rapport avec le jeu
		public string rejouerJ1 { get; set; }
		public bool victoireJ1 { get; set; }
		public bool gameOverJ1 { get; set; }

		#endregion

		#region Constructeur

		public ModeDeJeu()
		{
			reinitPartie();
		}

		#endregion

		#region Méthodes publiques

		public abstract void reinitPartie();

		public abstract string jouerPartie(string data, byte[] bytes, int nbBytes, Socket socketSender);

		public void jouerClassique(string data, byte[] bytes, int nbBytes, Socket socketSender)
        {
			while (!victoireJ1 && !gameOverJ1)
			{
				Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
				do
				{
					Console.WriteLine(Constantes.msgProposeLetter);
					data = Console.ReadLine();
				} while (!Utils.checkStringTry(data));

				Utils.sendMessageToServeur(socketSender, data.ToLower(), Constantes.typeMsgReponse);
				data = Utils.receiveMessageFromServeur(socketSender, bytes);
				Console.WriteLine(data);

				if (data.ToLower().Contains("victoire"))
					victoireJ1 = true;

				if (data.ToLower().Contains("defaite"))
					gameOverJ1 = true;
			}
		}

		public void askPlayAgain(Socket socketSender)
        {
			byte[] bytes = new byte[2048];
			string msg = Utils.receiveMessageFromServeur(socketSender, bytes);
			do
			{
				Console.WriteLine(msg);
				rejouerJ1 = Console.ReadLine().ToUpper();
			} while (!Utils.checkStringPlayAgain(rejouerJ1));
		}

		#endregion
	}
}
