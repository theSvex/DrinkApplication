using System.Collections.Generic;

namespace DrinkApplication.Models
{
    public class DrinkModel
    {
        public Drink drink { get; set; }
        public string ingredients { get; set; }
    }

    public class DisplayModel
    {
        public List<Ingredient> alcohols { get; set; }
        public List<Ingredient> fruits { get; set; }
        public List<Ingredient> drinks { get; set; }
        public List<Ingredient> vegetables { get; set; }
        public List<Ingredient> juices { get; set; }

        public List<DrinkModel> drinkModel { get; set; }

        public Dictionary<string, List<Ingredient>> dic { get; set; }
    }


}