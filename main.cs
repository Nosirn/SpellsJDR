namespace Projet
{
	public class M
	{
		public static void Main()
		{
			while (true)
			{
				// Demande du sort
				Console.WriteLine();
				Console.WriteLine("Écrivez votre sort : ");
				var sort = Console.ReadLine();
				while (sort == "" || sort == null)
				{
					Console.WriteLine();
					Console.WriteLine("Veuillez entrer un sort : ");
					sort = Console.ReadLine();
				}
				Console.WriteLine();

				if (sort == "exit") break;
				// Récupération des composants
				/*
				afficher(flatSpell(sort));
				Console.WriteLine("-_-_-_-_-_-_-_-_-_-_-_-_-_-_-_");
				*/
				try
				{
					afficher(calculCout(sort));
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}
		}

		/**	<summary> Sépare chaque mot du sort en blocs contenant le mot et ses arguments</summary>
			<param name="e">L'écriture du sort</param>
			<returns>La liste des mots-clés avec leurs arguments composant le sort</returns>
		*/
		public static string[] flatSpell(string e)
		{
			Dictionary<int, string> words = new Dictionary<int, string>() { { 0, "" } }; // Mots trouvés
			List<int> writable = new List<int>() { 0 }; // Indice des mots où l'on peut écrire (Non fermé par une ')')

			for (int i = 0; i < e.Length; i++)
			{
				bool addWritable = false;
				switch (e[i])
				{
					// Cas : Ajout d'un argument
					case ',':
						writable.RemoveAt(writable.Count - 1); // Dernier mot de la liste interdit à l'écriture
						goto LPAREN;

					// Cas : Déclaration d'arguments
					case '(':
					LPAREN:
						words.Add(words.Count, "");
						addWritable = true;
						goto default;

					// Cas : Fermeture d'arguments
					case ')':
						writable.RemoveAt(writable.Count - 1); // Dernier mot de la liste interdit à l'écriture
						goto default;

					// Cas par défaut : Ecriture d'une lettre
					default:
						foreach (KeyValuePair<int, string> word in words)
						{
							if (writable.Contains(word.Key) && !((words[word.Key] == "") && e[i] == ' ')) // Vérifie si le mot peut être complété (Droit d'écriture)
								words[word.Key] += e[i]; // Ajoute la lettre à la suite
						}
						if (addWritable) writable.Add(words.Count - 1);
						break;
				}

			}
			string[] res = new string[words.Count]; // Résultat de la fonction
			foreach (KeyValuePair<int, string> word in words)
			{
				res[word.Key] = word.Value;
			}
			return res;
		}

		/**	<summary> Obtenir le mot-clé principal du sort </summary>
			<param name="s"> L'écriture du sort </param>
			<returns> Le mot-clé principal </returns>
		*/
		public static string getMotPrincipal(string s)
		{
			string res = ""; int i = 0;
			while (s.Length > i && s[i] != '(') res += s[i++];
			return res;
		}

		/**	<summary> Obtenir les arguments du mot-clé principal </summary>
			<param name="s"> L'écriture du sort</param>
			<returns>La liste d'arguments du mot-clé principal</returns>
		*/
		public static string[] getArguments(string s)
		{
			string[] e = flatSpell(s);
			List<string> list = new List<string>(); // Résultat sous forme de liste

			for (int i = 1; i < e.Length; i++)
			{
				string current = e[i];
				list.Add(current);

				for (int j = 0; j < current.Length; j++)
					if (current[j] == '(' || current[j] == ',') i++;
			}

			string[] res = list.ToArray();
			return res;
		}
		public static void afficher<T>(T[] list) where T : IComparable<T>
		{
			// Afficher le coût d'un sort
			if (list is System.Byte[] && list.Length == 3)
			{
				Console.WriteLine("MC : " + list[0]);
				Console.WriteLine("MM : " + list[1]);
				Console.WriteLine("ME : " + list[2]);
			}
			// Afficher les autres tableaux
			else foreach (T elem in list) Console.WriteLine(elem);
		}
		public static string[] ToLower(string[] a)
		{
			List<string> res = new List<string>();
			foreach (string elem in a) res.Add(elem.ToLower());
			return res.ToArray();
		}
		public static byte[] Somme(byte[] a, byte[] b)
		{
			byte[] res = new byte[a.Length];

			for (byte r = 0; r < a.Length; r++)
				res[r] = (byte)(a[r] + b[r]);

			return res;
		}

		/** <summary> Récupère la valeur numérique d'une constante </summary>
			<param name="c"> La constante </param>
		*/
		public static byte constValue(string c)
		{
			return byte.Parse(c.Split(" ", 2)[1]);
		}

		public static byte selectCible(string cible)
		{
			if (cible.StartsWith("contact(")) return 1;
			if (cible == "entité") return 2;
			if (cible.StartsWith("objet(")) return 3;
			if (cible.StartsWith("projectile(")) return 4;
			if (cible.StartsWith("rayon(")) return 5;
			if (cible == "soi") return 6;
			if (cible.StartsWith("zone(")) return 7;
			else return 0;
		}

		public static byte[] coutCible(string cible, byte puissance)
		{
			byte[] res = new byte[3] { 0, 0, 0 };
			byte indexCible = selectCible(cible);

			switch (indexCible)
			{
				case 1: res = Somme(res, Mot.Contact(cible, puissance)); break;
				case 2: res = Somme(res, Mot.Entite()); break;
				case 3: res = Somme(res, Mot.Objet(cible)); break;
				case 4: res = Somme(res, Mot.Projectile(cible)); break;
				case 5: res = Somme(res, Mot.Rayon(cible)); break;
				case 6: res = Somme(res, Mot.Soi()); break;
				case 7: res = Somme(res, Mot.Zone(cible, puissance)); break;
				default: throw new Exception("CibleUnknown : La cible n'a pas été comprise.");
			}
			return res;
		}

		public static byte selectForme(string forme)
		{
			if (forme.StartsWith("boule(")) return 1;
			if (forme.StartsWith("cage(")) return 2;
			if (forme.StartsWith("fleur(")) return 3;
			if (forme.StartsWith("flèche(")) return 4;
			if (forme.StartsWith("lame(")) return 5;
			if (forme.StartsWith("lance(")) return 6;
			if (forme.StartsWith("lierre(")) return 7;
			if (forme == "ligne") return 8;
			else return 0;
		}

		public static byte[] coutForme(string forme)
		{
			byte[] res = new byte[3] { 0, 0, 0 };
			byte indexForme = selectForme(forme);

			switch (indexForme)
			{
				case 1: res = Somme(res, Mot.Boule(forme)); break;
				case 2: res = Somme(res, Mot.Cage(forme)); break;
				case 3: res = Somme(res, Mot.Fleur(forme)); break;
				case 4: res = Somme(res, Mot.Fleche(forme)); break;
				case 5: res = Somme(res, Mot.Lame(forme)); break;
				case 6: res = Somme(res, Mot.Lance(forme)); break;
				case 7: res = Somme(res, Mot.Lierre(forme)); break;
				case 8: res = Somme(res, Mot.Ligne()); break;
				default: throw new Exception("FormeUnknown : La forme n'a pas été comprise.");
			}
			return res;
		}

		public static byte selectTemps(string temps)
		{
			if (temps.StartsWith("constante")) return 1;
			else if (temps == "aura") return 2;
			else if (temps == "passif") return 3;
			else return 0;
		}

		/** <summary> Coût en mémoire des constantes LIBRES </summary>
			<param name="args"> Arguments du sort </param>
		*/
		public static byte coutMemoireConst(string[] args)
		{
			byte res = 0;
			foreach (string argument in args)
			{
				if (argument.StartsWith("constante"))
				{
					if (constValue(argument) < 0) throw new Exception("NegativeConstant");
					res += (byte)(constValue(argument) / 5);
				}
			}
			return res;
		}

		public static byte nbAddons(string s)
		{
			byte res = 0;
			while (res < s.Length && s[s.Length - 2 - res] == ')') res++;
			return res;
		}
		public static byte[] calculCout(string s)
		{
			string motPrincipal = getMotPrincipal(s).ToLower();
			string[] arguments = ToLower(getArguments(s));

			// Compte du nombre d'arguments pour avoir le bon nombre de paramètres sur un mot clé donné
			int nbArgs = arguments.Count();

			// Résultat du calcul (Important donc :D)
			byte[] res = new byte[3] { 0, 0, 0 };

			// Coût en mémoire des constantes LIBRES (N'est pas enclavé par un mot clé)
			res[1] = coutMemoireConst(arguments);

			// Cout Cible
			// Blacklist : Analyse (Aucune puissance)
			res = Somme(res, coutCible(arguments[0], (byte)(new string[1] { "analyse" }.Contains(motPrincipal) ? 0 : (byte)constValue(arguments[1]))));

			// Cout element
			switch (motPrincipal)
			{
				// ================================================================================
				// ANALYSE
				// ================================================================================
				case "analyse":
					if (nbArgs < 1 || nbArgs > 2) goto Error; // Manque des arguments
					res = Somme(res, Mot.Analyse());
					if (nbArgs > 1)
						goto Addon;
					break;
				// ================================================================================
				// ARMURE & ESPRIT
				// ================================================================================
				case "armure":
				case "esprit":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Manque des arguments
					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Armuresprit(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Armuresprit(constValue(arguments[1])));
					break;
				// ================================================================================
				// BRULE
				// ================================================================================
				case "brûle":
					if (nbArgs < 3 || nbArgs > 4) goto Error;
					res = Somme(res, Mot.Brule(constValue(arguments[1]), constValue(arguments[2])));
					goto Addon;
				// ================================================================================
				// EAU
				// ================================================================================
				case "eau":
					// Nombre d'arguments incorrect ?
					if (nbArgs < 2 || nbArgs > 4) goto Error;

					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Eau(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Eau(constValue(arguments[1])));
					break;
				// ================================================================================
				// FEU
				// ================================================================================
				case "feu":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Nombre d'arguments incorrect ?
					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Feu(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Feu(constValue(arguments[1])));
					break;
				// ================================================================================
				// FOUDRE
				// ================================================================================
				case "foudre":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Nombre d'arguments incorrect ?
					if (nbArgs > 3)
					{
						res = Somme(res, Mot.Foudre(constValue(arguments[1]), arguments[2], nbAddons(s)));
						goto Addon;
					}
					if (nbArgs > 2)
						res = Somme(res, Mot.Foudre(constValue(arguments[1]), arguments[2]));
					else res = Somme(res, Mot.Foudre(constValue(arguments[1])));
					break;
				// ================================================================================
				// GLACE
				// ================================================================================
				case "glace":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Manque des arguments

					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Glace(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Glace(constValue(arguments[1])));
					break;
				// ================================================================================
				// SOIN
				// ================================================================================
				case "soin":
					if (nbArgs < 2 || nbArgs > 4 || constValue(arguments[1]) > 10) goto Error; // Manque des arguments ou argument invalide

					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Soin(constValue(arguments[1]), arguments[2]));
						goto Addon;
					}
					else res = Somme(res, Mot.Soin(constValue(arguments[1])));
					break;
				// ================================================================================
				// SON
				// ================================================================================
				case "son":
					if (nbArgs < 2 || nbArgs > 4) goto Error;
					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Son(constValue(arguments[1]), constValue(arguments[2])));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Son(constValue(arguments[1])));
					break;
				// ================================================================================
				// TERRE
				// ================================================================================
				case "terre":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Manque des arguments

					if (nbArgs > 2)
					{
						res = Somme(res, Mot.Terre(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.Terre(constValue(arguments[1])));
					break;
				// ================================================================================
				// PERMÉABLE
				// ================================================================================
				case "perméable":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Nombre d'arguments incorrect ?
					if (nbArgs > 3)
					{
						res = Somme(res, Mot.Permeable(constValue(arguments[1]), nbAddons(s)));
						goto Addon;
					}
					else res = Somme(res, Mot.Permeable(constValue(arguments[1])));
					break;
				// ================================================================================
				// VIE PONDÉRÉ
				// ================================================================================
				case "vie pondéré":
					if (nbArgs < 2 || nbArgs > 4) goto Error; // Manque des arguments

					if (nbArgs > 2)
					{
						res = Somme(res, Mot.ViePondere(constValue(arguments[1]), arguments[2]));
						goto Addon; // Mot clé supplémentaire ?
					}
					else res = Somme(res, Mot.ViePondere(constValue(arguments[1])));
					break;

				// ======== Vent (INCONNU)
				case "vent": throw new NotImplementedException();

				// ================================================================================
				// DEFAULT -> Error
				// ================================================================================
				default:
				Error: // ERREUR !
					Console.WriteLine("Le sort \"" + s + "\" n'a pas été compris.");
					throw new Exception("Unhandled Spell");

				// ================================================================================
				// ADDON
				// ================================================================================
				Addon:
					if (nbArgs > 3) res = Somme(res, calculCout(arguments[arguments.Length - 1]));
					break;
			}

			return res;
		}
	}
}