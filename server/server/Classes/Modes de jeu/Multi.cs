using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace server.Classes.Modes_de_jeu
{
    abstract class  Multi : ModeDeJeu
    {
        #region Donnees membres

        public Socket[] clientsSockets { get; set; }

        #endregion

        #region Constructeur 

        public Multi()
        {

        }

        #endregion

        #region Methodes publiques

        public void sendMessageToBoth(string data)
        {
            Utils.sendMessageToClient(clientsSockets[0], data, Constantes.typeMsgMessage);
            Utils.sendMessageToClient(clientsSockets[1], data, Constantes.typeMsgMessage);
        }

        #endregion
    }
}
