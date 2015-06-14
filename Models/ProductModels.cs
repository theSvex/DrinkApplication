using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DrinkApplication.Models
{
    [Table("Ingredients")]
    public class Ingredient
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Ing_Id { get; set; }
        public string Ing_Name { get; set; }

        [ForeignKey("type")]
        public int Type_Id { get; set; }

        public virtual Types type { get; set; }
    }

    [Table("Types")]
    public class Types
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Type_Id { get; set; }
        public string Type_Name { get; set; }
    }

    public class IngredientsContext : DbContext
    {
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Types> Types { get; set; }
        public DbSet<Drink> Drinks { get; set; }
        public DbSet<Ing4Drink> Ing4Drinks { get; set; }
    }

    [Table("Ing4Drink")]
    public class Ing4Drink
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("DrinkId")]
        public int Drink_Id { get; set; }
        public virtual Drink DrinkId { get; set; }

        [ForeignKey("IngId")]
        public int Ing_Id { get; set; }
        public virtual Ingredient IngId { get; set; }
    }

    [Table("Drink")]
    public class Drink
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Drink_Id { get; set; }

        [StringLength(60, MinimumLength = 2)]
        [Required]
        public string Name { get; set; }

        public string Comment { get; set; }
        public int Views { get; set; }
    }
        
}