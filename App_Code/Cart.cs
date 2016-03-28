using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Cart
/// </summary>
public class Cart
{
    public int pkCartId { get ; set ; }
    public int ?fkUserId { get ; set ; }
    public DateTime DateCreated { get ; set ; }
    public bool CheckedOut { get ; set ; }

    public List<CartItem> CartItems { get ; set ; }

    public Cart(){
        fkUserId = WebSecurity.CurrentUserId;
        CartItems = new List<CartItem>();
        DateCreated = DateTime.Now;
    }

    public Cart(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM Cart WHERE pkCartId = @0", Id);

        pkCartId = Id;
        fkUserId = query.fkUserId;
        DateCreated = query.DateCreated;
        CheckedOut = query.CheckedOut;

        CartItems = new List<CartItem>();
        query = db.Query(@"SELECT pkCartItemId FROM CartItems WHERE fkCartId = @0", Id);
        foreach(var row in query){CartItems.Add(new CartItem(row.pkCartItemId));}

        db.Close();
        return;
    }

    public void addCart(){
        try{
            /* Open the connection to the database */
            var db = Database.Open("buSushi");
            
            /* Insert the new data into the database */
            db.Execute("INSERT INTO Cart (fkUserId, DateCreated, CheckedOut) VALUES (@0, @1, @2)", fkUserId, DateCreated, CheckedOut);
            /* Get the newly added ID */
            pkCartId = (int)db.GetLastInsertId();

            /* Close the connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyCart(){
        try{
            /* Open the connection to the database */
            var db = Database.Open("buSushi");

            /* Update the information in the database */
            db.Execute(@"UPDATE Cart SET fkUserId = @0, DateCreated = @1, CheckedOut @2 WHERE pkCartId = @3", fkUserId, DateCreated, CheckedOut, pkCartId);

            /* Close the connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteCart(){
        try{
            /* Open the connection to the database */
            var db = Database.Open("buSushi");

            /* First delete any cartitems tied to this cart */
            db.Execute("DELETE FROM CartItems WHERE fkCartId = @0", pkCartId);

            /* Delete the cart from the database */
            db.Execute("DELETE FROM Cart WHERE pkCartId = @0", pkCartId);

            /* Close the connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
