using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for State
/// </summary>
public class State
{
    public int pkStateId { get ; set ; }
    public string StateName { get ; set ; }
    public string StateAbbreviation { get ; set ; }

    public State(){}
    public State(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM States WHERE pkStateId = @0", Id);

        pkStateId = query.pkStateId;
        StateName = query.StateName;
        StateAbbreviation = query.StateAbbreviation;

        db.Close();
        return;
    }
}
