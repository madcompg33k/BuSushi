using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for UserStatus
/// </summary>
public class UserStatus
{
    public int pkUserStatusId { get ; set ; }
    public string Status { get ; set ; }
    public string StatusDescription { get ; set ; }

    public UserStatus(){}

    public UserStatus(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle(@"SELECT * FROM UserStatus WHERE pkUserStatusId = @0", Id);

        if(query != null){
            pkUserStatusId = query.pkUserStatusId;
            Status = query.Status;
            StatusDescription = query.StatusDescription;
        }

        db.Close();
        return;
    }
}
