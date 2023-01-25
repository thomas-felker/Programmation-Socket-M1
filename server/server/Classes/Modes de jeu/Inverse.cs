using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace server.Classes.Modes_de_jeu
{
    internal class Inverse : ModeDeJeu
    {
        #region Donnees membres

        public char Choice { get; set; }
        public int intChoice { get; set; }
        public int[] madeChoices { get; set; }

        #endregion

        #region Constructeur

        public Inverse() {
            nombreAleatoire = new Random();
        }

        #endregion

        #region Methodes publiques

        /** 
		 * Methode de reinitialisation des differentes variables utilisees pendant une partie
		 */
        public override void reinitPartie()
        {
            rejouer = "N";

            intChoice = 0;
            madeChoices = new int[26];

            victoire = false;
            gameOver = false;
            correct = false;
        }

        /**
		 * Ici le joueur propose un mot au serveur et ce dernier tente de le deviner.
		 *
		 * - Le joueur entre un mot a faire deviner au serveur.
		 * - Le serveur dispose de 6 essais pour le deviner.
		 * - Le serveur ne peut tester qu'une lettre par une lettre.
		 */
        public override string jouerPartie(string data, byte[] bytes, int nbBytes, Socket clientSocket)
        {
            reinitPartie();
            int compteur = 0;
            Utils.sendMessageToClient(clientSocket, Constantes.msgIntroInverse, Constantes.typeMsgMessage);

            /** Reception du mot à obfusquer */
            motADeviner = Utils.receiveMessageFromClient(clientSocket, bytes);
            Console.WriteLine(Constantes.msgMot + motADeviner);

            /** Obfuscation du mot */
            motCache = obfusquer(motADeviner);
            nbVies = 13;

            while ((victoire == false) && (gameOver == false))
            {
                /** Envoi de l'essai du serveur au client */
                do
                {
                    intChoice = nombreAleatoire.Next(97, 122);
                } while (choiceAlreadyTried(intChoice));
                madeChoices[compteur] = intChoice;
                compteur++;

                Choice = (char)intChoice;

                Console.WriteLine("Essai : " + Choice);
                Utils.sendMessageToClient(clientSocket, Constantes.msgMot + motCache +
                    Constantes.msgTryForUser + Choice.ToString(), Constantes.typeMsgMessage);

                /** Reception du traitement fait par le client */
                data = treatData(Choice.ToString());

                Utils.sendMessageToClient(clientSocket, data, Constantes.typeMsgMessage);
                if ((victoire == false) && (gameOver == false)) Thread.Sleep(1500);
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

        private Boolean choiceAlreadyTried (int value)
        {
            Boolean res = false;

            if (madeChoices.Length > 0)
                for (int i = 0; i < madeChoices.Length; i++)
                    if (madeChoices[i] == value)
                        res = true;

            return res;
        }

        private string treatData(string data)
        {
            /**
			  * Traitement des donnees reçues par le client :
			  *
			  * Si la longueur de data est egale à 1, alors on a reçu un caractère.
			  * On parcourt alors le mot à deviner pour verifier si ce caractère en fait partie et s'il n'a pas dejà ete devine,
			  * auquel cas on met le booleen correct à true.
			  */
            correct = false;
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
