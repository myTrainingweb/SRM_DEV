using PlusCP.Extensions;
using IP.Classess;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class Home
    {
        cDAL oDAL;
        string sql = string.Empty;
        public string TotalValue { get; set; }
        public string Menu { get; set; }
        public List<WebMenus> webMnu { get; set; }
        public string Message { get; set; }
        public string isAdmin { get; set; }

        public string Customer { get; set; }


        private string ProgramId = HttpContext.Current.Session["ProgramId"].ToString();
        private string signinId = HttpContext.Current.Session["SigninId"].ToString();
        public string firstName = HttpContext.Current.Session["FirstName"].ToString();
     
        private string isadmin = HttpContext.Current.Session["isAdmin"].ToString();
      //  private string customer = HttpContext.Current.Session["isCustomer"].ToString();


        public void GetMenus(string EmpName, String isadmin, string programName)
        {
            try
            {


                string WInITTeam = "Yousuf";
                if (isadmin == "True")
                {
                    sql = @"
                                WITH TMP AS
                                (
                                SELECT M.MnuId, RptCode, MnuType, MnuIcon, MnuTitle, MnuTitleShort, rptDesc, MnuDesc, MnuHyperlink, MnuTarget, MnuRights, MnuActive, MnuIsReady, MnuParent, AuthUser, AuthSite, DesignedBy
                                FROM SRM.Mnu  M 
                                WHERE MnuParent IS NULL 
                                AND MnuId IN (
                                                        SELECT DISTINCT MnuParent
                                                        FROM SRM.Mnu M
                                                        WHERE M.MnuActive = 1
                                                    )

                                UNION ALL 
                                SELECT M.MnuId, M.RptCode, M.MnuType, M.MnuIcon, M.MnuTitle, M.MnuTitleShort, M.rptDesc, M.MnuDesc, M.MnuHyperlink, M.MnuTarget, M.MnuRights, M.MnuActive, M.MnuIsReady, M.MnuParent, M.AuthUser, M.AuthSite, M.DesignedBy
                                FROM SRM.Mnu M
                                INNER JOIN TMP ON TMP.MnuId = M.MnuParent
                                WHERE 
							    M.MnuActive = '1' <MENUISREADY>
                                
							   
                                )
                                SELECT MnuId, RptCode, MnuType, MnuIcon, MnuTitle, MnuTitleShort, rptDesc, MnuDesc, MnuHyperlink, MnuTarget, MnuRights, MnuActive, MnuIsReady, MnuParent, AuthUser, AuthSite, DesignedBy 
                                FROM TMP
                                WHERE AuthSite = '<programName>' OR AuthSite = '*'


                     ";
                    sql = sql.Replace("<programName>", programName);
                }

                else
                {
                    sql = @"WITH TMP AS
                                (
                                SELECT M.MnuId, RptCode, MnuType, MnuIcon, MnuTitle, MnuTitleShort, rptDesc, MnuDesc, MnuHyperlink, MnuTarget, MnuRights, MnuActive, MnuIsReady, MnuParent, AuthUser,AuthSite, DesignedBy
                                FROM SRM.Mnu  M 
                                WHERE MnuParent IS NULL 
                                AND MnuId IN (
                                                        SELECT DISTINCT MnuParent
                                                        FROM SRM.Mnu M
                                                        INNER JOIN EP.UserMnuX UMX ON UMX.MnuId = M.MnuId
                                                        WHERE M.MnuActive = 1 AND UMX.UserId = '<signInId>'
                                                    )

                                UNION ALL 
                                SELECT M.MnuId, M.RptCode, M.MnuType, M.MnuIcon, M.MnuTitle, M.MnuTitleShort, M.rptDesc, M.MnuDesc, M.MnuHyperlink, M.MnuTarget, M.MnuRights, M.MnuActive, M.MnuIsReady, M.MnuParent, M.AuthUser, M.AuthSite, M.DesignedBy
                                FROM SRM.Mnu M
                                INNER JOIN SRM.UserMnuX X ON X.MnuId = M.MnuId
                                INNER JOIN TMP ON TMP.MnuId = M.MnuParent
                                WHERE X.UserId = '<signInId>' <MENUISREADY>
                                AND M.MnuActive = '1'
							   
                                )
                                SELECT MnuId, RptCode, MnuType, MnuIcon, MnuTitle, MnuTitleShort, rptDesc, MnuDesc, MnuHyperlink, MnuTarget, MnuRights, MnuActive, MnuIsReady, MnuParent, AuthUser,AuthSite, DesignedBy 
                                FROM TMP
                                WHERE AuthSite = '<programName>' OR AuthSite = '*'
								 ";

                    sql = sql.Replace("<programName>", programName);
                    sql = sql.Replace("<signInId>", signinId);

                }
                if (!WInITTeam.Contains(firstName))
                    sql = sql.Replace("<MENUISREADY>", "AND M.MnuIsReady = 1 ");
                else
                    sql = sql.Replace("<MENUISREADY>", "");

                sql += " ORDER BY MnuParent, MnuTitle ";         
                oDAL = new cDAL(cDAL.ConnectionType.INIT);



                DataTable dt = new DataTable();
                dt = oDAL.GetData(sql);

                webMnu = dt.AsEnumerable().Select(dataRow => new WebMenus
                {
                    MnuId = Convert.ToInt32(dataRow["MnuId"]),
                    MnuType = dataRow["MnuType"].ToString(),
                    MnuIcon = dataRow["MnuIcon"].ToString(),
                    MnuTitle = dataRow["MnuTitle"].ToString(),
                    MnuTitleShort = dataRow["MnuTitleShort"].ToString(),
                    MnuHyperlink = dataRow["MnuHyperlink"].ToString(),
                    MnuTarget = dataRow["MnuTarget"].ToString(),
                    MnuParent = dataRow["MnuParent"].ToString(),
                    DesignedBy = dataRow["DesignedBy"].ToString(),
                    RptCode = dataRow["RptCode"].ToString(),
                    MnuDesc = dataRow["MnuDesc"].ToString(),
                    RptDesc = dataRow["rptDesc"].ToString()

                }).ToList<WebMenus>();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public DataSet GetConnections()
        {
            try
            {
                var dal = new cDAL(cDAL.ConnectionType.INIT);
                var connections = dal.GetData(@"
                    SELECT ConText
                          ,ConValue
                          ,ConType
                          ,IsDropDown
                    FROM SRM.zConStr ")
                    // IsDropDown is Used For Select TEST,TRAN,PRODRPT ADDED by Junaid Kalwar
                    .ToList<BaseCWConnection>()
                    .Select(c => new BaseCWConnection
                    {
                        ConText = c.ConText,
                        ConType = c.ConType,
                        ConValue = BasicEncrypt.Instance.Encrypt(c.ConValue),
                        IsDropDown = c.IsDropDown
                    });
                DataSet dsconnection = new DataSet();
                DataTable dtConn = connections.ConvertToDataTable();
                dtConn.TableName = "CONN";
                dsconnection.Tables.Add(dtConn);
                return dsconnection;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool ChangePassword(String oldPwd, string newPwd, string confirmPwd)
        {
              string UserName = HttpContext.Current.Session["UserName"].ToString();
        cDAL oDAL = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            string sql = @"DECLARE @Result VARCHAR(100);
EXEC  utl.spUserChangePassword '<userName>','<oldPwd>','<newPWD>','<confirmPWD>','Plus',@Result output
SELECT @Result ";

            sql = sql.Replace("<userName>", UserName);
            sql = sql.Replace("<oldPwd>", oldPwd);
            sql = sql.Replace("<newPWD>", newPwd);
            sql = sql.Replace("<confirmPWD>", confirmPwd);
            
            string result = oDAL.GetObject(sql).ToString();
            if (result.Contains("OK"))
            {
                Message = "Password has been reset.";
                return true;
            }
            else
            {
                Message = "Password not changed.";
                return false;
            }
        }

        public string GetConnectionString(string conType)
        {
            cDAL oDAL = new cDAL(cDAL.ConnectionType.INIT);
            sql = "SELECT ConValue FROM SRM.zConStr WHERE ConType = '" + conType + "'";
            DataTable dt = oDAL.GetData(sql);
            return dt.Rows[0]["ConValue"].ToString();
        }
        public DataTable GetRecord() //Added by Huzaifa
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string query = string.Empty;
            query = @"select top 1 RECNUM, RptUrl from zLogQuery 
                        where SigninId = '" + HttpContext.Current.Session["SigninId"].ToString() + "'" +
                        "AND QueryExecTime = '" + cDAL.QueryExecTime + "'" +
                        "AND RowsFetched = '" + cDAL.RowsFetched + "'";
                        //"AND RemoteHost = '" + HttpContext.Current.Session["RemoteAddr"].ToString() + "'";
            DataTable dt = oDAL.GetData(query);
            return dt;
        }
    }
  
    public class BaseCWConnection
    {
        public string ConText { get; set; }
        public string ConValue { get; set; }
        public string ConType { get; set; }
        public string IsDropDown { get; set; }// added by junaid

    }

    public class WebMenus
    {
        public int MnuId { get; set; }
        public string MnuType { get; set; }
        public string MnuIcon { get; set; }
        public string MnuTitle { get; set; }
        public string MnuTitleShort { get; set; }
        public string MnuHyperlink { get; set; }
        public string MnuTarget { get; set; }
        public string MnuParent { get; set; }
        public string DesignedBy { get; set; }
        public string RptCode { get; set; }
        public string MnuDesc { get; set; }
        public string RptDesc { get; set; }
        public string isAdmin { get; set; }
    }
   
}