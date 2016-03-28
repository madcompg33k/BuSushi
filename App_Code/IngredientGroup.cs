using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;


/// <summary>
/// Summary description for IngredientGroup
/// </summary>

public class IngredientGroup
{
    public int pkGroupId { get; set; }
    public string GroupName { get; set; }
    public string GroupDescription { get ; set ; }

    public IngredientGroup(){}

    public IngredientGroup(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM IngredientGroups WHERE pkGroupId = @0", Id);

        pkGroupId = query.pkGroupId;
        GroupName = query.GroupName;
        GroupDescription = query.GroupDescription;

        db.Close();
        return;
    }


    public void addIngredientGroup(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("INSERT INTO IngredientGroups (GroupName, GroupDescription) VALUES (@0, @1)", GroupName, GroupDescription);
            /* Set pkProductId to the newly added ID */
            pkGroupId = db.GetLastInsertId();

            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void addIngredientGroup() */

    public void modifyIngredientGroup(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE IngredientGroups SET GroupName = @0, GroupDescription = @1 WHERE pkGroupId = @2", GroupName, GroupDescription, pkGroupId);

            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void modifyIngredientGroup() */

    public void deleteIngredientGroup(){
        try{
            var db = Database.Open("buSushi");
            var ingredientExec = db.Execute("DELETE FROM IngredientGroups WHERE pkGroupId = @0", pkGroupId);
            db.Close();
        } catch (Exception e){Console.Write(e.Message);}
    } /* #END void deleteIngredientGroup() */
}
