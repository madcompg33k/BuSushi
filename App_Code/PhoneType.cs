using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for PhoneType
/// </summary>
public class PhoneType
{
    public int pkPhoneTypeId { get ; set ; }
    public string PhoneTypeName { get ; set ; }
    public string PhoneTypeDescription { get ; set ; }

    public PhoneType(){}

    public PhoneType(int ?Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM PhoneTypes WHERE pkPhoneTypeId = @0", Id);

        if(query != null){
            pkPhoneTypeId = query.pkPhoneTypeId;
            PhoneTypeName = query.PhoneType;
            PhoneTypeDescription = query.PhoneTypeDescription;
        }

        db.Close();
        return;
    }

    public void addPhoneType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("INSERT INTO PhoneTypes (PhoneType, PhoneTypeDescription) VALUES (@0, @1)", PhoneTypeName, PhoneTypeDescription);
            pkPhoneTypeId = (int)db.GetLastInsertId();
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyPhoneType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE PhoneTypes SET PhoneType = @0, PhoneTypeDescription = @1 WHERE pkPhoneTypeId = @2", PhoneTypeName, PhoneTypeDescription, pkPhoneTypeId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deletePhoneType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM PhoneTypes WHERE pkPhoneTypeId = @0", pkPhoneTypeId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
