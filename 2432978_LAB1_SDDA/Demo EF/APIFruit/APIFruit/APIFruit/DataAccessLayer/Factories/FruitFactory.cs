using APIFruit.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFruit.DataAccessLayer.Factories
{
    public class FruitFactory
    {
        private Fruit CreateFromReader(MySqlDataReader mySqlDataReader)
        {
            int id = (int)mySqlDataReader["Id"];
            string nom = mySqlDataReader["Nom"].ToString();
            string couleur = mySqlDataReader["Couleur"].ToString();
            string origine = mySqlDataReader["Origine"].ToString();

            return new Fruit(id, nom, couleur,origine);
        }

        public Fruit CreateEmpty()
        {
            return new Fruit(0, string.Empty, string.Empty, string.Empty);
        }

        public Fruit[] GetAll()
        {
            List<Fruit> fruits = new List<Fruit>();
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM demo_apifruit ORDER BY Nom";

                mySqlDataReader = mySqlCmd.ExecuteReader();
                while (mySqlDataReader.Read())
                {
                    fruits.Add(CreateFromReader(mySqlDataReader));
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return fruits.ToArray();
        }

        public Fruit Get(int id)
        {
            Fruit fruit = null;
            MySqlConnection mySqlCnn = null;
            MySqlDataReader mySqlDataReader = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                MySqlCommand mySqlCmd = mySqlCnn.CreateCommand();
                mySqlCmd.CommandText = "SELECT * FROM demo_apifruit WHERE Id = @Id";
                mySqlCmd.Parameters.AddWithValue("@Id", id);

                mySqlDataReader = mySqlCmd.ExecuteReader();
                if (mySqlDataReader.Read())
                {
                    fruit = CreateFromReader(mySqlDataReader);
                }
            }
            finally
            {
                mySqlDataReader?.Close();
                mySqlCnn?.Close();
            }

            return fruit;
        }

        public void Save(Fruit fruit)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    if (fruit.Id == 0)
                    {
                        // On sait que c'est un nouveau produit avec Id == 0,
                        // car c'est ce que nous avons affecter dans la fonction CreateEmpty().
                        mySqlCmd.CommandText = "INSERT INTO demo_apifruit(Nom, Couleur, Origine) " +
                                               "VALUES (@Nom, @Couleur, @Origine)";
                    }
                    else
                    {
                        mySqlCmd.CommandText = "UPDATE demo_apifruit " +
                                               "SET Nom=@Nom, Couleur=@Couleur, Origine=@Origine " +
                                               "WHERE Id=@Id";

                        mySqlCmd.Parameters.AddWithValue("@Id", fruit.Id);
                    }

                    mySqlCmd.Parameters.AddWithValue("@Nom", fruit.Nom.Trim());
                    mySqlCmd.Parameters.AddWithValue("@Couleur", fruit.Couleur.Trim());
                    mySqlCmd.Parameters.AddWithValue("@Origine", fruit.Origine);

                    mySqlCmd.ExecuteNonQuery();

                    if (fruit.Id == 0)
                    {
                        // Si c'était un nouveau produit (requête INSERT),
                        // nous affectons le nouvel Id de l'instance au cas où il serait utilisé dans le code appelant.
                        fruit.Id = (int)mySqlCmd.LastInsertedId;
                    }
                }
            }
            finally
            {
                if (mySqlCnn != null)
                {
                    mySqlCnn.Close();
                }
            }
        }

        public void Delete(int id)
        {
            MySqlConnection mySqlCnn = null;

            try
            {
                mySqlCnn = new MySqlConnection(DAL.ConnectionString);
                mySqlCnn.Open();

                using (MySqlCommand mySqlCmd = mySqlCnn.CreateCommand())
                {
                    mySqlCmd.CommandText = "DELETE FROM demo_apifruit WHERE Id=@Id";
                    mySqlCmd.Parameters.AddWithValue("@Id", id);
                    mySqlCmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (mySqlCnn != null)
                {
                    mySqlCnn.Close();
                }
            }
        }
    }
}
