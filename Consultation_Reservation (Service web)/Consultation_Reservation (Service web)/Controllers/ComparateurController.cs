using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace Consultation_Reservation__Service_web_.Controllers
{
    public class Annonce
    {
        public List<Hotel> offres { get; set; }
        public List<Agence> agences { get; set; }

        public Annonce()
        {
            this.offres = new List<Hotel>();
            this.agences = new List<Agence>();
        }
    }

    public class ComparateurController : ApiController
    {
        Annonce annonces;

        ComparateurController()
        {
            annonces = new Annonce();
        }

        [Route("comparateur/hotels")]
        [AcceptVerbs("GET")]
        public Annonce getHotelsComparateur()
        {
            BDDAgences.GetAgences().ForEach(agence =>
            {
                foreach (Hotel hotel in BDDHotels.GetHotels(agence.id))
                {
                    annonces.offres.Add(hotel);
                    annonces.agences.Add(agence);
                }
            });

            return annonces;
        }

        [Route("comparateur/hotels/filtre/{city}/{startDate}/{endDate}/{price}/{capacity}")]
        [AcceptVerbs("GET")]
        public Annonce getHotelsFiltreComparateur(string city = "", string startDate = "", string endDate = "", string price = "", string capacity = "")
        {
            BDDAgences.GetAgences().ForEach(agence =>
            {
                foreach (Hotel hotel in BDDHotels.GetHotels(agence.id))
                {
                    if (hotel.localisation.adresse.ville.nom.Equals(city) && float.Parse(hotel.prix, CultureInfo.InvariantCulture.NumberFormat) <= float.Parse(price, CultureInfo.InvariantCulture.NumberFormat) && int.Parse(hotel.capacite) >= int.Parse(capacity))
                    {
                        annonces.offres.Add(hotel);
                        annonces.agences.Add(agence);
                    }
                }
            });

            return annonces;
        }

        [Route("comparateur/villes")]
        [AcceptVerbs("GET")]
        public string getVillesDisponiblesComparateur()
        {
            List<string> villes = new List<string>();

            foreach (Hotel hotel in BDDHotels.GetHotels())
            {
                villes.Add(hotel.localisation.adresse.ville.nom);
            }

            return string.Join(", ", villes);
        }

        [Route("comparateur/getOffres/{city}/{startDate}/{endDate}/{price}/{capacity}")]
        [AcceptVerbs("GET")]
        public List<Hotel> getOffresSelonCriteres(string city = "", string startDate = "", string endDate = "", string price = "", string capacity = "")
        {
            List<Hotel> hotels = new List<Hotel>();

            BDDAgences.GetAgences().ForEach(agence =>
            {
                foreach (Hotel hotel in BDDHotels.GetHotels(agence.id))
                {
                    if (hotel.localisation.adresse.ville.nom.Equals(city) && float.Parse(hotel.prix, CultureInfo.InvariantCulture.NumberFormat) <= float.Parse(price, CultureInfo.InvariantCulture.NumberFormat) && int.Parse(hotel.capacite) >= int.Parse(capacity))
                        hotels.Add(hotel);
                }

            });

            return hotels;
        }

        [Route("comparateur/nbOffres/{city}/{startDate}/{endDate}/{price}/{capacity}")]
        [AcceptVerbs("GET")]
        public Msg nbOffresSelonCriteres(string city = "", string startDate = "", string endDate = "", string price = "", string capacity = "")
        {
            startDate = Regex.Replace(startDate, @"_", "/");
            endDate = Regex.Replace(endDate, @"_", "/");
            int count = 0;

            BDDAgences.GetAgences().ForEach(agence =>
            {
                foreach (Hotel hotel in BDDHotels.GetHotels())
                {
                    if (hotel.localisation.adresse.ville.nom.Equals(city) && float.Parse(hotel.prix, CultureInfo.InvariantCulture.NumberFormat) <= float.Parse(price, CultureInfo.InvariantCulture.NumberFormat) && int.Parse(hotel.capacite) >= int.Parse(capacity))
                        count += 1;
                }

            });

            string msg = "";

            if (count == 1)
                msg = "[i] Félicitation, " + count + " Hôtel correspond à votre recherche !";
            else
                msg = "[i] Félicitation, " + count + " Hôtels correspondent à votre recherche !";

            return new Msg(msg, 1);
        }

        [Route("comparateur/checking/{numeroSaisie:int}/{city?}/{startDate?}/{endDate?}/{price?}/{capacity?}/{reservation?}")]
        [AcceptVerbs("GET")]
        public Msg VerificationSaisieUtilisateurComparateur(int numeroSaisie, string city = "", string startDate = "", string endDate = "", string price = "", string capacity = "", string reservation = "")
        {
            switch (numeroSaisie)
            {
                case 1:
                    foreach (Hotel hotel in BDDHotels.GetHotels())
                    {
                        if (hotel.localisation.adresse.ville.nom.Equals(city))
                        {
                            return new Msg("[i] Notre Comparateur a trouvé des offres dans cette Ville", 1);
                        }
                    }

                    return new Msg("/!\\ Notre Comparateur ne trouve aucune offre dans cette Ville, fin du programme.", 0);
                case 2:
                    startDate = Regex.Replace(startDate, @"_", "/");

                    if (!(new Regex(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$").IsMatch(startDate)))
                    {
                        return new Msg("/!\\ Le format de la saisie n'est pas respecté, fin du programme.", 0);
                    }

                    DateTime startDateUser = Convert.ToDateTime(startDate, new CultureInfo("fr-FR"));

                    if (startDateUser.Date < DateTime.Now.Date)
                    {
                        return new Msg("/!\\ La date que vous avez saisi est déjà passée, fin du programme.", 0);
                    }

                    return new Msg("[i] Le format de la saisie est correct.", 1);
                case 3:
                    startDate = Regex.Replace(startDate, @"_", "/");
                    endDate = Regex.Replace(endDate, @"_", "/");

                    if (!(new Regex(@"^([0-2][0-9]|(3)[0-1])(\/)(((0)[0-9])|((1)[0-2]))(\/)\d{4}$").IsMatch(endDate)))
                    {
                        return new Msg("/!\\ Le format de la saisie n'est pas respecté, fin du programme.", 0);
                    }

                    DateTime startDateFormat = Convert.ToDateTime(startDate, new CultureInfo("fr-FR"));
                    DateTime endDateFormat = Convert.ToDateTime(endDate, new CultureInfo("fr-FR"));

                    if (endDateFormat.Date < DateTime.Now.Date)
                    {
                        return new Msg("/!\\ La date que vous avez saisi est déjà passée, fin du programme.", 0);
                    }

                    if (startDateFormat.Date > endDateFormat.Date)
                    {
                        return new Msg("/!\\ La date de départ ne peut pas être avant la date d'arrivée, fin du programme.", 0);
                    }


                    return new Msg("[i] Le format de la saisie est correct.", 1);
                case 4:
                    double input4 = 0.0;

                    try
                    {
                        input4 = float.Parse(price, CultureInfo.InvariantCulture.NumberFormat);
                    }
                    catch (FormatException)
                    {
                        return new Msg("/!\\ Le format de la saisie n'est pas respecté, fin du programme.", 0);
                    }

                    foreach (Hotel hotel in BDDHotels.GetHotels())
                    {
                        if (hotel.localisation.adresse.ville.nom.Equals(city) && float.Parse(hotel.prix, CultureInfo.InvariantCulture.NumberFormat) <= input4)
                        {
                            return new Msg("[i] Notre Comparateur a trouvé une ou plusieurs offres à ce prix dans la Ville que vous avez choisi.", 1);
                        }
                    }

                    return new Msg("/!\\ Plus aucune offre de notre Comparateur possède les critères que vous avez saisi, fin de la réservation.", 0);
                case 5:
                    int input5 = 0;

                    try
                    {
                        input5 = int.Parse(capacity);
                    }
                    catch (FormatException)
                    {
                        return new Msg("/!\\ Le format de la saisie n'est pas respecté, fin du programme.", 0);
                    }

                    foreach (Hotel hotel in BDDHotels.GetHotels())
                    {
                        if (hotel.localisation.adresse.ville.nom.Equals(city) && float.Parse(hotel.prix, CultureInfo.InvariantCulture.NumberFormat) <= float.Parse(price, CultureInfo.InvariantCulture.NumberFormat) && int.Parse(hotel.capacite) >= int.Parse(capacity))
                        {
                            return new Msg("[i] Notre Comparateur a trouvé une ou plusieurs offres dans la Ville que vous avez choisi.", 1);
                        }
                    }

                    return new Msg("/!\\ Plus aucune offre de possède les critères que vous avez saisi, fin du programme.", 0);
                case 6:
                    if (!(reservation.Equals("Oui") || reservation.Equals("Non")))
                    {
                        return new Msg("/!\\ Le format de la saisie n'est pas respecté, fin du programme.", 0);
                    }

                    return new Msg("[i] Le format de la saisie est correct.", 1);
                default:
                    return new Msg("DEFAULT", 0);
            }
        }

        [Route("comparateur/prix")]
        [AcceptVerbs("GET")]
        public string FourchettePrixComparateur()
        {
            int min = int.Parse(BDDHotels.GetHotels()[0].prix);
            int max = int.Parse(BDDHotels.GetHotels()[0].prix);

            BDDHotels.GetHotels().ForEach(x => {
                if (int.Parse(x.prix) > max) max = int.Parse(x.prix);

                if (int.Parse(x.prix) < min) min = int.Parse(x.prix);
            });

            return "[" + min.ToString() + "," + max.ToString() + "]";
        }

        [Route("comparateur/isRunning")]
        [AcceptVerbs("GET")]
        public bool comparateurIsRunning()
        {
            return true;
        }
    }
}