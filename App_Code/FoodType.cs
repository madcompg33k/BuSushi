using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for FoodType
/// </summary>
public class FoodType
{
    public int pkFoodTypeId { get ; set ; }
    public string FoodTypeName { get ; set ; }
    public string FoodTypeDescription { get ; set ; }
    public int fkProductTypeId { get ; set ; }

    public FoodType(){}

    public FoodType(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM FoodTypes WHERE pkFoodTypeId = @0", Id);
        
        /* Set data */
        pkFoodTypeId = query.pkFoodTypeId;
        FoodTypeName = query.FoodTypeName;
        FoodTypeDescription = query.FoodTypeDescription;
        fkProductTypeId = query.fkProductTypeId;

        db.Close();
        return;
    }
    
    public void addFoodType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add food type to the database */
            db.Execute("INSERT INTO FoodTypes (FoodTypeName, FoodTypeDescription, fkProductTypeId) VALUES (@0, @1, 1)", FoodTypeName, FoodTypeDescription);
            /* Set pkFoodTypeId to the newly added ID */
            pkFoodTypeId = db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyFoodType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify food type information in the database */
            db.Execute("UPDATE FoodTypes SET FoodTypeName = @0, FoodTypeDescription = @1, fkProductTypeId = 1, WHERE pkFoodTypeId = @2", FoodTypeName, FoodTypeDescription, pkFoodTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteFoodType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the database */
            db.Execute("DELETE FROM FoodTypes WHERE pkFoodTypeId = @0", pkFoodTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
