using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivilizationRand
{
    public class RandomCreatureCollection
    {
        public String PlayerNumber { get; set; }
        public List<String> Civs { get; set; }

        public RandomCreatureCollection()
        {
            Civs = new List<string>();
        }

    }
}
