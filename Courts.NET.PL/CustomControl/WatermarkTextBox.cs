using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace STL.PL.CustomControl
{
    public class WatermarkTextBox : TextBox
    {
        private string _watermark;

        /// <summary>
        /// Gets or Sets the text that will be presented as the watermak hint
        /// </summary>
        public string Watermark
        {
            get { return _watermark; }
            set
            {
                _watermark = value;
                CueBannerHelper.SetCueBanner(this, value);
            }
        }

    }
}
