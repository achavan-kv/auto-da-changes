namespace STL.PL
{
    partial class DeliveryAreaPopup
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dvDelAreas = new System.Windows.Forms.DataGridView();
            this.setname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.setdescript = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.empeeno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dateamend = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columntype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dvDelAreas)).BeginInit();
            this.SuspendLayout();
            // 
            // dvDelAreas
            // 
            this.dvDelAreas.AllowUserToAddRows = false;
            this.dvDelAreas.AllowUserToDeleteRows = false;
            this.dvDelAreas.AllowUserToResizeRows = false;
            this.dvDelAreas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dvDelAreas.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.setname,
            this.setdescript,
            this.empeeno,
            this.dateamend,
            this.columntype});
            this.dvDelAreas.Location = new System.Drawing.Point(12, 34);
            this.dvDelAreas.Name = "dvDelAreas";
            this.dvDelAreas.Size = new System.Drawing.Size(504, 305);
            this.dvDelAreas.TabIndex = 0;
            this.dvDelAreas.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dvDelAreas_MouseUp);
            // 
            // setname
            // 
            this.setname.DataPropertyName = "setname";
            this.setname.HeaderText = "Name";
            this.setname.Name = "setname";
            this.setname.ReadOnly = true;
            // 
            // setdescript
            // 
            this.setdescript.DataPropertyName = "setdescript";
            this.setdescript.HeaderText = "Description";
            this.setdescript.Name = "setdescript";
            this.setdescript.ReadOnly = true;
            this.setdescript.Width = 350;
            // 
            // empeeno
            // 
            this.empeeno.DataPropertyName = "empeeno";
            this.empeeno.HeaderText = "Empeeno";
            this.empeeno.Name = "empeeno";
            this.empeeno.Visible = false;
            // 
            // dateamend
            // 
            this.dateamend.DataPropertyName = "dateamend";
            this.dateamend.HeaderText = "Date Amend";
            this.dateamend.Name = "dateamend";
            this.dateamend.Visible = false;
            // 
            // columntype
            // 
            this.columntype.DataPropertyName = "columntype";
            this.columntype.HeaderText = "Column Type";
            this.columntype.Name = "columntype";
            this.columntype.Visible = false;
            // 
            // DeliveryAreaPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 427);
            this.Controls.Add(this.dvDelAreas);
            this.Name = "DeliveryAreaPopup";
            this.Text = "Delivery Areas";
            ((System.ComponentModel.ISupportInitialize)(this.dvDelAreas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dvDelAreas;
        private System.Windows.Forms.DataGridViewTextBoxColumn setname;
        private System.Windows.Forms.DataGridViewTextBoxColumn setdescript;
        private System.Windows.Forms.DataGridViewTextBoxColumn empeeno;
        private System.Windows.Forms.DataGridViewTextBoxColumn dateamend;
        private System.Windows.Forms.DataGridViewTextBoxColumn columntype;
    }
}