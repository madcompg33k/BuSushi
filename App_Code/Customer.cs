using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Customer
/// </summary>

public class Customer : User
{
    public int pkCustomerId { get ; set ; }
    public string FName { get ; set ; }
    public string LName { get ; set ; }
    public DateTime CustomerCreated { get ; set ; }
    public CustomerAddress PrimaryAddress { get ; set ;}
    public CustomerAddress BillingAddress { get ; set ; }
    public string PhoneMain { get ; set ; }
    public string PhoneMainExt { get ; set ; }
    public PhoneType PhoneMainType { get ; set ; }
    public string PhoneSecondary { get ; set ; }
    public string PhoneSecondaryExt { get ; set ; }
    public PhoneType PhoneSecondaryType { get ; set ; }
    public bool MilitaryVerified { get ; set ; }
    
    public List<CustomerAddress> Addresses { get ; set ; }
    public List<CustomerRoll> Rolls { get ; set ; }
    public List<Product> FavoriteProducts { get ; set ; }
    public List<CustomerRoll> FavoriteRolls { get ; set ; }
    public List<Order> SavedOrders { get ; set ; }
    

    public Customer(){
        PrimaryAddress = new CustomerAddress();
        BillingAddress = new CustomerAddress();
        PhoneMainType = new PhoneType();
        PhoneSecondaryType = new PhoneType();

        Addresses = new List<CustomerAddress>();
        Rolls = new List<CustomerRoll>();

        FavoriteProducts = new List<Product>();
        FavoriteRolls = new List<CustomerRoll>();
    }

    public Customer(int UserId) : base(UserId){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM Customers WHERE fkUserId = @0", UserId);
        
        if(query != null){
            pkCustomerId = query.pkCustomerId;
            FName = query.FName;
            LName = query.LName;
            CustomerCreated = query.Created;
            if(query.fkPrimaryAddrId == 0){
                PrimaryAddress = new CustomerAddress(query.fkPrimaryAddrId, pkCustomerId);
            }
            if(query.fkBillingAddrId == 0){
                BillingAddress = new CustomerAddress(query.fkBillingAddrId, pkCustomerId);
            }
            
            PhoneMain = query.PhoneMain;
            PhoneMainExt = query.PhoneMainExt;
            PhoneMainType = new PhoneType(query.PhoneMainType);
            PhoneSecondary = query.PhoneSecondary;
            PhoneSecondaryExt = query.PhoneSecondaryExt;
            PhoneSecondaryType = new PhoneType(query.PhoneSecondaryType);
            MilitaryVerified = query.MilitaryVerified;

            /* Add the customer's addresses */
            Addresses = new List<CustomerAddress>();
            var addrQuery = db.Query(@"SELECT fkAddressId FROM AddressLookup WHERE fkCustomerId = @0", pkCustomerId);
            if(addrQuery.Any()){
                foreach(var row in addrQuery){Addresses.Add(new CustomerAddress(row.fkAddressId, pkCustomerId));}
            }

            /* Add the customer's custom rolls */
            Rolls = new List<CustomerRoll>();
            var rollQuery = db.Query(@"SELECT fkRollId FROM CustomerRollLookup WHERE fkCustomerId = @0", pkCustomerId);
            if(rollQuery.Any()){
                foreach(var row in rollQuery){Rolls.Add(new CustomerRoll(row.fkRollId));}
            }

            /* Get products customer has saved as a favorite */
            FavoriteProducts = new List<Product>();
            var prodQuery = db.Query("SELECT fkProductId FROM Customer_Favorite_Lookup WHERE fkUserId = @0", UserId);
            if(prodQuery.Any()){
                foreach(var row in prodQuery){FavoriteProducts.Add(new Product(row.fkProductId));}
            }

            /* Get customer rolls customer has saved as a favorite */
            FavoriteRolls = new List<CustomerRoll>();
            rollQuery = db.Query("SELECT fkCustomerRollId FROM Customer_Favorite_Lookup WHERE fkUserId = @0", UserId);
            if(rollQuery.Any()){
                foreach(var row in rollQuery){FavoriteRolls.Add(new CustomerRoll(row.fkCustomerRollId));}
            }
        } else{
            PrimaryAddress = new CustomerAddress();
            BillingAddress = new CustomerAddress();
            PhoneMainType = new PhoneType();
            PhoneSecondaryType = new PhoneType();

            Addresses = new List<CustomerAddress>();
            Rolls = new List<CustomerRoll>();

            FavoriteProducts = new List<Product>();
            FavoriteRolls = new List<CustomerRoll>();
        }
        

        /* Close the connection to the database */
        db.Close();

        return;
    }

    /* Return whether or not data was filled when Customer was instantiated */
    public bool hasId(){
        return pkCustomerId == 0 ? false : true;
    }

    public void updateMilitaryStatus(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE Customers SET MilitaryVerified = @0 WHERE pkCustomerId = @1", MilitaryVerified, pkCustomerId);

            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void setPrimaryAddr(CustomerAddress addr){
        try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            /* Delete any references to a primary address before setting the new one */
            db.Execute(@"UPDATE Customers SET fkPrimaryAddrId = 'NULL' WHERE pkCustomerId = @0", pkCustomerId);

            /* Modify base customer information */
            db.Execute(@"UPDATE Customers SET fkPrimaryAddrId = @0 WHERE pkCustomerId = @1", addr.pkAddressId, pkCustomerId);


            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void addCustomer(){
        //try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            int ?primaryAddressId = null;
            if(PrimaryAddress.pkAddressId != 0){primaryAddressId = PrimaryAddress.pkAddressId;}

            int ?billingAddressId = null;
            if(BillingAddress.pkAddressId != 0){billingAddressId = BillingAddress.pkAddressId;}

            int ?phoneMainTypeId = null;
            if(PhoneMainType.pkPhoneTypeId != 0){phoneMainTypeId = PhoneMainType.pkPhoneTypeId;}

            int ?phoneSecondaryTypeId = null;
            if(PhoneSecondaryType.pkPhoneTypeId != 0){phoneSecondaryTypeId = PhoneSecondaryType.pkPhoneTypeId;}

            /* Create the customer information */
            db.Execute(@"INSERT INTO Customers (fkUserId, fkPrimaryAddrId, fkBillingAddrId, FName, LName, Created,
                            PhoneMain, PhoneMainExt, PhoneMainType, PhoneSecondary, PhoneSecondaryExt, PhoneSecondaryType)
                         VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)", UserId, primaryAddressId, billingAddressId,
                         FName, LName, DateTime.Now, PhoneMain, PhoneMainExt, phoneMainTypeId,
                         PhoneSecondary, PhoneSecondaryExt, phoneSecondaryTypeId);
            pkCustomerId = (int)db.GetLastInsertId();
            
            /* Close connection to the database */
            db.Close();

        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyCustomer(){
        //try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            /* Update (base) User information */
            modifyUser();

            if(pkCustomerId == 0){
                addCustomer();
            }else{
                /* Get/Set classes so database updates work properly */
                int ?phoneMainTypeId = null;
                if(PhoneMainType.pkPhoneTypeId != 0){phoneMainTypeId = PhoneMainType.pkPhoneTypeId;}

                int ?phoneSecondaryTypeId = null;
                if(PhoneSecondaryType.pkPhoneTypeId != 0){phoneSecondaryTypeId = PhoneSecondaryType.pkPhoneTypeId;}

                /* Modify customer information */
                db.Execute(@"UPDATE Customers SET FName = @0, LName = @1, PhoneMain = @2, PhoneMainExt = @3, PhoneMainType = @4,
                                PhoneSecondary = @5, PhoneSecondaryExt = @6, PhoneSecondaryType = @7, MilitaryVerified = @8
                                WHERE pkCustomerId = @9", FName, LName, PhoneMain, PhoneMainExt, phoneMainTypeId,
                                PhoneSecondary, PhoneSecondaryExt, phoneSecondaryTypeId, MilitaryVerified, pkCustomerId);

            
            }

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteCustomer(){
        //try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            /* Delete Cart Information */
            foreach(CartItem items in UserCart.CartItems){
                db.Execute("DELETE FROM Order_Option_Lookup WHERE fkCartItemId = @0", items.pkCartItemId);
                db.Execute("DELETE FROM CartItems WHERE pkCartItemId = @0", items.pkCartItemId);
            }
            db.Execute("DELETE FROM Cart WHERE pkCartId = @0", UserCart.pkCartId);

            /* Delete Order Information */
            foreach(Order o in Orders){
                foreach(OrderItem oi in o.OrderItems){
                    db.Execute("DELETE FROM Order_Option_Lookup WHERE fkOrderItemId = @0", oi.pkOrderItemId);
                    db.Execute("DELETE FROM OrderItems WHERE pkOrderItemId = @0", oi.pkOrderItemId);
                }
                db.Execute("DELETE FROM Orders WHERE pkOrderId = @0", o.pkOrderId);
            }

            /* Delete All Lookup and Reference Information */
            db.Execute("DELETE FROM User_IP_Lookup WHERE fkUserId = @0", UserId);
            db.Execute("DELETE FROM Notification_Lookup WHERE fkUserId = @0", UserId);
            db.Execute("DELETE FROM Customer_Following_Lookup WHERE fkUserId = @0 OR fkFollowingId = @1", UserId, UserId);
            db.Execute("DELETE FROM Customer_Favorite_Lookup WHERE fkUserId = @0", UserId);
            db.Execute("DELETE FROM CustomerRollLookup WHERE fkCustomerId = @0", pkCustomerId);
            db.Execute("DELETE FROM AddressLookup WHERE fkCustomerId = @0", pkCustomerId);
            db.Execute("DELETE FROM DeliveryReadyAlert WHERE fkUserId = @0", UserId);
            db.Execute("DELETE FROM DeliveryReadyAlert WHERE EMail = @0", Email);
            db.Execute("DELETE FROM webpages_UsersInRoles WHERE UserId = @0", UserId);

            /* Delete Core User/Customer Data */
            db.Execute("DELETE FROM Customers WHERE pkCustomerId = @0", pkCustomerId);
            db.Execute("DELETE FROM UserProfile WHERE UserId = @0", UserId);
            db.Execute("DELETE FROM webpages_Membership WHERE UserId = @0", UserId);


            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }
}
