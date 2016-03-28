using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for OrderItem
/// </summary>
public class OrderItem
{
    public int pkOrderItemId { get ; set ; }
    public int fkOrderId { get ; set ; }
    public Product CIProduct { get ; set ; }
    public CustomerRoll CICustomerRoll { get ; set ; }
    public double Price { get ; set ; }
    public Discount CIDiscount { get ; set ; }
    public int Qty { get ; set ; }
    public int fkTaxZoneId { get ; set ; }
    public string Instructions { get ; set ; }
    public bool CateringItem { get ; set ; }
    public int ?fkMixNMatchRollId { get ; set ; }
    public bool isMixNMatchCustomerRoll { get ; set ; }
    public bool HasCrunch { get ; set ; }

    public List<CookingStyle> CookingStyles { get ; set ; }

    public OrderItem(){
        CookingStyles = new List<CookingStyle>();
        CIProduct = new Product();
        CICustomerRoll = new CustomerRoll();
        CIDiscount = new Discount();
    }
    
    public OrderItem(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM OrderItems WHERE pkOrderItemId = @0", Id);

        pkOrderItemId = Id;
        if (query.fkProductId != null){CIProduct = new Product(query.fkProductId);}
        else{CICustomerRoll = new CustomerRoll(query.fkRollId);}

        Price = query.Price;
        CIDiscount = new Discount(query.fkDiscountType);
        Qty = query.Qty;
        fkTaxZoneId = query.fkTaxZoneId;
        Instructions = query.Instructions;
        CateringItem = query.CateringItem;
        fkMixNMatchRollId = query.fkMixNMatchRollId;
        isMixNMatchCustomerRoll = query.isMixNMatchCustomerRoll;
        HasCrunch = query.HasCrunch;

        query = db.Query(@"SELECT fkCookingStyleId, fkOrderItemId FROM CookingStyleLookup WHERE fkOrderItemId = @0", Id);
        foreach(var row in query){CookingStyles.Add(new CookingStyle(row.fkCookingStyleId));}

        db.Close();
        return;
    }

    public void addOrderItem(){
        try{
            var db = Database.Open("buSushi");
            db.Execute(@"INSERT INTO OrderItems (fkOrderId, fkProductId, fkRollId, Price, fkDiscountId, Qty, fkTaxZoneId, Instructions,
                            CateringItem, fkMixNMatchRollId, isMixNMatchCustomerRoll, HasCrunch) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)",
                            fkOrderId, CIProduct.pkProductId, CICustomerRoll.pkCustomerRollId, Price, CIDiscount.pkDiscountId, Qty,
                            fkTaxZoneId, Instructions, CateringItem, fkMixNMatchRollId, isMixNMatchCustomerRoll, HasCrunch);
            /* Get the newly added ID */
            pkOrderItemId = db.GetLastInsertId();

            /* Re-add new lookup information */
            foreach(var row in CookingStyles){
                db.Execute("INSERT INTO CookingStyleLookup (fkCookingStyleId, fkOrderItemId) VALUES (@0, @1)", row.pkCookingStyleId, pkOrderItemId);
            }
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyOrderItem(){
        try{
            var db = Database.Open("buSushi");
            db.Execute(@"UPDATE OrderItems SET fkOrderId = @0, fkProductId = @1, fkRollId = @2, Price = @3, fkDiscountId = @4, Qty = @5,
                            fkTaxZoneId = @6, Instructions = @7, CateringItem = @8, fkMixNMatchRollId = @9, isMixNMatchCustomerRoll = @10,
                            HasCrunch = @11 WHERE pkOrderItemId = @12", fkOrderId, CIProduct.pkProductId, CICustomerRoll.pkCustomerRollId,
                            Price, CIDiscount.pkDiscountId, Qty, fkTaxZoneId, Instructions, CateringItem, fkMixNMatchRollId,
                            isMixNMatchCustomerRoll, pkOrderItemId, HasCrunch);
            
            /* Delete all lookup information */
            db.Execute("DELETE FROM CookingStyleLookup WHERE fkOrderItemId = @0", pkOrderItemId);

            /* Re-add new lookup information */
            foreach(var row in CookingStyles){
                db.Execute("INSERT INTO CookingStyleLookup (fkCookingStyleId, fkOrderItemId) VALUES (@0, @1)", row.pkCookingStyleId, pkOrderItemId);
            }
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteOrderItem(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM OrderItems WHERE pkOrderItemId = @0", pkOrderItemId);

            /* Delete all lookup information */
            db.Execute("DELETE FROM CookingStyleLookup WHERE fkOrderItemId = @0", pkOrderItemId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
