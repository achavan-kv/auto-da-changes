namespace STL.PL.Collections
{
   partial class ConditionsControl
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConditionsControl));
         this.dgvQualifiers = new System.Windows.Forms.DataGridView();
         this.dgvChosenQualifiers = new System.Windows.Forms.DataGridView();
         this.btnRemove = new System.Windows.Forms.Button();
         this.btnAdd = new System.Windows.Forms.Button();
         this.drpOperator = new System.Windows.Forms.ComboBox();
         this.textBox1 = new System.Windows.Forms.TextBox();
         this.textBox2 = new System.Windows.Forms.TextBox();
         this.btnOR = new System.Windows.Forms.Button();
         ((System.ComponentModel.ISupportInitialize)(this.dgvQualifiers)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.dgvChosenQualifiers)).BeginInit();
         this.SuspendLayout();
         // 
         // dgvQualifiers
         // 
         this.dgvQualifiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dgvQualifiers.Location = new System.Drawing.Point(34, 43);
         this.dgvQualifiers.Name = "dgvQualifiers";
         this.dgvQualifiers.Size = new System.Drawing.Size(260, 290);
         this.dgvQualifiers.TabIndex = 1;
         // 
         // dgvChosenQualifiers
         // 
         this.dgvChosenQualifiers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         this.dgvChosenQualifiers.Location = new System.Drawing.Point(537, 43);
         this.dgvChosenQualifiers.Name = "dgvChosenQualifiers";
         this.dgvChosenQualifiers.Size = new System.Drawing.Size(260, 290);
         this.dgvChosenQualifiers.TabIndex = 2;
         // 
         // btnRemove
         // 
         this.btnRemove.Enabled = false;
         this.btnRemove.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
         this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
         this.btnRemove.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.btnRemove.Location = new System.Drawing.Point(336, 115);
         this.btnRemove.Name = "btnRemove";
         this.btnRemove.Size = new System.Drawing.Size(40, 24);
         this.btnRemove.TabIndex = 4;
         // 
         // btnAdd
         // 
         this.btnAdd.Enabled = false;
         this.btnAdd.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
         this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
         this.btnAdd.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.btnAdd.Location = new System.Drawing.Point(448, 115);
         this.btnAdd.Name = "btnAdd";
         this.btnAdd.Size = new System.Drawing.Size(40, 24);
         this.btnAdd.TabIndex = 5;
         // 
         // drpOperator
         // 
         this.drpOperator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.drpOperator.DropDownWidth = 121;
         this.drpOperator.ItemHeight = 13;
         this.drpOperator.Location = new System.Drawing.Point(300, 166);
         this.drpOperator.Name = "drpOperator";
         this.drpOperator.Size = new System.Drawing.Size(130, 21);
         this.drpOperator.TabIndex = 41;
         // 
         // textBox1
         // 
         this.textBox1.Location = new System.Drawing.Point(436, 167);
         this.textBox1.MaxLength = 12;
         this.textBox1.Name = "textBox1";
         this.textBox1.Size = new System.Drawing.Size(40, 20);
         this.textBox1.TabIndex = 42;
         // 
         // textBox2
         // 
         this.textBox2.Location = new System.Drawing.Point(482, 167);
         this.textBox2.MaxLength = 12;
         this.textBox2.Name = "textBox2";
         this.textBox2.Size = new System.Drawing.Size(40, 20);
         this.textBox2.TabIndex = 43;
         // 
         // btnOR
         // 
         this.btnOR.Enabled = false;
         this.btnOR.ImeMode = System.Windows.Forms.ImeMode.NoControl;
         this.btnOR.Location = new System.Drawing.Point(390, 221);
         this.btnOR.Name = "btnOR";
         this.btnOR.Size = new System.Drawing.Size(51, 24);
         this.btnOR.TabIndex = 44;
         this.btnOR.Tag = "";
         this.btnOR.Text = "OR";
         this.btnOR.Visible = false;
         // 
         // ConditionsControl
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.Controls.Add(this.btnOR);
         this.Controls.Add(this.textBox2);
         this.Controls.Add(this.textBox1);
         this.Controls.Add(this.drpOperator);
         this.Controls.Add(this.btnAdd);
         this.Controls.Add(this.btnRemove);
         this.Controls.Add(this.dgvChosenQualifiers);
         this.Controls.Add(this.dgvQualifiers);
         this.Name = "ConditionsControl";
         this.Size = new System.Drawing.Size(838, 411);
         ((System.ComponentModel.ISupportInitialize)(this.dgvQualifiers)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.dgvChosenQualifiers)).EndInit();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.DataGridView dgvQualifiers;
      private System.Windows.Forms.DataGridView dgvChosenQualifiers;
      private System.Windows.Forms.Button btnRemove;
      private System.Windows.Forms.Button btnAdd;
      private System.Windows.Forms.ComboBox drpOperator;
      public System.Windows.Forms.TextBox textBox1;
      public System.Windows.Forms.TextBox textBox2;
      private System.Windows.Forms.Button btnOR;
   }
}
