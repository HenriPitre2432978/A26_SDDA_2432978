using APIFruit.DataAccessLayer.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIFruit.DataAccessLayer
{
    public class DAL
    {
        private FruitFactory _fruitFactory = null;

        public static string ConnectionString { get { return "Server=sql.decinfo-cchic.ca;Port=33306;Database=db_dev_pandre_savard;Uid=dev-pandre.savard;Pwd=Info2020"; } }


        public FruitFactory FruitFactory
        {
            get
            {
                if (_fruitFactory == null)
                {
                    _fruitFactory = new FruitFactory();
                }

                return _fruitFactory;
            }
        }

        public DAL()
        {
        }
    }
}
