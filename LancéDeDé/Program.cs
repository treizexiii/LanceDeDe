using System;
using System.Security.Cryptography.X509Certificates;

namespace LancéDeDé
{
    class Program
    {
        private static Random random = new Random();
        
        static void Main(string[] args)
        {
            AfficherMenu(); //on appelle la méthode correspondant au menu
            //on verrouille toute les touche autre que le chiffre 1 & 2 du clavier numérique et alphabéthique
            ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
            while(consoleKeyInfo.Key != ConsoleKey.D1 && consoleKeyInfo.Key != ConsoleKey.D2 && consoleKeyInfo.Key != ConsoleKey.NumPad1 && consoleKeyInfo.Key != ConsoleKey.NumPad2)
            {
                AfficherMenu();
                consoleKeyInfo = Console.ReadKey(true);
            }

            //Selon le choix effectuer on lance le je 1 ou 2 
            if(consoleKeyInfo.Key == ConsoleKey.D1 || consoleKeyInfo.Key == ConsoleKey.NumPad1)
            {
                Game1();
            }
            else
            {
                Game2();
            }
        }

        private static void AfficherMenu()//on affiche un menu proposant le jeu un ou 2
        {
            Console.Clear();
            Console.WriteLine("Veuillez choisir votre mode de jeu : ");
            Console.WriteLine("\t1 : Combattre des monstres");
            Console.WriteLine("\t2 : Combattre un boss");
        }

        private static void Game1()
        {
            Joueur player = new Joueur(150);//on initialise un joueur avec 150PV
            //on place les compteurs de point à 0
            int cptFacile = 0;
            int cptDifficile = 0;

            while(player.EstVivant)
            {
                MonstreFacile monstre = FabriqueDeMonstre();// si le joueur est vivant, on fabrique un monstre

                //Tant que le joueur ou le monstre sont vivants, on attaque a tour de role
                while(monstre.EstVivant && player.EstVivant)
                {
                    player.Attaque(monstre);
                    if (monstre.EstVivant)
                    {
                        monstre.Attaque(player);
                    }
                }

                //Si le joueur est tjrs vivant à la fin de la rencontre,
                //on incrémente les compteurs selon le monstre tiré
                if (player.EstVivant)
                {
                    if (monstre is MonstreDifficile)
                    {
                        cptDifficile++;
                    }
                    else
                    {
                        cptFacile++;
                    }
                }
                else
                {
                    Console.WriteLine("Bouh, vous est mort... ");
                    break;
                }
            }

            //Si le joueur et mort, on calcule les points cumulés
            Console.WriteLine("Bravo !!! Vous avez tué {0} monstres faciles et {1} monstres difficles." +
                "Vous avez {2} points", cptFacile, cptDifficile, cptFacile + cptDifficile * 2);
        }

        private static void Game2()
        {
            //on initialise le joueur et le boss avec leur PV respectifs
            Joueur player = new Joueur(150);
            Boss boss = new Boss(250);

            //Tant qu'ils sont vivant on attaque à tour de rôle, jusqu'à la mort de l'un d'eux
            while(player.EstVivant && boss.EstVivant)
            {
                player.Attaque(boss);
                if(boss.EstVivant)
                {
                    boss.Attaque(player);
                }
            }
            if(player.EstVivant)//Si le Boss meurt, victoire
            {
                Console.WriteLine("Bravo, vous avez vaincu le boss!!! ");
            }
            else//Si le joueur meurt, défaite
            {
                Console.WriteLine("Game Over... ");
            }
        }

        private static MonstreFacile FabriqueDeMonstre()
        {
            int pileFace = random.Next(2); //on fait un tirage à pile ou face pour le choix du monstre

            if ( pileFace == 1)
            {
                return new MonstreFacile();
            }
            else
            {
                return new MonstreDifficile();
            }
        }        
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>

    public static class De
    {
        private static Random random  = new Random();// on appelle la méthode de tirage de chiffre
        
        public static int LanceDe()//On effectue un lancé de dé classique à 6 faces
        {
            return random.Next(1, 7);
        }

        public static int LanceDe(int nouveauDe)//On effecteur un lancé de dé avec un dé dépendant de la valeur donnée
        {
            return random.Next(1, nouveauDe);
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>

    public class Joueur
    {        
        public int PtsDeVies { get; private set; }//Les PV restent privés à la class, il sont décomptés ici
        public bool EstVivant
        {
            get { return PtsDeVies > 0; }
        }

        public Joueur(int points)// on initialise un joueur avec le nombre de PV appeler en valeur
        {
            PtsDeVies = points;           
        }

        public void Attaque(MonstreFacile monstre)// méthode pour l'attaque d'un monstre
        {
            int lanceJoueur = LanceDe();
            int lanceMonstre = monstre.LanceDe();
            if (lanceJoueur > lanceMonstre)
            {
                monstre.SubitDegats();
            }
        }

        public void Attaque(Boss boss)// méthode pour l'attque d'un boss
        {
            int nbPoints = LanceDe(26);// on initialise les dégats effectué avec un lancé de dé
            boss.SubitDegats(nbPoints);
        }
        
        public int LanceDe()//on effectue un lancé de D6
        {
            return De.LanceDe();
        }

        public int LanceDe(int valeur)// on effectue un lancé de dé selon la valeur
        {
            return De.LanceDe(valeur);
        }

        public void SubitDegats(int degats)// on fait un test de bouclier afin de vérifier si le joueur prend des dégats
        {
            if (!BouclierFonctionne())
            {
                PtsDeVies -= degats;
            }
        }

        private bool BouclierFonctionne()//Test pile-face pour l'utlisation du bouclier
        {
            return De.LanceDe() <= 2;
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>

    public class Boss
    {
        public int ptsDeVie { get; private set; }
        public bool EstVivant
        {
            get { return ptsDeVie > 0; }
        }

        public Boss(int points)
        {
            ptsDeVie = points;
        }

        public void Attaque (Joueur personnage)
        {
            int nbPoint = LanceDe(26);
            personnage.SubitDegats(nbPoint);
        }

        public void SubitDegats(int valeur)// on fait subir au boss des dégats du montant de la valeur appeler
        {
            ptsDeVie -= valeur;
        }

        private int LanceDe(int valeur)//on appelle la méthode de lancé de dés en privé pour le boss
        {
            return De.LanceDe(valeur);
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>

    public class MonstreFacile
    {
        private const int degats = 10;        
        public bool EstVivant { get; private set; }

        public MonstreFacile()
        {            
            EstVivant = true;
        }

        public virtual void Attaque (Joueur joueur)
        {
            int lanceMonstre = LanceDe();
            int lanceJoueur = joueur.LanceDe();
            if(lanceMonstre>lanceJoueur)
            {
                joueur.SubitDegats(degats);
            }            
        }

        public void SubitDegats()
        {
            EstVivant = false;
        }

        public int LanceDe()
        {
            return De.LanceDe();
        }
    }

    /// <summary>
    /// //////////////////////////////////////////////////////////////////////
    /// </summary>

    class MonstreDifficile : MonstreFacile
    {
        //Pour les monstres difficiles on reprend la class monstre facile avec les même méthodes
        //on y ajoute uniquement les méthodes suplémentaires
        public const int degatsSort = 5;

        public override void Attaque(Joueur joueur)
        {
            base.Attaque(joueur);
            joueur.SubitDegats(SortMagique());
        }

        private int SortMagique()
        {
            int valeur = De.LanceDe();
            if(valeur == 6)
            {
                return 0;
            }return degatsSort * valeur;
        }
    }

    
}
