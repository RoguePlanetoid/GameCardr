using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WatermarkTextbox
{   
    /// <summary>WatermarkTextBox</summary>
    public class WatermarkTextBox : TextBox
    {

        #region Private Constants
        private const string VALUE_WATERMARK = "Watermark";
        private const string STYLE_WATERMARK = "WatermarkStyle";
        private const string CONTENT_WATERMARK = "WatermarkContent";
        #endregion

        #region Dependancy Properties
        public static readonly DependencyProperty WatermarkProperty =
	    DependencyProperty.Register(VALUE_WATERMARK, typeof(object), typeof(WatermarkTextBox), 
        new PropertyMetadata(OnWatermarkPropertyChanged));

		public static readonly DependencyProperty WatermarkStyleProperty =
	    DependencyProperty.Register(STYLE_WATERMARK, typeof(Style), typeof(WatermarkTextBox), null);
        #endregion

        #region Private Members
        ContentControl WatermarkContent;
        #endregion

        #region Private Methods
        /// <summary>Determine Watermark Content Visiblity</summary>
        private void DetermineWatermarkContentVisibility()
        {
            if (string.IsNullOrEmpty(this.Text))
            {
                this.WatermarkContent.Visibility = Visibility.Visible;
            }
            else
            {
                this.WatermarkContent.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>Watermark Style</summary>
        public Style WatermarkStyle
		{
			get { return base.GetValue(WatermarkStyleProperty) as Style; }
			set { base.SetValue(WatermarkStyleProperty, value); }
		}

        /// <summary>Watermark</summary>
		public object Watermark
		{
			get { return base.GetValue(WatermarkProperty) as object; }
			set { base.SetValue(WatermarkProperty, value); }
		}
        #endregion

        #region Public Methods
        /// <summary>ToggleWatermark</summary>
        public void ToggleWatermark()
        {
            DetermineWatermarkContentVisibility();
        }
        #endregion

        /// <summary>Constructor</summary>
        public WatermarkTextBox()
		{
			DefaultStyleKey = typeof(WatermarkTextBox);
		}

        #region Event Handlers
        /// <summary>OnApplyTemplate</summary>
        public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			this.WatermarkContent = this.GetTemplateChild(CONTENT_WATERMARK) as ContentControl;
			if(WatermarkContent != null)
			{
			  DetermineWatermarkContentVisibility();
			}
		}

        /// <summary>OnGotFocus</summary>
        /// <param name="e">Event</param>
		protected override void OnGotFocus(RoutedEventArgs e)
		{
			if (WatermarkContent != null && string.IsNullOrEmpty(this.Text))
			{
				this.WatermarkContent.Visibility = Visibility.Collapsed;
			}
			base.OnGotFocus(e);
		}

        /// <summary>OnLostFocus</summary>
        /// <param name="e">Event</param>
		protected override void OnLostFocus(RoutedEventArgs e)
		{
			if (WatermarkContent != null && string.IsNullOrEmpty(this.Text))
			{
				this.WatermarkContent.Visibility = Visibility.Visible;
			}
			base.OnLostFocus(e);
		}
        
        /// <summary>OnWatermarkPropertyChanged</summary>
        /// <param name="sender">Object</param>
        /// <param name="args">Arguments</param>
		private static void OnWatermarkPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			WatermarkTextBox watermarkTextBox = sender as WatermarkTextBox;
			if(watermarkTextBox != null && watermarkTextBox.WatermarkContent !=null)
			{
			  watermarkTextBox.DetermineWatermarkContentVisibility();
			}
		}

        #endregion
    }
}
