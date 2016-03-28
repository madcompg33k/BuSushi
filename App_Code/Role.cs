using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Role
/// </summary>
public class Role
{
    public int RoleId { get ; set ; }
    public string RoleName { get ; set ; }


    public Role(){}
    public Role(int Id){
        /* Open connection to the database */
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * from webpages_Roles WHERE RoleId = @0", Id);

        /* Check if data was returned, then assign data */
        if(query != null){
            RoleId = query.RoleId;
            RoleName = query.RoleName;   
        }

        /* Close connection to the database */
        db.Close();
        return;
    }

    public void addRole(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("INSERT INTO webpages_Roles (RoleName) VALUES (@0)", RoleName);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyRole(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE webpages_Roles SET RoleName = @0 WHERE RoleId = @1", RoleName, RoleId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteRole(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM webpages_Roles WHERE RoleId = @0", RoleId);
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
