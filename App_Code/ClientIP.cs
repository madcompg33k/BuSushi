using System;
using System.Collections.Generic;
using System.Web;

/// <summary>
/// Summary description for ClientIP
/// </summary>
public class ClientIP
{
    public int fkUserId { get ; set ; }
    public string IPAddress { get ; set ; }
    public DateTime timestamp { get ; set ; }
    public bool blocked { get ; set ; }

    public ClientIP(){}
    public ClientIP(int Id){
        
    }
}
