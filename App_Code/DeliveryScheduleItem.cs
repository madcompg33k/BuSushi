using System;
using System.Collections.Generic;
using System.Web;
using WebMatrix.Data;
using WebMatrix.WebData;
using System.Linq;


/// <summary>
/// Summary description for DeliveryScheduleItem
/// </summary>

public class DeliveryScheduleItem
{
    public int pkDeliveryId { get; set; }
    public DateTime DeliveryDate { get; set; }
    public City DCity { get ; set ; }

    public DeliveryScheduleItem(){
        DeliveryDate = DateTime.Today.AddDays(1);
        DCity = new City();
    }

    public DeliveryScheduleItem(int Id){
        var db = Database.Open("buSushi");
        var query = db.QuerySingle("SELECT * FROM DeliverySchedule WHERE pkDeliveryId = @0", Id);
        
        pkDeliveryId = Id;
        DeliveryDate = query.DeliveryDate;
        DCity = new City(query.fkCityId);

        db.Close();
        return;
    }

    public void addDeliveryScheduleItem(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Check if already exists */
            var exists = db.Query(@"SELECT DISTINCT convert(nvarchar(10), DeliveryDate, 10), fkCityId FROM DeliverySchedule WHERE DeliveryDate = @0 AND fkCityId = @1",
                                DeliveryDate.ToShortDateString(), DCity.pkCityId);

            if(!exists.Any()){
                /* Insert the data into the database */
                db.Execute("INSERT INTO DeliverySchedule (DeliveryDate, fkCityId) VALUES (@0, @1)", DeliveryDate, DCity.pkCityId);
                pkDeliveryId = (int)db.GetLastInsertId();
            }

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void modifyDeliveryScheduleItem(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Modify the database */
            db.Execute("UPDATE DeliverySchedule SET DeliveryDate = @0, fkCityId = @1 WHERE pkDeliveryId = @2", DeliveryDate, DCity.pkCityId, pkDeliveryId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }

    public void deleteDeliveryScheduleItem(){
        try{
            /* Open connection to the database */
            var db = Database.Open("buSushi");

            /* Delete data from the database */
            db.Execute("DELETE FROM DeliverySchedule WHERE pkDeliveryId = @0", pkDeliveryId);

            /* Close connection to the database */
            db.Close();
        }catch(Exception e){Console.Write(e.Message);}
    }
}