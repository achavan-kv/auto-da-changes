using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace STL.PL
{
   public partial class CustomerPhotograph : CommonForm
   {
      public CustomerPhotograph(string custid,string fileName, Form root, Form parent)
      {
         InitializeComponent();
         FormRoot = root;
         FormParent = parent;
         CustomerID = custid;
         FileName = fileName;
      }

      private string m_custid = String.Empty;
      public string CustomerID
      {
         get { return m_custid; }
         set { m_custid = value; }
      }

      private string m_fileName = String.Empty;
      public string FileName
      {
         get { return m_fileName; }
         set { m_fileName = value; }
      }

      private string m_path = String.Empty;
      private string path
      {
         get
         {
            return m_path;
         }
         set
         {
            m_path = value;
         }
      }

      private string m_signaturePath = String.Empty;
      private string signaturePath
      {
         get
         {
            return m_signaturePath;
         }
         set
         {
            m_signaturePath = value;
         }
      }

      private void CustomerPhotograph_Load(object sender, EventArgs e)
      {
         //string fileName = CustomerManager.GetCustomerPhoto(CustomerID, out Error);
         string signatureFileName = CustomerManager.GetCustomerSignature(CustomerID, out Error);
         string passedPath = String.Empty;
            if (LoadPhoto(FileName, "p", out passedPath))
            {
               pbPhoto.Image = Image.FromFile(passedPath);
               pbPhoto.SizeMode = PictureBoxSizeMode.CenterImage; 
               path = passedPath;
            }
            else
            {
               //ShowInfo("M_NOPHOTO", MessageBoxButtons.OK);
            }

         if (signatureFileName != String.Empty)
         {
         if (LoadPhoto(signatureFileName, "s", out passedPath))
         {
            pbSignature.Image = Image.FromFile(passedPath);
            signaturePath = passedPath;       
         }
         }
         else
         {
            //ShowInfo("M_NOSIGNATURE", MessageBoxButtons.OK);
         }
      }

      private void CustomerPhotograph_FormClosing(object sender, FormClosingEventArgs e)
      {
         if(pbPhoto.Image != null && path != String.Empty)
         {
         pbPhoto.Image.Dispose();
         FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
         fs.Close();
         try
         {
            File.Delete(path);
         }
         catch
         {
            //Try to delete the file
         }
         }

         if (pbSignature.Image != null && signaturePath != String.Empty)
         {
            pbSignature.Image.Dispose();
            FileStream fs1 = new FileStream(signaturePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            fs1.Close();
            try
            {
               File.Delete(signaturePath);
            }
            catch
            {
               //Try to delete the file
            }
         }
      }
   }
}