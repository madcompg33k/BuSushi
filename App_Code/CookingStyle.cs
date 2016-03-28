using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for CookingStyle
/// </summary>
public class CookingStyle
{
    public int pkCookingStyleId { get ; set ; }
    public string Name { get ; set ; }
    public string Description { get ; set ; }
    public int Price { get ; set ; }

    public CookingStyle(){}

    public CookingStyle(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM CookingStyles WHERE pkCookingStyleId = @0", Id);

        pkCookingStyleId = query.pkCookingStyleId;
        Name = query.Name;
        Description = query.Description;
        Price = query.Price;

        db.Close();
        return;
    }

    public void addCookingStyle(){
        try{
            var db = Database.Open("buSushi");
            var sql = "INSERT INTO CookingStyles (Name, Description, Price) VALUES (@0, @1, @2)";
            db.Execute(sql, Name, Description, Price);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    } /* #END addCookingStyle() */

    public void modifyCookingStyle(){
        try{
            var db = Database.Open("buSushi");
            var sql = "UPDATE CookingStyles SET Name = @0, Description = @1, Price = @2 WHERE pkCookingStyleId = @3";
            db.Execute(sql, Name, Description, Price, pkCookingStyleId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    } /* #END modifyCookingStyle() */

    public void deleteCookingStyle(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM CookingStyles WHERE pkCookingStyleId = @0", pkCookingStyleId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    } /* #END deleteCookingStyle() */
}
