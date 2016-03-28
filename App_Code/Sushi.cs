using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Sushi
/// </summary>
public class Sushi : Food
{
    public int pkSushiId { get ; set ; }
    public SushiType SType { get ; set ; }

    public Sushi(){
        SType = new SushiType();
        IType = ItemType.Sushi;
    }

    public Sushi(int Id) : base(Id){
        SType = new SushiType();
        IType = ItemType.Sushi;

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Sushi WHERE pkSushiId = @0", Id);
        pkSushiId = query.pkSushiId;
        SType = new SushiType(query.fkSushiTypeId);
        FType = new FoodType(SType.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);
        
        db.Close();
        return;
    }

    public Sushi(int SushiId, int FoodId, int ProductId) : base(FoodId, ProductId){
        SType = new SushiType();
        IType = ItemType.Sushi;

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Sushi WHERE pkSushiId = @0", SushiId);

        pkSushiId = query.pkSushiId;
        SType = new SushiType(query.fkSushiTypeId);
        FType = new FoodType(SType.fkFoodTypeId);
        PType = new ProductType(FType.fkProductTypeId);
            
        db.Close();
        return;
    }

    /* Change product into sushi */
    public Sushi(Product item){
        IType = ItemType.Sushi;

        pkProductId = item.pkProductId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;

        PType = new ProductType();
        FType = new FoodType();
        SType = new SushiType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = new List<Ingredient>();
    }
    
    /* Change food into sushi */
    public Sushi(Food item){
        IType = ItemType.Sushi;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;

        PType = item.PType;
        FType = new FoodType();
        SType = new SushiType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
    }

    /* Copy Constructor */
    public Sushi(Sushi item){
        IType = ItemType.Sushi;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;
        pkSushiId = item.pkSushiId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;
        
        PType = item.PType;
        FType = item.FType;
        SType = item.SType;

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;
    }

    /* Change roll into sushi */
    public Sushi(Roll item){
        IType = ItemType.Sushi;

        pkProductId = item.pkProductId;
        pkFoodId = item.pkFoodId;
        pkSushiId = item.pkSushiId;

        Name = item.Name;
        JPName = item.JPName;
        Description = item.Description;
        Cost = item.Cost;
        Price = item.Price;
        imgURL = item.imgURL;
        imgThumbURL = item.imgThumbURL;
        imgSpURL = item.imgSpURL;
        imgLargeURL = item.imgLargeURL;
        discontinued = item.discontinued;
        SpecialBeginDate = item.SpecialBeginDate;
        SpecialEndDate = item.SpecialEndDate;

        PType = item.PType;
        FType = item.FType;
        SType = new SushiType();

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
        Ingredients = item.Ingredients;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteRoll();
    }


    public void addSushi(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add the new food information */
            db.Execute("INSERT INTO Sushi (fkSushiTypeId, fkFoodId) VALUES (@0, @1)", SType.pkSushiTypeId, pkFoodId);
            /* Set pkSushiId to the newly added ID */
            pkSushiId = (int)db.GetLastInsertId();

            /* Close connection to the database */
            db.Close();
        //} catch(Exception e){Console.Write(e.Message);}
    } /* #END void addSushi() */

    public void addNewSushi(){
        //try{
            addProduct();
            addFood();
            addSushi();
        //}catch(Exception e){Console.Write(e.Message);}
    }


    public void modifySushi(){
        //try{
            /* Modify item on the product level */
            modifyProduct();

            /* Modify item on the food level */
            modifyFood();

            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify sushi information in the database */
            db.Execute("UPDATE Sushi SET fkSushiTypeId = @0, fkFoodId = @1 WHERE pkSushiId = @2", SType.pkSushiTypeId, pkFoodId, pkSushiId);
            
            /* Close connection to the database */
            db.Close();
        //} catch (Exception e){Console.Write(e.Message);}
    } /* #END modifySushi() */

    public void deleteSushi(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the Sushi table */
            db.Execute("DELETE FROM Sushi WHERE pkSushiId = @0", pkSushiId);
        //}catch (Exception e){Console.Write(e.Message);}
    } /* #END deleteSushi() */

}
