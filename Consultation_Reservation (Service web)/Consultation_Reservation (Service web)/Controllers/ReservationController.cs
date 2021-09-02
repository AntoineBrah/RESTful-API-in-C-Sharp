using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Consultation_Reservation__Service_web_.Controllers
{
    public class Reservation
    {
        public Client client { get; set; }
        public string idAgence { get; set; }
        public string idReservation { get; set; }
        public string nbPersonne { get; set; }
        public double nbNuit { get; set; }
        public string recapitulatif { get; set; }

        public Reservation()
        {
            this.client = new Client();
            this.idAgence = "";
            this.idReservation = "";
            this.nbPersonne = "";
            this.nbNuit = 0;
            this.recapitulatif = this.getRecapitulatifReservation();
        }

        public Reservation(string idAgence, string nom, string prenom, string carteBancaire, string idReservation, string nbPersonne, double nbNuit)
        {
            this.idAgence = idAgence;
            this.client = new Client(nom, prenom, carteBancaire);
            this.idReservation = idReservation;
            this.nbPersonne = nbPersonne;
            this.nbNuit = nbNuit;
            this.recapitulatif = this.getRecapitulatifReservation();
        }

        public Hotel getHotel()
        {
            return BDDHotels.GetHotels(idAgence).Find(hotel => hotel.id.Equals(this.idReservation));
        }

        public string getRecapitulatifReservation()
        {
            string recap = "";

            try
            {
                recap = "\n*********************************\n*** RÉCAPITULATIF RÉSERVATION ***\n*********************************\n" + "\n► Nom : " + client.nom + "\n► Prénom : " + client.prenom + "\n► Hôtel : " + this.getHotel().nom + "\n► Lieu : " + this.getHotel().localisation.pays + ", " + this.getHotel().localisation.adresse.ville.nom + "\n► Nombre : " + this.nbPersonne + " personne(s)" + "\n► Nombre de nuit : " + this.nbNuit + "\n► Tarif : " + double.Parse(this.nbPersonne) * double.Parse(this.getHotel().prix) * nbNuit + " euros" + "\n\n*********************************" + "\n*********************************";
            }
            catch
            {
                recap = "ERREUR : Aucun hôtel ne correspond à cet id";
            }

            return recap;
        }
    }

    public class ReservationController : ApiController
    {
        [Route("reservation/login/{idAgence}/{password}")]
        [AcceptVerbs("GET")]
        // On vérifie que l'Agence qui essaye de se connecter est bien une Agence partenaire
        public Msg VerificationAgencePartenaire(string idAgence, string password)
        {
            foreach (Agence ag in BDDAgences.GetAgences())
            {
                if (ag.id.Equals(idAgence) && ag.password.Equals(password))
                    return new Msg("[i] Vérification de l'Agence en cours....... OK\n[i] Il s'agit bien d'une Agence partenaire, vous avez désormais accès aux différentes offres d'Hôtels.", 1);
            }

            return new Msg("[i] Vérification de l'Agence en cours....... FAIL\n/!\\ Cette Agence ne fait pas parti de nos Agences partenaires, fin de la connexion.", 0);
        }

        [Route("reservation/agence/{idAgence}/{password}")]
        [AcceptVerbs("GET")]
        public Agence getAgenceReservation(string idAgence, string password)
        {
            Agence agence = new Agence();

            foreach (Agence ag in BDDAgences.GetAgences())
            {
                if (ag.id.Equals(idAgence) && ag.password.Equals(password))
                    agence = ag;
            }

            return agence;
        }

        [Route("reservation/traitement/{idAgence}/{nom}/{prenom}/{carteBancaire}/{id}/{nbPersonne}/{nbNuit}")]
        [AcceptVerbs("GET")]
        // L'utilisateur précise l'id de l'offre auquel il souhaite effectuer une réservation
        public Reservation traitementReservation(string idAgence, string nom, string prenom, string carteBancaire, string id, string nbPersonne, double nbNuit)
        {
            return new Reservation(idAgence, nom, prenom, carteBancaire, id, nbPersonne, nbNuit);
        }
    }
}