using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

public enum ItemType {Product, Food, Sushi, Roll};

/// <summary>
/// Summary description for Product
/// </summary>
public class Product
{
    public int pkProductId { get ; set ; }
    public string Name { get ; set ; }
    public string JPName { get ; set ; }
    public string Description { get ; set ; }
    public ProductType PType { get ; set ; }
    public decimal Cost { get ; set ; }
    public decimal Price { get ; set ; }
    public string imgURL { get ; set ; }
    public string imgThumbURL { get ; set ; }
    public string imgSpURL { get ; set ; }
    public string imgLargeURL { get ; set ; }
    public bool discontinued { get ; set ; }
    public ItemType IType { get ; set ; }
    public DateTime ?SpecialBeginDate { get ; set ; }
    public DateTime ?SpecialEndDate { get ; set ; }


    public List<Tag> Tags { get ; set ; }
    public List<ProductOption> ProductOptions { get ; set ; }
    
    public Product(){
        Tags = new List<Tag>();
        ProductOptions = new List<ProductOption>();
        PType = new ProductType();
        IType = ItemType.Product;
        SpecialBeginDate = null;
        SpecialEndDate = null;
    }

    public Product(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM Products WHERE pkId = @0", Id);

        pkProductId = query.pkId;
        Name = query.Name;
        JPName = query.JPName;
        Description = query.Desc;
        PType = new ProductType(query.Type);
        Cost = query.Cost;
        Price = query.Price;
        imgURL = query.imgURL;
        imgThumbURL = query.imgThumbURL;
        imgSpURL = query.imgSpUrL;
        imgLargeURL = query.imgLargeURL;
        discontinued = query.discontinued;
        IType = ItemType.Product;
        

        /* Add Tags */
        Tags = new List<Tag>();
        var tags = db.Query(@"SELECT t.* FROM Tags t
                            INNER JOIN TagLookup tl ON tl.fkTagId = t.pkTagId
                            WHERE tl.fkProductId = @0", pkProductId);
        foreach(var t in tags){Tags.Add(new Tag(t.pkTagId));}

        /* Add product options */
        ProductOptions = new List<ProductOption>();
        var options = db.Query(@"SELECT fkProductOptionId FROM Product_Option_Lookup WHERE fkProductId = @0", pkProductId);
        if(options.Any()){
            foreach(var o in options){
                ProductOptions.Add(new ProductOption(o.fkProductOptionId));
            }
        }

        /* Check if product is a special or not */
        var specialInfo = db.QuerySingle(@"SELECT * FROM Specials_Lookup WHERE fkProductId = @0
                            AND SpecialBeginDate <= (getdate()) AND SpecialEndDate >= (getdate())", Id);
        /* Check if records were returned */
        if(specialInfo != null){
            SpecialBeginDate = ((DateTime)specialInfo.SpecialBeginDate);
            SpecialEndDate = ((DateTime)specialInfo.SpecialEndDate);
        }else{
            SpecialBeginDate = null;
            SpecialEndDate = null;
        }


        db.Close();
        return;
    }

    /* Copy Constructor */
    public Product(Product item){
        IType = ItemType.Product;

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
        
        PType = item.PType;

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;
    }

    /* Change food into a product */
    public Product(Food item){
        IType = ItemType.Product;

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

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteFood();
    }

    /* Change sushi into a product */
    public Product(Sushi item){
        IType = ItemType.Product;

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
        
        Tags = item.Tags;
        ProductOptions = item.ProductOptions;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteSushi();
        item.deleteFood();
    }

    /* Change roll into a product */
    public Product(Roll item){
        IType = ItemType.Product;
        
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

        Tags = item.Tags;
        ProductOptions = item.ProductOptions;

        /* Delete corresponding derivitive data that we don't need */
        item.deleteRoll();
        item.deleteSushi();
        item.deleteFood();
    }


    /* Return whether or not data was filled when Product was instantiated */
    public bool hasId(){
        return pkProductId == 0 ? false : true;
    }

    public bool isSpecial(){
        if(DateTime.Now >= SpecialBeginDate && DateTime.Now <= SpecialEndDate){
            return true;
        }else{return false;}
    }

    public void addProduct(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Add the new product information */
            var productExec = @"INSERT INTO Products (Name, JPName, [Desc], Type, Price, imgURL, imgThumbURL, imgSpUrL, imgLargeURL, discontinued, Cost)
                                                VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10)";
            db.Execute(productExec, Name, JPName, Description, PType.pkProductTypeId, Price, imgURL, imgThumbURL, imgSpURL, imgLargeURL, discontinued, Cost);
            /* Set pkProductId to the newly added ID */
            pkProductId = (int)db.GetLastInsertId();

            /* Insert if special */
            if(SpecialBeginDate != DateTime.MinValue){
                if(SpecialBeginDate != null){
                    db.Execute("INSERT INTO Specials_Lookup (fkProductId, SpecialBeginDate, SpecialEndDate) VALUES (@0, @1, @2)", pkProductId, SpecialBeginDate, SpecialEndDate);
                }
            }

            /* Add Tags */
            if(Tags.Any()){
                foreach(var t in Tags){
                    db.Execute("INSERT INTO TagLookup (fkTagId, fkProductId) VALUES (@0, @1)", t.pkTagId, pkProductId);
                }
            }

            /* Add Options */
            if(ProductOptions.Any()){
                foreach(var o in ProductOptions){
                    db.Execute("INSERT INTO Product_Option_Lookup (fkProductOptionId, fkProductId) VALUES (@0, @1)", o.pkProductOptionId, pkProductId);
                }
            }


            /* Close connection to the database */
            db.Close();

        //} catch(Exception e){Console.Write(e.Message);}
    } /* #END void addProduct() */

    public void modifyProduct(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify product information in the database */
            var productExec = @"UPDATE Products SET Name = @0, JPName = @1, [Desc] = @2, Type = @3, Price = @4,
                                                    imgURL = @5, imgThumbURL = @6, imgSpUrL = @7, imgLargeURL = @8, discontinued = @9, Cost = @10
                                                WHERE pkId = @11";
            db.Execute(productExec, Name, JPName, Description, PType.pkProductTypeId, Price, imgURL, imgThumbURL, imgSpURL, imgLargeURL, discontinued, Cost, pkProductId);

            /* Delete lookup information then re-add current information */
            db.Execute("DELETE FROM TagLookup WHERE fkProductId = @0", pkProductId);
            db.Execute("DELETE FROM Product_Option_Lookup WHERE fkProductId = @0", pkProductId);
            db.Execute("DELETE FROM Specials_Lookup WHERE fkProductId = @0", pkProductId);

            /* Add Tags */
            if(Tags.Count > 0){
                foreach(var t in Tags){
                    db.Execute("INSERT INTO TagLookup (fkTagId, fkProductId) VALUES (@0, @1)", t.pkTagId, pkProductId);
                }
            }
            
            /* Add Options */
            if(ProductOptions.Any()){
                foreach(var o in ProductOptions){
                    db.Execute("INSERT INTO Product_Option_Lookup (fkProductOptionId, fkProductId) VALUES (@0, @1)", o.pkProductOptionId, pkProductId);
                }
            }

            /* Insert if special */
            if(SpecialBeginDate != DateTime.MinValue){
                if(SpecialBeginDate != null){db.Execute("INSERT INTO Specials_Lookup (fkProductId, SpecialBeginDate, SpecialEndDate) VALUES (@0, @1, @2)", pkProductId, SpecialBeginDate, SpecialEndDate);}
            }
            /* Close connection to the database */
            db.Close();
        //} catch (Exception e){Console.Write(e.Message);}
    } /* #END modifyProduct() */

    public void deleteProduct(){
        //try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete all lookup information referencing this item */
            db.Execute("DELETE FROM TagLookup WHERE fkProductId = @0", pkProductId);
            db.Execute("DELETE FROM Product_Option_Lookup WHERE fkProductId = @0", pkProductId);
            db.Execute("DELETE FROM Specials_Lookup WHERE fkProductId = @0", pkProductId);

            /* Check if any food references this product */
            var foodId = db.QueryValue("SELECT pkFoodId FROM Food WHERE fkProductId = @0", pkProductId);
            if(foodId != null){
                /* Check if any sushi references this food item */
                var sushiId = db.QueryValue("SELECT pkSushiId FROM Sushi WHERE fkFoodId = @0", foodId);
                if(sushiId != null){
                    /* Check if a roll references the sushi item */
                    var rollId = db.QueryValue("SELECT pkRollId FROM Rolls WHERE fkSushiId = @0", sushiId);
                    if(rollId != null){
                        db.Execute("DELETE FROM Order_Option_Lookup WHERE fkRollId = @0", rollId);
                        db.Execute("DELETE FROM Rolls WHERE pkRollId = @0", rollId);
                    }
                    /* Delete the sushi item */
                    db.Execute("DELETE FROM Sushi WHERE pkSushiId = @0", sushiId);
                }
                /* Delete the food item as well as lookup information */
                db.Execute("DELETE FROM FoodIngredientLookup WHERE fkFoodId = @0", foodId);
                db.Execute("DELETE FROM Food WHERE pkFoodId = @0", foodId);
            }
            
            /* Delete data from the products table */
            db.Execute("DELETE FROM Products WHERE pkId = @0", pkProductId);

            /* Close connection to the database */
            db.Close();
        //}catch (Exception e){Console.Write(e.Message);}
    } /* #END deleteProduct() */
}
