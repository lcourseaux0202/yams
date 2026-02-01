using System;
using System.IO;

public struct ChallengesStruct{
    public int id;
    public string nom;
    public string desc; 
    public string points;   
    public bool[] dejaJouer;
    public ChallengesStruct(int id, string nom, string desc, string points){
        this.id = id;
        this.nom = nom;
        this.desc = desc;
        this.points = points;
        dejaJouer = new bool[2]{false,false};
    }
}

public struct Joueur{
    public int id;
    public string pseudo;
    public string[] TabLances;
    public string[] TabChallenges;
    public int[] TabScores;
    public int scoreMineur;
    public int bonus;
    public int score;
    public Joueur(int id){
        this.id = id;
        Console.WriteLine("Entrez le pseudo du joueur {0}: ", id);
        pseudo = Console.ReadLine();
        TabLances = new string[13];
        TabChallenges = new string[13];
        TabScores = new int[13];
        scoreMineur = 0;
        bonus = 0;
        score = 0;
    }
}

class Yams{

    /*Fonction créant la liste des challenges*/
    static ChallengesStruct[] CreateChallenge(){
        /* Fonction retournant un tableau de ChallengesStruct après l'avoir initialisé. */
        //Déclaration de la liste contenant les challenges de type ChallengesStruct  
        ChallengesStruct[] ListeChallenge = new ChallengesStruct[13]{
            new ChallengesStruct(1,"nombre1","Obtenir le maximum de 1","Somme des dés ayant obtenu 1"),
            new ChallengesStruct(2,"nombre2","Obtenir le maximum de 2","Somme des dés ayant obtenu 2"),
            new ChallengesStruct(3,"nombre3","Obtenir le maximum de 3","Somme des dés ayant obtenu 3"),
            new ChallengesStruct(4,"nombre4","Obtenir le maximum de 4","Somme des dés ayant obtenu 4"),
            new ChallengesStruct(5,"nombre5","Obtenir le maximum de 5","Somme des dés ayant obtenu 5"),
            new ChallengesStruct(6,"nombre6","Obtenir le maximum de 6","Somme des dés ayant obtenu 6"),
            new ChallengesStruct(7,"brelan","Obtenir 3 dés identiques","Sommes des 3 dés identiques"),
            new ChallengesStruct(8,"carre","Obtenir 4 dés identiques","Sommes des 4 dés identiques"),
            new ChallengesStruct(9,"full","Obtenir 3 dés identiques + 2 dés identiques","25 points"),
            new ChallengesStruct(10,"petite","Obtenir 1-2-3-4 ou 2-3-4-5 ou 3-4-5-6","30 points"),
            new ChallengesStruct(11,"grande","Obtenir 1-2-3-4-5 ou 2-3-4-5-6","40 points"),
            new ChallengesStruct(12,"yams","Obtenir 5 dés identiques","50 points"),
            new ChallengesStruct(13,"chance","Obtenir le maximum de points","Le total des dés obtenus")
        }; 
        return ListeChallenge;
    }

    /*Procédure d'affichage*/
    static void AfficherDesAscii(int[] TabDes){
        /* Procédure affichant les dés lancés par le joueur. */
        int N = TabDes.Length;
        string chaine = "";
        string[,] deAscii = new string[6,5]{
            {"┌─────────┐","│         │","│    O    │","│         │","└─────────┘"},
            {"┌─────────┐","│  O      │","│         │","│      O  │","└─────────┘"},
            {"┌─────────┐","│  O      │","│    O    │","│      O  │","└─────────┘"},
            {"┌─────────┐","│  O   O  │","│         │","│  O   O  │","└─────────┘"},
            {"┌─────────┐","│  O   O  │","│    O    │","│  O   O  │","└─────────┘"},
            {"┌─────────┐","│  O   O  │","│  O   O  │","│  O   O  │","└─────────┘"}
        };
        
        for(int i = 0; i<N ; i++){
            //Boucle pour les lignes à afficher
            chaine = "           ";
            for(int j  = 0; j<N ; j++){
                //Pour chaque dé
                chaine += deAscii[TabDes[j]-1,i];
            }
            Console.WriteLine(chaine);
        }
    }

    static void AfficheTabChallenges(Joueur j, ChallengesStruct[] ListeChallenge, int tour, int[] TabDes){
        /* Procédure pour afficher le tableau des challenges disponibles. */
        Console.WriteLine("┌───────────────┬──────────┬──────────────────────────────────────────────────┬───────────────────────────────────┐");
        int n = ListeChallenge.Length;
        for(int i=0; i<n; i++){
            ChallengesStruct challenge = ListeChallenge[i];
            // Si le challenge est disponible
            if(!challenge.dejaJouer[j.id-1]){
                // Si le challenge est le dernier à afficher
                bool dernier = false;
                if(i == ListeChallenge.Length - tour){
                    dernier = true;
                }
                AfficheChallenge(challenge, dernier, TabDes);
            }
        }
    }

    static void AfficheChallenge(ChallengesStruct challenge, bool dernier, int[] TabDes){
        /* Procédure affichant les informations du challenge lors du choix. */
        // Caractères ASCII: ├ ┼ ┤ └ ┘ ┴ ┬ ─ │
        string str_id = ((challenge.id).ToString()).PadRight(3);
        string str_nom = ((challenge.nom).ToString()).PadRight(8);
        string str_desc = ((challenge.desc).ToString()).PadRight(48);
        string str_points = ((challenge.points).ToString()).PadRight(28);
        // Afficher l'apperçu des points à recevoir pour le challenge en fonction des dés obtenus
        str_points += " (" + MajScore(challenge.id, TabDes).ToString() + ")";
        str_points = str_points.PadRight(33);
        // Affichage du challenge dans la grille
        Console.WriteLine("│ Challenge {0} │ {1} │ {2} │ {3} │", str_id, str_nom, str_desc, str_points);
        if(dernier){
            Console.WriteLine("└───────────────┴──────────┴──────────────────────────────────────────────────┴───────────────────────────────────┘");
        }
        else{
            Console.WriteLine("├───────────────┼──────────┼──────────────────────────────────────────────────┼───────────────────────────────────┤");
        }
    }

    /*Fonction et Procédure pour le jeu*/

    static int DemanderRelance(){
        /* Fonction retournant le nombre de dés à relancer saisi par un joueur. */
        //Demander combien de dés on souhaite relancer
        int n;
        do{
            Console.WriteLine("Combien de des souhaitez-vous relancer ? (0-5)");
            try{
                n = int.Parse(Console.ReadLine());
                if (n < 0 || n > 5){
                    throw new OverflowException();
                }
            }
            catch (FormatException){
                Console.WriteLine("Erreur : Veuillez entrer un nombre entier.");
                n = -1; // Permet de continuer la boucle en cas d'erreur
            }
            catch (OverflowException){
                Console.WriteLine("Veuillez entrer un nombre entre 0 et 5.");
                n = -1; // Permet de continuer la boucle en cas d'erreur
            }
            
        }while((n < 0) || (n > 5));
        return n;
    }

    static void DemanderAffichage(Joueur j, ChallengesStruct[] ListeChallenge, int tour, int[] TabDes){
        /* Procédure pour demander au joueur s'il souhaite consulter les challenges qui lui restent. */
        string rep = "";
        do{
            Console.WriteLine("Souhaitez-vous afficher les challenges restants ? ('oui' ou 'non')");
            try{
                rep = Console.ReadLine();
            }
            catch (OverflowException){
                Console.WriteLine("Veuillez entrer un nombre entre 0 et 5.");
                rep = ""; // Permet de continuer la boucle en cas d'erreur
            }
        }while(rep != "oui" && rep != "non");
        if(rep == "oui"){
            Console.Clear();
            AfficheTabChallenges(j, ListeChallenge, tour, TabDes);
            Console.WriteLine();
            AfficherDesAscii(TabDes);
        }
    }

    static void Lancer(int[] Tab){
        /* Procédure retournant un tableau correspondant au résultat de lancés de dés. */
        //Mettre à jour la valeur de chaque dés
        Random rnd = new Random();
        for(int i=0; i<Tab.Length; i++){
            Tab[i] = rnd.Next(1,7);
        }
    }

    static void Relance(int[] TabDes, int n){
        // Si on demande de relancer tous les dés --> Pas besoin de demander quels dés relancer
        if(n == 5){
            Lancer(TabDes);
        }
        else{
            /* Procédure relancant n dés choisis par le joueur */
            //Obtenir un tableau de n dés lancés aléatoirement
            int[] tab = new int[n];
            Lancer(tab);
            //Obtenir le tableau des indices de dés à relancer
            int[] TabRelance = new int[n];
            //Demander quels dés on souhaite relancer
            for(int i=0; i<n; i++){
                int d;
                do{
                    Console.WriteLine("Quels des (entre 1-5) souhaitez-vous relancer ({0}/{1})?", i+1,n);
                    try{
                        d = int.Parse(Console.ReadLine());
                        if (d < 1 || d > 5){
                            throw new OverflowException();
                        }
                    }
                    catch (FormatException){
                        Console.WriteLine("Erreur : Veuillez entrer un nombre entier.");
                        d = -1; // Permet de continuer la boucle en cas d'erreur
                    }
                    catch (OverflowException){
                        Console.WriteLine("Veuillez entrer un nombre entre 1 et 5.");
                        d = -1; // Permet de continuer la boucle en cas d'erreur
                    }
                }while((d<1) || (d>5) || RechercheIntTab(TabRelance, d));
                TabRelance[i] = d;
                //Affecter à chaque dé de TabDes d'indice présent dans TabRelance sa valeur aléatoire respective dans tab
                TabDes[d-1] = tab[i];
            }
        }
    }

    static void Jouer(Joueur j, ChallengesStruct[] ListeChallenge, int tour, int[] TabDes){
        /*Procédure qui lance les dés et/ou les relance.*/
        //Lancer les dés
        Lancer(TabDes);
        //Affichage des dés
        AfficherDesAscii(TabDes);
        //Demander le nombre de dés à relancer
        DemanderAffichage(j, ListeChallenge, tour, TabDes);
        int nbRelance = DemanderRelance();
        if(nbRelance > 0){
            //Relancer le nombre de dés souhaité
            Relance(TabDes, nbRelance);
            //Affichage des dés
            AfficherDesAscii(TabDes);
            //Répéter depuis la 2ème étape
            DemanderAffichage(j, ListeChallenge, tour, TabDes);
            nbRelance = DemanderRelance();
            if (nbRelance > 0){
                //Relancer le nombre de dés souhaité
                Relance(TabDes, nbRelance);
                //Affichage des dés
                AfficherDesAscii(TabDes);
            }
        }
    }

    static int ChoisirChallenge(int[] TabDes, ref Joueur j, ChallengesStruct[] ListeChallenge, int tour){
        /*
        Il faut permettre au joueur de choisir parmis la liste de tous les
        challenges QUI SONT DISPONIBLES.
        */
        int[] ChallengesDispos = new int[13];
        int i = 0;
        // Affichage de la grille des challenges
        Console.WriteLine("┌───────────────┬──────────┬──────────────────────────────────────────────────┬───────────────────────────────────┐");
        foreach(ChallengesStruct challenge in ListeChallenge){
            // Si le challenge est disponible
            if(!challenge.dejaJouer[j.id-1]){
                // Si le challenge est le dernier à afficher
                bool dernier = false;
                if(i == ListeChallenge.Length - tour){
                    dernier = true;
                }
                AfficheChallenge(challenge, dernier, TabDes);
                // Ajouter le challenge au tableau des challenges disponibles
                ChallengesDispos[i] = challenge.id;
                i++;
            }
        }
        // Choix du challenge
        int challenge_choisi;
        do{
            Console.WriteLine("Quel challenge souhaitez-vous choisir ? (entre 1-13)");
            try{
                challenge_choisi = int.Parse(Console.ReadLine());
                if (challenge_choisi < 1 || challenge_choisi > 13){
                    throw new OverflowException();
                }
            }
            catch (FormatException){
                Console.WriteLine("Erreur : Veuillez entrer un nombre entier.");
                challenge_choisi = -1; // Permet de continuer la boucle en cas d'erreur
            }
            catch (OverflowException){
                Console.WriteLine("Veuillez entrer un nombre entre 1 et 13.");
                challenge_choisi = -1; // Permet de continuer la boucle en cas d'erreur
            }
        }while(challenge_choisi < 1 || challenge_choisi > 13 || !RechercheIntTab(ChallengesDispos, challenge_choisi));
        Console.WriteLine("Vous avez choisi le challenge {0}", challenge_choisi);
        // Mettre à jour le challenge choisi
        ListeChallenge[challenge_choisi-1].dejaJouer[j.id-1] = true;
        // Calcul du score obtenu
        int points = MajScore(challenge_choisi, TabDes);
        
        // Ajout du tableau des dés mis en forme dans le tableau des historiques de lancers pour ce tour
        j.TabLances[tour-1] = TabIntToString(TabDes);
        // Ajout du challenge choisi dans le tableau des historiques de challenge pour ce tour
        j.TabChallenges[tour-1] = ListeChallenge[challenge_choisi-1].nom;

        // Ajout du score princiaple 
        j.score += points;

        // Ajout du score mineur
        if(challenge_choisi >= 1 && challenge_choisi <= 6){ 
            j.scoreMineur += points;
        }
        
        Console.WriteLine("Points obtenus pour ce tour par {0}: {1}", j.pseudo, points);
        if(j.scoreMineur >= 63){
            Console.WriteLine("{0} a obtenu le bonus !", j.pseudo);
        }
        else{
            Console.WriteLine("Progression du bonus : {0}/63 points", j.scoreMineur);
        }
        Console.WriteLine("Continuer ?");
        Console.Read();
        Console.Clear();

        return points;
    }

    static int MajScore(int challenge, int[] TabDes){
        /* Procédure mettant à jour le score du joueur en fonction du challenge choisi et des dés lancés. */
        // Précondition : challenge compris entre 1 et 13
        int points = 0;
        switch(challenge){
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
            case 6:
                points = ChallengesMineurs(TabDes, challenge);
                break;
            case 7:
                points = CMajBrelan(TabDes);
                break;
            case 8:
                points = CMajCarre(TabDes);
                break;
            case 9:
                points = CMajFull(TabDes);
                break;
            case 10:
                points = CMajPtSuite(TabDes);
                break;
            case 11:
                points = CMajGdSuite(TabDes);
                break;
            case 12:
                points = CMajYams(TabDes);
                break;
            case 13:
                points = CMajChance(TabDes);
                break;
        }
        return points;
    }

    /*Fonction calculant le score pour les challenges*/
    static int ChallengesMineurs(int[] TabDes, int N){
        /* Fonction qui renvoie le score pour tous les challenges mineurs en fonction des dés lancés. */
        int sum = 0;
        foreach(int val in TabDes){
            if(val == N){
                sum += N;
            }
        }
        return sum;
    }

    static int CMajBrelan(int[] TabDes){
        /* Fonction qui renvoie le score pour tous les challenges majeurs Brelan. */
        int score = 0;
        for(int i = 1; i<=6; i++){
            if(nbIndentiqueTab(TabDes, i) >= 3){
                score = i * 3;
            }
        }
        return score;
    }

    static int CMajCarre(int[] TabDes){
        /* Fonction qui renvoie le score pour tous les challenges majeurs Carré. */
        int score = 0;
        for(int i = 1; i<=6; i++){
            if(nbIndentiqueTab(TabDes, i) >= 4){
                score = i * 4;
            }
        }
        return score;
    }

    static int CMajFull(int[] TabDes){
        /* Fonction qui renvoie le score pour tous les challenges majeurs Full. */
        int score = 0;
        for(int i = 1; i<=6; i++){
            if(nbIndentiqueTab(TabDes, i) == 3){
                for(int j = 1; j<=6; j++){
                    if(nbIndentiqueTab(TabDes, j) == 2){
                        score = 25;
                    }
                }
            }
        }
        return score;
    }
 
    static int CMajPtSuite(int[] TabDes){
        /* Fonction qui renvoie le score pour le challenge majeur Petite Suite. */
        int score = 0;
        int comp = 1;
        TriTab(TabDes);
        for(int i = 0; i<TabDes.Length-1 ;i++){
            if(TabDes[i]+1 == TabDes[i+1]){  
                comp += 1;
            }else{
                if(TabDes[i] != TabDes[i+1]){
                    comp = 1;
                }
            }
        }  
        if(comp >= 4){
            score = 30;
        }   
        return score;   
    }

    static int CMajGdSuite(int[] TabDes){
        /* Fonction qui renvoie le score pour le challenge majeur Grande Suite. */
        int score = 0;
        int comp = 1;
        TriTab(TabDes);
        for(int i = 0; i<TabDes.Length-1 ;i++){
            if(TabDes[i]+1 == TabDes[i+1]){  
                comp += 1;
            }else{
                if(TabDes[i] != TabDes[i+1]){
                    comp = 1;
                }
            }
        }  
        if(comp >= 5){
            score = 40;
        }   
        return score;   
    }

    static int CMajYams(int[] TabDes){
        /* Fonction qui renvoie le score pour le challenge majeur Yams. */
        int score = 50;
        int val = TabDes[0];
        foreach(int valDes in TabDes){
            if(val != valDes){
                score = 0;
            }
        }   
        return score;     
    }

    static int CMajChance(int[] TabDes){
        /* Fonction qui renvoie le score pour le challenge majeur Chance. */
        int sum = 0;
        foreach(int val in TabDes){
                sum += val;
        }
        return sum;
    }
    /*Fonction utilitaire*/
    static void TriTab(int[] tableau){
        /* Procédure qui trie un tableau en suivant la méthode du trie à bulle. */
        int taille = 5;
        for (int i = 0; i < taille - 1; i++){
            for (int j = 0; j < taille - i - 1; j++){
                if (tableau[j] > tableau[j + 1]){
                    int temp = tableau[j];
                    tableau[j] = tableau[j + 1];
                    tableau[j + 1] = temp;
                }
            }
        }
    }

    static int nbIndentiqueTab(int[] TabDes, int N){
        /* Fonction qui renvoie le nombre d'occurences de n dans le tableau. */
        int nbIndentique = 0;
        foreach(int val in TabDes){
            if(val == N){
                nbIndentique++;
            }
        }
        return nbIndentique;
    }

    static string TabIntToString(int[] Tab){
        /* Fonction retournant une chaine de caractère représentant un tableau. */
        int N = Tab.Length;
        string TabString = "[";
        for(int i = 0; i<N;i++){
            TabString += Tab[i].ToString();
            if(i != N-1) TabString += ",";
        } 
        TabString += "]";
        return TabString;
    }

    static bool RechercheIntTab(int[] tab, int x){
        /* 
        Fonction qui recherche si un entier est présent ou non dans un tableau à 
        une dimension et renvoie le booléen correspondant.
        */
        bool trouve = false;
        int i = 0;
        int N = tab.Length;
        while(i<N && !trouve){
            if(tab[i] == x){
                trouve = true;
            }
            i++;
        }
        return trouve;
    }

    /*Procédures pour généré le fichier JSON*/
    static void RemplirTourJSON(StreamWriter LeFichier, int tour, Joueur j){
        /* Procédure permettant de remplir la partie "rounds" du fichier JSON. */
        int idx = tour-1;
        int id = j.id;
        // Remplissage du fichier
        if(id == 1){
            LeFichier.WriteLine("\t  {");
        LeFichier.WriteLine("\t\t\"id\": {0},", tour);
        LeFichier.WriteLine("\t\t\"results\": [");
        }
        LeFichier.WriteLine("\t\t  {");
        LeFichier.WriteLine("\t\t\t\"id_player\": \"{0}\",", id);
        LeFichier.WriteLine("\t\t\t\"dice\": {0},", j.TabLances[idx]);
        LeFichier.WriteLine("\t\t\t\"challenge\": \"{0}\",", j.TabChallenges[idx]);
        LeFichier.WriteLine("\t\t\t\"score\": {0}", j.TabScores[idx]);
        if(id == 2){
            LeFichier.WriteLine("\t\t  }");
            LeFichier.WriteLine("\t\t]");
            if(tour == 13){
                LeFichier.WriteLine("\t  }");
            }
            else{
                LeFichier.WriteLine("\t  },");
            }
        }
        else{
            LeFichier.WriteLine("\t\t  },");
        }
    }
    
    static void CreerJSON(Joueur j1, Joueur j2){
        /* Procédure permettant de créer et remplir le fichier JSON. */
        // Obtenir la date courante
        DateTime DateCourante = DateTime.Now;
        // Formatage de la date
        string date = DateCourante.ToString("yyyy-MM-dd");
        // Formatage des heures-minutes
        string heure = DateCourante.ToString("HH-mm");
        // Création du fichier
        FileStream fs = new FileStream("Yams_" + date + "_" + heure + ".json", FileMode.Create, FileAccess.Write);
        StreamWriter LeFichier = new StreamWriter(fs);

        Joueur[] tabJoueurs = new Joueur[2]{j1,j2};
        
        // Remplissage du fichier
        LeFichier.WriteLine("{");
        LeFichier.WriteLine("\t\"parameters\": {");
        LeFichier.WriteLine("\t  \"code\": \"groupe7-001\",");
        LeFichier.WriteLine("\t  \"date\": \"{0}\"", date);
        LeFichier.WriteLine("\t},");
        LeFichier.WriteLine("\t\"players\": [");
        LeFichier.WriteLine("\t  {");
        LeFichier.WriteLine("\t    \"id\": 1,");
        LeFichier.WriteLine("\t    \"pseudo\": \"{0}\"", j1.pseudo);
        LeFichier.WriteLine("\t  },");
        LeFichier.WriteLine("\t  {");
        LeFichier.WriteLine("\t    \"id\": 2,");
        LeFichier.WriteLine("\t    \"pseudo\": \"{0}\"", j2.pseudo);
        LeFichier.WriteLine("\t  }");
        LeFichier.WriteLine("\t],");
        LeFichier.WriteLine("\t\"rounds\": [");
        for(int i=0; i<26; i++){
            RemplirTourJSON(LeFichier, (i/2)+1, tabJoueurs[i%2]);
        }
        LeFichier.WriteLine("\t],");
        LeFichier.WriteLine("\t\"final_result\": [");
        LeFichier.WriteLine("\t  {");
        LeFichier.WriteLine("\t\t\"id_player\": 1,");
        LeFichier.WriteLine("\t\t\"bonus\": {0},", j1.bonus);
        LeFichier.WriteLine("\t\t\"score\": {0}", j1.score);
        LeFichier.WriteLine("\t  },");
        LeFichier.WriteLine("\t  {");
        LeFichier.WriteLine("\t\t\"id_player\": 2,");
        LeFichier.WriteLine("\t\t\"bonus\": {0},", j2.bonus);
        LeFichier.WriteLine("\t\t\"score\": {0}", j2.score);
        LeFichier.WriteLine("\t  }");
        LeFichier.WriteLine("\t]");
        LeFichier.WriteLine("}");

        LeFichier.Close();
    }

    /*Procédure principale*/
    static void Main(){
        /* Procédure principale assurant tout le déroulement de la partie de Yams. */
        const int NB_DES = 5;
        Joueur j1 = new Joueur(1);
        Joueur j2 = new Joueur(2);
        int tour = 1; //Tour 1/13
        int[] TabDes = new int[NB_DES]; // Tableau contenant 5 entiers correspondants aux valeurs des dés (entre 1 et 6 inclus)
        
        ChallengesStruct[] ListeChallenge = CreateChallenge();

        // Déroulement principal de la partie
        Console.Clear(); // Nettoyer (vider) la console
        for(tour = 1; tour<=13; tour++){
            Console.WriteLine("Tour {0}/13", tour);
            // Tour joueur 1
            Console.WriteLine();
            Console.WriteLine("‹─────────────────────────────── Joueur 1 - {0} ───────────────────────────────›", j1.pseudo);
            Console.WriteLine();
            Jouer(j1, ListeChallenge, tour, TabDes);
            j1.TabScores[tour-1] = ChoisirChallenge(TabDes, ref j1, ListeChallenge, tour);
            // Tour joueur 2
            Console.WriteLine();
            Console.WriteLine("‹─────────────────────────────── Joueur 2 - {0} ───────────────────────────────›", j2.pseudo);
            Console.WriteLine();
            Jouer(j2, ListeChallenge, tour, TabDes);
            j2.TabScores[tour-1] = ChoisirChallenge(TabDes, ref j2, ListeChallenge, tour);
        }

        // Afficher vainqueur
        if(j1.score == j2.score){
            Console.WriteLine("Les deux joueurs sont à égalité avec un score de {0} points !", j1.score);
        }
        else if(j1.score > j2.score){
            Console.WriteLine("Le vainqueur est {0} avec un score de {1} points !", j1.pseudo, j1.score);
            Console.WriteLine("{0} a obtenu {1} points", j2.pseudo, j2.score);
        }else{
            Console.WriteLine("Le vainqueur est {0} avec un score de {1} points !", j2.pseudo, j2.score);
            Console.WriteLine("{0} a obtenu {1} points", j1.pseudo, j1.score);
        }
        // Création + Remplissage du fichier JSON
        CreerJSON(j1, j2);
    }
}
