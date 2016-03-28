using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for ProductOptionGroup
/// </summary>
public class ProductOptionGroup
{
    public int pkProductOptionGroupId { get ; set ; }
    public string GroupName { get ; set ; }
    public string GroupDescription { get ; set ; }

    public ProductOptionGroup(){}

    public ProductOptionGroup(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");
        /* Retrieve data from the database */
        var query = db.QuerySingle("SELECT * FROM ProductOptionGroups WHERE pkProductOptionGroupId = @0", Id);

        if(query != null){
            pkProductOptionGroupId = query.pkProductOptionGroupId;
            GroupName = query.GroupName;
            GroupDescription = query.GroupDescription;
        }
        /* Close connection to the database */
        db.Close();
    }

    /* Add ad new product option group to the database */
    public void addProductOptionGroup(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            db.Execute("INSERT INTO ProductOptionGroups (GroupName, GroupDescription) VALUES (@0, @1)");
            pkProductOptionGroupId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Modify product option group data in the database */
    public void modifyProductOptionGroup(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            db.Execute(@"UPDATE ProductOptionGroups SET GroupName = @0, GroupDescription = @1 WHERE pkProductOptionGroupId = @2", pkProductOptionGroupId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Delete product option group from the database */
    public void deleteProductOptionGroup(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            db.Execute("DELETE FROM ProductOptionGroups WHERE pkProductOptionGroupId = @0", pkProductOptionGroupId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
