using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Food
/// </summary>
public class Food : Product
{
    public int pkFoodId { get ; set ; }
    public FoodType FType { get ; set ; }

    public List<Ingredient> Ingredients { get ; set ; }
    //public List<CookingStyle> CookingStyles { get ; set ; }

    public Food()
    {
        Ingredients = new List<Ingredient>();
        //CookingStyles = new List<CookingStyle>();
        FType = new FoodType();
        IType = ItemType.Food;
    }

    public Food(int Id) : base(Id){
        IType = ItemType.Food;

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Food WHERE pkFoodId = @0", Id);

        pkFoodId = query.pkFoodId;
        FType = new FoodType(query.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);

        /* Add Ingredients */
        Ingredients = new List<Ingredient>();
        query = db.Query(@"SELECT i.pkIngredientId FROM Ingredients i
                            INNER JOIN FoodIngredientLookup fil ON fil.fkIngredientId = i.pkIngredientId
                            INNER JOIN Food f ON f.pkFoodId = fil.fkFoodId
                            WHERE f.pkFoodId = @0 AND fil.isSecondaryIngredient = 'False'", pkFoodId);
        foreach(var row in query){Ingredients.Add(new Ingredient(row.pkIngredientId));}

        /* Add Cooking Styles 
        CookingStyles = new List<CookingStyle>();
        query = db.Query(@"SELECT cs.* FROM CookingStyles cs
                            INNER JOIN CookingStyleLookup csl ON csl.fkCookingStyleId = cs.pkCookingStyleId
                            INNER JOIN Food f ON f.pkFoodId = csl.fkFoodId
                            WHERE f.pkFoodId = @0", Id);
        foreach(var row in query){CookingStyles.Add(new CookingStyle(row.pkCookingStyleId));}
        */
        
        db.Close();
        return;
    }

    public Food(int FoodId, int ProductId) : base(ProductId){
        IType = ItemType.Food;

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Food WHERE pkFoodId = @0", FoodId);

        pkFoodId = query.pkFoodId;
        FType = new FoodType(query.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);

        /* Add Ingredients */
        Ingredients = new List<Ingredient>();
        query = db.Query(@"SELECT i.pkIngredientId FROM Ingredients i
                            INNER JOIN FoodIngredientLookup fil ON fil.fkIngredientId = i.pkIngredientId
                            INNER JOIN Food f ON f.pkFoodId = fil.fkFoodId
                            WHERE f.pkFoodId = @0 AND fil.isSecondaryIngredient = 'False'", FoodId);
        foreach(var row in query){Ingredients.Add(new Ingredient(row.pkIngredientId));}

        /* Add Cooking Styles 
        query = db.Query(@"SELECT cs.pkCookingStyleId FROM CookingStyles cs
                            INNER JOIN CookingStyleLookup csl ON csl.fkCookingStyleId = cs.pkCookingStyleId
                            INNER JOIN Food f ON f.pkFoodId = csl.fkFoodId
                            WHERE f.pkFoodId = @0", FoodId);
        foreach(var row in query){CookingStyles.Add(new CookingStyle(row.pkCookingStyleId));}
        */
        
        db.Close();
        return;
    }

    /* Change product into food */
    public Food(Product item){
        IType = ItemType.Food;

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

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = new List<Ingredient>();
    }

    /* Copy Constructor */
    public Food(Food item){
        IType = ItemType.Food;

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
        FType = item.FType;

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
    }

    /* Change sushi into food */
    public Food(Sushi item){
        IType = ItemType.Food;

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

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteSushi();
    }

    /* Change a roll into food */
    public Food(Roll item){
        IType = ItemType.Food;

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

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteRoll();
        item.deleteSushi();
    }

    public decimal calculateCost(){
        /* Check for any ingredients selected, otherwise return 0 */
        if(Ingredients.Count < 1){return 0;}
        
        /* Set base roll cost and starting roll cost */
        decimal profit = 0;
        decimal baseCost = (decimal)0.84;
        decimal basePrice = 2;

        /* Add the cost of the ingredients */
        foreach(Ingredient i in Ingredients){profit += (i.Extra ? (i.IngredientCostPerPiece) * 2 : i.IngredientCostPerPiece);}
        
        /* Calculate profit from base cost/price per roll */
        profit += (basePrice - baseCost);

        /* Return cost of product */
        return profit;
    }

    public decimal calculatePrice(){
        /* Check for any ingredients selected, otherwise return 0 */
        if(Ingredients.Count < 1){return 0;}
        
        /* Set base roll cost and starting roll cost */
        var baseCost = (decimal)2.00;
        var currentRollCost = (decimal)0.00;

        /* Add the cost of the ingredients */
        foreach(Ingredient i in Ingredients){currentRollCost += (i.Extra ? (i.IngredientPricePerRoll) * 2 : i.IngredientPricePerRoll);}
        
        /* Add base cost */
        currentRollCost += baseCost;
        /* Return cost of product */
        return currentRollCost;
    }


    public void addFood(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add the new food information */
            db.Execute("INSERT INTO Food (fkFoodTypeId, fkProductId) VALUES (@0, @1)", FType.pkFoodTypeId, pkProductId);
            /* Set pkFoodId to the newly added ID */
            pkFoodId = (int)db.GetLastInsertId();
            
            /* Add Ingredients */
            if(Ingredients.Count > 0){
                foreach(var i in Ingredients){
                    db.Execute("INSERT INTO FoodIngredientLookup (fkFoodId, fkIngredientId, Extra, isSecondaryIngredient) VALUES (@0, @1, @2, @3)", pkFoodId, i.pkIngredientId, i.Extra, false);
                }
            }

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    } /* #END addFood() */

    public void addNewFood(){
        //try{
            /* Add the product level information first */
            addProduct();
            /* Add food information */
            addFood();
        //}catch(Exception e){Console.Write(e.Message);}
    }


    public void modifyFood(){
        //try{
            /* Modify item on the product level */
            modifyProduct();

            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify food information in the database */
            db.Execute("UPDATE Food SET fkFoodTypeId = @0, fkProductId = @1 WHERE pkFoodId = @2", FType.pkFoodTypeId, pkProductId, pkFoodId);
            
            /* Delete lookup information then re-add current information */
            db.Execute("DELETE FROM FoodIngredientLookup WHERE fkFoodId = @0", pkFoodId);
            //db.Execute("DELETE FROM CookingStyleLookup WHERE fkFoodId = @0", pkFoodId);

            /* Add Ingredients */
            if(Ingredients.Count > 0){
                foreach(var i in Ingredients){
                    db.Execute("INSERT INTO FoodIngredientLookup (fkFoodId, fkIngredientId, Extra, isSecondaryIngredient) VALUES (@0, @1, @2, @3)", pkFoodId, i.pkIngredientId, i.Extra, false);
                }
            }

            /* Close connection to the database */
            db.Close();
        //} catch (Exception e){Console.Write(e.Message);}
    } /* #END modifyFood() */

    public void deleteFood(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            
            /* Delete all lookup information referencing this food item */
            db.Execute("DELETE FROM FoodIngredientLookup WHERE fkFoodId = @0", pkFoodId);
            //p[db.Execute("DELETE FROM CookingStyleLookup WHERE fkFoodId = @0", pkFoodId);

            /* Delete data from the Food table */
            db.Execute("DELETE FROM Food WHERE pkFoodId = @0", pkFoodId);

            /* Delete data from the products table */
            //deleteProduct();
        //}catch(Exception e){Console.Write(e.Message);}
    } /* #END deleteFood() */
}
