using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for CartItem
/// </summary>
public class CartItem
{
    public int pkCartItemId { get ; set ; }
    public int fkCartId { get ; set ; }
    public Product CIProduct { get ; set ; }
    public CustomerRoll CICustomerRoll { get ; set ; }
    public decimal Price { get ; set ; }
    public Discount CIDiscount { get ; set ; }
    public int Qty { get ; set ; }
    public int ?fkTaxZoneId { get ; set ; }
    public decimal Total { get ; set ; }
    public string Instructions { get ; set ; }
    public bool CateringItem { get ; set ; }
    public int ?fkMixNMatchRollId { get ; set ; }
    public bool isMixNMatchCustomerRoll { get ; set ; }
    public decimal ItemProfit { get ; set ; }
    public decimal OptionsTotal { get ; set ; }

    public List<OrderOption> Options { get ; set ; }
    public List<ProductOption> ProductOptions { get ; set ; }

    public CartItem(){
        CIProduct = new Product();
        CICustomerRoll = new CustomerRoll();
        CIDiscount = new Discount();
        Options = new List<OrderOption>();
        ProductOptions = new List<ProductOption>();
    }
    
    public CartItem(int Id){
        CIProduct = new Product();
        CICustomerRoll = new CustomerRoll();
        CIDiscount = new Discount();

        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT *, Price * Qty AS Total FROM CartItems WHERE pkCartItemId = @0", Id);

        pkCartItemId = query.pkCartItemId;
        fkCartId = query.fkCartId;

        if(query.fkDiscountId != null){CIDiscount = new Discount(query.fkDiscountId);}
        else{CIDiscount = new Discount();}

        if (query.fkProductId != null){
            CIProduct = new Product(query.fkProductId);
            if(CIDiscount.hasId()){CIDiscount.getDiscountDollar(CIProduct.Price);}
        } else{
            CICustomerRoll = new CustomerRoll(query.fkRollId);
            if(CIDiscount.hasId()){CIDiscount.getDiscountDollar(CICustomerRoll.Price);}
        }

        Price = query.Price;
        
        Qty = query.Qty;
        fkTaxZoneId = query.fkTaxZoneId;
        Total = query.Total;
        Instructions = query.Instructions;
        CateringItem = query.CateringItem;
        fkMixNMatchRollId = query.fkMixNMatchRollId;
        isMixNMatchCustomerRoll = query.isMixNMatchCustomerRoll;
        ItemProfit = query.ItemProfit;

        /* Get order options */
        Options = new List<OrderOption>();
        var optionsQuery = db.Query("SELECT fkOptionId FROM Order_Option_Lookup WHERE fkCartItemId = @0 AND fkOptionId IS NOT NULL", Id);
        if(optionsQuery.Any()){
            foreach(var row in optionsQuery){
                OrderOption op = new OrderOption(row.fkOptionId);
                Options.Add(op);
                OptionsTotal += op.OptionPrice;
            }
        }

        /* Get product options (t-shirt size, what dressing, etc.) */
        ProductOptions = new List<ProductOption>();
        var productOptionsQuery = db.Query("SELECT fkProductOptionId FROM Order_Option_Lookup WHERE fkCartItemId = @0 AND fkProductOptionId IS NOT NULL", Id);
        if(productOptionsQuery.Any()){
            foreach(var row in productOptionsQuery){
                ProductOption op = new ProductOption(row.fkProductOptionId);
                ProductOptions.Add(op);
            }
        }

        db.Close();
        return;
    }

    public bool hasId(){
        return pkCartItemId == 0 ? false : true;
    }

    public decimal calculateTax(){return 0;}


    public decimal calculateProfit(){
        decimal profit = 0;
        decimal baseCost = (decimal)0.84;
        decimal basePrice = 2;

        /* Check if item is a product */
        if(CIProduct.hasId()){
            /* Check if food item */
            if(CIProduct.IType == ItemType.Food){
                /* Calculate profit in ingredients */
                foreach(Ingredient i in ((Food)CIProduct).Ingredients){profit += (i.IngredientPricePerRoll - i.IngredientCostPerPiece);}
            }
            /* Check if roll item */
            else if(CIProduct.IType == ItemType.Roll){
                /* Calculate profit in ingredients */
                foreach(Ingredient i in ((Food)CIProduct).Ingredients){profit += (i.IngredientPricePerRoll - i.IngredientCostPerPiece);}
                /* Calculate profit in secondary ingredients */
                foreach(Ingredient i in ((Roll)CIProduct).SecondaryIngredients){profit += (i.IngredientPricePerRoll - i.IngredientCostPerPiece);}
            }
        }
        /* Check if item is a customer roll */
        else if(CICustomerRoll.pkCustomerRollId != 0){
            /* Calculate profit in ingredients */
            foreach(Ingredient i in CICustomerRoll.IngredientsInside){profit += (i.IngredientPricePerRoll - i.IngredientCostPerPiece);}
            /* Calculate profit in secondary ingredients */
            foreach(Ingredient i in CICustomerRoll.IngredientsTop){profit += (i.IngredientPricePerRoll - i.IngredientCostPerPiece);}
        }

        /* Calculate profit in options */
        foreach(OrderOption op in Options){profit += (op.OptionPrice - op.OptionCost);}

        /* Calculate profit from base cost/price per roll */
        profit += (basePrice - baseCost);

        /* Return profit made */
        return profit;
    }


    public void addCartItem(){
        try{
            int ?productId = null;
            if(CIProduct.pkProductId != 0){productId = CIProduct.pkProductId;}

            int ?rollId = null;
            if(CICustomerRoll.pkCustomerRollId != 0){rollId = CICustomerRoll.pkCustomerRollId;}

            int ?discountId = null;
            if(CIDiscount.pkDiscountId != 0){discountId = CIDiscount.pkDiscountId;}

            int ?taxId = null;
            if(fkTaxZoneId != 0){taxId = fkTaxZoneId;}

            int ?mixNMatchId = null;
            if(fkMixNMatchRollId != 0){mixNMatchId = fkMixNMatchRollId;}

            var db = Database.Open("buSushi");
            db.Execute(@"INSERT INTO CartItems (fkCartId, fkProductId, fkRollId, Price, fkDiscountId, Qty, fkTaxZoneId, Instructions,
                    CateringItem, fkMixNMatchRollId, isMixNMatchCustomerRoll, ItemProfit) VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10, @11)",
                    fkCartId, productId, rollId, Price, discountId, Qty, taxId, Instructions, CateringItem, mixNMatchId, isMixNMatchCustomerRoll, ItemProfit);
            /* Get the newly added ID */
            pkCartItemId = (int)db.GetLastInsertId();

            /* Insert options selected */
            foreach(OrderOption option in Options){
                db.Execute(@"INSERT INTO Order_Option_Lookup (fkOptionId, fkCartItemId, fkOrderItemId, fkDiscountId) VALUES (@0, @1, @2, @3)", option.pkOptionId, pkCartItemId, null, discountId);
            }

            /* Insert product option chosen */
            foreach(ProductOption option in ProductOptions){
                db.Execute("INSERT INTO Order_Option_Lookup (fkProductOptionId, fkCartItemId, fkDiscountId) VALUES (@0, @1, @2)", option.pkProductOptionId, pkCartItemId, discountId);
            }
            
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Change quantity of items in the cart */
    public void updateCartItemQty(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");
            /* Update Qty */
            db.Execute("UPDATE CartItems SET Qty = @0 WHERE pkCartItemId = @1", Qty, pkCartItemId);
            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Change selected options for an item in the cart */
    public void updateCartItemOptions(){
        /* Open connection to the database */
        var db = Database.Open("buSushi");
        
        /* Remove old information first */
        db.Execute("REMOVE FROM Order_Option_Lookup WHERE fkCartItemId = @0", pkCartItemId);
        
        /* Add newly selected options */
        foreach(OrderOption option in Options){
            db.Execute(@"INSERT INTO Order_Option_Lookup (fkOptionId, fkCartItemId) VALUES (@0, @1)", option.pkOptionId, pkCartItemId);
        }

        foreach(ProductOption option in ProductOptions){
                db.Execute("INSERT INTO Order_Option_Lookup (fkProductOptionId, fkCartItemId) VALUES (@0, @1)", option.pkProductOptionId, pkCartItemId);
            }

        /* Close connection to the database */
        db.Close();
    }

    public void modifyCartItem(){
        try{
            int ?productId = null;
            if(CIProduct.pkProductId != 0){productId = CIProduct.pkProductId;}

            int ?rollId = null;
            if(CICustomerRoll.pkCustomerRollId != 0){rollId = CICustomerRoll.pkCustomerRollId;}

            int ?discountId = null;
            if(CIDiscount.pkDiscountId != 0){discountId = CIDiscount.pkDiscountId;}

            int ?taxId = null;
            if(fkTaxZoneId != 0){taxId = fkTaxZoneId;}

            int ?mixNMatchId = null;
            if(fkMixNMatchRollId != 0){mixNMatchId = fkMixNMatchRollId;}


            var db = Database.Open("buSushi");
            db.Execute(@"UPDATE CartItems SET fkCartId = @0, fkProductId = @1, fkRollId = @2, Price = @3, fkDiscountId = @4, Qty = @5,
                            fkTaxZoneId = @6, Instructions = @7, CateringItem = @8, fkMixNMatchRollId = @9, isMixNMatchCustomerRoll = @10,
                            ItemProfit = @11 WHERE pkCartItemId = @12", fkCartId, productId, rollId, Price, discountId, Qty, taxId,
                            Instructions, CateringItem, mixNMatchId, isMixNMatchCustomerRoll, ItemProfit, pkCartItemId);
            
            /* Delete old lookup data, then re-insert new (if any) data /*/
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkCartItemId = @0", pkCartItemId);
            foreach(OrderOption o in Options){
                db.Execute("INSERT INTO Order_Option_Lookup (fkOptionId, fkCartItemId, fkOrderItemId, fkDiscountId) VALUES (@0, @1, @2, @3)", o.pkOptionId, pkCartItemId, null, discountId);
            }

            foreach(ProductOption option in ProductOptions){
                db.Execute("INSERT INTO Order_Option_Lookup (fkProductOptionId, fkCartItemId, fkDiscountId) VALUES (@0, @1, @2)", option.pkProductOptionId, pkCartItemId, discountId);
            }
            
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteCartItem(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete lookup information */
            db.Execute("DELETE FROM Order_Option_Lookup WHERE fkCartItemId = @0", pkCartItemId);

            /* Delete data from the database */
            db.Execute("DELETE FROM CartItems WHERE pkCartItemId = @0", pkCartItemId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
