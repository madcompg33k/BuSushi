using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for ProductType
/// </summary>
public class ProductType
{
    public int pkProductTypeId { get ; set ; }
    public string ProductTypeName { get ; set ; }
    public string ProductTypeDescription { get ; set ; }

    public ProductType(){}
    public ProductType(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM ProductTypes WHERE pkProductTypeId = @0", Id);

        pkProductTypeId = query.pkProductTypeId;
        ProductTypeName = query.ProductTypeName;
        ProductTypeDescription = query.ProductTypeDesc;

        db.Close();
        return;
    }

    public void addProductType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add sushi type to the database */
            db.Execute("INSERT INTO ProductTypes (ProductTypeName, ProductTypeDescription) VALUES (@0, @1)", ProductTypeName, ProductTypeDescription);
            /* Set pkFoodTypeId to the newly added ID */
            pkProductTypeId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyProductType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify sushi type information in the database */
            db.Execute(@"UPDATE ProductTypes SET ProductTypeName = @0, ProductTypeDescription = @1
                         WHERE pkProductTypeId = @2", ProductTypeName, ProductTypeDescription, pkProductTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteProductType(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the database */
            db.Execute("DELETE FROM ProductTypes WHERE pkProductTypeId = @0", pkProductTypeId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
