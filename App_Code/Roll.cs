using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Roll
/// </summary>
public class Roll : Sushi
{
    public int pkRollId { get ; set ; }
    public RollType RType { get ; set ; }

    public List<Ingredient> SecondaryIngredients { get ; set ; }
    public List<OrderOption> Options { get ; set ; }

    public Roll()
    {
        SecondaryIngredients = new List<Ingredient>();
        Options = new List<OrderOption>();
        RType = new RollType();
        IType = ItemType.Roll;
    }

    public Roll(int Id) : base(Id){
        SecondaryIngredients = new List<Ingredient>();
        RType = new RollType();
        IType = ItemType.Roll;
        
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Rolls WHERE pkRollId = @0", Id);

        pkRollId = query.pkRollId;
        RType = new RollType(query.fkRollTypeId);
        SType = new SushiType(RType.fkSushiTypeId);
        FType = new FoodType(SType.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);

        /* Add Ingredients */
        query = db.Query(@"SELECT i.pkIngredientId FROM Ingredients i
                                     INNER JOIN IngredientGroups ig
                                     ON ig.pkGroupId = i.fkGroupId
                                     INNER JOIN FoodIngredientLookup fil
                                     ON fil.fkIngredientId = i.pkIngredientId
                                     INNER JOIN Food f
                                     ON f.pkFoodId = fil.fkFoodId
                                     WHERE f.pkFoodId = @0 AND fil.isSecondaryIngredient = 'True'", pkFoodId);
        foreach(var row in query){SecondaryIngredients.Add(new Ingredient(row.pkIngredientId));}

        /* Add Product Options */
        Options = new List<OrderOption>();
        var optionsQuery = db.Query("SELECT fkOptionId FROM Order_Option_Lookup WHERE fkRollId = @0", Id);
        if(optionsQuery.Any()){foreach(var row in optionsQuery){Options.Add(new OrderOption(row.fkOptionId));}}

        db.Close();
        return;
    }

    public Roll(int RollId, int SushiId, int FoodId, int ProductId) : base(SushiId, FoodId, ProductId){
        SecondaryIngredients = new List<Ingredient>();
        RType = new RollType();
        IType = ItemType.Roll;
        
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Rolls WHERE pkRollId = @0", RollId);

        pkRollId = query.pkRollId;
        RType = new RollType(query.fkRollTypeId);
        SType = new SushiType(RType.fkSushiTypeId);
        FType = new FoodType(SType.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);

        /* Add Ingredients */
        query = db.Query(@"SELECT i.*, ig.*, fil.*, f.*
                                     FROM Ingredients i
                                     INNER JOIN IngredientGroups ig
                                     ON ig.pkGroupId = i.fkGroupId
                                     INNER JOIN FoodIngredientLookup fil
                                     ON fil.fkIngredientId = i.pkIngredientId
                                     INNER JOIN Food f
                                     ON f.pkFoodId = fil.fkFoodId
                                     WHERE f.pkFoodId = @0 AND fil.isSecondaryIngredient = 'True'", pkFoodId);
        foreach(var row in query){SecondaryIngredients.Add(new Ingredient(row.pkIngredientId));}

        /* Add Product Options */
        Options = new List<OrderOption>();
        var optionsQuery = db.Query("SELECT fkOptionId FROM Order_Option_Lookup WHERE fkRollId = @0", RollId);
        if(optionsQuery.Any()){foreach(var row in optionsQuery){Options.Add(new OrderOption(row.fkOptionId));}}

        db.Close();
        return;
    }

    /* Change product into a roll */
    public Roll(Product item){
        IType = ItemType.Roll;

        pkProductId = item.pkProductId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;

        PType = new ProductType();
        FType = new FoodType();
        SType = new SushiType();
        RType = new RollType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = new List<Ingredient>();
        SecondaryIngredients = new List<Ingredient>();
        Options = new List<OrderOption>();
    }

    /* Change food into a roll */
    public Roll(Food item){
        IType = ItemType.Roll;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;

        PType = item.PType;
        FType = new FoodType();
        SType = new SushiType();
        RType = new RollType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
        SecondaryIngredients = new List<Ingredient>();
        Options = new List<OrderOption>();
    }

    /* Change sushi into a roll */
    public Roll(Sushi item){
        IType = ItemType.Roll;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;
        pkSushiId = item.pkSushiId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;
        
        PType = item.PType;
        FType = item.FType;
        SType = new SushiType();
        RType = new RollType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
        SecondaryIngredients = new List<Ingredient>();
        Options = new List<OrderOption>();
    }


    /* Get data from one roll object and set */
    public Roll(Roll item){
        IType = ItemType.Roll;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;
        pkSushiId = item.pkSushiId;
        pkRollId = item.pkRollId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;
        
        PType = item.PType;
        FType = item.FType;
        SType = item.SType;
        RType = item.RType;

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
        SecondaryIngredients = item.SecondaryIngredients;
        Options = item.Options;
    }


    public decimal calculateCost(){
        /* Check for any ingredients selected, otherwise return 0 */
        if(SecondaryIngredients.Count < 1 && Ingredients.Count < 1){return 0;}
        
        /* Set base roll cost and starting roll cost */
        decimal profit = 0;
        decimal baseCost = (decimal)0.84;
        decimal basePrice = 2;

        bool isFirst = true; /* Is first non-topping ingredient */
        Ingredient ITD = null; /* Ingredient to double */
        
        foreach(Ingredient i in SecondaryIngredients){
            if(!string.Equals(i.IGroup.GroupName, "Toppings") && isFirst && ITD == null){
                isFirst = false; /* Found first non-topping ingredient so set isFirst false for future ingredients */
                ITD = i; /* Set ingredient to double to current ingredient */
            } else if(!string.Equals(i.IGroup.GroupName, "Toppings") && ITD != null){ITD = null;}
            profit += (i.Extra ? (i.IngredientPricePerRoll * 2) : i.IngredientPricePerRoll); /* Add cost of ingredient */
        }
        /* If only one non-topping ingredient over the top, double the cost of that ingredient */
        if(ITD != null){profit += (ITD.Extra ? (ITD.IngredientPricePerRoll * 2) : ITD.IngredientPricePerRoll);}

        /* Add the cost of the ingredients inside */
        foreach(Ingredient i in Ingredients){profit += (i.Extra ? (i.IngredientPricePerRoll) * 2 : i.IngredientPricePerRoll);}

        /* Calculate profit from base cost/price per roll */
        profit += (basePrice - baseCost);

        return profit;
    }


    public decimal calculatePrice(){
        /* Check for any ingredients selected, otherwise return 0 */
        if(SecondaryIngredients.Count < 1 && Ingredients.Count < 1){return 0;}
        
        /* Set base roll cost and starting roll cost */
        var baseCost = (decimal)2.00;
        var currentRollCost = (decimal)0.00;

        bool isFirst = true; /* Is first non-topping ingredient */
        Ingredient ITD = null; /* Ingredient to double */
        
        foreach(Ingredient i in SecondaryIngredients){
            if(!string.Equals(i.IGroup.GroupName, "Toppings") && isFirst && ITD == null){
                isFirst = false; /* Found first non-topping ingredient so set isFirst false for future ingredients */
                ITD = i; /* Set ingredient to double to current ingredient */
            } else if(!string.Equals(i.IGroup.GroupName, "Toppings") && ITD != null){ITD = null;}
            currentRollCost += (i.Extra ? (i.IngredientPricePerRoll * 2) : i.IngredientPricePerRoll); /* Add cost of ingredient */
        }
        /* If only one non-topping ingredient over the top, double the cost of that ingredient */
        if(ITD != null){currentRollCost += (ITD.Extra ? (ITD.IngredientPricePerRoll * 2) : ITD.IngredientPricePerRoll);}

        /* Add the cost of the ingredients inside */
        foreach(Ingredient i in Ingredients){currentRollCost += (i.Extra ? (i.IngredientPricePerRoll) * 2 : i.IngredientPricePerRoll);}

        currentRollCost += baseCost;

        return currentRollCost;
    }

    public void addRoll(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add the new food information */
            db.Execute("INSERT INTO Rolls (fkRollTypeId, fkSushiId) VALUES (@0, @1)", RType.pkRollTypeId, pkSushiId);
            /* Set pkRollId to the newly added ID */
            pkRollId = (int)db.GetLastInsertId();


            /* Add Secondary (on top) Ingredients */
            if(SecondaryIngredients.Count > 0){
                foreach(var i in SecondaryIngredients){
                    db.Execute("INSERT INTO FoodIngredientLookup (fkFoodId, fkIngredientId, Extra, isSecondaryIngredient) VALUES (@0, @1, @2, @3)", pkFoodId, i.pkIngredientId, i.Extra, true);
                }
            }

            /* Add Product Options */
            if(Options.Count > 0){
                foreach(OrderOption option in Options){
                    db.Execute(@"INSERT INTO Order_Option_Lookup (fkOptionId, fkRollId) VALUES (@0, @1)", option.pkOptionId, pkRollId);
                }
            }

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void addNewRoll(){
        //try{
            /* Add all base class information first, then add roll */
            addProduct();
            addFood();
            addSushi();
            addRoll();
        //}catch(Exception e){Console.Write(e.Message);}
    }


    public void modifyRoll(){
        //try{
            /* Modify item on the product level */
            modifyProduct();

            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete lookup information then re-add current information */
            db.Execute("DELETE FROM FoodIngredientLookup WHERE fkFoodId = @0", pkFoodId);
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkRollId = @0", pkRollId);

            /* Modify item on the food level */
            modifyFood();

            /* Modify item on the sushi level */
            modifySushi();


            /* Modify food information in the database */
            db.Execute("UPDATE Rolls SET fkRollTypeId = @0, fkSushiId = @1 WHERE pkRollId = @2", RType.pkRollTypeId, pkSushiId, pkRollId);

            /* Add Ingredients */
            if(SecondaryIngredients.Count > 0){
                foreach(var i in SecondaryIngredients){
                    db.Execute("INSERT INTO FoodIngredientLookup (fkFoodId, fkIngredientId, Extra, isSecondaryIngredient) VALUES (@0, @1, @2, @3)", pkFoodId, i.pkIngredientId, i.Extra, true);
                }
            }

            /* Add Product Options */
            if(Options.Count > 0){
                foreach(OrderOption option in Options){db.Execute(@"INSERT INTO Order_Option_Lookup (fkOptionId, fkRollId) VALUES (@0, @1)", option.pkOptionId, pkRollId);}
            }
            
            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteRoll(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete all lookup information referencing this food item */
            db.Execute("DELETE FROM FoodIngredientLookup WHERE fkFoodId = @0", pkFoodId);
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkRollId = @0", pkRollId);

            /* Delete data from the Food table */
            db.Execute("DELETE FROM Rolls WHERE pkRollId = @0", pkRollId);
        //}catch(Exception e){Console.Write(e.Message);}
    }
}
