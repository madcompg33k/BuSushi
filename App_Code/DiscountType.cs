using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for DiscountType
/// </summary>
public class DiscountType
{
    public int pkDiscountTypeId { get ; set ; }
    public string DiscountTypeName { get ; set ; }

    public DiscountType(){}

    public DiscountType(int Id){
        var db = Database.Open("buSushi");

        var query = db.QuerySingle(@"SELECT * FROM DiscountTypes WHERE pkDiscountTypeId = @0", Id);
        pkDiscountTypeId = Id;
        DiscountTypeName = query.DiscountType;

        db.Close();
        return;
    }

    public void addDiscountType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("INSERT INTO DiscountTypes (DiscountType) VALUES (@0)", DiscountTypeName);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyDiscountType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE DiscountTypes SET DiscountType = @0 WHERE pkDiscountTypeId = @1", DiscountTypeName, pkDiscountTypeId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteDiscountType(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM DiscountTypes WHERE pkDiscountTypeId = @0", pkDiscountTypeId);
        }catch(Exception e){Console.Write(e.Message);}
    }
}
