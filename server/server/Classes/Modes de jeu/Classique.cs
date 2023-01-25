using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Threading;

namespace server.Classes.Modes_de_jeu
{
	/// Classe visant à decrire le comportement du mode de jeu CLASSIQUE

	class Classique : ModeDeJeu
	{
		#region Donnees membres

		DateTime StartTime { get; set; }
		int TimeLimit { get; set; }
		bool Chrono { get; set; }

		#endregion

		#region Constructeur

		public Classique(bool withChrono = false) {
			Chrono = withChrono;
		}

		#endregion

		#region Methodes publiques

		/** 
		 * Methode de reinitialisation des differentes variables utilisees pendant une partie
		 */
		public override void reinitPartie()
		{	
			rejouer = "N";
			motADeviner = "";
			motCache = "";

			Thread.Sleep(100);
			motADeviner = listeMots[nombreAleatoire.Next(0, listeMots.Capacity-1)];

			Thread.Sleep(100);
			motCache = obfusquer(motADeviner);

			setHP();

			victoire = false;
			gameOver = false;
			correct = false;
			erreur = false;
		}

		/**
		 * Ici le joueur joue une partie de pendu classique contre l'ordinateur :
		 *
		 * - Le serveur choisit un mot au hasard dans une liste de mots puis l'obfusque.
		 * - Le joueur dispose de 6 essais pour le deviner.
		 * - Le joueur ne peut tester qu'une lettre par une lettre.
		 */
		public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket clientSocket)
        {
			reinitPartie();
			Utils.sendMessageToClient(clientSocket, Constantes.msgIntroClassique, Constantes.typeMsgMessage);

			if (Chrono)
			{
				Utils.sendMessageToClient(clientSocket, Constantes.msgChrono, Constantes.typeMsgMessage);
				data = Utils.receiveMessageFromClient(clientSocket, bytes);

				TimeLimit = int.Parse(data) == 0 ? 30 : int.Parse(data);
				StartTimer();
			}

			/** Le jeu continue tant que la partie n'est ni gagnee ni perdue. */
			while ((victoire == false) && (gameOver == false))
			{
				/** On remet les variables booleennes temporaires à faux et on envoie le message au client. */
				correct = false;
				erreur = false;

				/** Envoi du mot obfusque au serveur */
				Utils.sendMessageToClient(clientSocket, Constantes.msgMot + motCache + Constantes.msgVie + nbVies, Constantes.typeMsgQuestion);

				/** Reception et affichage du caractère envoye par le client */
				data = Utils.receiveMessageFromClient(clientSocket, bytes);
				Console.WriteLine(Constantes.msgPrintTry + data);

				// Traitement du caractère reçu
				data = treatData(data);

				// Affichage du temps restant si le joueur joue avec un chrono
				if (Chrono && !noTimeLeft()) {
					data += "\n\nIl reste " + getRemainingTime() + " secondes !";
				}

				Utils.sendMessageToClient(clientSocket, data, Constantes.typeMsgMessage);
			}

			Thread.Sleep(100);

			/**
			 * La partie est finie, on attend la reponse du client pour savoir s'il souhaite rejouer ou non et
			 * on retourne la reponse au serveur qui l'interprète.
			 */
			Utils.sendMessageToClient(clientSocket, Constantes.msgPlayAgain, Constantes.typeMsgRejouer);
			rejouer = Utils.receiveMessageFromClient(clientSocket, bytes);
			Console.WriteLine("Rejouer = " + rejouer);
			return rejouer;
		}

		#endregion

		#region Methodes privees

		private void StartTimer()
		{
			StartTime = DateTime.Now;
		}

		private bool noTimeLeft()
		{
			var currentTime = DateTime.Now;
			TimeSpan timeSpan = currentTime - StartTime;
			return TimeLimit - timeSpan.TotalSeconds < 0;
		}

		private string getRemainingTime()
        {
			return ((TimeLimit) - (DateTime.Now - StartTime).TotalSeconds).ToString();

		}

		private string treatData (string data)
        {
			/**
				 * Traitement des donnees reçues par le client :
				 *
				 * Si la longueur de data est egale à 1, alors on a reçu un caractère.
				 * On parcourt alors le mot à deviner pour verifier si ce caractère en fait partie et s'il n'a pas dejà ete devine,
				 * auquel cas on met le booleen correct à true.
				 */
			if (data.Length == 1 && data.ToString() != "")
			{
				checkIfCorrect(data);
			}

			/** Si la longueur est differente de 1, deux cas se distinguent : */
			else
			{
				/** Soit la longueur est egale à la longueur du mot, auquel cas on considère cela comme un essai. */
				if (data.Length == motADeviner.Length)
				{
					correct = data == motADeviner;
					victoire = correct;
				}
			}

			// Si le chrono est actif, on verifie s'il reste du temps
			if (Chrono && noTimeLeft())
			{
				nbVies = 0;
				data = Constantes.msgDefeat + Constantes.msgTimeOut + listeASCII[nbVies]
					+ Constantes.msgWordReveal + motADeviner;
				gameOver = true;
			}
			/**
			 * Si l'essai est correct, on verifie si c'est une victoire et on envoie le message approprie.
			 */
			else if (correct)
			{
				if (victoire)
					data = Constantes.msgVictory + Constantes.msgWordReveal + motADeviner;
				else
					data = Constantes.msgCorrect;
			}

			/**
			 * Sinon, alors l'essai est un echec et on enlève un point de vie au joueur en envoyant le message approprie.
			 */
			else
			{
				if (nbVies > 0) nbVies--;

				if (nbVies != 0)
					data = Constantes.msgFalse + listeASCII[nbVies];
				else
				{
					data = Constantes.msgDefeat + listeASCII[nbVies] + Constantes.msgWordReveal + motADeviner;
					gameOver = true;
				}
			}
			return data;
		}

		#endregion
	}
}
