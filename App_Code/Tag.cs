using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for Tag
/// </summary>
public class Tag
{
    public int pkTagId { get ; set ; }
    public string TagName { get ; set ; }

    public Tag(){}

    public Tag(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM Tags WHERE pkTagId = @0", Id);

        pkTagId = query.pkTagId;
        TagName = query.Tag;

        db.Close();
        return;
    }

    public void addTag(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("INSERT INTO Tags (Tag) VALUES (@0)", TagName);
            db.Close();
        } catch(Exception e){Console.Write(e.Message);}
    } /* #END addTag() */

    public void modifyTag(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("UPDATE Tags SET Tag = @0 WHERE pkTagId = @1", TagName, pkTagId);
            db.Close();
        } catch(Exception e){Console.Write(e.Message);}
    } /* #END modifyTag() */

    public void deleteTag(){
        try{
            var db = Database.Open("buSushi");
            db.Execute("DELETE FROM Tags WHERE pkTagId = @0", pkTagId);
            db.Close();
        } catch(Exception e){Console.Write(e.Message);}
    }
}
