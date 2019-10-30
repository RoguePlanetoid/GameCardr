using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GameCardr
{
    public partial class StarControl : UserControl
    {

#region Private Members
        private DisplayValue _display = DisplayValue.None;
#endregion

#region Public Enums
        public enum DisplayValue
        {
            None = 0,
            Full = 1,
            Half = 2,
            Quarter = 3,
            ThreeQuarter = 4
        }
#endregion

#region Constructor
        /// <summary>Constructor</summary>
        public StarControl()
        {
            InitializeComponent();
        }
#endregion

#region Public Properties

        /// <summary>Display</summary>
        public DisplayValue Display
        {
            get { return _display; }
            set
            {
                _display = value;
                switch (_display)
                {
                    case DisplayValue.Full:
                        FullStar.Visibility = Visibility.Visible;
                        HalfStar.Visibility = Visibility.Collapsed;
                        QuarterStar.Visibility = Visibility.Collapsed;
                        ThreeQuarterStar.Visibility = Visibility.Collapsed;
                        break;
                    case DisplayValue.Half:
                        FullStar.Visibility = Visibility.Collapsed;
                        HalfStar.Visibility = Visibility.Visible;
                        QuarterStar.Visibility = Visibility.Collapsed;
                        ThreeQuarterStar.Visibility = Visibility.Collapsed;
                        break;
                    case DisplayValue.Quarter:
                        FullStar.Visibility = Visibility.Collapsed;
                        HalfStar.Visibility = Visibility.Collapsed;
                        QuarterStar.Visibility = Visibility.Visible;
                        ThreeQuarterStar.Visibility = Visibility.Collapsed;
                        break;
                    case DisplayValue.ThreeQuarter:
                        FullStar.Visibility = Visibility.Collapsed;
                        HalfStar.Visibility = Visibility.Collapsed;
                        QuarterStar.Visibility = Visibility.Collapsed;
                        ThreeQuarterStar.Visibility = Visibility.Visible;
                        break;
                    default:
                        FullStar.Visibility = Visibility.Collapsed;
                        HalfStar.Visibility = Visibility.Collapsed;
                        QuarterStar.Visibility = Visibility.Collapsed;
                        ThreeQuarterStar.Visibility = Visibility.Collapsed;
                        break;
                }
            }

        }

#endregion
    }
}
