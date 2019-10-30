using System;
using System.Net;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;

namespace GameCardr
{
    /// <summary>GamerCard</summary>
    public class GamerCard : INotifyPropertyChanged
    {
        #region Private Constants
        private const string BLANK = "";
        // General
        private const string FORMAT_NUMBER = "#,###";
        private const string FORMAT_REP = "Rep {0}%";
        private const string FORMAT_DATETIME = "d MMMM yyyy, h:mm tt";
        private const string FORMAT_UPDATED = "Updated {0}";
        private const string FORMAT_NAME_LOCATION = "{0}, {1}";
        // Properties
        private const string PROP_TAG = "Tag";
        private const string PROP_LINK = "Link";
        private const string PROP_STARS = "Stars";
        private const string PROP_REPUTATION = "Reputation";
        private const string PROP_AVATAR = "Avatar";
        private const string PROP_PICTURE = "Picture";
        private const string PROP_SCORE = "Score";
        private const string PROP_GAMERSCORE = "GamerScore";
        private const string PROP_ACCOUNT = "Account";
        private const string PROP_GRADIENT = "Gradient";
        private const string PROP_LOCATION = "Location";
        private const string PROP_MOTTO = "Motto";
        private const string PROP_NAME = "Name";
        private const string PROP_BIO = "Bio";
        private const string PROP_GAMES = "Games";
        private const string PROP_FRIENDS = "Friends";
        private const string PROP_UPDATED = "Updated";
        private const string PROP_WHENUPDATED = "WhenUpdated";
        #endregion

        #region Private Members
        private string tag;
        private string link;
        private string location = BLANK;
        private string motto = BLANK;
        private string name = BLANK;
        private string bio = BLANK;
        private Picture avatar;
        private Picture picture;
        private int score;
        private AccountType account;
        private List<Game> games = new List<Game>();
        private DateTime updated = DateTime.Now;
        private List<StarControl> stars = new List<StarControl>();
        private List<String> friends = new List<String>();
        #endregion

        #region Public Enums
        public enum AccountType { Free = 0, Gold = 1 }
        #endregion

        #region Public Members
        public SolidColorBrush Gold = new SolidColorBrush(Color.FromArgb(255, 250, 220, 90));
        public SolidColorBrush Free = new SolidColorBrush(Color.FromArgb(255, 180, 180, 180)); 
        #endregion

        #region Event Handler
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>NotifyPropertyChanged</summary>
        /// <param name="info">Property Name</param>
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        #endregion

        #region Public Methods

        /// <summary>GetNameLocation</summary>
        /// <param name="name">Name</param>
        /// <param name="location">Location</param>
        /// <returns>Name and/or Location Text</returns>
        private string GetNameLocation()
        {

            if (Name != BLANK && Location != BLANK)
            {
                return string.Format(FORMAT_NAME_LOCATION, Name, Location);
            }
            else if (Name != BLANK)
            {
                return Name;
            }
            else if (Location != BLANK)
            {
                return Location;
            }
            else
            {
                return BLANK;
            }
        }
        #endregion

        #region Public Properties
        /// <summary>Gamer Tag</summary>
        /// <example>RoguePlanetoid</example>
        public string Tag
        {
            get { return tag; }
            set
            {
                tag = value;
                NotifyPropertyChanged(PROP_TAG);
            }
        }

        /// <summary>Link</summary>
        /// <example>/en-GB/MyXbox/Profile?gamertag=RoguePlanetoid</example>
        public string Link
        {
            get { return link; }
            set
            {
                link = value;
                NotifyPropertyChanged(PROP_LINK);
            }
        }

        /// <summary>Avatar</summary>
        /// <example>http://avatar.xboxlive.com/avatar/RoguePlanetoid/avatar-body.png</example>
        public Picture Avatar
        {
            get { return avatar; }
            set
            {
                avatar = value;
                NotifyPropertyChanged(PROP_AVATAR);
            }
        }

        /// <summary>Gamer Picture / Tile</summary>
        /// <example>http://image.xboxlive.com/global/t.475207f5/tile/0/2804a</example>
        public Picture Picture
        {
            get { return picture; }
            set
            {
                picture = value;
                NotifyPropertyChanged(PROP_PICTURE);
            }
        }

        /// <summary>Stars</summary>
        public List<StarControl> Stars
        {
            get { return stars; }
            set
            {
                stars = value;
                NotifyPropertyChanged(PROP_STARS);
            }
        }

        /// <summary>Gamer Score</summary>
        /// <example>1200</example>
        public int Score
        {
            get { return score; }
            set
            {
                score = value;
                NotifyPropertyChanged(PROP_SCORE);
                NotifyPropertyChanged(PROP_GAMERSCORE);
            }
        }
        /// <summary>GamerScore</summary>
        public string GamerScore
        {
            get { return score.ToString(FORMAT_NUMBER); }
        }
        /// <summary>Account</summary>
        /// <example>Gold / Free</example>
        public AccountType Account
        {
            get { return account; }
            set
            {
                account = value;
                NotifyPropertyChanged(PROP_ACCOUNT);
                NotifyPropertyChanged(PROP_GRADIENT);
            }
        }

        /// <summary>Brush</summary>
        public SolidColorBrush Brush
        {
            get { return Account == AccountType.Gold ? Gold : Free; }
        }

        /// <summary>Location</summary>
        public string Location
        {
            get { return location; }
            set
            {
                location = value;
                NotifyPropertyChanged(PROP_LOCATION);
            }
        }

        /// <summary>Motto</summary>
        public string Motto
        {
            get { return motto; }
            set
            {
                motto = value;
                NotifyPropertyChanged(PROP_MOTTO);
            }
        }

        /// <summary>Name</summary>
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged(PROP_NAME);
            }
        }

        /// <summary>Bio</summary>
        public string Bio
        {
            get { return bio; }
            set
            {
               bio = value;
                NotifyPropertyChanged(PROP_BIO);
            }
        }

        /// <summary>Member Name / Location</summary>
        public string NameLocation { get { return GetNameLocation(); } }

        /// <summary>Played Games</summary>
        public List<Game> Games
        {
            get { return games; }
            set
            {
                games = value;
                NotifyPropertyChanged(PROP_GAMES);
            }
        }

        /// <summary>Friends List</summary>
        public List<String> Friends
        {
            get { return friends; }
            set
            {
                friends = value;
                NotifyPropertyChanged(PROP_FRIENDS);
            }
        }

        /// <summary>Updated</summary>
        public DateTime Updated
        {
            get { return updated; }
            set
            {
                updated = value;
                NotifyPropertyChanged(PROP_UPDATED);
                NotifyPropertyChanged(PROP_WHENUPDATED);
            }
        }
        /// <summary>When Updated</summary>
        public string WhenUpdated
        {
            get { return String.Format(FORMAT_UPDATED, Updated.ToString(FORMAT_DATETIME)); }
        }
        #endregion
    }
}