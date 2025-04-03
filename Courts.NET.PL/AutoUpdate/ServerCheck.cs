using System;
using System.Windows.Forms;
using Blue.Cosacs.Shared.Services;
using Blue.Cosacs.Shared.Services.CosacsConfig;
using STL.Common.Static;
using STL.PL.Utils;

namespace STL.PL
{
    public class ServerCheck : CommonForm
    {
        public void CheckConnection(string version, string branchno, Action<bool, string> logAction, Action<Exception> ShowErrorAction, Action PostErrorAction)
        {
            Client.Call(new CheckConnRequest
            {
                Version = version,
                BranchNo = branchno,
                CurrentDate = DateTime.Now,
                Login = Credential.User,
                Password = Credential.Password,
                MachineName = Environment.MachineName
            },
            response =>
            {
                if (response.IsLocked)
                {
                    MessageBox.Show("Your account is currently locked. Please contact your System Administrator.", "Account Locked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (response.IsCorrectServer)
                    MessageBox.Show(String.Format("There is difference between the Client ({0}) and Server ({1}) versions. You will not be able to login until this client is updated.", version, response.WrongServerVersion), "Client Version Incorrect - Login Suspended.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (response.DatetimeMismatch)
                    MessageBox.Show("There is difference between the Client and Server date or time. Please check if they correct.", "Client Time Incorrect - Login Suspended.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                else if (!response.IsValidBranch)
                {
                    MessageBox.Show(String.Format("The selected branch {0} is not a valid branch.", branchno), "Invalid Branch Selected.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ConfigMaintenance config = new ConfigMaintenance(true);
                    config.FormRoot = MainForm.Current;
                    config.FormParent = MainForm.Current;
                    config.ShowDialog();
                    PostErrorAction();
                }
                else
                {
                    Config.StoreType = response.Storetype;
                    Config.CountryCode = response.Country.Trim();               //IP - 18/04/12 - #9494
                    Credential.Name = response.FullName;
                    // TODO Credential.Cookie = 
                    Credential.UserId = response.UserId;
                    Credential.Roles = new string[] { }; // TODO Permissions response.UserRole };
                    Credential.SetPermissions(response.Permissions);
                    logAction(response.ShouldChangePassword, response.FullName);
                    Config.Connected = true;
                }
            }, (response, exception) =>
            {
                PostErrorAction();
                if (exception is Blue.Cosacs.Shared.Net.ServerException)
                {
                    var exceptionName = ((Blue.Cosacs.Shared.Net.ServerException)exception).ServerExceptionName;
                    var exMessage = ((Blue.Cosacs.Shared.Net.ServerException)exception).ServerStackTrace;
                    if (exceptionName == "WebException")
                        MessageBox.Show("There is an error connecting to the Web Server. \n\n" + exMessage, "Error Connecting to Web Server.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (exceptionName == "SqlException")
                        MessageBox.Show("There is an error connecting to the Database Server. \n\n" + exMessage, "Error Connecting to Database Server.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    else if (exceptionName == "SecurityException")
                        MainForm.Current.statusBar1.Text = "Incorrect user name password combination. Please try again.";
                    else ShowErrorAction(exception);
                }
                else
                {
                    ShowErrorAction(exception);
                }

            }, MainForm.Current);
        }
    }
}
