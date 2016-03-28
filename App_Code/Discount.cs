using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Discount
/// </summary>
public class Discount
{
    public int pkDiscountId { get ; set ; }
    public DiscountType DType { get ; set ; }
    public decimal DiscountAmt { get ; set ; }
    public decimal DiscountDollar { get ; set ; }
    public string DiscountCode { get ; set ; }
    public string DiscountReason { get ; set ; }
    public DateTime ?DiscountStartDate { get ; set ; }
    public DateTime ?DiscountEndDate { get ; set ; }
    public bool isActive { get ; set ; }
    public Role MinRoleRequired { get ; set ; }
    public int MaxUses { get ; set ; }
    public int NumUses { get ; set ; }
    public Product ReferenceProduct { get ; set ; }

    public Discount(){
        DType = new DiscountType();
        MinRoleRequired = new Role();
        ReferenceProduct = new Product();
    }

    public Discount(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM Discounts WHERE pkDiscountId = @0", Id);

        pkDiscountId = Id;
        DType = new DiscountType(query.fkDiscountType);
        DiscountAmt = query.DiscountAmt;
        DiscountCode = query.DiscountCode;
        DiscountReason = query.DiscountReason;
        DiscountStartDate = query.DiscountStartDate;
        DiscountEndDate = query.DiscountEndDate;
        isActive = query.isActive;
        MinRoleRequired = new Role(query.fkMinRoleId);
        MaxUses = query.MaxUses;
        NumUses = query.NumUses;
        
        if(query.fkProductId != null){ReferenceProduct = new Product(query.fkProductId);}
        else{ReferenceProduct = new Product();}
        
        db.Close();
        return;
    }


    public void getDiscountDollar(decimal price){
        decimal amount = 0;
        /* Figure out functionality for Buy 1 (or 2) / Get 1 Free */
        if(string.Equals(DType.DiscountTypeName, "Percent")){ amount = price * DiscountAmt; }
        else{ amount = DiscountAmt; }
        DiscountDollar = amount;
    }

    public bool hasId(){
        return pkDiscountId == 0 ? false : true;
    }

    public void addDiscount(){
        try{
            int ?productId = null;
            if(ReferenceProduct.pkProductId != 0){productId = ReferenceProduct.pkProductId;}

            var db = Database.Open("buSushi");
            db.Execute(@"INSERT INTO Discounts (fkDiscountType, DiscountAmt, DiscountCode, DiscountReason, DiscountStartDate, DiscountEndDate, isActive, fkMinRoleId,
                            MaxUses, NumUses, fkProductId) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10)",
                            DType.pkDiscountTypeId, DiscountAmt, DiscountCode, DiscountReason, DiscountStartDate, DiscountEndDate,
                            isActive, MinRoleRequired.RoleId, MaxUses, NumUses, productId);
            pkDiscountId = (int)db.GetLastInsertId();
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyDiscount(){
        try{
            int ?productId = null;
            if(ReferenceProduct.pkProductId != 0){productId = ReferenceProduct.pkProductId;}

            var db = Database.Open("buSushi");
            db.Execute(@"UPDATE Discounts SET fkDiscountType = @0, DiscountAmt = @1, DiscountCode = @2, DiscountReason = @3, DiscountStartDate = @4,
                            DiscountEndDate = @5, isActive = @6, fkMinRoleId = @7, MaxUses = @8, NumUses = @9, fkProductId = @10 WHERE pkDiscountId = @11",
                            DType.pkDiscountTypeId, DiscountAmt, DiscountCode, DiscountReason, DiscountStartDate, DiscountEndDate, isActive,
                            MinRoleRequired.RoleId, MaxUses, NumUses, productId, pkDiscountId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deactivateDiscount(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE Discounts SET isActive = @0 WHERE pkDiscountId = @1", isActive, pkDiscountId);
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteDiscount(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM Discounts WHERE pkDiscountId = @0", pkDiscountId);
        }catch(Exception e){Console.Write(e.Message);}
    }
}
