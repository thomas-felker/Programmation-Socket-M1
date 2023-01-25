using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using server;

namespace server.Classes.Modes_de_jeu
{
    class PvP : Multi
    {
        
		#region Donnees membres

        public int chercheur { get; set; }
        public int decideur { get; set; }
		public string rejoueChercheur { get; set; }
		public string rejoueDecideur { get; set; }

        #endregion

        #region Constructeur

        public PvP(ClassServer.Serveur serveur)
        {
            chercheur = nombreAleatoire.Next(0, 1);
            decideur = chercheur == 0 ? 1 : 0;

			clientsSockets = new Socket[2];
			
			clientsSockets[0] = serveur.clientSocket;
			Utils.sendMessageToClient(clientsSockets[0], Constantes.attenteConnexionAdversaire, Constantes.typeMsgMessage);

            Console.WriteLine(Constantes.attenteConnexion);
			clientsSockets[1] = serveur.runConnexion();

			// Envoie du type de Client 
			Utils.sendMessageToClient(clientsSockets[1], Constantes.typePvPSocket, Constantes.typeMsgType);
			Thread.Sleep(250);
		}

        #endregion

        #region Methodes publiques

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket clientSocket)
        {
            reinitPartie();

			Thread.Sleep(250);

            sendMessageToBoth("\nBienvenue dans le jeu du pendu en PvP !" + "\nLe joueur qui cherche le mot dispose de " + nbVies + " essais !" + "\n");

			Thread.Sleep(500);

			Utils.sendMessageToClient(clientsSockets[decideur], Constantes.msgIntroDecideur, Constantes.typeMsgMessage); 
			Utils.sendMessageToClient(clientsSockets[chercheur], Constantes.msgIntroChercheur, Constantes.typeMsgMessage);

			// Thread.Sleep(250);

            motADeviner = Utils.receiveMessageFromClient(clientsSockets[decideur], bytes);
            motCache = obfusquer(motADeviner);
			setHP();
			/** Le jeu continue tant que la partie n'est ni gagnee ni perdue. */
			while ((victoire == false) && (gameOver == false))
			{
				/** On remet les variables booleennes temporaires à faux et on envoie le message au client. */
				correct = false;
				erreur = false;

				/** Envoi du mot obfusque au serveur */
				Utils.sendMessageToClient(clientsSockets[chercheur], Constantes.msgMot + motCache + Constantes.msgVie + nbVies, Constantes.typeMsgQuestion);

				/** Reception et affichage du caractère envoye par le chercheur */
				data = Utils.receiveMessageFromClient(clientsSockets[chercheur], bytes);
				Console.WriteLine(Constantes.msgPrintTry + data);

				// Envoi de l'essai au decideur
				Utils.sendMessageToClient(clientsSockets[decideur], Constantes.msgGameDecideur + data, Constantes.typeMsgMessage);

				data = treatData(data);

				// Envoi du resultat du traitement aux 2 joueurs
				sendMessageToBoth(data);
			}

			Thread.Sleep(100);

			// 
			Thread ThreadRejoueChercheur = new Thread(receiveRejouerChercheur);
			ThreadRejoueChercheur.Start();
			Thread ThreadRejoueDecideur = new Thread(receiveRejouerDecideur);
			ThreadRejoueDecideur.Start();

			ThreadRejoueChercheur.Join();
			ThreadRejoueDecideur.Join();

			Console.WriteLine(Constantes.msgChercheur + rejoueChercheur);
			Console.WriteLine(Constantes.msgDecideur + rejoueDecideur);

			if (rejoueChercheur != rejoueDecideur)
			{
				rejouer = "N";
			}
			else
			{
				rejouer = rejoueChercheur == "O" ? "O" : "N";
			}

			sendMessageToBoth(rejouer);

			if (rejouer == "N")
			{
				Utils.sendMessageToClient(clientsSockets[1], Constantes.closingConnexion, Constantes.typeMsgFin);
				Console.WriteLine(Utils.receiveMessageFromClient(clientsSockets[1], bytes));
			}

			return rejouer;
        }

        public override void reinitPartie()
        {
            chercheur = chercheur == 0 ? 1 : 0;
            decideur = chercheur == 0 ? 1 : 0;

			rejouer = "N";
			motADeviner = "";
			motCache = "";

			victoire = false;
			gameOver = false;
			correct = false;
			erreur = false;
		}


        #endregion

        #region Methodes Privees

        private void receiveRejouerChercheur()
        {
			byte[] bytes = new byte[2048];
			Utils.sendMessageToClient(clientsSockets[chercheur], Constantes.msgPlayAgain, Constantes.typeMsgRejouer);
			rejoueChercheur = Utils.receiveMessageFromClient(clientsSockets[chercheur], bytes);
		}

		private void receiveRejouerDecideur()
		{
			byte[] bytes = new byte[2048];
			Utils.sendMessageToClient(clientsSockets[decideur], Constantes.msgPlayAgain, Constantes.typeMsgRejouer);
			rejoueDecideur = Utils.receiveMessageFromClient(clientsSockets[decideur], bytes);
		}

		private string treatData(string data)
		{
			/**
			 * Traitement des donnees reçues par le serveur :
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

			/**
			 * Si l'essai est correct, on verifie si c'est une victoire et on envoie le message approprie.
			 */
			if (correct)
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
