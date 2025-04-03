using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace STL.PL.StoreCard
{
    partial class StoreCardTextBox
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

         

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.context = new System.Windows.Forms.ContextMenu();
            this.menuCopy = new System.Windows.Forms.MenuItem();
            this.menuPaste = new System.Windows.Forms.MenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            // 
            // context
            // 
            this.context.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuCopy,
            this.menuPaste});
            // 
            // menuCopy
            // 
            this.menuCopy.Index = 0;
            this.menuCopy.Text = "Copy";
            this.menuCopy.Click += new System.EventHandler(this.OnMenuCopy);
            // 
            // menuPaste
            // 
            this.menuPaste.Index = 1;
            this.menuPaste.Text = "Paste";
            this.menuPaste.Click += new System.EventHandler(this.OnMenuPaste);
            // 
            // StoreCardTextBox
            // 
            this.ContextMenu = this.context;
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnKeyPress);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
