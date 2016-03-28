using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for OrderStatus
/// </summary>
public class OrderStatus
{
    public int pkOrderStatusId { get ; set ; }
    public string Status { get ; set ; }
    public string StatusDescription { get ; set ; }

    public OrderStatus(){}

    public OrderStatus(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");
        /* Retrieve data from the database */
        var query = db.QuerySingle(@"SELECT * FROM OrderStatus WHERE pkOrderStatusId = @0", Id);

        /* Assign Data */
        pkOrderStatusId = Id;
        Status = query.Status;
        StatusDescription = query.StatusDescription;

        /* Close connection to the database */
        db.Close();

        /* Return the data */
        return;
    }

    public void addOrderStatus(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Insert the new data into the database */
            db.Execute("INSERT INTO OrderStatus (Status, StatusDescription) VALUES (@0, @1)", Status, StatusDescription);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyOrderStatus(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify the database */
            db.Execute("UPDATE OrderStatus SET Status = @0, StatusDescription = @1 WHERE pkOrderStatusId = @2", Status, StatusDescription, pkOrderStatusId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteOrderStatus(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete the data from the database */
            db.Execute("DELETE FROM OrderStatus WHERE pkOrderStatusId = @0", pkOrderStatusId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
