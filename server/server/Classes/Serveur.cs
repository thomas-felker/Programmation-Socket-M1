using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using server.Classes.Modes_de_jeu;
using server.Classes;
using System.Threading;

namespace ClassServer
{
	/// Classe visant à decrire le comportement de l'application côte serveur

	class Serveur
	{
		#region Donnees membres

		// Threads 
		private Thread[] threads;

		// Donnees reseau
		private IPHostEntry ipHost { get; set; }
		private IPAddress ipAddr { get; set; }
		private IPEndPoint localEndPoint { get; set; }
		public Socket listener { get; set; }
		public Socket clientSocket { get; set; }

		// Donnees d'echange
		private string data { get; set; }
		private byte[] bytes { get; set; }
		private int nbBytes { get; set; }
		private string selectMDJ { get; set; }

		// Donnees de jeu
		private ModeDeJeu partie { get; set; }
		private string rejouer { get; set; }

		#endregion

		#region Constructeur

		public Serveur()
		{
			threads = new Thread[10];
			bytes = new byte[2048];
		}

		#endregion

		#region Methodes publiques

		/**
		* Methode visant à decrire le comportement du serveur lors de son execution
		*/
		public void ExecuteServer()
		{
			Console.WriteLine(Constantes.messageServeur);

			ipHost = Dns.GetHostEntry(Dns.GetHostName());  // Obtention d'informations relatives à la machine hôte
			ipAddr = ipHost.AddressList[0];				   // Affectation de l'adresse IPv6 de l'hôte (0 pour IPv6, 1 pour IPv4)
			localEndPoint = new IPEndPoint(ipAddr, 11111); // Utilisation du port 11111

			listener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp); // Utilisation du mode connecte (TCP)

			try
			{
				listener.Bind(localEndPoint); // Liaison du socket serveur à l'IP
				listener.Listen(10);          // Definition de la limite à 10 clients maximum

				run();
			}
			catch (Exception e)
			{ 
				Console.WriteLine(server.Classes.Constantes.erreurClient);
				clientSocket.Close();
				run();
			}
		}

		public Socket runConnexion()
		{
			Console.WriteLine(Constantes.attenteConnexion);
			Socket tmp = listener.Accept(); // Le programme se met en pause, dans l'attente d'un client qui le contacterait.

			Utils.sendMessageToClient(tmp, Constantes.connexionEtablie + " : IP = " + ipHost.AddressList[1], Constantes.typeMsgInit);
			Console.WriteLine(Utils.receiveMessageFromClient(tmp, bytes));
			return tmp;
		}

		#endregion

		#region Methodes privees

		private void run()
		{
			do
			{
				clientSocket = runConnexion();
				clientTreatment(clientSocket);
				Console.WriteLine(Constantes.msgDeconnexionClient);
			} while (true);
		}

		private void clientTreatment(Socket clientSocket)
        {
			// Envoie du type de Client 
			Utils.sendMessageToClient(clientSocket, Constantes.typeMainSocket, Constantes.typeMsgType);

			// Les etapes prealables s'etant deroulees avec succès, l'application peut demarrer.
			do
			{
				// Envoi du menu au client
				Utils.sendMessageToClient(clientSocket, Constantes.messageMenu, Constantes.typeMsgMenu);

				// Reception et affichage de la selection de mdj par le client
				selectMDJ = Utils.receiveMessageFromClient(clientSocket, bytes);
				Console.WriteLine("\nMode de jeu selectionne : " + selectMDJ);

				// Lancement d'une partie en fonction du mode de jeu selectionne par le joueur
				selectionMDJ(selectMDJ);

				do
				{
					if (partie != null)
					{
						rejouer = partie.jouerPartie(data, bytes, nbBytes, clientSocket);
					}
				} while (rejouer.Contains("O")); // Le jeu continue tant que le joueur souhaite rejouer.
			} while (selectMDJ != "0");
			Utils.sendMessageToClient(clientSocket, Constantes.closingConnexion, Constantes.typeMsgFin);
			Console.WriteLine(Utils.receiveMessageFromClient(clientSocket, bytes));
		}

		private void selectionMDJ (string MDJ)
        {
			switch (int.Parse(MDJ))
			{
				case 1:
					partie = new Classique();
					break;
				case 2:
					partie = new Classique(true);
					break;
				case 3:
					partie = new PvP(this);
					break;
				case 4:
					partie = new PvPServeur(this);
					break;
				case 5:
					partie = new Inverse();
					break;
				default:
					partie = null;
					rejouer = "N";
					selectMDJ = "0";
					break;
			}
		}

		#endregion
	}
}
