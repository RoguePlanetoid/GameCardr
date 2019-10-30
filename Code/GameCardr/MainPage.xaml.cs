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
using Microsoft.Phone.Controls;
using System.Text.RegularExpressions;
using Microsoft.Phone.Net.NetworkInformation;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.IO.IsolatedStorage;
using Microsoft.Devices;

namespace GameCardr
{
    /// <summary>MainPage</summary>
    public partial class MainPage : PhoneApplicationPage
    {
        #region Private Constants
        private const int ZERO = 0;
        private const string BLANK = "";
        private const string CAPTION = "GameCardr";
        private const string MSG_CLEAR = "Delete all GamerCards from list?";
        private const string MSG_LABEL = "Enter Gamer Tag";
        private const string MSG_VALID = "Enter Valid Gamer Tag";
        private const string MSG_CONNECTION = "Connection unavailable";
        private const string WEBSITE = "http://www.gamecardr.com";
        private const string CONTACT_EMAIL = "GameCardr <contact@gamecardr.com>";
        private const string CONTACT_SUBJECT = "Contact GameCardr (2.0.0)";
        private const string ABOUT = "GameCardr v2.0.0 by Comentsys";
        // Elements
        private const string SCROLL_VIEWER = "ScrollViewer";
        // Validation
        private readonly Regex VALIDATION = new Regex("^[a-zA-Z0-9_\\s]*$");
        private const int MIN_LENGTH = 0;
        private const int MAX_LENGTH = 16;
        // State Information
        private const string TEXTBOX_NAME = "GamerTag";
        private const string TEXTBOX_FOCUS = "textbox-focus";
        private const string TEXTBOX_TEXT = "textbox-text";
        private const string CARD_SELECTED = "card-selected";
        private const string PIVOT_SELECTED = "pivot-selected";
        // Scroll States
        private const string CARD_SCROLL = "card-scroll";
        private const string RECENT_SCROLL = "recent-scroll";
        private const string FRIEND_SCROLL = "friend-scroll";
        // URLS
        private const string URL_PAGE = "http://marketplace.xbox.com/Title/{0}";
        // Haptic Time
        private const int HAPTIC_TIME = 100;
        #endregion

        #region Private Members
        // State Information
        private bool hasPivotSelected = false;
        private bool hasLayoutUpdated = true;
        #endregion

        #region Constructor
        /// <summary>Constructor</summary>
        public MainPage()
        {
            InitializeComponent();
            Cards.ItemsSource = App.Framework.GamerCards;
            this.DataContext = App.Framework.GamerCard;
            App.Framework.Open();
            Framework.Completed += (object sender, EventArgs e) =>
            {
                Progress.IsIndeterminate = false;
                if (Cards.Items.Count > ZERO)
                {
                    Cards.SelectedIndex = Cards.Items.Count - 1;
                    GamerTag.Text = BLANK;
                    GamerTag.ToggleWatermark();
                }
            };
            Framework.Failed += (object sender, EventArgs e) =>
            {
                Progress.IsIndeterminate = false;
                MessageBox.Show(App.Framework.Message);
            };
            
        }
        #endregion

        #region Private Methods

        /// <summary>Haptic</summary>
        private void Haptic()
        {
            VibrateController vibrate = VibrateController.Default;
            vibrate.Start(TimeSpan.FromMilliseconds(HAPTIC_TIME));
        }

        /// <summary>GoSearch</summary>
        /// <param name="query">Search Query</param>
        private void GoSearch(string query)
        {
            try
            {
                SearchTask task = new SearchTask();
                task.SearchQuery = query;
                task.Show();
            }
            catch
            {
                // Do Nothing on Exception
            }
        }

        /// <summary>GoEmail</summary>
        private void GoEmail(string to, string subject)
        {
            try
            {
                EmailComposeTask task = new EmailComposeTask();
                task.To = to;
                task.Subject = subject;
                task.Body = BLANK;
                task.Show();
            }
            catch
            {
                // Do Nothing on Exception
            }
        }

        /// <summary>GoTo</summary>
        /// <param name="url">Website Address</param>
        private void GoTo(string url)
        {
            try
            {
                WebBrowserTask task = new WebBrowserTask();
                task.URL = HttpUtility.UrlEncode(url);
                task.Show();
            }
            catch
            {
                // Do Nothing on Exception
            }
        }

        /// <summary>GoToPage</summary>
        /// <param name="id">ID</param>
        private void GoToPage(string id)
        {
            GoTo(string.Format(URL_PAGE, id));
        }

        /// <summary>GoMarketplace</summary>
        /// <param name="search">Search Term</param>
        private void GoMarketplace(string search)
        {
            try
            {
                if (search != BLANK)
                {
                    MarketplaceSearchTask task = new MarketplaceSearchTask();
                    task.ContentType = MarketplaceContentType.Applications;
                    task.SearchTerms = search;
                    task.Show();
                }
            }
            catch
            {
                // Do Nothing on Exception
            }
        }

        /// <summary>Has State Item</summary>
        /// <param name="key">State Key</param>
        /// <returns>True if Present, False if Not</returns>
        private bool hasStateItem(string key)
        {
            try
            {
                return PhoneApplicationService.Current.State.ContainsKey(key);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>RemoveStateItem</summary>
        /// <param name="key">State Key</param>
        private void removeStateItem(string key)
        {
            try
            {
                PhoneApplicationService.Current.State.Remove(key);
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>Add State Item</summary>
        /// <param name="key">Name</param>
        /// <param name="value">Object</param>
        private void addStateItem(string key, object value)
        {
            try
            {
                if (hasStateItem(key))
                {
                    removeStateItem(key);
                }
                PhoneApplicationService.Current.State.Add(key, value);
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>Get State Item</summary>
        /// <param name="key">State Name</param>
        private object getStateItem(string key)
        {
            try
            {
                return PhoneApplicationService.Current.State[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Has Setting Item</summary>
        /// <param name="key">Setting Name</param>
        /// <returns>True if Present, False if Not</returns>
        private bool hasSettingItem(string key)
        {
            return IsolatedStorageSettings.ApplicationSettings.Contains(key);
        }

        /// <summary>Remove Setting Item</summary>
        /// <param name="key">Setting Name</param>
        private void removeSettingItem(string key)
        {
            try
            {
                IsolatedStorageSettings.ApplicationSettings.Remove(key);
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>Set Setting Item</summary>
        /// <param name="key">Setting Name</param>
        /// <param name="value">Setting Value</param>
        private void setSettingItem(string key, object value)
        {
            try
            {
                IsolatedStorageSettings.ApplicationSettings[key] = value;
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>Add Setting Item</summary>
        /// <param name="key">Name</param>
        /// <param name="value">Object</param>
        private void addSettingItem(string key, object value)
        {
            try
            {
                if (hasSettingItem(key))
                {
                    setSettingItem(key, value);
                }
                else
                {
                    IsolatedStorageSettings.ApplicationSettings.Add(key, value);
                }
                IsolatedStorageSettings.ApplicationSettings.Save();
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>Get Setting Item</summary>
        /// <param name="key">Setting Name</param>
        private object getSettingItem(string key)
        {
            try
            {
                return IsolatedStorageSettings.ApplicationSettings[key];
            }
            catch
            {
                return null;
            }
        }

        /// <summary>Select</summary>
        private void Select()
        {
            try
            {
                if (Cards.Items.Count > ZERO)
                {
                    Cards.SelectedIndex = ZERO;
                }
                else
                {
                    App.Framework.Reset();
                    this.DataContext = App.Framework.GamerCard;
                }
            }
            catch
            {
                // Do Nothing
            }
        }

        /// <summary>SaveScrollState</summary>
        /// <param name="listbox">ListBox</param>
        /// <param name="name">State Name</param>
        private void SaveScrollState(ListBox listbox, string name)
        {
            ScrollViewer viewer = ((VisualTreeHelper.GetChild(listbox, ZERO) as FrameworkElement).FindName(SCROLL_VIEWER) as ScrollViewer);
            addStateItem(name, viewer.VerticalOffset);
        }

        /// <summary>LoadScrollState</summary>
        /// <param name="listbox">ListBox</param>
        /// <param name="name">State Name</param>
        private void LoadScrollState(ListBox listbox, string name)
        {
            if (hasStateItem(name))
            {
                listbox.Loaded += delegate
                {
                    try
                    {
                        ScrollViewer viewer = ((VisualTreeHelper.GetChild(listbox, ZERO) as FrameworkElement).FindName(SCROLL_VIEWER) as ScrollViewer);
                        viewer.ScrollToVerticalOffset((double)getStateItem(name));
                    }
                    catch
                    {
                        // Do Nothing on Exception
                    }
                };
            }
        }
        #endregion

        #region Public Methods

        /// <summary>SaveState</summary>
        public void SaveState()
        {
            try
            {
                addStateItem(TEXTBOX_TEXT, GamerTag.Text);
                if (Cards.SelectedItem != null)
                {
                    addSettingItem(CARD_SELECTED, Cards.SelectedIndex);
                }
                addStateItem(PIVOT_SELECTED, Pivot.SelectedIndex);
                // Lists
                SaveScrollState(Cards, CARD_SCROLL);
                SaveScrollState(Recent, RECENT_SCROLL);
            }
            catch
            {
                // Just state information, don't care if it doesn't save
            }
        }

        /// <summary>LoadState</summary>
        public void LoadState()
        {
            try
            {
                if (hasStateItem(TEXTBOX_TEXT)) { GamerTag.Text = (string)getStateItem(TEXTBOX_TEXT); }
                if (hasSettingItem(CARD_SELECTED)) { Cards.SelectedIndex = (int)getSettingItem(CARD_SELECTED); }
                if (hasStateItem(PIVOT_SELECTED)) { hasPivotSelected = true; }
                // Lists
                LoadScrollState(Cards, CARD_SCROLL);
                LoadScrollState(Recent, RECENT_SCROLL);
            }
            catch
            {
                // Just state information, don't care if it doesn't load
            }
            Cards.ItemsSource = App.Framework.GamerCards;
            this.DataContext = App.Framework.GamerCard;
        }

        /// <summary>IsValid</summary>
        /// <param name="source">String</param>
        /// <returns>True if Valid, False if Not</returns>
        public bool IsValid(string source)
        {
            return source.Length > MIN_LENGTH 
                && source.Length <= MAX_LENGTH 
                && VALIDATION.IsMatch(source);
        }

        /// <summary>Add Tag</summary>
        public void Add()
        {
            if (IsValid(GamerTag.Text))
            {
                if (NetworkInterface.GetIsNetworkAvailable())
                {
                    Progress.IsIndeterminate = true;
                    App.Framework.Add(GamerTag.Text);
                    this.Focus();
                }
                else
                {
                    MessageBox.Show(MSG_CONNECTION);
                }
            }
            else
            {
                MessageBox.Show(MSG_VALID);
            }
        }

        /// <summary>Add</summary>
        /// <param name="ZuneTag">ZuneTag</param>
        public void Add(string Tag)
        {
            GamerTag.Text = Tag;
            Pivot.SelectedIndex = ZERO;
            Add();
        }

        /// <summary>Remove</summary>
        public void Remove()
        {
            if (Cards.SelectedItem != null)
            {
                App.Framework.Remove((GamerCard)Cards.SelectedItem);
                Select();
            }
        }

        /// <summary>Refresh</summary>
        public void Refresh()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (Cards.SelectedItem != null)
                {
                    App.Framework.Refresh((GamerCard)Cards.SelectedItem);
                }
                else
                {
                    foreach (GamerCard item in Cards.Items)
                    {
                        App.Framework.Refresh((GamerCard)item);
                    }
                }
            }
            else
            {
                MessageBox.Show(MSG_CONNECTION);
            }
        }

        /// <summary>Delete</summary>
        public void Delete()
        {
            if (MessageBox.Show(MSG_CLEAR, CAPTION, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                App.Framework.Clear();
                Select();
            }
        }
        #endregion

        #region Event Handlers

        /// <summary>GamerTag KeyUp</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void GamerTag_KeyUp(object sender, KeyEventArgs e) { if (e.Key == Key.Enter) Add(); }

        /// <summary>Website</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Website_Click(object sender, EventArgs e) { GoTo(WEBSITE); }

        /// <summary>Contact</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Contact_Click(object sender, EventArgs e) { GoEmail(CONTACT_EMAIL, CONTACT_SUBJECT); }

        /// <summary>About</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void About_Click(object sender, EventArgs e) { MessageBox.Show(ABOUT, CAPTION, MessageBoxButton.OK); }

        /// <summary>Add Tag</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Add_Click(object sender, EventArgs e)
        {
            Add();
        }

        /// <summary>Remove Tag</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Remove_Click(object sender, EventArgs e)
        {
            Remove(); 
        }

        /// <summary>Refresh</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Refresh_Click(object sender, EventArgs e)
        {
            Refresh();
        }

        /// <summary>Delete Tags</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Delete_Click(object sender, EventArgs e)
        {
            Delete();
        }

        /// <summary>On Navigated From</summary>
        /// <param name="e">Event</param>
        /// <remarks>Save State</remarks>
        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            SaveState();
        }

        /// <summary>On Navigated To</summary>
        /// <param name="e">Event</param>
        /// <remarks>Load State on on App.IsActivated</remarks>
        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            LoadState();
        }

        /// <summary>Cards Changed</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Cards_Changed(object sender, SelectionChangedEventArgs e)
        {
            if (Cards.SelectedItem != null)
            {
                App.Framework.GamerCard = (GamerCard)Cards.SelectedItem;
                this.DataContext = App.Framework.GamerCard;
            }
        }

        /// <summary>Pivot_SelectionChanged</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationBar.IsVisible = Pivot.SelectedIndex == ZERO;
        }

        /// <summary>Layout Updated</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        /// <remarks>Triggered once whole Page has Displayed, use for Focus</remarks>
        private void PhoneApplicationPage_LayoutUpdated(object sender, EventArgs e)
        {
            if (hasLayoutUpdated)
            {
                hasLayoutUpdated = false;
                if (hasPivotSelected)
                {
                    Pivot.SelectedIndex = (int)getStateItem(PIVOT_SELECTED);
                }
            } 
        }

        /// <summary>Game Menu</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Game_Menu(object sender, RoutedEventArgs e)
        {
            GoToPage((String)((MenuItem)sender).Tag);
        }

        /// <summary>Search Menu</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Search_Menu(object sender, RoutedEventArgs e)
        {
            GoSearch((String)((MenuItem)sender).Tag);
        }

        /// <summary>Marketplace Menu</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Marketplace_Menu(object sender, RoutedEventArgs e)
        {
            GoMarketplace((String)((MenuItem)sender).Tag);
        }

        /// <summary>Profile Social</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Profile_Page(object sender, RoutedEventArgs e)
        {
            GoTo(App.Framework.GamerCard.Link);
        }

        /// <summary>Add Friend</summary>
        /// <param name="sender">Object</param>
        /// <param name="e">Event</param>
        private void Add_Friend(object sender, RoutedEventArgs e)
        {
            Haptic(); // Vibrate
            Add((String)((Button)sender).Tag);
        }
        #endregion
  
    }
}