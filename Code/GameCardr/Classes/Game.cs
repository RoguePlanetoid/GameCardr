using System;

namespace GameCardr
{
    /// <summary>Games List Item</summary>
    public class Game
    {
        #region Public Properties
        /// <summary>ID</summary>
        public string ID { get; set; }

        /// <summary>Game Title</summary>
        /// <example>Alan Wake</example>
        public string Title { get; set; }

        /// <summary>Link</summary>
        /// <example>/en-GB/GameCenter/Achievements?titleId=1297287374&amp;compareTo=RoguePlanetoid</example>
        public string Link { get; set; }

        /// <summary>LastPlayed</summary>
        /// <example>26/04/2011</example>
        public string LastPlayed { get; set; }

        /// <summary>EarnedGamerscore</summary>
        /// <example>380</example>
        public string EarnedGamerscore { get; set; }

        /// <summary>AvailableGamerscore</summary>
        /// <example>1000</example>
        public string AvailableGamerscore { get; set; }

        /// <summary>EarnedAchievements</summary>
        /// <example>28</example>
        public string EarnedAchievements { get; set; }

        /// <summary>AvailableAchievements</summary>
        /// <example>50</example>
        public string AvailableAchievements { get; set; }

        /// <summary>PercentageComplete</summary>
        /// <example>56%</example>
        public string PercentageComplete { get; set; }

        /// <summary>Game Image/ Tile </summary>
        /// <example>http://tiles.xbox.com/tiles/PO/0Y/02dsb2JgbA9ECgR8GgMfVl9WL2ljb24vMC84MDAwIAABAAAAAPw37SM=.jpg</example>
        public Picture Image { get; set; }

        /// <summary>Game Cover</summary>
        /// <example>http://tiles.xbox.com/consoleAssets/4D5308D6/en-GB/smallboxart.jpg</example>
        public Picture Cover { get; set; }
        #endregion
    }
}
