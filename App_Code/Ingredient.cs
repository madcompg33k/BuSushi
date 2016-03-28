using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Ingredient
/// </summary>

public class Ingredient
{
    public int pkIngredientId { get; set; }
    public string IngredientName { get; set; }
    public string IngredientAltName { get ; set ; }
    public bool SecondaryIngredient { get ; set ; }
    public string Description { get ; set ; }
    public IngredientGroup IGroup { get ; set ; }
    public decimal IngredientCostPerPiece { get ; set ;} /* Bu Sushi's cost for ingredient(s)/roll */
    public decimal IngredientPricePerRoll { get ; set ; } /* Price charged for ingredient(s)/roll */
    public bool Active { get ; set ; }
    public bool MakeARollIngredient { get ; set ; }
    public bool Extra { get ; set ; }

    public Ingredient(){
        IGroup = new IngredientGroup();
        Active = true;
        MakeARollIngredient = true;
        Extra = false;
    }

    public Ingredient(int Id){
        IGroup = new IngredientGroup();

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT i.*, ig.* FROM Ingredients i
                                        INNER JOIN IngredientGroups ig ON ig.pkGroupId = i.fkGroupId
                                        WHERE i.pkIngredientId = @0", Id);
        pkIngredientId = query.pkIngredientId;
        IngredientName = query.IngredientName;
        IngredientAltName = query.IngredientAltName;
        SecondaryIngredient = query.TopIngredient;
        Description = query.Description;
        IGroup.pkGroupId = query.pkGroupId;
        IGroup.GroupName = query.GroupName;
        IGroup.GroupDescription = query.GroupDescription;
        IngredientCostPerPiece = query.IngredientCostPerPiece;
        IngredientPricePerRoll = query.IngredientPricePerRoll;
        Active = query.Active;
        MakeARollIngredient = query.MakeARollIngredient;
        Extra = false;

        db.Close();
        return;
    }

    public void addIngredient(){
        try{
            var db = Database.Open("buSushi");
            var ingredientExec = @"INSERT INTO Ingredients (IngredientName, IngredientAltName, TopIngredient, Description, fkGroupId, IngredientCostPerPiece, IngredientPricePerRoll, Active, MakeARollIngredient)
                                                    VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8)";
            db.Execute(ingredientExec, IngredientName, IngredientAltName, SecondaryIngredient, Description, IGroup.pkGroupId, IngredientCostPerPiece, IngredientPricePerRoll, Active, MakeARollIngredient);
            /* Set pkProductId to the newly added ID */
            pkIngredientId = (int)db.GetLastInsertId();
            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void addIngredient() */

    public void modifyIngredient(){
        try{
            var db = Database.Open("buSushi");
            var ingredientExec = @"UPDATE Ingredients SET IngredientName = @0, IngredientAltName = @1, TopIngredient = @2,
                                                        Description = @3, fkGroupId = @4, IngredientCostPerPiece = @5,
                                                        IngredientPricePerRoll = @6, Active = @7, MakeARollIngredient = @8
                                                        WHERE pkIngredientId = @9";
            db.Execute(ingredientExec, IngredientName, IngredientAltName, SecondaryIngredient, Description, IGroup.pkGroupId, IngredientCostPerPiece,
                                        IngredientPricePerRoll, Active, MakeARollIngredient, pkIngredientId);
            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void modifyIngredient() */

    public void deleteIngredient(){
        try{
            var db = Database.Open("buSushi");
            var ingredientExec = db.Execute("DELETE FROM Ingredients WHERE pkIngredientId = @0", pkIngredientId);
            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void deleteIngredient() */
}
