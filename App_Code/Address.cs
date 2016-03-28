using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Address
/// </summary>
public class Address
{
    public int pkAddressId { get ; set ; }
    public string Address1 { get ; set ; }
    public string Address2 { get ; set ; }
    public City AddrCity { get ; set ; }


    public Address(){AddrCity = new City();}
    public Address(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT a.*, c.pkCityId, s.pkStateId FROM Addresses a
                            INNER JOIN Cities c ON c.pkCityId = a.fkCityId
                            INNER JOIN States s ON s.pkStateId = c.fkStateId
                            WHERE a.pkAddressId = @0", Id);
        if(query != null){
            pkAddressId = query.pkAddressId;
            Address1 = query.Address1;
            Address2 = query.Address2;
            AddrCity = new City(query.pkCityId);
        }else{AddrCity = new City();}
        
        db.Close();
        return;
    }

    public void addAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            int ?Id = db.QueryValue(@"SELECT pkAddressId FROM Addresses a1 WHERE a1.Address1 = @0 AND a1.Address2 = @1
                                            AND a1.fkCityId = @2", Address1.Replace(".", string.Empty).Trim(),
                                            Address2.Replace(".", string.Empty).Trim(), AddrCity.pkCityId);
            if(Id == null){
                /* Insert the data into the database */
                db.Execute("INSERT INTO Addresses (Address1, Address2, fkCityId) VALUES (@0, @1, @2)",
                                Address1.Replace(".", string.Empty).Trim(),
                                Address2.Replace(".", string.Empty).Trim(), AddrCity.pkCityId);
                /* Get the newly added ID */
                pkAddressId = (int)db.GetLastInsertId();
            } else{pkAddressId = db.QueryValue(@"SELECT pkAddressId FROM Addresses a1 WHERE a1.Address1 = @0 AND a1.Address2 = @1
                                            AND a1.fkCityId = @2", Address1.Replace(".", string.Empty).Trim(),
                                            Address2.Replace(".", string.Empty).Trim(), AddrCity.pkCityId);}
            

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify the database */
            db.Execute("UPDATE Addresses SET Address1 = @0, Address2 = @1, fkCityId = @2 WHERE pkAddressId = @3",
            Address1.Replace(".", string.Empty).Trim(), Address2.Replace(".", string.Empty).Trim(), AddrCity.pkCityId, pkAddressId);

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteAddress(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete the data from the database */
            db.Execute("DELETE FROM Addresses WHERE pkAddressId = @0", pkAddressId);

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }
}
