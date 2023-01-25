using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace server.Classes.Modes_de_jeu
{
	abstract class ModeDeJeu
	{
		#region Donnees membres

		// Donnees en rapport avec le serveur
		public string data { get; set; }

		// Donnees en rapport avec le jeu
		public Random nombreAleatoire { get; set; }
		public List<string> listeMots { get; set; }
		public List<string> listeASCII { get; set; }

		// Donnees en rapport avec la partie
		public string rejouer { get; set; }
		public string motADeviner { get; set; }
		public string motCache { get; set; }
		public int nbVies { get; set; }
		public bool victoire { get; set; }
		public bool gameOver { get; set; }
		public bool correct { get; set; }
		public bool erreur { get; set; }

		#endregion

		#region Constructeur

		public ModeDeJeu()
		{
			initPartie();
			nombreAleatoire = new Random();
		}

		#endregion

		#region Methodes publiques

		public abstract void reinitPartie();

		public abstract string jouerPartie(string data, byte[] bytes, int nbBytes, Socket clientSocket);

		public void checkIfCorrect (string data)
        {
			char temp = data.ToCharArray()[0];

			for (int i = 0; i < motCache.Length; i++)
			{
				if (temp == motADeviner[i] && motCache[i] == '*')
				{
					correct = true;
					var tempChar = motCache.ToCharArray();
					tempChar[i] = data.ToCharArray()[0];
					motCache = "";

					for (int j = 0; j < tempChar.Length; j++)
						motCache += tempChar[j];
				}
			}

			victoire = (motCache == motADeviner);
		}

		public int setHP ()
        {
			if (motADeviner.Length < 7)
			{
				nbVies = motADeviner.Length / 2;
			}
			else if (motADeviner.Length < 13)
			{
				nbVies = motADeviner.Length / 2 + 2;
			}
			else
			{
				nbVies = 13;
			}
			return nbVies;
		}

		public string obfusquer(string mot)
		{
			string res = "";
			int indiceLettreADevoiler = nombreAleatoire.Next(0, mot.Length - 1);
			char lettreClaire = mot[indiceLettreADevoiler];

			/**
			 * Obfuscation des caractères en fonction du caractère aleatoire choisi dans le mot à deviner en clair
			 */
			for (int i = 0; i < mot.Length; i++)
			{
				if (i == indiceLettreADevoiler || mot[i] == lettreClaire)
					res += lettreClaire;
				else
					res += "*";
			}

			return res;
		}

		#endregion

		#region Methodes privees

		private void initPartie()
		{
			initListes();
		}

		private void initListes()
		{
			listeMots = new List<string>();

			using (System.IO.StreamReader r = new System.IO.StreamReader("../../../data/listeMots.json"))
			{
				string json = r.ReadToEnd();
				listeMots = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
			}

			listeASCII = new List<string>();

			using (System.IO.StreamReader r = new System.IO.StreamReader("../../../data/listeASCII.json"))
			{
				string json = r.ReadToEnd();
				listeASCII = Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(json);
			}
		}

		#endregion
	}
}
