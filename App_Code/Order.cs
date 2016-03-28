using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Order
/// </summary>
public class Order
{
    public int pkOrderId { get ; set ; }
    public int fkUserId { get ; set ; }
    public DateTime DateTimePlaced { get ; set ; }
    public DateTime DateTimeModified { get ; set ; }
    public OrderStatus OStatus { get ; set ; }
    public DeliveryScheduleItem DeliveryDay { get ; set ; }
    public CustomerAddress OAddress { get ; set ; }
    public bool Favorite { get ; set ; }

    public List<OrderItem> OrderItems { get ; set ; }

    public Order()
    {
        OStatus = new OrderStatus();
        DeliveryDay = new DeliveryScheduleItem();
        OAddress = new CustomerAddress();
        OrderItems = new List<OrderItem>();
    }

    public Order(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Get data from the database */
        var query = db.QuerySingle("SELECT * FROM Orders WHERE pkOrderId = @0", Id);
        var customerId = db.QueryValue("SELECT pkCustomerId FROM Customers WHERE fkUserId = @0", query.fkUserId);
        /* Assign data */
        pkOrderId = Id;
        fkUserId = query.fkUserId;
        DateTimePlaced = query.DateTimePlaced;
        DateTimeModified = query.DateTimeModified;
        OStatus = new OrderStatus(query.fkOrderStatusId);
        DeliveryDay = new DeliveryScheduleItem(query.fkDeliveryId);
        OAddress = new CustomerAddress(query.fkAddressLookupId,customerId);
        Favorite = query.Favorite;

        OrderItems = new List<OrderItem>();
        query = db.Query(@"SELECT * FROM OrderItems WHERE fkOrderId = @0", pkOrderId);
        foreach(var item in query){OrderItems.Add(new OrderItem(item.pkOrderItemId));}

        /* Close connection to the database */
        db.Close();
        /* Return the data */
        return;
    }

    public void addOrder(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Insert the data into the database */
            db.Execute(@"INSERT INTO Orders (fkUserId, DateTimePlaced, DateTimeModified, fkOrderStatusId, fkDeliveryId, fkAddressLookupId, Favorite)
                            VALUES (@0, @1, @2, @3, @4, @5, @6)", fkUserId, DateTimePlaced, DateTimeModified, OStatus.pkOrderStatusId,
                            DeliveryDay.pkDeliveryId, OAddress.pkAddressLookupId, Favorite);
            /* Get the newly added ID */
            pkOrderId = db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyOrder(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify the database */
            db.Execute(@"UPDATE Orders SET fkUserId = @0, DateTimePlaced = @1, DateTimeModified = @2, fkOrderStatusId = @3, fkDeliveryId = @4,
                            fkAddressLookupId = @5, Favorite = @6 WHERE pkOrderId = @7", fkUserId, DateTimePlaced, DateTimeModified, OStatus.pkOrderStatusId,
                            DeliveryDay.pkDeliveryId, OAddress.pkAddressLookupId, pkOrderId, Favorite);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteOrder(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* First delete OrderItems tied to this order */
            db.Execute("DELETE FROM OrderItems WHERE fkOrderId = @0", pkOrderId);

            /* Delete the order from the database */
            db.Execute("DELETE FROM Orders WHERE pkOrderId = @0", pkOrderId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
