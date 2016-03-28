using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for OrderOption
/// </summary>
public class OrderOption
{
    public int pkOptionId { get ; set ; }
    public string OptionName { get ; set ; }
    public string OptionDescription { get ; set ; }
    public decimal OptionPrice { get ; set ; }
    public decimal OptionCost { get ; set ; }
    public bool Active { get ; set ; }

    public OrderOption(){
        Active = true;
    }
    
    public OrderOption(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Get option data */
        var query = db.QuerySingle(@"SELECT * FROM OrderOptions WHERE pkOptionId = @0", Id);
        /* Check if data was returned, then assign data */
        if(query != null){
            pkOptionId = query.pkOptionId;
            OptionName = query.OptionName;
            OptionDescription = query.OptionDescription;
            OptionPrice = query.OptionPrice;
            OptionCost = query.OptionCost;
            Active = query.Active;
        }

        /* Close the connection to the database */
        db.Close();
        /* Return the object */
        return;
    }

    /* Add new order option to the database */
    public void addOrderOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Insert new data into the database */
            db.Execute("INSERT INTO OrderOptions (OptionName, OptionDescription, OptionCost, OptionPrice, Active) VALUES (@0, @1, @2, @3, @4)",
                            OptionName, OptionDescription, OptionCost, OptionPrice, Active);
            /* Set unique Id to the newly inserted Id */
            pkOptionId = (int)db.GetLastInsertId();
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Modify order option in the database */
    public void modifyOrderOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Update information in the database */
            db.Execute("UPDATE OrderOptions SET OptionName = @0, OptionDescription = @1, OptionCost = @2, OptionPrice = @3, Active = @4 WHERE pkOptionId = @5",
                            OptionName, OptionDescription, OptionCost, OptionPrice, Active, pkOptionId);
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Delete order option from the database */
    public void deleteOrderOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Delete all lookup information first */
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkOptionId = @0", pkOptionId);
            /* Delete option from the database */
            db.Execute("DELETE FROM OrderOptions WHERE pkOptionId = @0", pkOptionId);
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
