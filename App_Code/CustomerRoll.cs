using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for CustomerRoll
/// </summary>

public class CustomerRoll
{
    public int pkCustomerRollId { get ; set ; }
    public int ?fkCustomerId { get ; set ; }
    public string RollName { get ; set ; }
    public RollType RType { get ; set ; }
    public decimal Price { get ; set ; }
    public string imgURL { get ; set ; }
    public string imgThumbURL { get ; set ; }
    public string imgSpURL { get ; set ; }
    public bool isActive { get ; set ; }
    public bool Public { get ; set ; }
    public DateTime ?SpecialBeginDate { get ; set ; }
    public DateTime ?SpecialEndDate { get ; set ; }

    public List<Ingredient> IngredientsTop { get ; set ; }
    public List<Ingredient> IngredientsInside { get ; set ; }
    public List<CookingStyle> CookingStyles { get ; set ; }
    public List<Tag> Tags { get ; set ; }
    
    public CustomerRoll(){
        RType = new RollType();
        IngredientsTop = new List<Ingredient>();
        IngredientsInside = new List<Ingredient>();
        CookingStyles = new List<CookingStyle>();
        Tags = new List<Tag>();
        SpecialBeginDate = null;
        SpecialEndDate = null;
    }

    public CustomerRoll(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT cr.*, crl.* FROM CustomerRolls cr
                                        INNER JOIN RollTypes ct ON ct.pkRollTypeId = cr.fkRollTypeId
                                        LEFT JOIN CustomerRollLookup crl ON crl.fkRollId = cr.pkRollId
                                        WHERE cr.pkRollId = @0", Id);
        pkCustomerRollId = query.pkRollId;
        fkCustomerId = query.fkCustomerId;
        RollName = query.RollName;
        pkCustomerRollId = query.pkRollId;
        RType = new RollType(query.fkRollTypeId);
        Price = query.Price;
        imgURL = query.imgURL;
        imgThumbURL = query.imgThumbURL;
        imgSpURL = query.imgSpURL;
        isActive = query.isActive;
        Public = query.Public;

        IngredientsTop = new List<Ingredient>();
        query = db.Query(@"SELECT i.pkIngredientId, crl.fkRollId FROM Ingredients i
                            INNER JOIN CustomerRollIngredientLookup crl
                            ON crl.fkIngredientId = i.pkIngredientId
                            WHERE crl.fkRollId = @0 AND crl.TopIngredient = @1", Id, true);
        foreach(var row in query){IngredientsTop.Add(new Ingredient(row.pkIngredientId));}

        IngredientsInside = new List<Ingredient>();
        query = db.Query(@"SELECT i.pkIngredientId, crl.fkRollId FROM Ingredients i
                            INNER JOIN CustomerRollIngredientLookup crl
                            ON crl.fkIngredientId = i.pkIngredientId
                            WHERE crl.fkRollId = @0 AND crl.TopIngredient = @1", Id, false);
        foreach(var row in query){IngredientsInside.Add(new Ingredient(row.pkIngredientId));}

        /* Add these once CartItems/OrderItems are better implemented */
        CookingStyles = new List<CookingStyle>();
        Tags = new List<Tag>();
        
        /* Check if roll is a special or not */
        var specialInfo = db.Query(@"SELECT SpecialBeginDate, SpecialEndDate FROM Specials_Lookup WHERE fkRollId = @0
                                        AND SpecialBeginDate <= (getdate()) AND SpecialEndDate >= (getdate())", Id);
        if(specialInfo.Any()){
            SpecialBeginDate = ((DateTime)specialInfo.First().SpecialBeginDate);
            SpecialEndDate = ((DateTime)specialInfo.First().SpecialEndDate);
        } else{
            SpecialBeginDate = null;
            SpecialEndDate = null;
        }
    }

    /* Return whether or not data was filled when Roll was instantiated */
    public bool hasId(){
        return pkCustomerRollId == 0 ? false : true;
    }
    
    public decimal calculatePrice(){
        /* Check for any ingredients selected, otherwise return 0 */
        if(IngredientsTop.Count < 1 && IngredientsInside.Count < 1){return 0;}

        /* Set base roll cost and starting roll cost */
        var baseCost = (decimal)2.00;
        var currentRollCost = (decimal)0.00;

        bool isFirst = true; /* Is first non-topping ingredient */
        Ingredient ITD = null; /* Ingredient to double */
        
        foreach(Ingredient i in IngredientsTop){
            if(!string.Equals(i.IGroup.GroupName, "Toppings") && isFirst && ITD == null){
                isFirst = false; /* Found first non-topping ingredient so set isFirst false for future ingredients */
                ITD = i; /* Set ingredient to double to current ingredient */
            } else if(!string.Equals(i.IGroup.GroupName, "Toppings") && ITD != null){ITD = null;}
            currentRollCost += (i.Extra ? (i.IngredientPricePerRoll * 2) : i.IngredientPricePerRoll); /* Add cost of ingredient */
        }
        /* If only one non-topping ingredient over the top, double the cost of that ingredient */
        if(ITD != null){currentRollCost += (ITD.Extra ? (ITD.IngredientPricePerRoll * 2) : ITD.IngredientPricePerRoll);}

        /* Add the cost of the ingredients inside */
        foreach(Ingredient i in IngredientsInside){currentRollCost += (i.Extra ? (i.IngredientPricePerRoll) * 2 : i.IngredientPricePerRoll);}

        currentRollCost += baseCost;

        return currentRollCost;
    }

    public void Activate(){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Set isActive to true */
        isActive = true;

        db.Execute("UPDATE CustomerRolls SET isActive = @0 WHERE pkRollId = @1", isActive, pkCustomerRollId);
        /* Close connection to the database */
        db.Close();
    }

    public void Deactivate(){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Set isActive to false */
        isActive = false;

        db.Execute("UPDATE CustomerRolls SET isActive = @1 WHERE pkRollId = @1", isActive, pkCustomerRollId);

        /* Remove from specials if item was a special */
        db.Execute("DELETE FROM Specials_Lookup WHERE fkRollId = @0", pkCustomerRollId);

        /* Close connection to the database */
        db.Close();
    }

    public void setSpecial(){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Delete old lookup information */
        db.Execute("DELETE FROM Specials_Lookup WHERE fkRollId = @0", pkCustomerRollId);
        /* Add if special */
        if(SpecialBeginDate != null){db.Execute("INSERT INTO Specials_Lookup (fkRollId, SpecialBeginDate, SpecialEndDate) VALUES (@0, @1, @2)",
                                            pkCustomerRollId, SpecialBeginDate, SpecialEndDate);}
        
        /* Close connection to the database */
        db.Close();
    }

    public void removeSpecial(){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Delete old lookup information */
        db.Execute("DELETE FROM Specials_Lookup WHERE fkRollId = @0", pkCustomerRollId);
        
        /* Close connection to the database */
        db.Close();
    }

    public void addRoll(){
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add the new information */
            db.Execute(@"INSERT INTO CustomerRolls (fkRollTypeId, Price) VALUES (@0, @1)", RType.pkRollTypeId, Price);
            /* Get the newly added item's id */
            pkCustomerRollId = (int)db.GetLastInsertId();

            /* If customer saved the roll, save information */
            if(fkCustomerId != null){
                db.Execute(@"INSERT INTO CustomerRollLookup (fkCustomerId, fkRollId, RollName, imgURL, imgThumbURL, imgSpURL, Public)
                             VALUES (@0, @1, @2, @3, @4, @5, @6)",
                            fkCustomerId, pkCustomerRollId, RollName, imgURL, imgThumbURL, imgSpURL, Public);}
            
            /* Add if special */
            if(SpecialBeginDate != null){db.Execute("INSERT INTO Specials_Lookup (fkRollId, SpecialBeginDate, SpecialEndDate) VALUES (@0, @1, @2)",
                                                    pkCustomerRollId, SpecialBeginDate, SpecialEndDate);}

            /* Add Top Ingredients */
            foreach(Ingredient i in IngredientsTop){
                db.Execute(@"INSERT INTO CustomerRollIngredientLookup (fkRollId, fkIngredientId, Extra, TopIngredient)
                                VALUES (@0, @1, @2, @3)", pkCustomerRollId, i.pkIngredientId, i.Extra, true);
            }
            /* Add Inside Ingredients */
            foreach(Ingredient i in IngredientsInside){
                db.Execute(@"INSERT INTO CustomerRollIngredientLookup (fkRollId, fkIngredientId, Extra, TopIngredient)
                                VALUES (@0, @1, @2, @3)", pkCustomerRollId, i.pkIngredientId, i.Extra, false);
            }

            /* Add new lookup information
            foreach(var row in CookingStyles){
                db.Execute("INSERT INTO CookingStyleLookup (fkCookingStyleId, fkCartItemId) VALUES (@0, @1)", row.pkCookingStyleId, pkCartItemId);
            }*/
    }

    public void modifyRoll(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify customer roll information in the database */
            db.Execute("UPDATE CustomerRolls SET fkRollTypeId = @0, Price = @1 WHERE pkRollId = @2", RType.pkRollTypeId, Price, pkCustomerRollId);

            /* If customer saved the roll, modify information, otherwise attempt to delete just in case it already exists */
            if(fkCustomerId != null){
                db.Execute(@"UPDATE CustomerRollLookup SET fkCustomerId = @0, RollName = @1, imgURL = @2, imgThumbURL = @3,
                                imgSpURL = @4, Public = @5 where fkRollId = @6", fkCustomerId, RollName, imgURL, imgThumbURL,
                                imgSpURL, Public, pkCustomerRollId);}
            else{db.Execute(@"DELETE FROM CustomerRollLookup WHERE fkRollId = @0", pkCustomerRollId);}

            /* Delete lookup information and then re-add new information */
            db.Execute("DELETE FROM CustomerRollIngredientLookup WHERE fkRollId = @0", pkCustomerRollId);
            db.Execute("DELETE FROM Specials_Lookup WHERE fkRollId = @0", pkCustomerRollId);

            /* Add Top Ingredients */
            foreach(Ingredient i in IngredientsTop){
                db.Execute(@"INSERT INTO CustomerRollIngredientLookup (fkRollId, fkIngredientId, Extra, TopIngredient)
                                VALUES (@0, @1, @2, @3)", pkCustomerRollId, i.pkIngredientId, i.Extra, true);
            }

            /* Add Inside Ingredients */
            foreach(Ingredient i in IngredientsInside){
                db.Execute(@"INSERT INTO CustomerRollIngredientLookup (fkRollId, fkIngredientId, Extra, TopIngredient)
                                VALUES (@0, @1, @2, @3)", pkCustomerRollId, i.pkIngredientId, i.Extra, false);
            }

            /* Add if special */
            if(SpecialBeginDate != null){db.Execute("INSERT INTO Specials_Lookup (fkRollId, SpecialBeginDate, SpecialEndDate) VALUES (@0, @1, @2)",
                                                pkCustomerRollId, SpecialBeginDate, SpecialEndDate);}

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteRoll(){
        try{
            var db = Database.Open("buSushi");

            /* Delete lookup information first */
            db.Execute("DELETE FROM CustomerRollLookup WHERE fkRollId = @0", pkCustomerRollId);
            db.Execute("DELETE FROM CustomerRollIngredientLookup WHERE fkRollId = @0", pkCustomerRollId);
            db.Execute("DELETE FROM Specials_Lookup WHERE fkRollId = @0", pkCustomerRollId);
            db.Execute("DELETE FROM CartItems WHERE fkRollId = @0", pkCustomerRollId);
            db.Execute("DELETE FROM OrderItems WHERE fkRollId = @0", pkCustomerRollId);

            /* Delete customer roll */
            db.Execute("DELETE FROM CustomerRolls WHERE pkRollId = @0", pkCustomerRollId);

            /* Delete all lookup information 
            db.Execute("DELETE FROM CookingStyleLookup WHERE fkCartItemId = @0", pkCartItemId);
            */
        }catch (Exception e){Console.Write(e.Message);}
    } /* #END deleteProductRoll() */

}
