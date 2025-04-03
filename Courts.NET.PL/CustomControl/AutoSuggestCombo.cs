using System;
using System.Windows.Forms;

namespace STL.PL.CustomControl
{
    public class AutoSuggestCombo : ComboBox
    {
        /// <summary>
        /// The text that will be presented as the watermak hint
        /// </summary>
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

        /// <summary>
        /// Create a new TextBox that supports watermak hint
        /// </summary>
        public AutoSuggestCombo()
        {
            this.AutoCompleteMode = AutoCompleteMode.Suggest;
            this.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (this.IsHandleCreated)
                CueBannerHelper.SetCueBanner(this, _watermark);
        }
    }
}
