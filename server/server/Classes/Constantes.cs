using System;
using System.Collections.Generic;
using System.Text;

namespace server.Classes
{
    class Constantes
    {
		public static string serveur = "[SERVEUR]$";

		// Messages serveur
		public static string messageServeur = "\n+---------+" + "\n| Serveur |" + "\n+---------+";

		public static string attenteConnexion = "\nEn attente d'une connexion...";
		public static string attenteConnexionAdversaire = "\nEn attente de l'adversaire...";
        public static string connexionEtablie = "\nConnexion etablie";
		public static string erreurClient = "Fermeture du client de maniere impromptue";
		public static string msgDeconnexionClient = "\nClient deconnecte";
		public static string closingConnexion = "Fermeture de la connexion.";
		public static string msgPlayAgain = "\nVoulez-vous rejouer ? (O/N)\n";

		public static string msgPrintTry = "\nCaractere recu : ";
		public static string msgChercheur = "Chercheur : ";
		public static string msgDecideur = "Decideur : ";
		public static string msgJ1 = "J1 : ";
		public static string msgJ2 = "J2 : ";

		// Types de client / socket
		public static string typeMainSocket = "main";
		public static string typePvPSocket = "PvP";
		public static string typePvPServeurSocket = "PvPServeur";

		// Messages to client
		public static string messageMenu = "\nSelectionnez une option en entrant le chiffre correspondant :\n" +
			"\n1 - CLASSIQUE          : L'ordinateur vous propose un mot, vous avez 6 essais pour le deviner." +
			"\n2 - CLASSIQUE - CHRONO : La partie est chronometree." +
			"\n3 - PVP                : Deux joueurs jouent l'un contre l'autre en se connectant au meme serveur. L'un des deux joueurs propose un mot a l'autre et ce dernier tente de le deviner." +
			"\n4 - PVP - SERVEUR      : Deux joueurs s'affrontent et le mot a deviner est fixe par le serveur." +
			"\n5 - INVERSE            : Le joueur propose un mot au serveur et ce dernier tente de le deviner." +
			"\n0 - QUITTER\n";
		public static string msgIntroClassique = defaultPotence + "\nBienvenue dans le jeu du pendu !";
		public static string msgIntroInverse = "\nBienvenue dans le jeu du pendu ! Entrez un mot a faire deviner au serveur : ";

		public static string msgTryForUser = "\nLe serveur essaye le caractere : ";

		public static string msgChrono = "\nEntrez le temps voulu en secondes (30s par defaut) pour le chrono : \n";
		public static string msgMot = "\nMot a deviner : ";
		public static string msgVie = "\nVie(s) : ";
		public static string msgTimeOut = "\nLe temps limite est ecoule !\n";
		public static string msgVictory = "\nVictoire !\n";
		public static string msgDefeat = "\nDefaite !\n";
		public static string msgWordReveal = "\nLe mot a deviner etait : ";
		public static string msgCorrect = "\nCorrect !";
		public static string msgFalse = "\nFaux, essayez encore !\n";

		public static string msgIntroDecideur = "\nVous etes le decideur ! Entrez un mot a faire deviner a votre adversaire : ";
		public static string msgIntroChercheur = "\nVous etes le chercheur ! Attendons que votre adversaire choisisse son mot.";
		public static string msgGameDecideur = "\nVotre adversaire a essaye ceci : ";

		public static string defaultPotence = "\n  +---+\n  |   |\n      |\n      |\n      |\n      |\n=========";

		// Types de messages
		public static string typeMsgInit = "INIT@";
		public static string typeMsgType = "TYPE@";
		public static string typeMsgMenu = "MENU@";
		public static string typeMsgQuestion = "QUESTION@";
		public static string typeMsgRejouer = "REJOUER@";
		public static string typeMsgMessage = "MESSAGE@";
		public static string typeMsgFin = "FIN@";
	}
}
