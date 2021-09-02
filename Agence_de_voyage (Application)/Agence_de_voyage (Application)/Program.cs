using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Agence_de_voyage__Application_
{
    public class Msg
    {
        public string msg { get; set; }
        public int code { get; set; }

        public Msg()
        {
            this.msg = "";
            this.code = 0;
        }
        public Msg(string msg, int code)
        {
            this.msg = msg;
            this.code = code;
        }
    }
    public class Client
    {
        public string nom { get; set; }
        public string prenom { get; set; }
        public string carteBancaire { get; set; }

        public Client()
        {
            this.nom = "";
            this.prenom = "";
            this.carteBancaire = "";
        }

        public Client(string nom, string prenom, string carteBancaire)
        {
            this.nom = nom;
            this.prenom = prenom;
            this.carteBancaire = carteBancaire;
        }

        public override string ToString()
        {
            return "Nom : " + this.nom + "\nPrénom : " + this.prenom + "\nCarte bancaire : " + this.carteBancaire;
        }
    }
    public class Reservation
    {
        public Client client { get; set; }
        public string idAgence { get; set; }
        public string idReservation { get; set; }
        public string nbPersonne { get; set; }
        public double nbNuit { get; set; }
        public string recapitulatif { get; set; }
    }
    public class Agence
    {
        public string id { get; set; }
        public string nom { get; set; }
        public string password { get; set; }

        public Agence()
        {
            this.id = "";
            this.nom = "";
            this.password = "";
        }

        public Agence(string id, string nom, string password)
        {
            this.id = id;
            this.nom = nom;
            this.password = password;
        }
    }
    public class Hotel
    {
        public string id { get; set; }
        public string nom { get; set; }
        public Localisation localisation { get; set; }
        public string classement { get; set; }
        public string capacite { get; set; }
        public string prix { get; set; }
        public string infoHotel { get; set; }
        public string imgChambre { get; set; }
        public byte[] file { get; set; }

        public Hotel()
        {
            this.id = "";
            this.nom = "";
            this.localisation = new Localisation();
            this.classement = "";
            this.capacite = "";
            this.prix = "";
            this.file = new byte[] { 0 };
            this.imgChambre = "";
            this.infoHotel = this.ToString();
        }

        public override string ToString()
        {
            return "Identifiant : " + this.id + "\nNom : " + this.nom + "\nLocalisation : {\n" + this.localisation.ToString() + "\n}" + "\nClassement : " + this.classement + "\nCapacité : " + this.capacite + "\nPrix : " + this.prix;
        }
    }
    public class Localisation
    {
        public string pays { get; set; }
        public Adresse adresse { get; set; }

        public Localisation()
        {
            this.pays = "";
            this.adresse = new Adresse();
        }

        public override string ToString()
        {
            return "\tPays : " + this.pays + "\n\tAdresse : {\n" + this.adresse.ToString() + "\n\t}";
        }
    }
    public class Adresse
    {
        public string numero { get; set; }
        public string rue { get; set; }
        public Ville ville { get; set; }
        public string positionGPS { get; set; }

        public Adresse()
        {
            this.numero = "";
            this.rue = "";
            this.ville = new Ville();
            this.positionGPS = "";
        }

        public override string ToString()
        {
            return "\t\tNuméro : " + this.numero + "\n\t\tRue : " + this.rue + "\n\t\tVille : {\n" + this.ville.ToString() + "\n\t\t}" + "\n\t\tPosition GPS : " + this.positionGPS;
        }
    }
    public class Ville
    {
        public string nom { get; set; }
        public string codePostal { get; set; }

        public Ville()
        {
            this.nom = "";
            this.codePostal = "";
        }

        public override string ToString()
        {
            return "\t\t\tNom : " + this.nom + "\n\t\t\tCode postal : " + this.codePostal;
        }
    }

    class Program
    {
        static HttpClient client = new HttpClient();

        static async Task<Msg> GetMsgAsync(string path)
        {
            Msg msg = new Msg();

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                msg = await response.Content.ReadAsAsync<Msg>();
            }

            return msg;
        }
        static async Task<List<Hotel>> GetHotelsAsync(string path)
        {
            List<Hotel> hotel = new List<Hotel>();

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                hotel = await response.Content.ReadAsAsync<List<Hotel>>();
            }

            return hotel;
        }
        static async Task<Reservation> GetReservationAsync(string path)
        {
            Reservation res = new Reservation();

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                res = await response.Content.ReadAsAsync<Reservation>();
            }

            return res;
        }
        static async Task<string> GetStringAsync(string path)
        {
            string str = "";

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                str = await response.Content.ReadAsAsync<string>();
            }

            return str;
        }
        static async Task<Agence> GetAgenceAsync(string path)
        {
            Agence ag = new Agence();

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                ag = await response.Content.ReadAsAsync<Agence>();
            }

            return ag;
        }
        public class SystemeReservation
        {
            string url;

            Agence agenceConsultation;
            Agence agenceReservation;

            bool connecteConsultation;
            bool connecteReservation;

            string idReservation;
            string nbPersonne;
            double nbNuit;

            public SystemeReservation(string url)
            {
                this.url = url;

                agenceConsultation = new Agence();
                agenceReservation = new Agence();

                connecteConsultation = false;
                connecteReservation = false;

                idReservation = "";
                nbPersonne = "";
                nbNuit = 0;

                System.Console.WriteLine("[i] : Information");
                System.Console.WriteLine("[*] : Demande de saisie");
                System.Console.WriteLine("/!\\ : Arrêt du Système de Réservation");
            }

            public async Task<int> LoginConsultation(string idAgence, string motDePasse)
            {

               Msg response = new Msg();

                try
                {
                    response = await GetMsgAsync(url + "consultation/login/" + idAgence + "/" + motDePasse);
                    agenceConsultation = await GetAgenceAsync(url + "consultation/agence/" + idAgence + "/" + motDePasse);
                }
                catch
                {
                    System.Console.WriteLine("\n/!\\ Le Web Service doit être lancé avant l'application, fin du programme.");
                    response.code = 0;
                }

                if (response.code == 1)
                    connecteConsultation = true;

                System.Console.WriteLine("\n" + response.msg);

                return response.code;
            }

            public async Task<int> LoginReservation(string idAgence, string motDePasse)
            {
                Msg response = new Msg();

                if (!connecteConsultation)
                    return 0;

                response = await GetMsgAsync(url + "reservation/login/" + idAgence + "/" + motDePasse);
                agenceReservation = await GetAgenceAsync(url + "reservation/agence/" + idAgence + "/" + motDePasse);

                if (response.code == 1)
                    connecteReservation = true;

                System.Console.WriteLine("\n" + response.msg);

                return response.code;
            }

            public async Task<int> MenuConsultation()
            {
                if (!connecteConsultation)
                    return 0;

                CultureInfo culture = new CultureInfo("fr-FR");

                Msg response = new Msg();

                string cityUserInput = "";
                string startUserInput = "";
                string endUserInput = "";
                string priceUserInput = "";
                string nbrUserInput = "";
                string souhaiteReserver = "";

                System.Console.WriteLine("\n[i] Vous êtes connecté sur l'agence : " + agenceConsultation.nom + " (id : " + agenceConsultation.id + ")");

                System.Console.WriteLine("\n****************************");
                System.Console.WriteLine("*** CONSULTATION D'HÔTEL ***");
                System.Console.WriteLine("****************************");
                
                System.Console.Write("\n[i] Villes disponibles : ");

                System.Console.WriteLine(string.Join(", ", await GetStringAsync(url + "consultation/" + agenceConsultation.id + "/hotels")));

                System.Console.WriteLine("\n[*] Dans quelle Ville souhaitez-vous réserver un Hôtel ?");
                System.Console.Write("> ");

                cityUserInput = Console.ReadLine();
                
                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/1/" + cityUserInput)).code == 0)
                {
                    System.Console.WriteLine(response.msg);
                    connecteConsultation = false;
                    return response.code;
                }

                
                System.Console.WriteLine("\n[*] Quelle est la date d'arrivée ? (format : jj/mm/aaaa)");
                System.Console.Write("> ");
                startUserInput = Console.ReadLine();

                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/2/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_"))).code == 0)
                {
                    System.Console.WriteLine(response.msg);
                    connecteConsultation = false;
                    return response.code;
                }
                
                System.Console.WriteLine("\n[*] Quelle est la date de départ ? (format : jj/mm/aaaa)");
                System.Console.Write("> ");
                endUserInput = Console.ReadLine();

                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/3/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_"))).code == 0)
                {
                    System.Console.WriteLine(response.msg);
                    connecteConsultation = false;
                    return response.code;
                }
                
                nbNuit = (Convert.ToDateTime(endUserInput, new CultureInfo("fr-FR")) - Convert.ToDateTime(startUserInput)).TotalDays;
                
                System.Console.WriteLine("\n[i] Les prix appartiennent à l'intervalle suivant : " + await GetStringAsync(url + "consultation/" + agenceConsultation.id + "/prix"));
                System.Console.WriteLine("\n[*] Combien êtes-vous prêt à dépenser maximum ?");
                System.Console.Write("> ");
                priceUserInput = Console.ReadLine();

                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/4/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput)).code == 0)
                {
                    System.Console.WriteLine(response.msg);
                    connecteConsultation = false;
                    return response.code;
                }

                
                System.Console.WriteLine("\n[i] Tous les Hôtels de notre Agence sont 5 étoiles");

                System.Console.WriteLine("\n[*] Combien de personnes maximum faut-il héberger ?");
                System.Console.Write("> ");
                nbrUserInput = Console.ReadLine();

                
                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/5/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput)).code == 0)
                {
                    System.Console.WriteLine(response.msg);
                    connecteConsultation = false;
                    return response.code;
                }


                if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/hotels/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput)).code >= 1)
                    System.Console.WriteLine(response.msg);

                
                List<Hotel> reservationUtilisateur = await GetHotelsAsync(url + "consultation/" + agenceConsultation.id + "/getHotels/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput);

                
                foreach (Hotel hotel in reservationUtilisateur)
                {
                    System.Console.WriteLine("\n" + hotel.ToString());

                    System.Console.WriteLine("\n[*] Souhaitez-vous réserver cet Hôtel ? (Oui/Non)");
                    System.Console.Write("> ");
                    souhaiteReserver = Console.ReadLine();

                    if ((response = await GetMsgAsync(url + "consultation/" + agenceConsultation.id + "/checking/6/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput + "/" + souhaiteReserver)).code == 0)
                    {
                        System.Console.WriteLine(response.msg);
                        connecteConsultation = false;
                        return response.code;
                    }

                    if (souhaiteReserver.Equals("Oui"))
                    {
                        idReservation = hotel.id;
                        nbPersonne = nbrUserInput;
                        System.Console.WriteLine("\n[i] Vous avez souhaité réserver cet Hôtel. Une image de votre future chambre va être affiché...");
                        Thread.Sleep(1000);
                        string filePath = "room.jpg";

                        try
                        {
                            File.WriteAllBytes(filePath, Convert.FromBase64String(hotel.imgChambre));
                            System.Diagnostics.Process.Start(filePath);
                        }
                        catch (Exception e)
                        {
                            System.Console.WriteLine(e.StackTrace);
                        }

                        return 1;
                    } 
                }
                

                System.Console.WriteLine("/!\\ Vous avez décidé de ne pas réserver d'Hôtel, fin de la réservation.");

                connecteConsultation = false;

                return 0;
            }

            public async Task<int> MenuReservation()
            {
                if (!connecteReservation)
                    return 0;

                string lastname = "";
                string firstname = "";
                string creditCard = "";

                System.Console.WriteLine("\n[i] Vous êtes connecté sur l'agence : " + agenceReservation.nom + " (id : " + agenceReservation.id + ")");

                System.Console.WriteLine("\n***************************");
                System.Console.WriteLine("*** RÉSERVATION D'HÔTEL ***");
                System.Console.WriteLine("***************************");

                System.Console.WriteLine("\n[i] Vous avez choisi de réserver l'offre d'identifiant : " + idReservation);

                System.Console.WriteLine("\n[*] Veuillez saisir votre Nom : ");
                System.Console.Write("> ");
                lastname = Console.ReadLine();

                lastname = Regex.Replace(lastname, @"/", "_");

                System.Console.WriteLine("\n[*] Veuillez saisir votre Prénom : ");
                System.Console.Write("> ");
                firstname = Console.ReadLine();

                firstname = Regex.Replace(firstname, @"/", "_");

                System.Console.WriteLine("\n[*] Veuillez saisir votre Numéro de Carte Bleu : ");
                System.Console.Write("> ");
                creditCard = Console.ReadLine();

                creditCard = Regex.Replace(creditCard, @"/", "_");

                System.Console.WriteLine("\n[i] La Réservation a bien eu lieu !");

                Reservation res = await GetReservationAsync(url + "reservation/traitement/" + agenceReservation.id + "/" + lastname + "/" + firstname + "/" + creditCard + "/" + idReservation + "/" + nbPersonne + "/" + nbNuit);

                System.Console.WriteLine(res.recapitulatif);

                System.Console.WriteLine("\n[i] Notre Agence vous remercie pour votre confiance, à bientôt !");

                return 1;
            }
        }

        static void Main(string[] args)
        {
            SystemeReservation sys = new SystemeReservation("https://localhost:44340/");

            /* Connexion au Web Service Consultation */
            sys.LoginConsultation("@identifiant1", "agence34").GetAwaiter().GetResult();

            sys.MenuConsultation().GetAwaiter().GetResult();

            /* Connexion au Web Service Réservation */
            sys.LoginReservation("@identifiant1", "agence34").GetAwaiter().GetResult();

            sys.MenuReservation().GetAwaiter().GetResult();

        }
    }
}
