using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security.Permissions;
using System.Net;
using System.Configuration;
using System.Linq;
using System.Collections.Generic;
using System.IO;

public class cImpersonate : IDisposable
{
    [DllImport("advapi32.dll", SetLastError = true)]
    public static extern bool LogonUser(
            String lpszUsername,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public extern static bool CloseHandle(IntPtr handle);

    private static IntPtr tokenHandle = new IntPtr(0);
    private static WindowsImpersonationContext impersonatedUser;
    cLog oLog = new cLog();
    // If you incorporate this code into a DLL, be sure to demand that it
    // runs with FullTrust.
    [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
    public cImpersonate()
    {
        //string keyFileName = @"C:\Clover\appuser.txt";
        string keyFileName = @"C:\Connection\appuser.txt";
        string domainName = "", userName = "", password = "";

        try
        {
            List<string> lines = File.ReadLines(keyFileName).ToList();

            domainName = lines[0];
            userName = lines[1];
            password = lines[2];
        }
        catch (Exception ex)
        {
            oLog.RecordError(ex.Message,ex.StackTrace,"Impersonation");
        }

        try
        {
            // Use the unmanaged LogonUser function to get the user token for
            // the specified user, domain, and password.
            //const int LOGON32_PROVIDER_DEFAULT = 0;
            const int LOGON_TYPE_NEW_CREDENTIALS = 9;

            // Passing this parameter causes LogonUser to create a primary token.
            //const int LOGON32_LOGON_INTERACTIVE = 2;
            const int LOGON32_PROVIDER_WINNT50 = 3;
            
            tokenHandle = IntPtr.Zero;

            // ---- Step - 1 
            // Call LogonUser to obtain a handle to an aCcess token.
            bool returnValue = LogonUser(
                                userName,
                                domainName,
                                password,
                                LOGON_TYPE_NEW_CREDENTIALS,
                                LOGON32_PROVIDER_WINNT50,
                                ref tokenHandle);         // tokenHandle - new security token

            if (false == returnValue)
            {
                int ret = Marshal.GetLastWin32Error();
                Console.WriteLine("LogonUser call failed with error code : " + ret);
                throw new System.ComponentModel.Win32Exception(ret);
            }

            // ---- Step - 2 
            WindowsIdentity newId = new WindowsIdentity(tokenHandle);

            // ---- Step - 3 
            impersonatedUser = newId.Impersonate();
        }
        catch (Exception ex)
        {
            oLog.RecordError(ex.Message, ex.StackTrace, "Impersonation");
        }
    }

    public void Dispose()
    {
        impersonatedUser.Undo();
        // Free the tokens.
        if (tokenHandle != IntPtr.Zero)
            CloseHandle(tokenHandle);
    }
}
