using System;
using System.Collections.Generic;
using System.Text;

namespace client.Classes
{
    class Constantes
    {
        public static string client = "[CLIENT]$";

        // Types de client / socket
        public static string typeMainSocket = "main";
        public static string typePvPSocket = "PvP";
        public static string typePvPServeurSocket = "PvPServeur";

        // Messages to user
        public static string msgProposeLetter = "Proposez une lettre !\n";

        // Messages to server
        public static string connexionWithServeurEstablished = "Connection etablie.";
        public static string closingConnexion = "Fermeture de la connexion.";

        // Type de msg
        public static string typeMsgInit = "INIT@";
        public static string typeMsgMDJ = "MDJ@";
        public static string typeMsgRejouer = "REJOUER@";
        public static string typeMsgReponse = "REPONSE@";
        public static string typeMsgMessage = "MESSAGE@";
        public static string typeMsgFin = "FIN@";
    }
}
