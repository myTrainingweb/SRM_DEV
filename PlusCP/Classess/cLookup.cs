using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class cLookup
{
    cDAL oDAL;
    string sql = "";
    public cLookup()
    {
        oDAL = new cDAL(cDAL.ConnectionType.INIT);
    }

    public string GetSysValue(string sysCode)
    {
        sql = @"SELECT SysValue FROM dbo.zSysIni WHERE SysCode = '" + sysCode + "'";
        return oDAL.GetObject(sql).ToString();
    }
}
