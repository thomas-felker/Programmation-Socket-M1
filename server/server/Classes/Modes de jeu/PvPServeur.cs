using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server.Classes.Modes_de_jeu
{
    class PvPServeur : Multi
    {
        #region Donnees membres 

        DateTime startTime { get; set; }
        public int timeLimit { get; set; }

        private Thread threadP1 { get; set; }
        private Thread threadP2 { get; set; }

        string rejoueJ1 { get; set; }
        public double timeJ1 { get; set; }
        public bool victoireJ1 { get; set; }
        public string motCacheJ1 { get; set; }

        string rejoueJ2 { get; set; }
        public double timeJ2 { get; set; }
        public bool victoireJ2 { get; set; }
        public string motCacheJ2 { get; set; }

        #endregion

        public PvPServeur(ClassServer.Serveur serveur)
        {
            clientsSockets = new Socket[2];

            clientsSockets[0] = serveur.clientSocket;
            Utils.sendMessageToClient(clientsSockets[0], Constantes.attenteConnexionAdversaire, Constantes.typeMsgMessage);

            Console.WriteLine(Constantes.attenteConnexion);
            clientsSockets[1] = serveur.runConnexion();

            // Envoie du type de Client 
            Utils.sendMessageToClient(clientsSockets[1], Constantes.typePvPServeurSocket, Constantes.typeMsgType);
            Thread.Sleep(250);
        }

        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket clientSocket)
        {
            reinitPartie();
            sendMessageToBoth("\nBienvenue dans le jeu du pendu ! Vous disposez chacun de " + nbVies + " essais et de " + timeLimit + " secondes !\n");
            Thread.Sleep(250);

            threadP1 = new Thread(launchP1);
            threadP1.Start();
            threadP2 = new Thread(launchP2);
            threadP2.Start();

            threadP1.Join();
            threadP2.Join();

            /** Affichage du vainqueur */
            sendMessageToBoth(getMessageEndgame());

            Thread.Sleep(100);

            /**
			 * La partie est finie, on attend la reponse du client pour savoir s'il souhaite rejouer ou non et
			 * on retourne la reponse au serveur qui l'interprète.
			 */
            Thread ThreadRejoueJ1 = new Thread(receiveRejouerJ1);
            ThreadRejoueJ1.Start();
            Thread ThreadRejoueJ2 = new Thread(receiveRejouerJ2);
            ThreadRejoueJ2.Start();

            ThreadRejoueJ1.Join();
            ThreadRejoueJ2.Join();

            Console.WriteLine(Constantes.msgJ1 + rejoueJ1);
            Console.WriteLine(Constantes.msgJ2 + rejoueJ2);

            if (rejoueJ1 != rejoueJ2)
            {
                rejouer = "N";
            }
            else
            {
                rejouer = rejoueJ1 == "O" ? "O" : "N";
            }

            sendMessageToBoth(rejouer);

            Thread.Sleep(100);

            if (rejouer == "N")
            {
                Utils.sendMessageToClient(clientsSockets[1], Constantes.closingConnexion, Constantes.typeMsgFin);
                Console.WriteLine(Utils.receiveMessageFromClient(clientsSockets[1], bytes));
            }

            return rejouer;
        }

        public override void reinitPartie()
        {
            rejouer = "N";
            motADeviner = "";
            motCache = "";

            Thread.Sleep(100);
            motADeviner = listeMots[nombreAleatoire.Next(0, listeMots.Capacity - 1)];

            Thread.Sleep(100);
            motCache = obfusquer(motADeviner);
            setHP();

            victoireJ1 = false;
            motCacheJ1 = motCache;
            timeJ1 = 0;

            victoireJ2 = false;
            motCacheJ2 = motCache;
            timeJ2 = 0;

            timeLimit = 60;
        }

        public void launchP1()
        {
            int nbViesJ1 = setHP();
            bool gameOverJ1 = false;
            bool correctJ1 = false;
            launchGameIndependant(clientsSockets[0], nbViesJ1, victoireJ1, gameOverJ1, correctJ1, timeJ1, motCacheJ1, 1);
        }

        public void launchP2()
        {
            int nbViesJ2 = setHP();
            bool gameOverJ2 = false;
            bool correctJ2 = false;
            launchGameIndependant(clientsSockets[1], nbViesJ2, victoireJ2, gameOverJ2, correctJ2, timeJ2, motCacheJ2, 2);
        }

        public void launchGameIndependant(Socket clientSocket, int _nbVies, bool _victoire, bool _correct, bool _gameOver, double remainingTime, string _motCache, int joueur)
        {
            byte[] bytes = new byte[2048];
            string _data;
            startTimer();

            Utils.sendMessageToClient(clientSocket, "Vous etes le joueur " + joueur, Constantes.typeMsgMessage);

            /** Le jeu continue tant que la partie n'est ni gagnee ni perdue. */
            while ((_victoire == false) && (_gameOver == false))
            {
                /** On remet les variables booleennes temporaires à faux et on envoie le message au client. */
                _correct = false;

                /** Envoi du mot obfusque au serveur */
                Utils.sendMessageToClient(clientSocket, Constantes.msgMot + _motCache + Constantes.msgVie + _nbVies, Constantes.typeMsgQuestion);

                /** Reception et affichage du caractère envoye par le client */
                _data = Utils.receiveMessageFromClient(clientSocket, bytes);
                Console.WriteLine(Constantes.msgPrintTry + _data);

                /**
				 * Traitement des donnees reçues par le client :
				 *
				 * Si la longueur de data est egale à 1, alors on a reçu un caractère.
				 * On parcourt alors le mot à deviner pour verifier si ce caractère en fait partie et s'il n'a pas dejà ete devine,
				 * auquel cas on met le booleen _correct à true.
				 */
                if (_data.Length == 1 && _data.ToString() != "")
                {
                    #region Check if correct : oblige de separer pour les threads
                    char temp = _data.ToCharArray()[0];

                    for (int i = 0; i < _motCache.Length; i++)
                    {
                        if (temp == motADeviner[i] && _motCache[i] == '*')
                        {
                            _correct = true;
                            var tempChar = _motCache.ToCharArray();
                            tempChar[i] = _data.ToCharArray()[0];
                            _motCache = "";

                            for (int j = 0; j < tempChar.Length; j++)
                                _motCache += tempChar[j];
                        }
                    }
                    #endregion

                    _victoire = (_motCache == motADeviner);
                }

                /** Si la longueur est differente de 1, deux cas se distinguent : */
                else
                {
                    /** Soit la longueur est egale à la longueur du mot, auquel cas on considère cela comme un essai. */
                    if (_data.Length == motADeviner.Length)
                    {
                        _correct = _data == motADeviner;
                        _victoire = _correct;
                    }
                }


                if (noTimeLeft())
                {
                    nbVies = 0;
                    _data = Constantes.msgDefeat + Constantes.msgTimeOut + listeASCII[_nbVies]
                        + Constantes.msgWordReveal + motADeviner;
                    gameOver = true;
                }
                /**
				 * Si l'essai est _correct, on verifie si c'est une _victoire et on envoie le message approprie.
				 */
                else if (_correct)
                {
                    if (_victoire)
                    {
                        remainingTime = (double)(startTime - DateTime.Now).TotalSeconds;
                        _data = Constantes.msgVictory + Constantes.msgWordReveal + motADeviner;

                        if (joueur == 1)
                        {
                            victoireJ1 = true;
                            timeJ1 = remainingTime;
                        }
                        else
                        {
                            victoireJ2 = true;
                            timeJ2 = remainingTime;
                        }
                    }
                    else
                        _data = Constantes.msgCorrect;
                }

                /**
				 * Sinon, alors l'essai est un echec et on enlève un point de vie au joueur en envoyant le message approprie.
				 */
                else
                {
                    _nbVies--;

                    if (_nbVies != 0)
                        _data = Constantes.msgFalse + listeASCII[_nbVies];
                    else
                        _data = Constantes.msgDefeat + listeASCII[_nbVies] + Constantes.msgWordReveal + motADeviner;
                }

                /** On verifie si le nombre de points de vie du client est à 0, auquel cas c'est un Game Over. */
                _gameOver = _nbVies == 0;

                if (!noTimeLeft() && !(_gameOver || _victoire))
                {
                    string timeLeft = ((timeLimit) - (DateTime.Now - startTime).TotalSeconds).ToString();
                    _data += "\nIl reste " + timeLeft + " secondes !";
                }

                Utils.sendMessageToClient(clientSocket, _data, Constantes.typeMsgMessage);
            }
        }


        public void receiveRejouerJ1()
        {
            byte[] bytes = new byte[2048];
            Utils.sendMessageToClient(clientsSockets[0], Constantes.msgPlayAgain, Constantes.typeMsgRejouer);
            rejoueJ1 = Utils.receiveMessageFromClient(clientsSockets[0], bytes);
        }

        public void receiveRejouerJ2()
        {
            byte[] bytes = new byte[2048];
            Utils.sendMessageToClient(clientsSockets[1], Constantes.msgPlayAgain, Constantes.typeMsgRejouer);
            rejoueJ2 = Utils.receiveMessageFromClient(clientsSockets[1], bytes);
        }

        private void startTimer()
        {
            startTime = DateTime.Now;
        }

        private bool noTimeLeft()
        {
            var currentTime = DateTime.Now;
            TimeSpan timeSpan = currentTime - startTime;
            return timeLimit - timeSpan.TotalSeconds < 0;
        }

        private string getMessageEndgame()
        {
            string res;

            if (victoireJ1 && victoireJ2)
            {
                string vainqueur = timeJ1 > timeJ2 ? "1" : "2";
                res = "Victoire du joueur " + vainqueur + " ! Il a ete plus rapide !";
            }
            else if (victoireJ1)
                res = "Victoire du joueur 1 !";
            else if (victoireJ2)
                res = "Victoire du joueur 2 !";
            else
                res = "Defaite des 2 joueurs !";

            return res;
        }
    };
}
