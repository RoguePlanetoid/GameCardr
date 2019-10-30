using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.IO.IsolatedStorage;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.ComponentModel;

namespace GameCardr
{
    /// <summary>Main Functionality</summary>
    public class Framework : INotifyPropertyChanged
    {
        #region Private Constants
        private const int ZERO = 0;
        private const string SPACE = " ";
        // File
        private const string PATTERN_ALL = "*.*";
        // XML
        private const string XML_FILE = "data.xml";
        private const string XML_VERSION = "1.0";
        private const string XML_ENCODING = "utf-8";
        private const string XML_STANDALONE = "yes";
        private const string XML_ROOT = "gamercardr";
        private const string XML_CARD = "card";
        private const string XML_TAG = "tag";
        private const string XML_ZONE = "zone";
        private const string XML_AVATAR = "avatar";
        private const string XML_PICTURE = "tile";
        private const string XML_REP = "rep";
        private const string XML_SCORE = "score";
        private const string XML_LOCATION = "location";
        private const string XML_MOTTO = "motto";
        private const string XML_NAME = "name";
        private const string XML_BIO = "bio";
        private const string XML_ACCOUNT = "account";
        private const string XML_GAME = "game";
        private const string XML_UPDATED = "updated";
        private const string XML_ID = "id";
        private const string XML_TITLE = "title";
        private const string XML_IMAGE = "src";
        private const string XML_LINK = "link";
        private const string XML_COVER = "cover";
        private const string XML_STAR = "star";
        private const string XML_LAST_PLAYED = "lastplayed";
        private const string XML_EARNED_GAMERSCORE = "earnedgamerscore";
        private const string XML_AVAILABLE_GAMERSCORE = "availablegamerscore";
        private const string XML_EARNED_ACHIEVEMENTS = "earnedachievements";
        private const string XML_AVAILABLE_ACHIEVEMENTS = "availableachievements";
        private const string XML_PERCENTAGE_COMPLETE = "percentagecomplete";
        private const string XML_FRIENDS = "friends";
        private const string XML_FRIEND = "friend";
        // Values
        private const string VALUE_SPACE = "&nbsp;";
        private const string VALUE_GAMERCARD = "XbcGamercard";
        private const string VALUE_GAMERTAG = "Gamertag";
        private const string VALUE_GAMERPIC = "Gamerpic";
        private const string VALUE_GAMERSCORE = "Gamerscore";
        private const string VALUE_LOCATION = "Location";
        private const string VALUE_MOTTO = "Motto";
        private const string VALUE_NAME = "Name";
        private const string VALUE_BIO = "Bio";
        private const string VALUE_REPUTATION = "RepContainer";
        private const string VALUE_PLAYED_GAMES = "PlayedGames";
        private const string VALUE_TITLE = "Title";
        private const string VALUE_LAST_PLAYED = "LastPlayed";
        private const string VALUE_EARNED_GAMERSCORE = "EarnedGamerscore";
        private const string VALUE_AVAILABLE_GAMERSCORE = "AvailableGamerscore";
        private const string VALUE_EARNED_ACHIEVEMENTS = "EarnedAchievements";
        private const string VALUE_AVAILABLE_ACHIEVEMENTS = "AvailableAchievements";
        private const string VALUE_PERCENTAGE_COMPLETE = "PercentageComplete";
        private const string VALUE_GOLD = "Gold";
        private const string VALUE_BODY = "Body";
        private const string VALUE_STAR = "Star";
        private const string VALUE_STAR_FULL = "Star Full";
        private const string VALUE_STAR_HALF = "Star Half";
        private const string VALUE_STAR_QUARTER = "Star Quarter";
        private const string VALUE_STAR_THREE_QUARTER = "Star ThreeQuarter";
        private const string VALUE_LARGE = "large";
        private const string VALUE_SMALL = "small";
        private const string VALUE_COVER_PREFIX = "title=";
        private const string VALUE_COVER_SUFFIX = "&";
        // HTML
        private const string TAG_BODY_OPEN = "<body>";
        private const string TAG_BODY_CLOSE = "</body>";
        private const string HTML_DIV = "div";
        private const string HTML_SPAN = "span";
        private const string HTML_IMG = "img";
        private const string HTML_ANCHOR = "a";
        private const string HTML_ITEM = "li";
        private const string ATTR_CLASS = "class";
        private const string ATTR_HREF = "href";
        private const string ATTR_SRC = "src";
        private const string ATTR_TITLE = "title";
        private const string ATTR_ID = "id";
        // Elements
        private const string PARSE_BODY = @"\<body\>(.*?)\</body\>";
        private const string URL_AVATAR = "http://avatar.xboxlive.com/avatar/{0}/avatar-body.png";
        private const string URL_SERVICE = "http://gamercard.xbox.com/en-gb/{0}.card";
        private const string URL_COVER = "http://tiles.xbox.com/consoleAssets/{0}/en-GB/{1}boxart.jpg";
        // GamerTag
        private const int TAG_LENGTH = 16;
        private const string TAG_VALID = "^[a-zA-Z0-9_\\s]$";
        // Errors
        private const string ERR_DOWNLOAD = "There was a problem downloading GamerCard for {0}";
        private const string ERR_PARSE = "There was a problem reading GamerCard for {0}";
        private const string ERR_VALID = "GamerTag contains disallow characters or is too long";
        private const string ERR_LOAD = "Unable to open Gamer Cards";
        private const string ERR_SAVE = "Unable to save Gamer Cards";
        // Properties
        private const string PROP_GAMERCARD = "GamerCard";
        private const string PROP_GAMERCARDS = "GamerCards";  
        #endregion

        #region Static Members
        private IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication();
        #endregion

        #region Private Members
        private GamerCard gamerCard = new GamerCard();
        private ObservableCollection<GamerCard> gamerCards = new ObservableCollection<GamerCard>();
        private XElement xml;
        private string message;
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

        #region Private Methods
        /// <summary>FromBytes</summary>
        /// <param name="filename">Filename</param>
        /// <param name="bytes">Byte Array</param>
        private void FromBytes(string filename, byte[] bytes)
        {
            using (IsolatedStorageFileStream location = new IsolatedStorageFileStream(filename, FileMode.Create, storage))
            {
                location.Write(bytes, 0, bytes.Length);
                location.Dispose();
                location.Close();
            }
        }
        /// <summary>ToBytes</summary>
        /// <param name="filename">Filename</param>
        /// <returns>Byte Array</returns>
        private byte[] ToBytes(string filename)
        {
            byte[] bytes = null;
            if (storage.FileExists(filename))
            {
                using (IsolatedStorageFileStream location = new IsolatedStorageFileStream(filename, FileMode.Open, storage))
                {
                    bytes = new byte[location.Length];
                    location.Read(bytes, 0, (Int32)location.Length);
                    location.Dispose();
                    location.Close();
                }
            }
            return bytes;
        }
        /// <summary>FromFile</summary>
        /// <param name="filename">Filename</param>
        /// <returns>BitmapImage from File</returns>
        public BitmapImage FromFile(string filename)
        {
            try
            {
                BitmapImage bitmap = new BitmapImage();
                using (MemoryStream stream = new MemoryStream(ToBytes(filename)))
                {
                    bitmap.SetSource(stream);
                }
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>ToFile</summary>
        /// <param name="address">URL</param>
        /// <param name="filename">Filename</param>
        public void ToFile(Uri address, string filename)
        {
            WebClient client = new WebClient();
            client.OpenReadCompleted += (object sender, OpenReadCompletedEventArgs e) =>
            {
                try
                {
                    if (!e.Cancelled && e.Error == null)
                    {
                        Byte[] bytes = null;
                        BitmapImage bitmap = new BitmapImage();
                        using (Stream stream = e.Result)
                        {
                            if (stream != null & stream.Length > 0)
                            {
                                stream.Position = 0;
                                using (BinaryReader reader = new BinaryReader(stream))
                                {
                                    bytes = reader.ReadBytes((Int32)stream.Length);
                                }
                            }
                        }
                        using (MemoryStream stream = new MemoryStream(bytes))
                        {
                            bitmap.SetSource(stream);
                        }
                        FromBytes(filename, bytes);
                    }
                }
                catch
                {
                    Message = ERR_SAVE;
                    Failed(this, EventArgs.Empty);
                }
            };
            client.OpenReadAsync(address);          
        }

        /// <summary>GetGamerItem</summary>
        /// <param name="element">Element</param>
        /// <param name="name">Item Name</param>
        /// <returns>Item Value</returns>
        private string GetGamerItem(XElement element, string name)
        {
            return (from item in element.Descendants()
                where (string)item.Attribute(ATTR_ID) == name
                select item).Single().Value;
        }

        /// <summary>GetGameItem</summary>
        /// <param name="element">XElement</param>
        /// <param name="name">Item Name</param>
        /// <returns>Item Value</returns>
        private string GetGameItem(XElement element, string name)
        {
            return (from item in element.Element(HTML_ANCHOR).Descendants(HTML_SPAN)
             where (string)item.Attribute(ATTR_CLASS) == name
             select item).Single().Value;
        }

        /// <summary>DecToHex</summary>
        /// <param name="value">Integer</param>
        /// <returns>HexValue</returns>
        private string DecToHex(string value)
        {
            return Convert.ToString(int.Parse(value), 16);
        }

        /// <summary>GetID</summary>
        /// <param name="link">URL</param>
        /// <returns>Get ID from Link</returns>
        private string GetID(string link)
        {
           int start = link.IndexOf(VALUE_COVER_PREFIX) + VALUE_COVER_PREFIX.Length;
           return link.Substring(start, link.IndexOf(VALUE_COVER_SUFFIX) - start);
        }

        /// <summary>GetCover</summary>
        /// <param name="id">ID Number for Image</param>
        /// <param name="isLarge">Large or Small</param>
        /// <returns>Cover URI</returns>
        private Uri GetCover(string id, bool isLarge)
        {
            return new Uri(string.Format(URL_COVER,DecToHex(id), isLarge ? VALUE_LARGE : VALUE_SMALL));  
        }

        /// <summary>PictureFromUri</summary>
        /// <param name="uri">Uri</param>
        /// <returns>Picture</returns>
        private Picture PictureFromUri(Uri uri)
        {
            Picture picture = new Picture();
            picture.Image = new BitmapImage(uri);
            picture.Filename = Guid.NewGuid().ToString();
            ToFile(uri, picture.Filename);
            return picture;
        }

        /// <summary>PictureFromFile</summary>
        /// <param name="filename">Filename</param>
        /// <returns>Picture</returns>
        private Picture PictureFromFile(string filename)
        {
            Picture picture = new Picture();
            picture.Filename = filename;
            picture.Image = FromFile(filename);
            return picture;
        }

        /// <summary>ToXML</summary>
        /// <returns>XML Object</returns>
        private XElement ToXML()
        {
            XElement root = new XElement(XML_ROOT);
            foreach (GamerCard item in GamerCards)
            {
                XElement node = new XElement(XML_CARD);
                node.Add(new XElement(XML_TAG, item.Tag));
                node.Add(new XElement(XML_LINK, item.Link));
                node.Add(new XElement(XML_AVATAR, item.Avatar.Filename));
                node.Add(new XElement(XML_PICTURE, item.Picture.Filename));
                // New Fields
                node.Add(new XElement(XML_LOCATION, item.Location));
                node.Add(new XElement(XML_MOTTO, item.Motto));
                node.Add(new XElement(XML_NAME, item.Name));
                node.Add(new XElement(XML_BIO, item.Bio));
                foreach (StarControl star in item.Stars)
                {
                    XElement subnode = new XElement(XML_REP);
                    subnode.Add(new XElement(XML_STAR,(int)(star.Display)));
                    node.Add(subnode);
                }
                node.Add(new XElement(XML_SCORE, item.Score));
                node.Add(new XElement(XML_ACCOUNT, (int)(item.Account)));
                node.Add(new XElement(XML_UPDATED, item.Updated));
                foreach (Game game in item.Games)
                {
                    XElement subnode = new XElement(XML_GAME);
                    subnode.Add(new XAttribute(XML_LINK,game.Link));
                    subnode.Add(new XAttribute(XML_TITLE, game.Title));
                    subnode.Add(new XAttribute(XML_IMAGE, game.Image.Filename));
                    // New Fields
                    subnode.Add(new XAttribute(XML_ID, game.ID));
                    subnode.Add(new XAttribute(XML_COVER, game.Cover.Filename));
                    subnode.Add(new XAttribute(XML_LAST_PLAYED, game.LastPlayed));
                    subnode.Add(new XAttribute(XML_EARNED_GAMERSCORE, game.EarnedGamerscore));
                    subnode.Add(new XAttribute(XML_AVAILABLE_GAMERSCORE, game.AvailableGamerscore));
                    subnode.Add(new XAttribute(XML_EARNED_ACHIEVEMENTS, game.EarnedAchievements));
                    subnode.Add(new XAttribute(XML_AVAILABLE_ACHIEVEMENTS, game.AvailableAchievements));
                    subnode.Add(new XAttribute(XML_PERCENTAGE_COMPLETE, game.PercentageComplete));
                    node.Add(subnode);
                }
                root.Add(node);
            }
            return root;
        }
        /// <summary>FromXML</summary>
        /// <param name="xml">XML Data</param>
        private void FromXml(XElement xml)
        {
            gamerCards.Clear();
            if (xml.Name.LocalName == XML_ROOT)
            {
                foreach (XElement node in xml.Descendants(XML_CARD))
                {
                    GamerCard item = new GamerCard();
                    item.Tag = node.Element(XML_TAG).Value;
                    item.Link = node.Element(XML_LINK).Value;
                    item.Avatar = PictureFromFile(node.Element(XML_AVATAR).Value);
                    item.Picture = PictureFromFile(node.Element(XML_PICTURE).Value);
                    // New Fields
                    item.Location = node.Element(XML_LOCATION).Value;
                    item.Motto = node.Element(XML_MOTTO).Value;
                    item.Name = node.Element(XML_NAME).Value;
                    item.Bio = node.Element(XML_BIO).Value;
                    foreach (XElement subnode in node.Elements(XML_REP))
                    {
                        StarControl star = new StarControl();
                        star.Display = (GameCardr.StarControl.DisplayValue)int.Parse(subnode.Element(XML_STAR).Value);
                        item.Stars.Add(star);
                    }
                    item.Score = int.Parse(node.Element(XML_SCORE).Value);
                    item.Account = (GamerCard.AccountType)int.Parse(node.Element(XML_ACCOUNT).Value);
                    item.Updated = DateTime.Parse(node.Element(XML_UPDATED).Value);
                    foreach (XElement subnode in node.Elements(XML_GAME))
                    {
                        Game game = new Game();
                        game.Link = subnode.Attribute(XML_LINK).Value;
                        game.Title = subnode.Attribute(XML_TITLE).Value;
                        game.Image = PictureFromFile(subnode.Attribute(XML_IMAGE).Value);
                        // New Fields
                        game.ID = subnode.Attribute(XML_ID).Value;
                        game.Cover = PictureFromFile(subnode.Attribute(XML_COVER).Value);
                        game.LastPlayed = subnode.Attribute(XML_LAST_PLAYED).Value;
                        game.EarnedGamerscore = subnode.Attribute(XML_EARNED_GAMERSCORE).Value;
                        game.AvailableGamerscore = subnode.Attribute(XML_AVAILABLE_GAMERSCORE).Value;
                        game.EarnedAchievements = subnode.Attribute(XML_EARNED_ACHIEVEMENTS).Value;
                        game.AvailableAchievements = subnode.Attribute(XML_AVAILABLE_ACHIEVEMENTS).Value;
                        game.PercentageComplete = subnode.Attribute(XML_PERCENTAGE_COMPLETE).Value;
                        item.Games.Add(game);
                    }
                    gamerCards.Add(item);
                }
            }
        }
        /// <summary>Validate</summary>
        /// <param name="tag">GamerTag</param>
        /// <returns>True if Valid, False if Not</returns>
        private bool Validate(string tag)
        {
            if (tag.Length > 0 && tag.Length <= TAG_LENGTH)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>Get</summary>
        /// <param name="tag">GamerTag</param>
        /// <returns>GamerCard</returns>
        private GamerCard Get(string tag)
        {
            return (from GamerCard item in GamerCards
                    where item.Tag == tag
                    select item).FirstOrDefault();
        }
        /// <summary>Exists</summary>
        /// <param name="tag">Tag</param>
        /// <returns>True if Exists, False if Not</returns>
        private bool Exists(string tag)
        {
            return (from GamerCard item in GamerCards
                    where item.Tag == tag
                    select item).Any();
        }

        /// <summary>GetBody</summary>
        /// <param name="source">HTML</param>
        /// <returns>HTML Body</returns>
        private string GetBody(string source)
        {
            return source.Substring(source.IndexOf(TAG_BODY_OPEN), 
                source.IndexOf(TAG_BODY_CLOSE) - source.IndexOf(TAG_BODY_OPEN) + 7);
        }

        /// <summary>Sanitise</summary>
        /// <param name="source">Source</param>
        /// <returns>Target</returns>
        private string Sanitise(string source)
        {
            return source.Replace(VALUE_SPACE, SPACE);
        }

        /// <summary>Parse</summary>
        /// <param name="source">Source HTML</param>
        /// <param name="tag">Gamer Tag</param>
        private void Parse(string source, string tag)
        {
            try
            {
                GamerCard gamer = new GamerCard();
                xml = XElement.Parse(Sanitise(GetBody(source))); // Parse HTML

                // Card
                XElement xmlCard = (from element in xml.Descendants() 
                                    where element.Name == HTML_DIV 
                                    select element).First();

                // Link
                gamer.Link = (from element in xmlCard.Descendants()
                              where (string)element.Attribute(ATTR_ID) == VALUE_GAMERTAG
                              select element).Single().Attribute(ATTR_HREF).Value;

                // Gamer Tag
                gamer.Tag = (from element in xmlCard.Descendants()
                             where (string)element.Attribute(ATTR_ID) ==VALUE_GAMERTAG
                             select element).Single().Value;

                // Avatar
                gamer.Avatar = PictureFromUri(new Uri(String.Format(URL_AVATAR, gamer.Tag)));

                // Account
                gamer.Account = xmlCard.FirstAttribute.Value.Contains(VALUE_GOLD)
                     ? GamerCard.AccountType.Gold : GamerCard.AccountType.Free;

                // Gamer Picture
                gamer.Picture = PictureFromUri(new Uri((from element in xmlCard.Descendants()
                              where (string)element.Attribute(ATTR_ID) == VALUE_GAMERPIC
                              select element).Single().Attribute(ATTR_SRC).Value));

                // Gamer Score
                try { gamer.Score = int.Parse(GetGamerItem(xmlCard, VALUE_GAMERSCORE)); }
                catch { gamer.Score = ZERO; }

                // Location
                gamer.Location = GetGamerItem(xmlCard, VALUE_LOCATION);

                // Motto
                gamer.Motto = GetGamerItem(xmlCard, VALUE_MOTTO);

                // Name
                gamer.Name = GetGamerItem(xmlCard, VALUE_NAME);

                // Bio
                gamer.Bio = GetGamerItem(xmlCard, VALUE_BIO);

                // Reputation Container
                XElement xmlRep = (from element in xmlCard.Descendants()
                                       where (string)element.Attribute(ATTR_CLASS) == VALUE_REPUTATION
                                       select element).Single();

                // Repututation
                IEnumerable<XElement> reputation = (from element in xmlRep.Descendants(HTML_DIV)
                                                    where element.Attribute(ATTR_CLASS).Value.StartsWith(VALUE_STAR)
                                                    select element);

                // Stars
                List<StarControl> stars = new List<StarControl>();
                foreach (XElement element in reputation)
                {
                    StarControl star = new StarControl();
                    switch (element.Attribute(ATTR_CLASS).Value)
                    {
                        case VALUE_STAR_FULL:
                            star.Display = StarControl.DisplayValue.Full;
                            stars.Add(star);
                            break;
                        case VALUE_STAR_HALF:
                            star.Display = StarControl.DisplayValue.Half;
                            stars.Add(star);
                            break;
                        case VALUE_STAR_QUARTER:
                            star.Display = StarControl.DisplayValue.Quarter;
                            stars.Add(star);
                            break;
                        case VALUE_STAR_THREE_QUARTER:
                            star.Display = StarControl.DisplayValue.ThreeQuarter;
                            stars.Add(star);
                            break;
                        default:
                            break;
                    };
                }
                gamer.Stars = stars;

                // Played Section
                XElement xmlPlayed = (from element in xmlCard.Descendants()
                                      where (string)element.Attribute(ATTR_ID) == VALUE_PLAYED_GAMES
                                      select element).Single();

                // Played Games
                List<Game> games = new List<Game>();
                foreach (XElement element in xmlPlayed.Descendants(HTML_ITEM))
                {
                    try
                    {
                        Game game = new Game();
                        game.Link = element.Element(HTML_ANCHOR).Attribute(ATTR_HREF).Value;
                        game.Image = PictureFromUri(new Uri(element.Element(HTML_ANCHOR).Element(HTML_IMG).Attribute(ATTR_SRC).Value));
                        game.ID = GetID(game.Link);
                        game.Cover = PictureFromUri(GetCover(game.ID, false));
                        game.Title = GetGameItem(element, VALUE_TITLE);
                        game.LastPlayed = GetGameItem(element, VALUE_LAST_PLAYED);
                        game.EarnedGamerscore = GetGameItem(element, VALUE_EARNED_GAMERSCORE);
                        game.AvailableGamerscore = GetGameItem(element, VALUE_AVAILABLE_GAMERSCORE);
                        game.EarnedAchievements = GetGameItem(element, VALUE_EARNED_ACHIEVEMENTS);
                        game.AvailableAchievements = GetGameItem(element, VALUE_AVAILABLE_ACHIEVEMENTS);
                        game.PercentageComplete = GetGameItem(element, VALUE_PERCENTAGE_COMPLETE);
                        games.Add(game);
                    }
                    catch
                    {
                        // Do Nothing
                    }
                }
                gamer.Games = games;
                // Date Updated
                gamer.Updated = DateTime.Now; 
                if (Exists(tag))
                {
                    int position;
                    GamerCard previous = new GamerCard();
                    previous = Get(tag);
                    position = GamerCards.IndexOf(previous);
                    Remove(previous);
                    GamerCards.Insert(position, gamer);
                }
                else
                {
                    GamerCards.Add(gamer);
                }
                Save();
                Completed(this, EventArgs.Empty);
                gamer = null;
            }
            catch
            {
                message = String.Format(ERR_PARSE, tag);
                Failed(this, EventArgs.Empty);
            }
        }

        /// <summary>Delete</summary>
        /// <param name="pattern">File Pattern</param>
        private void Delete(string pattern)
        {
            foreach (string filename in storage.GetFileNames(pattern))
            {
                storage.DeleteFile(filename);
            }
        }
        /// <summary>DeleteFile</summary>
        /// <param name="filename">Filename</param>
        private void DeleteFile(string filename)
        {
            if (storage.FileExists(filename))
            {
                storage.DeleteFile(filename);
            }
        }
        #endregion


        #region Public Methods
        /// <summary>Add</summary>
        /// <param name="tag">Tag</param>
        public void Add(string tag)
        {
            try
            {
                WebClient client = new WebClient();
                client.DownloadStringCompleted += (object sender, DownloadStringCompletedEventArgs e) =>
                {
                    try
                    {
                        if (!e.Cancelled && e.Error == null) { Parse(e.Result, tag); }
                    }
                    catch
                    {
                        message = String.Format(ERR_DOWNLOAD, tag);
                    }
                };
                if (Validate(tag))
                {
                    client.DownloadStringAsync(new Uri(String.Format(URL_SERVICE, HttpUtility.HtmlEncode(tag))));
                }
                else
                {
                    message = ERR_VALID;
                }
            }
            catch
            {
                message = ERR_DOWNLOAD;
                Failed(this, EventArgs.Empty);
            } 
        }

        /// <summary>Refresh</summary>
        /// <param name="gamer">GamerCard</param>
        public void Refresh(GamerCard gamer)
        {
            Add(gamer.Tag);
        }

        /// <summary>Remove Tag</summary>
        /// <param name="gamer">GamerCard</param>
        public void Remove(GamerCard gamer)
        {
            DeleteFile(gamer.Picture.Filename);
            DeleteFile(gamer.Avatar.Filename);
            foreach (Game game in gamer.Games)
            {
                DeleteFile(game.Image.Filename);
            }
            GamerCards.Remove(gamer);
            Save();
        }

        /// <summary>Clear Tags</summary>
        public void Clear()
        {
            Delete(PATTERN_ALL);
            GamerCards.Clear();
        }

        /// <summary>Open XML</summary>
        public void Open()
        {
            try
            {
                if (storage.FileExists(XML_FILE))
                {
                    using (IsolatedStorageFileStream location = new IsolatedStorageFileStream(XML_FILE, FileMode.Open, storage))
                    {
                        using (StreamReader file = new StreamReader(location))
                        {
                            FromXml(XElement.Parse(file.ReadToEnd()));
                        }
                    }
                }
            }
            catch
            {
                Message = ERR_LOAD;
                Failed(this, EventArgs.Empty);
            }          
        }

        /// <summary>Save XML</summary>
        public void Save()
        {
            try
            {
                using (IsolatedStorageFileStream location = new IsolatedStorageFileStream(XML_FILE, FileMode.Create, storage))
                {
                    using (StreamWriter file = new StreamWriter(location))
                    {
                        XDocument doc = new XDocument(new XDeclaration(XML_VERSION, XML_ENCODING, XML_STANDALONE), ToXML());
                        doc.Save(file);
                    }
                }
            }
            catch
            {
                Message = ERR_SAVE;
                Failed(this, EventArgs.Empty);
            }
        }

        /// <summary>Reset Tag</summary>
        public void Reset()
        {
            GamerCard = null;
            NotifyPropertyChanged(PROP_GAMERCARD);
        }
        #endregion

        #region Public Properties
        /// <summary>GamerCard</summary>
        public GamerCard GamerCard { get { return gamerCard; } set { gamerCard = value; NotifyPropertyChanged(PROP_GAMERCARD); } }
        /// <summary>GamerCards</summary>
        public ObservableCollection<GamerCard> GamerCards { get { return gamerCards; } set { gamerCards = value; NotifyPropertyChanged(PROP_GAMERCARDS); } }
        /// <summary>Message</summary>
        public string Message { get { return message; } set { message = value; } }
        #endregion

        #region Events
        /// <summary>Completed Event</summary>
        public static event EventHandler Completed = delegate { };
        /// <summary>Failed Event</summary>
        public static event EventHandler Failed = delegate { };
        #endregion
    }
}