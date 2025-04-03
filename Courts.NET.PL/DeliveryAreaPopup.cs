using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Crownwood.Magic.Menus;

namespace STL.PL
{
    public partial class DeliveryAreaPopup : CommonForm
    {
        public DeliveryAreaPopup()
        {
            InitializeComponent();
        }

        public string SelectedDeliveryArea = "";

        public DeliveryAreaPopup(Form root, Form parent,DataTable delAreas, string deliveryArea)
        {
            InitializeComponent();
            FormRoot = root;
            FormParent = parent;

            dvDelAreas.DataSource = delAreas;


            if (deliveryArea != string.Empty)
            {
                foreach (DataRow dr in ((DataTable)dvDelAreas.DataSource).Rows)
                {
                    if (Convert.ToString(dr["setname"]) == deliveryArea)
                    {
                        
                        dvDelAreas.CurrentCell =  dvDelAreas.Rows[delAreas.Rows.IndexOf(dr)].Cells[0];
                    }
                }
            }
        }

        private void dvDelAreas_MouseUp(object sender, MouseEventArgs e)
        {
            var index = 0;

            if (dvDelAreas.Rows.Count > 0)
            {
                index = dvDelAreas.CurrentRow.Index;

                if (e.Button == MouseButtons.Right)
                {

                    DataGridView ctl = (DataGridView)sender;

                    MenuCommand m1 = new MenuCommand("Select");

                    m1.Click += new System.EventHandler(this.DeliveryArea_Click);
               
                    PopupMenu popup = new PopupMenu();
                    popup.Animate = Animate.Yes;
                    popup.AnimateStyle = Animation.SlideHorVerPositive;
                    popup.MenuCommands.Add(m1);

                    MenuCommand selected = popup.TrackPopup(ctl.PointToScreen(new Point(e.X, e.Y)));
                }
            }
        }


        private void DeliveryArea_Click(object sender, EventArgs e)
        {
            SelectedDeliveryArea = Convert.ToString(((DataTable)dvDelAreas.DataSource).Rows[dvDelAreas.CurrentRow.Index]["setname"]);
            this.Close();
   
        }
    }
}
