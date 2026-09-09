using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFruit.Models
{
    public class Fruit
    {
        public int Id { get; set; }
        public string Nom { get; set; }
        public string Couleur { get; set; }
        public string Origine { get; set; }

        // Constructeur vide requis pour la désérialisation
        public Fruit()
        {
        }

        public Fruit(int id, string nom, string couleur, string origine)
        {
            Id = id;
            Nom = nom;
            Couleur = couleur;
            Origine = origine;
        }
    }
}
