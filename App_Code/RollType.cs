using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;


/// <summary>
/// Summary description for RollType
/// </summary>

public class RollType
{
    public int pkRollTypeId { get; set; }
    public string RollTypeName { get; set; }
    public string RollTypeDescription { get ; set ; }
    public bool hasSecondaryIngredients { get ; set ; }
    public int IngredientsAllowed { get ; set ; }
    public int SecondaryIngredientsAllowed { get ; set ; }
    public int fkSushiTypeId { get ; set ; }


    public RollType(){}

    public RollType(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM RollTypes WHERE pkRollTypeId = @0", Id);

        pkRollTypeId = query.pkRollTypeId;
        RollTypeName = query.RollTypeName;
        RollTypeDescription = query.RollTypeDescription;
        hasSecondaryIngredients = query.hasSecondaryIngredients;
        IngredientsAllowed = query.IngredientsAllowed;
        SecondaryIngredientsAllowed = query.SecondaryIngredientsAllowed;
        fkSushiTypeId = query.fkSushiTypeId;

        db.Close();
        return;
    }

    public void addRollType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add roll type to the database */
            var query = @"INSERT INTO RollTypes (RollTypeName, RollTypeDescription, hasSecondaryIngredients, IngredientsAllowed,
                                                 SecondaryIngredientsAllowed, fkSushiTypeId)
                        VALUES (@0, @1, @2, @3, @4, 1)";
            db.Execute(query, RollTypeName, RollTypeDescription, hasSecondaryIngredients, IngredientsAllowed, SecondaryIngredientsAllowed);
            /* Set pkRollTypeId to the newly added ID */
            pkRollTypeId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyRollType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify roll type information in the database */
            var query = @"UPDATE RollTypes SET RollTypeName = @0, RollTypeDescription = @1, hasSecondaryIngredients = @2,
                                 IngredientsAllowed = @3, SecondaryIngredientsAllowed = @4, fkSushiTypeId = 1";
            db.Execute(query, RollTypeName, RollTypeDescription, hasSecondaryIngredients, IngredientsAllowed, SecondaryIngredientsAllowed);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteRollType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the database */
            db.Execute("DELETE FROM RollTypes WHERE pkRollTypeId = @0", pkRollTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
