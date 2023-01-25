using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;
using client.Classes.Modes_de_jeu;
using client.Classes;
using System.Threading;

namespace ClassClient
{
	/// Classe visant à décrire le comportement de l'application côté client

	class Client
	{
		#region Données membres

		private ModeDeJeu partie { get; set; }

		private string data { get; set; }
		private string rejouer { get; set; }
		private int selectedMDJ { get; set; }

		private int nbBytes { get; set; }
		private byte[] message { get; set; }
		private byte[] bytes { get; set; }

		private IPHostEntry ipHost;
		private IPAddress ipAddr;
		private IPEndPoint localEndPoint;

		private Socket socketSender;

		#endregion

		#region Constructeur

		/**
		 * Constructeur par défaut
		 */
		public Client()
		{
			message = new byte[2048];
			bytes = new byte[2048];
			selectedMDJ = 10;
		}

		#endregion

		#region Méthodes publiques

		/**
		* Méthode visant à décrire le comportement du client lors de son exécution
		*/
		public void ExecuteClient()
		{
			Console.WriteLine("\n+--------+\n| Client |\n+--------+");

			try
			{
				ipHost = Dns.GetHostEntry(Dns.GetHostName());
				ipAddr = ipHost.AddressList[0];
				localEndPoint = new IPEndPoint(ipAddr, 11111);
				var test = ipHost.AddressList[1];

				socketSender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				try
				{
					socketSender.Connect(localEndPoint);
					Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
					Utils.sendMessageToServeur(socketSender, Constantes.connexionWithServeurEstablished, Constantes.typeMsgInit);

					string typeClient = Utils.receiveMessageFromServeur(socketSender, bytes);

					do
					{
						if (typeClient == "main")
						{
							selectMDJMain();
						}
						else
						{
							selectMDJAdversaire(typeClient);
						}

						// On entre dans le jeu si le joueur a sélectionné autre chose que 0.
						if (selectedMDJ != 0)
						{
							createGame();

							// Déroulement de la partie tant que rejouer contient "O"
							do
							{
								if (partie != null)
								{
									rejouer = partie.jouerPartie(data, bytes, nbBytes, socketSender);
								}
							} while (rejouer.Contains("O"));

							if (typeClient != "main")
							{
								/** Si le client est un adversaire et que la partie n'est pas relancée, 
								 * on met selectedMDJ à 0 pour lancer la procédure de fermeture du client */
								selectedMDJ = 0;
							}
						}
					} while (selectedMDJ != 0);

					Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
					Utils.sendMessageToServeur(socketSender, Constantes.closingConnexion, Constantes.typeMsgFin);

					Thread.Sleep(250);

					// On ferme le client.
					socketSender.Shutdown(SocketShutdown.Both);
					socketSender.Close();
				}

				// Gestion des exceptions relatives au socket
				catch (ArgumentNullException ane) { Console.WriteLine("ArgumentNullException : {0}", ane.ToString()); }
				catch (SocketException se) { Console.WriteLine("SocketException : {0}", se.ToString()); }
				catch (Exception ex) { Console.WriteLine("Unexpected exception : {0}", ex.ToString()); }
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
		}

		#endregion

		#region Méthodes privées

		private void selectMDJMain()
        {
			string MDJ = "";
			// Affichage du menu et sélection du mode de jeu par le joueur
			data = Utils.receiveMessageFromServeur(socketSender, bytes);

			do
			{
				Console.WriteLine("{0}", data);
				MDJ = Console.ReadLine();
			} while (!Utils.checkStringMDJChoice(MDJ));

			// Envoie du mode de jeu sélectionné par le joueur au serveur
			Utils.sendMessageToServeur(socketSender, MDJ, Constantes.typeMsgMDJ);

			selectedMDJ = int.Parse(MDJ);
		}

		private void selectMDJAdversaire(string socket)
        {
			switch (socket)
			{
				case "PvP":
					selectedMDJ = 13;
					break;
				case "PvPServeur":
					selectedMDJ = 14;
					break;
				default:
					selectedMDJ = 0;
					break;
			}
		}

		private void createGame()
        {
			// Création de la partie en fonction du mode de jeu sélectionné par le joueur
			switch (selectedMDJ)
			{
				case 1:
					partie = new Classique();
					break;
				case 2:
					partie = new Classique(true);
					break;
				case 3:
					partie = new PvP();
					Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
					break;
				case 4:
					partie = new PvPServeur(1);
					Console.WriteLine(Utils.receiveMessageFromServeur(socketSender, bytes));
					break;
				case 5:
					partie = new Inverse();
					break;
				case 13:
					partie = new PvP();
					break;
				case 14:
					partie = new PvPServeur(2);
					break;
				default:
					partie = null; rejouer = "N";
					break;
			}
		}

		#endregion
	}
}
