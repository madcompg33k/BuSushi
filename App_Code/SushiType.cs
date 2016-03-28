using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for SushiType
/// </summary>

public class SushiType
{
    public int pkSushiTypeId { get; set; }
    public int fkFoodTypeId { get ; set ; }
    public string SushiTypeName { get; set; }
    public string SushiTypeDescription { get ; set ; }

    public SushiType(){}
    public SushiType(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM SushiTypes WHERE pkSushiTypeId = @0", Id);

        pkSushiTypeId = query.pkSushiTypeId;
        fkFoodTypeId = query.fkFoodTypeId;
        SushiTypeName = query.SushiTypeName;
        SushiTypeDescription = query.SushiTypeDescription;

        db.Close();
        return;
    }


    public void addSushiType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add sushi type to the database */
            db.Execute("INSERT INTO SushiTypes (fkFoodTypeId, SushiTypeName, SushiTypeDescription) VALUES (1, @0, @1)", SushiTypeName, SushiTypeDescription);
            /* Set pkSushiTypeId to the newly added ID */
            pkSushiTypeId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifySushiType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify sushi type information in the database */
            db.Execute(@"UPDATE SushiTypes SET fkFoodTypeId = 1, SushiTypeName = @0, SushiTypeDescription = @1
                         WHERE pkSushiTypeId = @2", SushiTypeName, SushiTypeDescription, pkSushiTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteSushiType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the database */
            db.Execute("DELETE FROM SushiTypes WHERE pkSushiTypeId = @0", pkSushiTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
