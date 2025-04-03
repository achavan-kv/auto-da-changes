using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using STL.Common.Collections;
using System.Windows.Forms;
using System.IO;

namespace STL.PL.Collections
{
    public partial class LetterMerge : CommonForm
    {
        private string error = "";
        private Object oMissing = System.Reflection.Missing.Value;
        private Object oFalse = false;
        private Object oTrue = true;
        private Word._Document wrdDoc;
       
        private string templateDirectory = "";

        public LetterMerge(Form root, Form parent)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;
            templateDirectory = Directory.GetParent(System.Reflection.Assembly.GetAssembly(this.GetType()).Location).FullName + "\\Templates\\";
        }

        private void LetterMerge_Load(object sender, EventArgs e)
        {
            try
            {
                //To Do - Deal with storetype, non-courts name, visibility, etc
                DataSet dsNonCourtsBranch = CollectionsManager.GetBranchByStoreType('N', out Error);
                if (Error.Length == 0 && dsNonCourtsBranch.Tables.Count > 0 && dsNonCourtsBranch.Tables[0].Rows.Count > 0)
                {
                    gbNonCourtsButton.Visible = true;
                }
                else
                {
                    gbNonCourtsButton.Visible = false;
                }

                DataSet ds = EodManager.GetInterfaceControl("","COLLECTIONS", false, out error);    //UAT1010 Charges letters not required
                dgvRunNo.DataSource = ds.Tables[0].DefaultView;
            }
            catch(Exception ex)
            {
                Catch(ex, "LetterMerge_Load");
            }
        }

        private void dgvRunNo_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                if (dgvRunNo.DataSource != null && dgvRunNo.CurrentRow != null)
                {
                    txtRunNo.Text = ((DataView)dgvRunNo.DataSource)[dgvRunNo.CurrentRow.Index]["runno"].ToString().Trim();
                    dgvLetterCodes.DataSource = null;
                }
            }
            catch (Exception ex)
            {
                Catch(ex, "dgvRunNo_SelectionChanged");
            }
        }

        private void btnLoadLetters_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet ds = CollectionsManager.GetDistinctLetterCodesByRunNo(Convert.ToInt16(txtRunNo.Text.Trim()), out error);
                //DataSet ds = CollectionsManager.GetDistinctLetterCodesByRunNo(0, out error);
                dgvLetterCodes.DataSource = ds.Tables[0].DefaultView;
                dgvLetterCodes.AutoGenerateColumns = false;
                chkIncludeSpouse.Checked = false;
                chkIncludeGuarantor.Checked = false;
            }
            catch (Exception ex)
            {
                Catch(ex, "btnLoadLetters_Click");
            }
        }

        private void btnGeneratePrint_Click(object sender, EventArgs e)
        {
            try
            {              
                Cursor.Current = Cursors.WaitCursor;
                if (dgvLetterCodes.DataSource == null || dgvLetterCodes.CurrentRow == null)
                {
                    return;
                }

                bool isCourts = (sender as Button).Tag.ToString().Trim() == "Courts";
                string templateSubdirectory = isCourts ? "Courts\\" : "NonCourts\\";
                
                //-- Making sure Word is installed and it's not already running -------------------------
                //--TODO
                //---------------------------------------------------------------------------------------

                string letterCode = ((DataView)dgvLetterCodes.DataSource)[dgvLetterCodes.CurrentRow.Index]["lettercode"].ToString().Trim();
                // UAT1010 jec 12/03/10 - pass runno selected
                DataSet dsLetterFields = CollectionsManager.LoadLetterFieldsbyCode(letterCode, Convert.ToInt32(txtRunNo.Text.Trim()), isCourts? 'C' : 'N', 
                                                                                    chkIncludeSpouse.Checked, chkIncludeGuarantor.Checked, out error);

                if (dsLetterFields.Tables.Count <= 0 || dsLetterFields.Tables[0].Rows.Count <= 0)
                {
                    MessageBox.Show("No Accounts Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                               
                string template_M_FileName = letterCode.Trim() + "_template.doc";
                string template_S_FileName = letterCode.Trim() + "_S_template.doc";
                string template_G_FileName = letterCode.Trim() + "_G_template.doc";
                
                //-- Checking whether template files exist-----------------------------------------------
                string filesNotFound = "";
                if (File.Exists(templateDirectory + templateSubdirectory + template_M_FileName) == false)
                {
                    filesNotFound = filesNotFound + " " + template_M_FileName;
                }
                if (chkIncludeSpouse.Checked && File.Exists(templateDirectory + templateSubdirectory + template_S_FileName) == false)
                {
                    filesNotFound = filesNotFound + " " + template_S_FileName;
                }
                if (chkIncludeGuarantor.Checked && File.Exists(templateDirectory + templateSubdirectory + template_G_FileName) == false)
                {
                    filesNotFound = filesNotFound + " " + template_G_FileName;
                }

                if (filesNotFound.Trim() != "")
                {
                    MessageBox.Show("Template(s) " + filesNotFound.Trim() + " is(are) not found under the directory " + (templateDirectory + templateSubdirectory).TrimEnd('\\'), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                //---------------------------------------------------------------------------------------
                
                //-- Printing the Main Account Holder letter---------------------------------------------
                long tick1 = DateTime.Now.Ticks;
                StartMailMerge(dsLetterFields.Tables[0], templateDirectory + templateSubdirectory + template_M_FileName, letterCode + "_M");
                Console.WriteLine("Total Time Taken 1" + (DateTime.Now.Ticks - tick1));
                //---------------------------------------------------------------------------------------


                //-- Printing the Spouse letter----------------------------------------------------------
                if (chkIncludeSpouse.Checked) 
                {
                    if (dsLetterFields.Tables.Contains("SPOUSE") && dsLetterFields.Tables["SPOUSE"].Rows.Count > 0)
                    {
                        long tick2 = DateTime.Now.Ticks;
                        StartMailMerge(dsLetterFields.Tables["SPOUSE"], templateDirectory + templateSubdirectory + template_S_FileName, letterCode + "_S");
                        Console.WriteLine("Total Time Taken 2" + (DateTime.Now.Ticks - tick2));
                    }
                    else
                    {
                        MessageBox.Show("No Spouse Details Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }                
                //---------------------------------------------------------------------------------------

                //-- Printing the Guarantor letter-------------------------------------------------------
                if (chkIncludeGuarantor.Checked)
                {
                    if (dsLetterFields.Tables.Count > 2 && dsLetterFields.Tables[2].Rows.Count > 0)
                    {
                        long tick3 = DateTime.Now.Ticks;
                        StartMailMerge(dsLetterFields.Tables[2], templateDirectory + templateSubdirectory + template_G_FileName, letterCode + "_G");
                        Console.WriteLine("Total Time Taken 3" + (DateTime.Now.Ticks - tick3));
                    }
                    else
                    {
                        MessageBox.Show("No Guarantor Details Found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                //---------------------------------------------------------------------------------------
            }
            catch(Exception ex)
            {
                Catch(ex, "btnCtryourtsGeneratePrint_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void FillRow(Word._Document oDoc, int rowIndex, DataRow dr)
        {
            for (int i = 0; i < dr.ItemArray.Length; i++)
            {
                oDoc.Tables.Item(1).Cell(rowIndex, i + 1).Range.InsertAfter(dr[i].ToString());
            }
        }

        private void StartMailMerge(DataTable dtFields, string templateFilePath, string fileTypeLiteral)
        {
            string strDataFileName = "";
            Word.Application wrdApp = null;

            try
            {
                Object oName = templateFilePath;
                wrdApp = new Word.Application();
                wrdApp.Visible = false;  //-- Make it invisible, until the data file is created

                // Open the file to insert data.
                wrdDoc = wrdApp.Documents.Open(ref oName, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing);

                // Create a MailMerge Data file.
                CreateMailMergeDataFile(dtFields, out strDataFileName, wrdApp);

                wrdApp.Visible = true;  //-- Now make it visible

                // Perform mail merge.
                wrdDoc.MailMerge.Destination = Word.WdMailMergeDestination.wdSendToNewDocument;
                wrdDoc.MailMerge.Execute(ref oFalse);

                Object mergedFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\merged_" + fileTypeLiteral + "_" + 
                                        DateTime.Today.Day + "_" + DateTime.Today.Month + "_" + DateTime.Today.Year + "_" + DateTime.Now.Ticks.ToString() + ".doc";
                wrdApp.ActiveDocument.SaveAs(ref mergedFileName, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            }
            catch(Exception ex)
            {
                try
                {
                    if (wrdApp != null)  //If word application is not null, we must close it
                        ((Word._Application)wrdApp).Quit(ref oFalse, ref oMissing, ref oMissing);
                }
                finally
                {
                    throw ex;
                }               
            }
            finally
            {
                if (wrdApp != null && wrdDoc != null)
                {                    
                    wrdDoc.Saved = true;
                    wrdDoc.Close(ref oFalse, ref oMissing, ref oMissing); //-- Original template document.
                    wrdDoc = null;
                } 

                if(strDataFileName != "")
                    System.IO.File.Delete(strDataFileName); //-- Clean up temp data file                
            }       
        }

        private void CreateMailMergeDataFile(DataTable dtFields, out String tempDataFileName, Word.Application wrdApp)
        {
            Word._Document oDataDoc = null;

            Object oFileName = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\\Temp_Data" + Guid.NewGuid().ToString() + ".doc";
            tempDataFileName = oFileName.ToString();

            try
            {
                string strHeader = "";
                foreach (DataColumn dc in dtFields.Columns)
                {
                    strHeader = strHeader + ", " + dc.ColumnName;
                }
                strHeader = strHeader.Remove(0, 2);
                Object oHeader = strHeader;
                
                wrdDoc.MailMerge.CreateDataSource(ref oFileName, ref oMissing,
                                                ref oMissing, ref oHeader, ref oMissing, ref oMissing,
                                                ref oMissing, ref oMissing, ref oMissing);

                //Open the data file to insert data. (Temporary Document)
                oDataDoc = wrdApp.Documents.Open(ref oFileName, ref oMissing,
                        ref oMissing, ref oFalse, ref oMissing, ref oMissing,
                        ref oTrue, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oFalse, ref oMissing, ref oMissing,
                        ref oMissing);

                for (int i = 0; i < dtFields.Rows.Count; i++)
                {
                    oDataDoc.Tables.Item(1).Rows.Add(ref oMissing);
                    FillRow(oDataDoc, i + 2, dtFields.Rows[i]);
                }

                oDataDoc.Save();
            }
            finally
            {
                if (oDataDoc != null)
                    oDataDoc.Close(ref oFalse, ref oMissing, ref oMissing);
            }
        }

        ////This is a test method. Plz don't delete
        //private void ReadTemplateFile(DataTable dtFields)
        //{
        //    //This is a test method. Plz don't delete
        //    string strRTFContents = "";
        //    StreamReader sr = new StreamReader(@"C:\C2_template.txt");
        //    strRTFContents = sr.ReadToEnd();
        //    sr.Close();
            
        //    string[] strSplitted = strRTFContents.Split(new String[] { "**" }, StringSplitOptions.RemoveEmptyEntries);
            
        //    //wrdApp = new COM_Word.Application();
        //    int count = 0;

            
        //    foreach (DataRow dr in dtFields.Rows)
        //    {
        //        long tick1 = DateTime.Now.Ticks;

        //        StringBuilder builder = new StringBuilder();

        //        for (int i = 0; i < strSplitted.Length; i++)
        //        {
                    
        //            if (i % 2 == 0)
        //            {
        //                builder.Append(strSplitted[i]);
        //            }
        //            else
        //            {
        //                //StringBuilder tempBuilder = new StringBuilder(strSplitted[i]);

        //                ////for (int j = 0; j < dtFields.Columns.Count; j++)
        //                ////{
        //                ////    tempBuilder.Replace(dtFields.Columns[j].ToString(), dr[j].ToString());                        
        //                ////}
        //                ////if(strSplitted[i] == "titlke
        //                if(dtFields.Columns.Contains(strSplitted[i]))
        //                    builder.Append(dr[strSplitted[i]]);
        //            }
        //        }


        //        Console.WriteLine("APPENDING " + (DateTime.Now.Ticks - tick1));

        //        //Object oFileName = @"C:\LetterMerged\" + count; 

        //        //COM_Word.Document oDataDoc = wrdApp.Documents.Open(ref oFileName, ref oMissing,
        //        //        ref oMissing, ref oFalse, ref oMissing, ref oMissing,
        //        //        ref oTrue, ref oMissing, ref oMissing, ref oMissing,
        //        //        ref oMissing, ref oFalse, ref oMissing, ref oMissing,
        //        //        ref oMissing, ref oMissing);
                
                
        //        //COM_Word.Range rng = oDataDoc.Range(ref start1,ref missing);
              
        //        //rng.InsertAfter(”Hello World!”);
                               
        //        tick1  = DateTime.Now.Ticks;
            
        //        StreamWriter sw = new StreamWriter(@"C:\LetterMerged\C2_" + (++count) +".doc");
        //        sw.Write(builder.ToString());
        //        sw.Close();
        //        Console.WriteLine("WRITING " + (DateTime.Now.Ticks - tick1));
        //    }                       
        //}

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Word.Application tempWrdApp = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                if (dgvLetterCodes.DataSource == null || dgvLetterCodes.CurrentRow == null)
                {
                    return;
                }

                bool isCourts = (sender as Button).Tag.ToString().Trim() == "Courts";
                string templateSubdirectory = isCourts ? "Courts\\" : "NonCourts\\";

                //-- Making sure Word is installed ------------------------------------------------------
                //--TODO
                //---------------------------------------------------------------------------------------
                string letterCode = ((DataView)dgvLetterCodes.DataSource)[dgvLetterCodes.CurrentRow.Index]["lettercode"].ToString().Trim();
                string templateFileName = letterCode.Trim() + "_template.doc";
                Object oName = (templateDirectory + templateSubdirectory + templateFileName);

                if (File.Exists(templateDirectory + templateSubdirectory + templateFileName) == false)
                {
                    MessageBox.Show("Template " + templateFileName + " is not found under the directory " + (templateDirectory + templateSubdirectory).TrimEnd('\\'), "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                
                tempWrdApp = new Word.Application();
                tempWrdApp.Visible = true;  
 
                long tick1 = DateTime.Now.Ticks;
                tempWrdApp.Documents.Open(ref oName, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oTrue, ref oMissing, ref oMissing,
                        ref oMissing);        
                Console.WriteLine("Total Time Taken to open the doc " + (DateTime.Now.Ticks - tick1));
            }
            catch (Exception ex)
            {
                try
                {
                    if (tempWrdApp != null)  //If word application is not null, we must close it
                        tempWrdApp.Quit(ref oMissing, ref oMissing, ref oMissing);
                }
                catch
                {
                }

                Catch(ex, "btnCourtsEdit_Click");
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
