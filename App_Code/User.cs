using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for User
/// </summary>
public class User
{
    public int UserId { get; set; }
    public string Username { get ; set ; }
    public string Email { get; set; }
    public DateTime CreateDate { get ; set ; }
    public bool isConfirmed { get ; set ; }
    public UserStatus UStatus { get ; set ; }
    public DateTime ?LastPasswordFailureDate { get ; set ; }
    public int PasswordFailuresSinceLastSuccess { get ; set ; }
    public DateTime ?PasswordChangedDate { get ; set ; }
    
    public List<Role> Roles { get ; set ; }
    public Cart UserCart { get ; set ; }
    public List<Order> Orders { get ; set ; }
    public List<Order> SavedOrders { get ; set ; }
    public List<User> Following { get ; set ; }

    public User(){
        UStatus = new UserStatus();
        Roles = new List<Role>();
        UserCart = new Cart();
        Orders = new List<Order>();
        SavedOrders = new List<Order>();
        Following = new List<User>();
    }

    public User(int Id){
        var db = Database.Open("buSushi");
        var sql = @"SELECT wm.UserId, wm.CreateDate, wm.IsConfirmed, wm.LastPasswordFailureDate, wm.PasswordFailuresSinceLastSuccess,
                           PasswordChangedDate, us.*, up.Email, up.Username
                           FROM UserProfile up
                           INNER JOIN webpages_Membership wm
                           ON wm.UserId = up.UserId
                           LEFT JOIN UserStatus us
                           ON us.pkUserStatusId = wm.fkUserStatusId
                           WHERE wm.UserId = @0";
        var query = db.QuerySingle(sql, Id);

        UserId = query.UserId;
        Username = query.Username;
        Email = query.Email;
        CreateDate = query.CreateDate;
        isConfirmed = query.isConfirmed;
        UStatus = new UserStatus(query.pkUserStatusId);
        LastPasswordFailureDate = query.LastPasswordFailureDate;
        PasswordFailuresSinceLastSuccess = query.PasswordFailuresSinceLastSuccess;
        PasswordChangedDate = query.PasswordChangedDate;

        /* Add roles to customer */
        Roles = new List<Role>();
        query = db.Query(@"SELECT wr.* FROM webpages_Roles wr
                                INNER JOIN webpages_UsersInRoles wuir
                                ON wuir.RoleId = wr.RoleId
                                WHERE wuir.UserId = @0", Id);
        foreach(var row in query){Roles.Add(new Role(row.RoleId));}

        /* Check for an open cart */
        UserCart = new Cart();
        query = db.QueryValue("SELECT pkCartId FROM Cart WHERE fkUserId = @0", UserId);
        if (query != null){UserCart = new Cart(query);}

        /* Add the customer's current and previous orders */
        Orders = new List<Order>();
        var orderQuery = db.Query("SELECT pkOrderId FROM Orders WHERE fkUserId = @0", UserId);
        if(orderQuery.Any()){
            foreach(var row in orderQuery){Orders.Add(new Order(query.pkOrderId));}
        }

        /* Get orders that the user has saved */
        SavedOrders = new List<Order>();
        var savedOrderQuery = db.Query(@"SELECT fkOrderId FROM Customer_Favorite_Lookup WHERE fkUserId = @0", UserId);
        if(savedOrderQuery.Any()){
            foreach(var row in savedOrderQuery){SavedOrders.Add(new Order(query.fkOrderId));}
        }

        Following = new List<User>();
        /* Get what other users this user is following */
        var fol = db.Query(@"SELECT fkFollowingId FROM Customer_Following_Lookup WHERE fkUserId = @0", UserId);
        if( fol.Any() ){
            foreach(var row in fol){Following.Add(new User(row.fkFollowingId));}
        }

        db.Close();
        return;
    }

    public void activateUser(){
        try{
            var db = Database.Open("buSushi");
            var activeId = db.QueryValue("SELECT pkUserStatusId FROM UserStatus WHERE Status = 'Active'");
            db.Execute("UPDATE webpages_Membership SET fkUserStatusId = @0 WHERE UserId = @1", activeId, UserId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
    public void banUser(){
        try{
            var db = Database.Open("buSushi");
            var banId = db.QueryValue("SELECT pkUserStatusId FROM UserStatus WHERE Status = 'Banned'");
            db.Execute("UPDATE webpages_Membership SET fkUserStatusId = @0 WHERE UserId = @1", banId, UserId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void updateUserStatus(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE webpages_Membership SET isConfirmed = @0, fkUserStatusId = @1 WHERE UserId = @2", isConfirmed, UStatus.pkUserStatusId, UserId);

            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyUser(){
        try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");
            
            /* Modify (base) User information */
            db.Execute(@"UPDATE UserProfile SET Username = @0, Email  = @1 WHERE UserId = @2", Username, Email, UserId);

            /* Close connection to the database */
            db.Close();
            
        }catch(Exception e){Console.Write(e.Message);}
    }
}

