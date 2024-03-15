using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace PlusCP.Models
{
    public class UserRoleX
    {
        cDAL oDAL;
        #region Fields
        [Required]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,15}$", ErrorMessage = "Your password must be at least 1 number, special character, upper case, lower case ")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "FirstName")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "LastName")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Id { get; set; }
        [Display(Name = "Id:")]
        public string Name { get; set; }
        public string Message { get; set; }

        public string MnuId { get; set; }
        [Display(Name = "Menu Id:")]

        public string MnuTitle { get; set; }

        [Display(Name = "Primary use:")]
        public string primaryUse { get; set; }

        public string isAdmin { get; set; }
        public bool isActiveUser { get; set; }
        public string isCustomer { get; set; }
        int vendorCounter = 0;

        public string lblMsg { get; set; }

        public List<Hashtable> lstUser { get; set; }
        public List<object> lstMst = new List<object>();
        public List<ArrayList> lstMnu { get; set; }
        public List<ArrayList> lstProgram { get; set; }
        public List<ArrayList> lstProgramById { get; set; }
        public List<ArrayList> FinallstProgram { get; set; }
        public List<ArrayList> lstMnuById { get; set; }
        public List<ArrayList> FinallstMnu { get; set; }
        public System.Web.Script.Serialization.JavaScriptSerializer serializer { get; set; }
        public string filterString { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorMessageUpdate { get; set; }

        #endregion

        public bool GetList(string IscreateUser,string type)
        {
            oDAL = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            string query = string.Empty;
            query = @"SELECT '' AS Edit
                            ,usr.ID
                            ,usr.FirstName
                            ,usr.LastName
                            ,usr.USERNAME
                            ,usr.EMAIL
                            ,CASE WHEN usr.ACTIVE = 1 THEN 'Y' ELSE 'N' END ACTIVE
                            ,CASE WHEN usrInfo.IsCustomer = 1 THEN 'Y' ELSE 'N' END IsCustomer
                            ,usrInfo.PrimaryUse
                            ,usr.PlusLastLogOn
                            ,usrInfo.CreatedBy
                            , FORMAT(usrInfo.CreatedOn, 'yyyy.MM.dd') AS CreatedOn
                      FROM[PlusExt].[dbo].[User] usr
                      LEFT JOIN[PlusRS].[EP].[UserInfo] usrInfo ON usr.Id = usrInfo.UserId ";

            if (!string.IsNullOrEmpty(type) && type != "-1")
            {
                query += "WHERE usr.ACTIVE ='@type'";
                //query = query.Replace("@type", type.ToString());
            }
            query = query.Replace("@type", type.ToString());

            if (IscreateUser == "Y")
            {
                query += "ORDER BY usr.ID DESC";
            }

            else
            {
                query += "ORDER BY IsCustomer DESC ,usr.FirstName ";
            }

            DataTable dt = oDAL.GetData(query);

            if (oDAL.HasErrors)
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }
            else
            {
                if (dt.Rows.Count > 0)
                    lstUser = cCommon.ConvertDtToHashTable(dt);
                return true;

            }
        } 

        public bool GetNewUser(string Username, string Password, string FirstName, string LastName, string Email)
        {
            string UserName = HttpContext.Current.Session["UserName"].ToString();
            cDAL oDAL = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            string sql = @"DECLARE @Result VARCHAR(100);
EXEC  utl.spUser 0, '<Username>','<Password>','<FirstName>','<LastName>','<Email>', 1, 'INSERT',@Result output
SELECT @Result ";

            //sql = sql.Replace("<Id>", Id);
            sql = sql.Replace("<Username>", Username);
            sql = sql.Replace("<Password>", Password);
            sql = sql.Replace("<FirstName>", FirstName);
            sql = sql.Replace("<LastName>", LastName);
            sql = sql.Replace("<Email>", Email);

            string result = oDAL.GetObject(sql).ToString();
            if (result.Contains("OK"))
            {
                oDAL = new cDAL();

                sql = @"INSERT INTO [dbo].[zLogEmail] ([SenderName],[SenderEmail],[SendTo],[Subject],[Body],[Html],[SentOn]) 
                     VALUES('PlusEP', 'noreply@reconext.com' , '" + Email + "' , 'A partner portal account created.', " +
                     "'<table> <tr>  Hi " + FirstName + ", <br/> <br/>    Username: " + Username + "<br/>  Password: " + Password + "<br/><br/> <Font color=green> <b>A Partner Portal Account Created </B></font> <br/> <br/>  Thanks </td>  <br/> <br/> </tr> <tr> <p> <Font color=red> Note: This Email is system generated. </font></p> </tr>  </table>' " +
                     " ,'Y' , NULL )  ";

                oDAL.Execute(sql);
                Message = "User has been Created.";
                return true;
            }
            else
            {
                Message = "User has not been Created.";
                return false;
            }
        }

        public bool GetProgram()
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);

            try
            {
                string sql = "SELECT ID, (Name + ' - ' + Site) AS NAME FROM pls.Program ORDER BY Name ";
                DataTable dtProgram = new DataTable();
               oDAL = new cDAL(cDAL.ConnectionType.ACTIVE);
                dtProgram = oDAL.GetData(sql);
                lstProgram = cCommon.ConvertDtToArrayList(dtProgram);
                if (!oDAL.HasErrors)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetMnu()
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);

            try
            {
                string sql = @"SELECT mnuid, mnutitle FROM [EP].[Mnu]
WHERE MnuActive = 1 AND MnuIsReady = 1 AND MNUPARENT IS NOT NULL
ORDER BY MnuTitle ";
                DataTable dtMnu = new DataTable();
                oDAL = new cDAL(cDAL.ConnectionType.INIT);
                dtMnu = oDAL.GetData(sql);
                lstMnu = cCommon.ConvertDtToArrayList(dtMnu);
                if (!oDAL.HasErrors)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetDataById(string Userid)
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            
            cDAL oDAL1 = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            DataTable dtActive = new DataTable();
            DataTable dtProgram = new DataTable();
            DataTable dtProgram1 = new DataTable();
            DataTable dtIsAdmin = new DataTable();
            DataTable dtMnu = new DataTable();
            DataTable dtMnu1 = new DataTable();
            //Program
            string isActiveUserQuery= @"SELECT FirstName,LastName,Active FROM [PlusExt].[dbo].[User] 
WHERE Id = '" + Userid + "' ";

            string programQuery1 = @"SELECT ID, NAME
                FROM (SELECT cast(ProgramID as int) as ID, Program as NAME, 1 AS SortOrder FROM [PlusRS].[EP].[UserProgramX] WHERE UserId = '" + Userid + "'" +
                " UNION ALL SELECT ID, (Name + ' - ' + Site) AS NAME, 2 AS SortOrder FROM Plus.pls.Program WHERE ID NOT IN " +
                "(SELECT ProgramID FROM [PlusRS].[EP].[UserProgramX] WHERE UserId = '" + Userid + "') ) AS CombinedData ORDER BY SortOrder, NAME ";

            string programQuery = @"SELECT CAST(ProgramID AS INT) AS ProgramID,Program FROM [PlusRS].[EP].[UserProgramX] 
WHERE UserId = '" + Userid + "' ";

            //IsAdmin
            string isAdminQuery = @"SELECT isAdmin,IsCustomer,PrimaryUse FROM [PlusRS].[EP].UserInfo 
WHERE UserId = '" + Userid + "' ";
            isAdminQuery += "order by isAdmin desc";
            //Menu
            string MenuQuery = @"SELECT MNUID FROM [PlusRS].[EP].[UserMnuX] 
WHERE USERID = '" + Userid + "' ";

            //string MenuQuery1 = @"SELECT MNUID,MnuTitle 
            //                    FROM( SELECT um.MNUID as MNUID, m.MnuTitle as MnuTitle,1 as SortOrder FROM [PlusRS].[EP].[UserMnuX] as um "+
            //                    "left join [PlusRS].[EP].[Mnu] as m on um.MnuId=m.MnuId WHERE USERID ='" + Userid + "' " +
            //                    "UNION ALL " +
            //                    "SELECT mnuid as MNUID, mnutitle as MnuTitle,2 as SortOrder FROM [PlusRS].[EP].[Mnu] " +
            //                    "WHERE MnuActive = 1 AND " +
            //                    "MnuIsReady = 1 AND " +
            //                    "MNUPARENT IS NOT NULL AND " +
            //                    "MNUID not in (SELECT MNUID FROM [PlusRS].[EP].[UserMnuX] WHERE USERID ='" + Userid + "') ) as CombinedData ORDER BY SortOrder,MnuTitle";
            string MenuQuery1 = @"SELECT a.mnuid, a.mnutitle, b.MnuTitle AS ParentMnu FROM [PlusRS].[EP].[Mnu] a
  INNER JOIN [PlusRS].[EP].[Mnu] b ON b.MnuId = a.mnuparent
WHERE a.mnuactive = 1 AND a.mnuisready = 1 AND a.mnuparent IS NOT NULL
ORDER BY b.MnuTitle, a.mnutitle";
            dtActive = oDAL1.GetData(isActiveUserQuery);
            dtProgram = oDAL.GetData(programQuery);
            dtProgram1 = oDAL.GetData(programQuery1);
            dtIsAdmin = oDAL.GetData(isAdminQuery);
            if (dtIsAdmin.Rows.Count > 0)
            {
                isAdmin = dtIsAdmin.Rows[0]["isAdmin"].ToString();
                isCustomer = dtIsAdmin.Rows[0]["IsCustomer"].ToString();
                primaryUse = dtIsAdmin.Rows[0]["PrimaryUse"].ToString();
            }
            isActiveUser=(Convert.ToInt32(dtActive.Rows[0]["Active"]) == 1)?true:false;
            FirstName = dtActive.Rows[0]["FirstName"].ToString();
            LastName = dtActive.Rows[0]["LastName"].ToString();
            dtMnu = oDAL.GetData(MenuQuery);
            dtMnu1 = oDAL.GetData(MenuQuery1);
            lstProgramById = cCommon.ConvertDtToArrayList(dtProgram);
            FinallstProgram = cCommon.ConvertDtToArrayList(dtProgram1);
            lstMnuById = cCommon.ConvertDtToArrayList(dtMnu);
            FinallstMnu = cCommon.ConvertDtToArrayList(dtMnu1);

            return true;
        
        }

        public bool SaveAxs(string userId, string program, string programName, string isAdmin,string isCustomer, string mnus, string primaryUse,bool isActiveUserchange)
        {
            ErrorMessageUpdate = "";
            #region UpdateUserRoleX_Get_current_Session
            //added by Junaid for DefaultProgram of current user
            string Default_Program = HttpContext.Current.Session["DefaultProgram"].ToString();
            //added by Junaid for DefaultDB of current user
            string Default_DB = HttpContext.Current.Session["DefaultDB"].ToString();
            //added by Junaid for DefaultDB of current user Name
            string ModifiedBy = HttpContext.Current.Session["FirstName"].ToString();
            #endregion


            //check if User Active needs a change
            cDAL oDAL1 = new cDAL(cDAL.ConnectionType.PLUS_EXT);
            //var change = (isActiveUserchange == true)? 1 : 0;
            //{
            //    string Activechange = @"UPDATE [PlusExt].[dbo].[User] SET Active = '" + change+"' where Id='"+ userId + "'";

            //    oDAL1.Execute(Activechange);
            //}

            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            DataTable dtCurrentUser = new DataTable();
            if (isAdmin == "true")
            {
                /**
                 * Delete role from Table
                 * Delete Commented by Muhammad Junaid Directed By Kashif Bhai
                 * string deleteRole = @"DELETE FROM [PlusRS].[EP].UserInfo 
                 * WHERE USERID = '" + userId + "'";   
                 * oDAL.AddQuery(deleteRole);
                 * **/

                #region UpdateUserRoleX
                string SearchUserID = @"select * from[PlusRS].[EP].UserInfo  WHERE USERID = '" + userId + "'";
                dtCurrentUser = oDAL.GetData(SearchUserID);
                if (dtCurrentUser.Rows.Count > 0)
                {
                   
                    
                    string updateUser = @"  UPDATE [PlusRS].[EP].UserInfo 
                                            SET    isAdmin='<is_Admin>',
                                                    DefaultProgram='<Default_Program>',
                                                    DefaultDB='<Default_DB>',
                                                    ModifiedBy='<Modified_By>',
                                                    ModifiedOn=GETDATE(),
                                                    IsCustomer='<isCustomer>',
                                                    PrimaryUse='<primaryUse>'
                                            where   UserId='<UserId>'";
                    updateUser = updateUser.Replace("<is_Admin>", isAdmin);
                    updateUser = updateUser.Replace("<Default_Program>", Default_Program);
                    updateUser = updateUser.Replace("<Default_DB>", Default_DB);
                    updateUser = updateUser.Replace("<Modified_By>", ModifiedBy);
                    updateUser = updateUser.Replace("<isCustomer>", isCustomer);
                    updateUser = updateUser.Replace("<primaryUse>", primaryUse);
                    updateUser = updateUser.Replace("<UserId>", userId);
                    oDAL.AddQuery(updateUser);
                }
                else
                { 
                    string createdBy = HttpContext.Current.Session["FirstName"].ToString();
                    string insertRole = @"INSERT INTO [PlusRS].[EP].UserInfo 
                                                  ( USERID, ISADMIN, CREATEDBY, PRIMARYUSE) 
                                           VALUES ( '<userId>','<isAdmin>','<createdBy>')";

                            insertRole = insertRole.Replace("<userId>", userId);
                            insertRole = insertRole.Replace("<isAdmin>", isAdmin);
                            insertRole = insertRole.Replace("<createdBy>", createdBy);
                            insertRole = insertRole.Replace("<primaryUse>", primaryUse);
                    oDAL.AddQuery(insertRole);
                }
                #endregion
            }

            else {
                bool check = checkData(userId, program, isAdmin, mnus, isCustomer);
                if (check == true)
                {
                    string createdBy = HttpContext.Current.Session["FirstName"].ToString();
                  /**
                  * Delete role from Table
                  * Delete Commented by Muhammad Junaid Directed By Kashif Bhai
                  * string deleteRole = @"DELETE FROM [PlusRS].[EP].UserInfo WHERE USERID = '" + userId + "'";
                  * oDAL.AddQuery(deleteRole);
                  **/

                    // Delete Program from Table 
                    string deleteProgram = @"DELETE FROM [PlusRS].[EP].UserProgramX 
WHERE USERID = '" + userId + "'";
                    oDAL.AddQuery(deleteProgram);

                    // Delete Menu from Table 
                    string deleteMnu = @"DELETE FROM [PlusRS].[EP].USERMNUX 
WHERE USERID = '" + userId + "'";
                    oDAL.AddQuery(deleteMnu);

                    string insertProgram = "";
                    string[] arrProgram = program.Split(',');
                    string[] arrProgramName = programName.Split(',');
        #region UpdateUserRoleX
        string SearchUserID = @"select * from[PlusRS].[EP].UserInfo  WHERE USERID = '" + userId + "'";
        dtCurrentUser = oDAL.GetData(SearchUserID);
                    var CBisActive = (isActiveUserchange == true) ? "Y" : "N";
                    if (dtCurrentUser.Rows.Count > 0)
        {
            string updateUser = @"  UPDATE [PlusRS].[EP].UserInfo 
                                SET    isAdmin='<is_Admin>',
                                        DefaultProgram='<Default_Program>',
                                        DefaultDB='<Default_DB>',
                                        ModifiedBy='<Modified_By>',
                                        ModifiedOn=GETDATE() ,
                                        IsCustomer='<isCustomer>',
                                        isActive='<CBisActive>',
                                        PrimaryUse='<primaryUse>'
                                where   UserId='<UserId>'";
            updateUser=updateUser.Replace("<is_Admin>", isAdmin);
            updateUser=updateUser.Replace("<Default_Program>", arrProgram[0]);
            updateUser=updateUser.Replace("<Default_DB>", Default_DB);
            updateUser=updateUser.Replace("<Modified_By>", ModifiedBy);
            updateUser = updateUser.Replace("<isCustomer>", isCustomer);
            updateUser = updateUser.Replace("<CBisActive>", CBisActive);
            updateUser = updateUser.Replace("<primaryUse>", primaryUse);
             
                        updateUser =updateUser.Replace("<UserId>", userId);

            oDAL.AddQuery(updateUser);
        }
        else
        {
        string insertRole = @"INSERT INTO [PlusRS].[EP].UserInfo 
                                ( UserId, IsAdmin, DefaultProgram,CreatedBy) 
                                VALUES ('<userId>','<isAdmin>','<defaultProgram>','<createdBy>')";
        insertRole = insertRole.Replace("<userId>", userId);
        insertRole = insertRole.Replace("<isAdmin>", isAdmin);
        insertRole = insertRole.Replace("<defaultProgram>", arrProgram[0]);
        insertRole = insertRole.Replace("<createdBy>", createdBy);
         insertRole = insertRole.Replace("<CBisActive>", CBisActive);
                        oDAL.AddQuery(insertRole);
                   
         }
        #endregion
              

                    foreach (string item in arrProgram)
                    {

                        insertProgram = @"INSERT INTO [PlusRS].[EP].[userProgramX] 
            (
             userid, 
             ProgramId, 
             Program,
             createdby
             ) 
            VALUES     
        (
            '<userId>', 
            '<programId>',
            '<program>',
            '<createdBy>'

) ";
                        insertProgram = insertProgram.Replace("<userId>", userId);
                        insertProgram = insertProgram.Replace("<programId>", item);
                        insertProgram = insertProgram.Replace("<program>", arrProgramName[vendorCounter]);
                        insertProgram = insertProgram.Replace("<createdBy>", createdBy);
                        vendorCounter = vendorCounter + 1;
                        oDAL.AddQuery(insertProgram);
                    }

                    string insertMnu = "";
                    string[] arrMnu = mnus.Split(',');
                    foreach (string item in arrMnu)
                    {
                        insertMnu = @"INSERT INTO [PlusRS].[EP].UserMnuX
            (
             userid, 
             MnuId, 
             createdby
             ) 
            VALUES     
        (
            '<userId>', 
            '<MnuId>', 
            '<createdBy>'
) ";
                        insertMnu = insertMnu.Replace("<userId>", userId);
                        insertMnu = insertMnu.Replace("<MnuId>", item);
                        insertMnu = insertMnu.Replace("<createdBy>", createdBy);
                        oDAL.AddQuery(insertMnu);
                    }
                   
                }
                else
                {
                    lblMsg = "N";
                    return false;
                }
            }

            oDAL.Commit();
            if (oDAL1.HasErrors)
            {
                ErrorMessageUpdate  = oDAL1.InternalErrMsg;
                //string sqlError = "Update denied";
            }
            if (!oDAL.HasErrors)
            {
                return true;
            }
            else
            {
                ErrorMessage = oDAL.ErrMessage;
                return false;
            }

        }

        public bool GetRoleButton()
        {


            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            string signinId = HttpContext.Current.Session["SigninId"].ToString();

            string query = @"SELECT isAdmin from [EP].[UserInfo] where USERID = '" + signinId + "'";
            DataTable dtIsAdmin = new DataTable();
            dtIsAdmin = oDAL.GetData(query);
            if (dtIsAdmin.Rows.Count > 0)
                isAdmin = dtIsAdmin.Rows[0]["isAdmin"].ToString();

            if (!oDAL.HasErrors)
                return true;
            else
                return false;
        }

        public bool checkData(string Userid, string program, string isAdmin, string mnu, string isCustomer)
        {
            oDAL = new cDAL(cDAL.ConnectionType.INIT);
            DataTable dtProgram = new DataTable();
            DataTable dtIsAdmin = new DataTable();
            DataTable dtMnu = new DataTable();
            string programQuery = @"SELECT ProgramId FROM [PlusRS].[EP].[UserProgramX] 
WHERE UserId = '" + Userid + "' ";

            //IsAdmin
            string isAdminQuery = @"SELECT isAdmin, isCustomer FROM [PlusRS].[EP].UserInfo 
WHERE UserId = '" + Userid + "' ";

            //Menu
            string MenuQuery = @"SELECT MNUID FROM [PlusRS].[EP].[UserMnuX] 
WHERE USERID = '" + Userid + "' ORDER BY MNUID DESC";

            dtProgram = oDAL.GetData(programQuery);
            dtIsAdmin = oDAL.GetData(isAdminQuery);
            dtMnu = oDAL.GetData(MenuQuery);

            string pvsProgram = convertDataTableToString(dtProgram);
            string pvsMnu = convertDataTableToString(dtMnu);

            string pvsIsAdmin;
            string pvsIsCustomer;
            if (dtIsAdmin.Rows.Count > 0)
            {
                pvsIsAdmin = dtIsAdmin.Rows[0]["IsAdmin"].ToString();
                pvsIsCustomer = dtIsAdmin.Rows[0]["IsCustomer"].ToString();
            }
            else {
                pvsIsAdmin = "false";
                pvsIsCustomer = "false";

            }
            //if (pvsIsAdmin == "True")
            //    pvsIsAdmin = "true";
            //else if(pvsIsAdmin == "False")
            //    pvsIsAdmin = "false";

            if (pvsProgram == program && pvsIsAdmin == isAdmin && pvsMnu == mnu && pvsIsCustomer == isCustomer)
                return false;
            else
                return true;

        }

        public static string convertDataTableToString(DataTable dataTable)
        {
            string data = string.Empty;
            int rowsCount = dataTable.Rows.Count;
            for (int i = 0; i < rowsCount; i++)
            {
                DataRow row = dataTable.Rows[i];
                int columnsCount = dataTable.Columns.Count;
                for (int j = 0; j < columnsCount; j++)
                {
                    data += row[j];
                    if (j == columnsCount - 1)
                    {
                        if (i != (rowsCount - 1))
                            data += ",";
                    }
                    else
                        data += "";
                }
            }
            return data;
        }
    }
}

public class clsProgram
{
    public string Id { get; set; }
    public string Name { get; set; }
}