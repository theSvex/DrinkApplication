using DrinkApplication.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrinkApplication.Controllers
{
    public class HomeController : Controller
    {
        private IngredientsContext IC = new IngredientsContext();
        private DisplayModel disp = new DisplayModel();
        private UsersContext UC = new UsersContext();

        public ActionResult Index()
        {
            ViewBag.Message = "This is aplication called DrinkApp";

            disp.alcohols = IC.Ingredients.Where(x => x.Type_Id == 1).ToList();
            disp.drinks = IC.Ingredients.Where(x => x.Type_Id == 4).ToList();
            disp.juices = IC.Ingredients.Where(x => x.Type_Id == 3).ToList();
            disp.vegetables = IC.Ingredients.Where(x => x.Type_Id == 5).ToList();
            disp.fruits = IC.Ingredients.Where(x => x.Type_Id == 2).ToList();

            var tmp = IC.Drinks.ToList();
            var tmp2 = IC.Ing4Drinks.ToList();

            disp.drinkModel = new List<DrinkModel>();
            foreach (var item in tmp)
            {
                disp.drinkModel.Add(new DrinkModel()
                {
                    drink = new Drink()
                    {
                        Drink_Id = item.Drink_Id,
                        Name = item.Name,
                        Comment = item.Comment
                    },
                    ingredients = tmp2
                        .Where(x => x.Drink_Id == item.Drink_Id)
                        .Select(x => x.IngId.Ing_Name)
                        .ToList()
                        .Aggregate((a, x) => a + ", " + x)
                });
            }

            Dictionary<string, List<Ingredient>> dic = new Dictionary<string, List<Ingredient>>();
            dic.Add("Alcohols", disp.alcohols);
            dic.Add("Drinks", disp.drinks);
            dic.Add("Juices", disp.juices);
            dic.Add("Vegetables", disp.vegetables);
            dic.Add("Fruits", disp.fruits);
            disp.dic = dic;
            return View(disp);
        }

        public ActionResult AddNew()
        {
            ViewBag.Message = "Add new item";


            if (User.Identity.IsAuthenticated)
            {
                DisplayModel newDisp = new DisplayModel();
                newDisp.alcohols = IC.Ingredients.Where(x => x.Type_Id == 1).ToList();
                newDisp.drinks = IC.Ingredients.Where(x => x.Type_Id == 4).ToList();
                newDisp.juices = IC.Ingredients.Where(x => x.Type_Id == 3).ToList();
                newDisp.vegetables = IC.Ingredients.Where(x => x.Type_Id == 5).ToList();
                newDisp.fruits = IC.Ingredients.Where(x => x.Type_Id == 2).ToList();
                Dictionary<string, List<Ingredient>> dic = new Dictionary<string, List<Ingredient>>();
                dic.Add("Alcohols", newDisp.alcohols);
                dic.Add("Drinks", newDisp.drinks);
                dic.Add("Juices", newDisp.juices);
                dic.Add("Vegetables", newDisp.vegetables);
                dic.Add("Fruits", newDisp.fruits);
                newDisp.dic = dic;
                return View(newDisp);
            }
            return View();
        }

        [HttpPost]
        public void AddNewRecord(Drink drink, string ings)
        {
            List<string> ingredients = ings.Substring(0,ings.Length-1).Split(';').ToList();
            IC.Drinks.Add(new Drink() { Name = drink.Name, Comment = drink.Comment });
            IC.SaveChanges();
            int id = IC.Drinks.Where(x => x.Name == drink.Name).Select(x => x.Drink_Id).First();
            foreach (var item in ingredients)
            {
                IC.Ing4Drinks.Add(new Ing4Drink() { 
                    Drink_Id = id, 
                    Ing_Id = int.Parse(item) });
                IC.SaveChanges();
            }
        }

        public ActionResult AddFav()
        {
            ViewBag.Message = "Favourite drinks!";
            if (User.Identity.IsAuthenticated)
            {
                UserProfile tmp = UC.UserProfiles.Where(x => x.UserName == User.Identity.Name).First();
                List<int> favs = UC.FavItems.Where(x => x.UserId == tmp.UserId).Select(x => x.Drink_Id).ToList();
                List<Drink> tmp2 = new List<Drink>(IC.Drinks.Where(x => favs.Contains(x.Drink_Id)).ToList());
                return View(tmp2);
            }
            return View();
        }

        public ActionResult AddNewFav(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                UserProfile tmp = UC.UserProfiles.Where(x => x.UserName == User.Identity.Name).First();
                var isFav = UC.FavItems.Where(x => x.Drink_Id == id && x.UserId == tmp.UserId).FirstOrDefault();
                if (isFav == null)
                {
                    UC.FavItems.Add(new FavouriteItems() { Drink_Id = id, UserId = tmp.UserId });
                    UC.SaveChanges();
                }
            }
            return RedirectToAction("AddFav", "Home");
        }

        public ActionResult RemoveFav(int id)
        {
            UserProfile tmp = UC.UserProfiles.Where(x => x.UserName == User.Identity.Name).First();
            FavouriteItems tmp2 = UC.FavItems.Where(x => x.Drink_Id == id && x.UserId == tmp.UserId).FirstOrDefault();
            UC.FavItems.Remove(tmp2);//.RemoveAll(x => x.Drink_Id == id && x.UserId == tmp.UserId);
            UC.SaveChanges();
            return RedirectToAction("AddFav", "Home");
        }

        public ActionResult Popular()
        {
            ViewBag.Message = "Most popular choices";
            List<string> numbers = new List<string>() { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };
            ViewBag.Numbers = numbers;

            List<Drink> mostPopular = IC.Drinks.OrderByDescending(x => x.Views).Take(9).ToList();

            return View(mostPopular);
        }

        public ActionResult ResultAction(string str)
        {
            var tmp = IC.Drinks.ToList();
            var tmp2 = IC.Ing4Drinks.ToList();
            List<DrinkModel> drinksView = new List<DrinkModel>();
            foreach (var item in tmp)
            {
                drinksView.Add(new DrinkModel()
                {
                    drink = new Drink()
                    {
                        Drink_Id = item.Drink_Id,
                        Name = item.Name,
                        Comment = item.Comment
                    },
                    ingredients = tmp2
                        .Where(x => x.Drink_Id == item.Drink_Id)
                        .Select(x => x.IngId.Ing_Name)
                        .ToList()
                        .Aggregate((a, x) => a + ", " + x)
                });
            }
            List<int> tmpIds;
            if (!string.IsNullOrEmpty(str))
            {
                str = str.Substring(0, str.Length - 1);
                tmpIds = str.Split(';').Select(x => int.Parse(x)).ToList();
            }
            else
            {
                tmpIds = IC.Ingredients.Select(x => x.Ing_Id).ToList();
            }
            
            List<string> viewIngs = new List<string>(IC.Ingredients.Where(x => tmpIds.Contains(x.Ing_Id)).Select(x => x.Ing_Name));
            List<DrinkModel> result = new List<DrinkModel>();
            foreach (var item in drinksView)
            {
                foreach (var item2 in viewIngs)
                {
                    if (item.ingredients.Contains(item2))
                    {
                        result.Add(item);
                        break;
                    }
                }
            }
            foreach (var item in result)
            {
                Drink updateDrink = IC.Drinks.FirstOrDefault(x => x.Drink_Id == item.drink.Drink_Id);
                updateDrink.Views++;
                IC.SaveChanges();
            }
            return View(result);
        }

        public ActionResult Search(string text)
        {
            string text2 = text;
            var tmp = IC.Drinks.ToList();
            var tmp2 = IC.Ing4Drinks.ToList();
            List<DrinkModel> drinksView = new List<DrinkModel>();
            if (text != "" && text != null)
            {
                foreach (var item in tmp)
                {
                    if (item.Name.ToLower().Contains(text2.ToLower()))
                    {
                        drinksView.Add(new DrinkModel()
                        {
                            drink = new Drink()
                            {
                                Drink_Id = item.Drink_Id,
                                Name = item.Name,
                                Comment = item.Comment
                            },
                            ingredients = tmp2
                                .Where(x => x.Drink_Id == item.Drink_Id)
                                .Select(x => x.IngId.Ing_Name)
                                .ToList()
                                .Aggregate((a, x) => a + ", " + x)
                        });
                    }
                }
                foreach (var item in drinksView)
                {
                    Drink updateDrink = IC.Drinks.FirstOrDefault(x => x.Drink_Id == item.drink.Drink_Id);
                    updateDrink.Views++;
                    IC.SaveChanges();
                }
            }
            else if(text == "")
            {
                foreach (var item in tmp)
                {
                    drinksView.Add(new DrinkModel()
                    {
                        drink = new Drink()
                        {
                            Drink_Id = item.Drink_Id,
                            Name = item.Name,
                            Comment = item.Comment
                        },
                        ingredients = tmp2
                            .Where(x => x.Drink_Id == item.Drink_Id)
                            .Select(x => x.IngId.Ing_Name)
                            .ToList()
                            .Aggregate((a, x) => a + ", " + x)
                    });
                }
            }
            else
            {
                drinksView = new List<DrinkModel>();
            }

            return View(drinksView);
        }
    }
}
