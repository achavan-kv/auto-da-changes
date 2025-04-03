using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using STL.Common;
using STL.PL.WS5;
using STL.Common.Constants.ColumnNames;
using STL.Common.Constants.TableNames;
using System.Xml;
using STL.Common.Static;
using Crownwood.Magic.Menus;
using System.Collections.Specialized;
using System.Data;
using System.Web.Services.Protocols;

namespace STL.PL
{
	/// <summary>
	/// Popup prompt to request a date to be entered for journalling
	/// sundry transactions.
	/// </summary>
	public class JournalDate : CommonForm
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ErrorProvider errors;
		private System.Windows.Forms.DateTimePicker dtJournalDate;
        private new string Error = "";
        private Button btnOK;
        private IContainer components;

		public JournalDate(Form parent)
		{
			InitializeComponent();
			FormParent = parent;
			// take off around 6 months 
			dtJournalDate.Value = DateTime.Today.AddDays(-180);
			dtJournalDate.MaxDate = StaticDataManager.GetServerDateTime();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.dtJournalDate = new System.Windows.Forms.DateTimePicker();
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.btnOK = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(160, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please enter a date to journal sundry transactions up to";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dtJournalDate
            // 
            this.dtJournalDate.Location = new System.Drawing.Point(24, 64);
            this.dtJournalDate.Name = "dtJournalDate";
            this.dtJournalDate.Size = new System.Drawing.Size(144, 20);
            this.dtJournalDate.TabIndex = 1;
            this.dtJournalDate.Validated += new System.EventHandler(this.dtJournalDate_Validated);
            // 
            // errors
            // 
            this.errors.ContainerControl = this;
            this.errors.DataMember = "";
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(58, 100);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // JournalDate
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(194, 135);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dtJournalDate);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "JournalDate";
            this.Text = "Journal Date";
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

      //69301 Code removed from dtJournalDate_Validating event because it is not firing. The functionality is now contained in IP's btnOK_Click event.
        //private void dtJournalDate_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        //{
        // //try
        // //{
        // //   Wait();

        // //   decimal total  = 0;

        // //   AccountManager.GetSundryAccountTransactionTotal(Convert.ToInt16(Config.BranchCode), dtJournalDate.Value, out total, out Error);
        // //   if(Error.Length > 0)
        // //      ShowError(Error);
        // //   else
        // //   {
        // //      ((TransferTransaction)FormParent).JournalValue = total;
        // //      ((TransferTransaction)FormParent).JournalDate = dtJournalDate.Value;
        // //      Close();
        // //   }
        // //}
        // //catch(Exception ex)
        // //{
        // //   Catch(ex, "btnTotal_Click");
        // //}
        // //finally
        // //{
        // //   StopWait();
        // //}
        //}

        private void dtJournalDate_Validated(object sender, EventArgs e)
        {

        }
        // Code move to here from dtJournalDate_Validating method 
        // dtJournalDate_Validating method does not get activated in .Net v2
        //private void JournalDate_FormClosed(object sender, FormClosedEventArgs e)
        //{
        //    try
        //    {
        //        Wait();

        //        decimal total = 0;

        //        AccountManager.GetSundryAccountTransactionTotal(Convert.ToInt16(Config.BranchCode), dtJournalDate.Value, out total, out Error);
        //        if (Error.Length > 0)
        //            ShowError(Error);
        //        else
        //        {
        //            ((TransferTransaction)FormParent).JournalValue = total;
        //            ((TransferTransaction)FormParent).JournalDate = dtJournalDate.Value;
        //            Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Catch(ex, "btnTotal_Click");
        //    }
        //    finally
        //    {
        //        StopWait();
        //    }
        //}

        //IP - Added an 'OK' button and added the code that was previously in the
        // JournalDate_FormClosed event handler to the below event handler.
        private void btnOK_Click(object sender, EventArgs e)
        {

            try
            {
                Wait();

                decimal total = 0;

                AccountManager.GetSundryAccountTransactionTotal(Convert.ToInt16(Config.BranchCode), dtJournalDate.Value, out total, out Error);
                if (Error.Length > 0)
                    ShowError(Error);
                else
                {
                    ((TransferTransaction)FormParent).JournalValue = total;
                    ((TransferTransaction)FormParent).JournalDate = dtJournalDate.Value;
                    Close();
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "btnOK_Click");
            }
            finally
            {
                StopWait();
            }

        }     

	}
}
