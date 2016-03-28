using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for ProductOption
/// </summary>
public class ProductOption
{
    public int pkProductOptionId { get ; set ; }
    public string OptionName { get ; set ; }
    public string OptionDescription { get ; set ; }
    public ProductOptionGroup OGroup { get ; set ; }
    public bool Active { get ; set ; }

    public ProductOption(){
        OGroup = new ProductOptionGroup();
        Active = true;
    }
    
    public ProductOption(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");

        /* Get option data */
        var query = db.QuerySingle(@"SELECT * FROM ProductOptions WHERE pkProductOptionId = @0", Id);
        /* Check if data was returned, then assign data */
        if(query != null){
            pkProductOptionId = query.pkProductOptionId;
            OGroup = new ProductOptionGroup(query.fkProductOptionGroupId);
            OptionName = query.OptionName;
            OptionDescription = query.OptionDescription;
            Active = query.Active;
        }

        /* Close the connection to the database */
        db.Close();
        /* Return the object */
        return;
    }


    /* Add new product option to the database */
    public void addProductOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Insert new data into the database */
            db.Execute("INSERT INTO ProductOptions (fkProductOptionGroupId, OptionName, OptionDescription, Active) VALUES (@0, @1, @2, @3)",
                            OGroup.pkProductOptionGroupId, OptionName, OptionDescription, Active);
            /* Set unique Id to the newly inserted Id */
            pkProductOptionId = (int)db.GetLastInsertId();
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Modify product option in the database */
    public void modifyProductOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Update information in the database */
            db.Execute("UPDATE ProductOptions SET fkProductOptionGroupId = @0, OptionName = @1, OptionDescription = @2, Active = @3 WHERE pkProductOptionId = @4",
                        OGroup.pkProductOptionGroupId, OptionName, OptionDescription, Active, pkProductOptionId);
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Delete product option from the database */
    public void deleteProductOption(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Delete all lookup information first */
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkProductOptionId = @0", pkProductOptionId);
            /* Delete option from the database */
            db.Execute("DELETE FROM ProductOptions WHERE pkProductOptionId = @0", pkProductOptionId);
            
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
