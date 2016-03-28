using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;

/// <summary>
/// Summary description for City
/// </summary>
public class City
{
    public int pkCityId { get ; set ; }
    public string CityName { get ; set ; }
    public string ZipCode { get ; set ; }
    public State CState { get ; set ; }

    public City(){CState = new State();}
    public City(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM Cities WHERE pkCityId = @0", Id);

        pkCityId = query.pkCityId;
        CityName = query.CityName;
        ZipCode = query.ZipCode;
        CState = new State(query.fkStateId);

        db.Close();
        return;
    }

    /* Add a new city to the database */
    public void addCity(){
        //try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            var id = db.QueryValue("SELECT pkCityId FROM Cities WHERE CityName = @0 AND ZipCode = @1 AND fkStateId = @2", CityName, ZipCode, CState.pkStateId);

            if(id == null){
                /* Add new city to the database */
                db.Execute("INSERT INTO Cities (CityName, ZipCode, fkStateId) VALUES (@0, @1, @2)", CityName, ZipCode, CState.pkStateId);
            }

            /* Close connection to the database */
            db.Close();
        //}catch(Exception e){Console.Write(e.Message);}
    }

    /* Modify a city in the database */
    public void modifyCity(){
        try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            /* Add new city to the database */
            db.Execute("UPDATE Cities SET CityName = @0, ZipCode = @1, fkStateId = @2", CityName, ZipCode, CState.pkStateId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    /* Delete a city from the database */
    public void deleteCity(){
        try{
            /* Open a connection to the database */
            var db = Database.Open("buSushi");

            /* Add new city to the database */
            db.Execute("DELETE FROM Cities WHERE pkCityId = @0", pkCityId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}
