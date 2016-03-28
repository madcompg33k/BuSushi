using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for CustomerAddress
/// </summary>
public class CustomerAddress : Address
{
    public int pkAddressLookupId { get ; set ; }
    public int fkCustomerId { get ; set ; }
    public bool isBilling { get ; set ; }
    public bool isPrimary { get ; set ; }
    public string Nickname { get ; set ; }
    public string SpInstructions { get ; set ; }

    public CustomerAddress(){}

    public CustomerAddress(int AddressLookupId){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM AddressLookup WHERE pkAddressLookupId = @0", AddressLookupId);

        if(query != null){
            pkAddressLookupId = query.pkAddressLookupId;
            fkCustomerId = query.fkCustomerId;
            isBilling = query.isBilling;
            isPrimary = query.isPrimary;
            Nickname = query.Nickname;
            SpInstructions = query.SpInstructions;

            Address thisAddress = new Address(query.fkAddressId);
            pkAddressId = query.fkAddressId;
            Address1 = thisAddress.Address1;
            Address2 = thisAddress.Address2;
            AddrCity = new City(thisAddress.AddrCity.pkCityId);
        }

        db.Close();
        return;
    }

    public CustomerAddress(int AddressId, int CustomerId) : base(AddressId){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM AddressLookup WHERE fkAddressId = @0 AND fkCustomerId = @1", AddressId, CustomerId);

        if(query != null){
            pkAddressLookupId = query.pkAddressLookupId;
            fkCustomerId = query.fkCustomerId;
            isBilling = query.isBilling;
            isPrimary = query.isPrimary;
            Nickname = query.Nickname;
            SpInstructions = query.SpInstructions;
        }

        db.Close();
        return;
    }

    public void setPrimaryAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Set address lookup information to primary in the database */
            db.Execute("UPDATE AddressLookup SET isPrimary = 'True' WHERE pkAddressLookupId = @0", pkAddressId);

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void setBillingAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Set address lookup information to billing in the database */
            db.Execute("UPDATE AddressLookup SET isBilling = 'True' WHERE pkAddressLookupId = @0", pkAddressId);

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void addCustomerAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add primary address information first */
            addAddress();

            if(isPrimary){db.Execute("UPDATE AddressLookup SET isPrimary = 'False' WHERE fkCustomerId = @0 AND isPrimary = 'True'", fkCustomerId);}
            if(isBilling){db.Execute("UPDATE AddressLookup SET isBilling = 'False' WHERE fkCustomerId = @0 AND isBilling = 'True'", fkCustomerId);}

            /* Insert the data into the database */
            db.Execute(@"INSERT INTO AddressLookup (fkCustomerId, fkAddressId, isBilling, isPrimary, Nickname, SpInstructions)
                            VALUES (@0, @1, @2, @3, @4, @5)", fkCustomerId, pkAddressId, isBilling, isPrimary, Nickname, SpInstructions);
            /* Get the newly added ID */
            pkAddressLookupId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyCustomerAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify the database */
            db.Execute(@"UPDATE AddressLookup SET fkCustomerId = @0, fkAddressId = @1, isBilling = @2, isPrimary = @3, Nickname = @4, SpInstructions @5
                            WHERE pkAddressLookupId = @6", fkCustomerId, pkAddressId, isBilling, isPrimary, Nickname, SpInstructions, pkAddressLookupId);

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteCustomerAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete the data from the database */
            db.Execute("DELETE FROM AddressLookup WHERE pkAddressLookupId = @0", pkAddressLookupId);
            
            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }
}
