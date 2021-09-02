using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Comparateur__Application_
{
    public class PrintImage
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr CreateFile(
            string lpFileName,
            int dwDesiredAccess,
            int dwShareMode,
            IntPtr lpSecurityAttributes,
            int dwCreationDisposition,
            int dwFlagsAndAttributes,
            IntPtr hTemplateFile);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetCurrentConsoleFont(
            IntPtr hConsoleOutput,
            bool bMaximumWindow,
            [Out][MarshalAs(UnmanagedType.LPStruct)] ConsoleFontInfo lpConsoleCurrentFont);

        [StructLayout(LayoutKind.Sequential)]
        internal class ConsoleFontInfo
        {
            internal int nFont;
            internal Coord dwFontSize;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct Coord
        {
            [FieldOffset(0)]
            internal short X;
            [FieldOffset(2)]
            internal short Y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        private const int GENERIC_READ = unchecked((int)0x80000000);
        private const int GENERIC_WRITE = 0x40000000;
        private const int FILE_SHARE_READ = 1;
        private const int FILE_SHARE_WRITE = 2;
        private const int INVALID_HANDLE_VALUE = -1;
        private const int OPEN_EXISTING = 3;

        public static Size GetConsoleFontSize()
        {
            // getting the console out buffer handle
            IntPtr outHandle = CreateFile("CONOUT$", GENERIC_READ | GENERIC_WRITE,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                IntPtr.Zero,
                OPEN_EXISTING,
                0,
                IntPtr.Zero);
            int errorCode = Marshal.GetLastWin32Error();
            if (outHandle.ToInt32() == INVALID_HANDLE_VALUE)
            {
                throw new IOException("Unable to open CONOUT$", errorCode);
            }

            ConsoleFontInfo cfi = new ConsoleFontInfo();
            if (!GetCurrentConsoleFont(outHandle, false, cfi))
            {
                throw new InvalidOperationException("Unable to get font information.");
            }

            return new Size(cfi.dwFontSize.X, cfi.dwFontSize.Y);
        }
        public static void PrintImageInConsole(string filePath, Point position)
        {
            Point location = position;
            Size imageSize = new Size(40, 10);

            using (Graphics g = Graphics.FromHwnd(GetConsoleWindow()))
            {
                using (Image image = Image.FromFile(filePath))
                {
                    Size fontSize = GetConsoleFontSize();

                    Rectangle imageRect = new Rectangle(
                        location.X * fontSize.Width,
                        location.Y * fontSize.Height,
                        imageSize.Width * fontSize.Width,
                        imageSize.Height * fontSize.Height);
                    g.DrawImage(image, imageRect);
                }
            }
        }
        public static Point getCursorPosition()
        {
            Point defPnt = new Point();
            GetCursorPos(ref defPnt);

            defPnt.X = 1;

            return defPnt; 
        }
        public static void saveImage(string fileName, string string64)
        {
            File.WriteAllBytes(fileName, Convert.FromBase64String(string64));
        }

        public static void PerfectWriteLine(string strToPrint, int maxLength)
        {
            int dif = maxLength - strToPrint.Length;

            if (dif > 0) {
                for (int i = 0; i < dif; i++)
                {
                    if (i == dif - 1)
                        strToPrint += "║";
                    else
                        strToPrint += " ";
                }
            }

            System.Console.WriteLine(strToPrint);
        }

    }
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
        }

        public string getLocalisation()
        {
            return localisation.adresse.numero + " " + localisation.adresse.rue + ", " + localisation.adresse.ville.codePostal + " " + localisation.adresse.ville.nom + ", " + localisation.pays;
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
    public class Annonce
    {
        public List<Hotel> offres { get; set; }
        public List<Agence> agences { get; set; }

        public Annonce()
        {
            this.offres = new List<Hotel>();
            this.agences = new List<Agence>();
        }
        public void PrintAnnonce()
        {
            if (offres.Count != 0 && agences.Count != 0)
            {
                for (int i = 0; i < offres.Count; i++)
                {
                    Console.Clear();
                    System.Console.WriteLine("[i] Listing des différentes offres en cours : offre n°" + (i+1) + "/" + offres.Count);
                    string imgName = "room" + i + ".jpg";
                    PrintImage.saveImage(imgName, offres[i].imgChambre);


                    System.Console.WriteLine("\n╔═══════════════════════════════════════════════════════════════════════════╗");
                    System.Console.WriteLine("║ [" + offres[i].id + "] " + "Offre proprosée par l'agence : " + agences[i].nom + "\t\t\t    ║");
                    System.Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════╣");
                    PrintImage.PerfectWriteLine("║ ► Nom: " + offres[i].nom, 77);
                    PrintImage.PerfectWriteLine("║ ► Lieu : " + offres[i].getLocalisation(), 77);
                    PrintImage.PerfectWriteLine("║ ► Note: " + offres[i].classement, 77);
                    PrintImage.PerfectWriteLine("║ ► Capacité : " + offres[i].capacite, 77);
                    PrintImage.PerfectWriteLine("║ ► Tarif : " + offres[i].prix, 77);
                    System.Console.WriteLine("╠═══════════════════════════════════════════════════════════════════════════╣");
                    
                    for(int j=0; j<12; j++)
                    {
                        PrintImage.PerfectWriteLine("║", 77);
                    }

                    try
                    {
                        PrintImage.PrintImageInConsole(imgName, new Point(2, 11));
                    }
                    catch
                    {
                        System.Console.WriteLine("[!] L'Affichage de l'image dans la console a échoué.");
                    }

                    System.Console.WriteLine("╚═══════════════════════════════════════════════════════════════════════════╝");

                    System.Console.WriteLine("\n[i] Merci d'appuyer sur la touche [ENTRÉE] pour passer à l'offre suivante...");
                    string enter = Console.ReadLine();

                }

                System.Console.WriteLine("[i] Fin du listing des offres.");
            }
        }
    }

    class Program
    {
        static HttpClient client = new HttpClient();
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
        static async Task<bool> GetBoolAsync(string path)
        {
            bool boolean = false;

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                boolean = await response.Content.ReadAsAsync<bool>();
            }

            return boolean;
        }
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
        static async Task<Annonce> GetAnnonceAsync(string path)
        {
            Annonce annonce = new Annonce();

            HttpResponseMessage response = await client.GetAsync(path);

            if (response.IsSuccessStatusCode)
            {
                annonce = await response.Content.ReadAsAsync<Annonce>();
            }

            return annonce;
        }

        public class Comparateur
        {
            string url;
            Annonce annonce { get; set; }
            string choixUser { get; set; }
            public Comparateur(string url)
            {
                this.url = url;
                this.annonce = new Annonce();
                choixUser = "0";

                System.Console.WriteLine("[i] : Information");
                System.Console.WriteLine("[*] : Demande de saisie");
                System.Console.WriteLine("[!] : Avertissement");
                System.Console.WriteLine("/!\\ : Arrêt du Système");
            }
            public async Task<int> MenuComparateur()
            {
                bool run = false;
                CultureInfo culture = new CultureInfo("fr-FR");

                Msg response = new Msg();

                string cityUserInput = "";
                string startUserInput = "";
                string endUserInput = "";
                string priceUserInput = "";
                string nbrUserInput = "";
                double nbNuit = 0.0;

                try
                {
                    run = await GetBoolAsync(url + "comparateur/isRunning");
                }
                catch
                {
                    System.Console.WriteLine("\n/!\\ Le Web Service doit être lancé avant l'application, fin du programme.");
                }

                if (!run)
                    return 0;

                System.Console.WriteLine("\n*************************************");
                System.Console.WriteLine("*** COMPARATEUR D'OFFRES D'HÔTELS ***");
                System.Console.WriteLine("*************************************\n");

                System.Console.WriteLine("[i] Bienvenue sur notre comparateur d'offres d'hôtels, vous trouverez une large offre au meilleur prix !\n");


                do {
                    System.Console.WriteLine("╔═══════════════════════════════╗");
                    System.Console.WriteLine("║        MENU COMPARATEUR       ║");
                    System.Console.WriteLine("╠═══════════════════════════════╣");
                    System.Console.WriteLine("║ 1. Afficher toutes les offres ║");
                    System.Console.WriteLine("║ 2. Filtrer certaines offres   ║");
                    System.Console.WriteLine("║ 3. Quitter le programme       ║");
                    System.Console.WriteLine("╚═══════════════════════════════╝\n");


                    System.Console.WriteLine("\n[*] Je souhaite :");
                    System.Console.Write("> ");

                    choixUser = Console.ReadLine();

                    if (!choixUser.Equals("1") && !choixUser.Equals("2") && !choixUser.Equals("3"))
                        System.Console.WriteLine("\n[!] Saisie incorrecte veuillez réessayer");

                } while (!choixUser.Equals("1") && !choixUser.Equals("2") && !choixUser.Equals("3"));

                switch (choixUser)
                {
                    case "1":
                        annonce = await GetAnnonceAsync(url + "comparateur/hotels");
                        annonce.PrintAnnonce();

                        break;
                    case "2":
                        System.Console.Write("\n[i] Villes disponibles : ");
                        System.Console.Write(await GetStringAsync(url + "comparateur/villes"));

                        System.Console.WriteLine("\n\n[*] Dans quelle Ville souhaitez-vous réserver un Hôtel ?");
                        System.Console.Write("> ");

                        cityUserInput = Console.ReadLine();
                       
                        if ((response = await GetMsgAsync(url + "comparateur/checking/1/" + cityUserInput)).code == 0)
                        {
                            System.Console.WriteLine(response.msg);
                            return response.code;
                        }

                        System.Console.WriteLine("\n[*] Quelle est la date d'arrivée ? (format : jj/mm/aaaa)");
                        System.Console.Write("> ");
                        startUserInput = Console.ReadLine();

                        if ((response = await GetMsgAsync(url + "comparateur/checking/2/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_"))).code == 0)
                        {
                            System.Console.WriteLine(response.msg);
                            return response.code;
                        }

                        System.Console.WriteLine("\n[*] Quelle est la date de départ ? (format : jj/mm/aaaa)");
                        System.Console.Write("> ");
                        endUserInput = Console.ReadLine();

                        if ((response = await GetMsgAsync(url + "comparateur/checking/3/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_"))).code == 0)
                        {
                            System.Console.WriteLine(response.msg);
                            return response.code;
                        }

                        nbNuit = (Convert.ToDateTime(endUserInput, new CultureInfo("fr-FR")) - Convert.ToDateTime(startUserInput)).TotalDays;

                        System.Console.WriteLine("\n[i] Les prix appartiennent à l'intervalle suivant : " + await GetStringAsync(url + "comparateur/prix"));
                        System.Console.WriteLine("\n[*] Combien êtes-vous prêt à dépenser maximum ?");
                        System.Console.Write("> ");
                        priceUserInput = Console.ReadLine();

                        if ((response = await GetMsgAsync(url + "comparateur/checking/4/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput)).code == 0)
                        {
                            System.Console.WriteLine(response.msg);
                            return response.code;
                        }

                        System.Console.WriteLine("\n[*] Combien de personnes maximum faut-il héberger ?");
                        System.Console.Write("> ");
                        nbrUserInput = Console.ReadLine();


                        if ((response = await GetMsgAsync(url + "comparateur/checking/5/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput)).code == 0)
                        {
                            System.Console.WriteLine(response.msg);
                            return response.code;
                        }

                        if ((response = await GetMsgAsync(url + "comparateur/nbOffres/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput)).code >= 1)
                            System.Console.WriteLine(response.msg);

                        annonce = await GetAnnonceAsync(url + "comparateur/hotels/filtre/" + cityUserInput + "/" + Regex.Replace(startUserInput, @"/", "_") + "/" + Regex.Replace(endUserInput, @"/", "_") + "/" + priceUserInput + "/" + nbrUserInput);
                        annonce.PrintAnnonce();

                        break;

                    default:
                        break;
                }

                System.Console.WriteLine("\n[i] Notre équipe vous remercie pour votre confiance, à bientôt !");

                return 1;
            }
        }

        static void Main(string[] args)
        {
            Comparateur comparateur = new Comparateur("https://localhost:44340/");
            comparateur.MenuComparateur().GetAwaiter().GetResult();
        }
    }
}
