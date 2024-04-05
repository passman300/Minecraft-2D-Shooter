using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace PASS2V2
{
    public class Assets
    {
        public static ContentManager Content { get; set; }

        private static string loadPath; // path to load assets from

        // fonts
        public static SpriteFont debugFont;
        public static SpriteFont minecraftBold;
        public static SpriteFont minecraftRegular;
        public static SpriteFont minecraftEvening;

        // player images
        public static Texture2D alexImg;
        public static Texture2D steveImg;

        // tile images
        public static Texture2D grassImg1;
        public static Texture2D grassImg2;
        public static Texture2D dirtImg;
        public static Texture2D coblestoneImg;

        // mob images
        public static Texture2D villagerImg;
        public static Texture2D skeletonImg;
        public static Texture2D pillagerImg;
        public static Texture2D creeperImg;
        public static Texture2D endermanImg;

        // mob effects
        public static Texture2D bloodImgImg;
        public static Texture2D explodeImg;
        public static Texture2D shieldImg;

        // arrow images
        public static Texture2D arrowUpImg;
        public static Texture2D arrowDownImg;

        // menu background images
        public static Texture2D menuImg1;
        public static Texture2D menuImg2;
        public static Texture2D menuImg3;

        // button images
        public static Texture2D buttonImg;
        // blank pixel
        public static Texture2D blankPixel;

        // shop button images
        public static Texture2D speedShopImg;
        public static Texture2D damageShopImg;
        public static Texture2D fireRateShopImg;
        public static Texture2D pointMutiShopImg;

        // upgrades icon images
        public static Texture2D damageIconImg;
        public static Texture2D fireRateIconImg;
        public static Texture2D pointMutiIconImg;
        public static Texture2D speedIconImg;

        // game titles
        public static Texture2D gameTitleImg;
        public static Texture2D shopTitleImg;
        public static Texture2D statsTitleImg;

        /// <summary>
        /// method loads all assets to the game
        /// </summary>
        public static void Initialize()
        {
            // load all fonts
            loadPath = "Fonts";

            // load fonts
            debugFont = Load<SpriteFont>("debugFont");
            minecraftBold = Load<SpriteFont>("minecraftBold");
            minecraftRegular = Load<SpriteFont>("minecraftRegular");
            minecraftEvening = Load<SpriteFont>("minecraftEvening");

            // load all images
            loadPath = "MinecraftImages/Sized";

            // load player images
            alexImg = Load<Texture2D>("Alex_64");
            steveImg = Load<Texture2D>("Steve_64");

            // load tile images
            grassImg1 = Load<Texture2D>("Grass1_64");
            grassImg2 = Load<Texture2D>("Grass2_64");
            dirtImg = Load<Texture2D>("Dirt_64");
            coblestoneImg = Load<Texture2D>("Cobblestone_64");

            // load mob images
            villagerImg = Load<Texture2D>("Villager_64");
            skeletonImg = Load<Texture2D>("Skeleton_64");
            pillagerImg = Load<Texture2D>("Pillager_64");
            creeperImg = Load<Texture2D>("Creeper_64");
            endermanImg = Load<Texture2D>("Enderman_64");

            // load mob effects
            bloodImgImg = Load<Texture2D>("Splat_64");
            explodeImg = Load<Texture2D>("Explode_200");
            shieldImg = Load<Texture2D>("Shield_48");

            // load arrow images
            arrowUpImg = Load<Texture2D>("ArrowUp");
            arrowDownImg = Load<Texture2D>("ArrowDown");

            // load menu background images
            menuImg1 = Load<Texture2D>("MenuBG1");
            menuImg2 = Load<Texture2D>("MenuBG2");
            menuImg3 = Load<Texture2D>("MenuBG3");

            // load button images
            buttonImg = Load<Texture2D>("Button");

            // load blank pixel
            blankPixel = Load<Texture2D>("BlankPixel");

            // shop button images
            speedShopImg = Load<Texture2D>("ShopSpeedBoost_300");
            damageShopImg = Load<Texture2D>("ShopDamageBoost_300");
            fireRateShopImg = Load<Texture2D>("ShopFireRateBoost_300");
            pointMutiShopImg = Load<Texture2D>("ShopPointsBoost_300");

            // load upgrades icon images
            speedIconImg = Load<Texture2D>("IconSpeed_32");
            damageIconImg = Load<Texture2D>("IconDamage_32");
            fireRateIconImg = Load<Texture2D>("IconFireRate_32");
            pointMutiIconImg = Load<Texture2D>("IconPoints_32");

            // load game titles
            gameTitleImg = Load<Texture2D>("Title");
            shopTitleImg = Load<Texture2D>("ShopTitle");
            statsTitleImg = Load<Texture2D>("StatsTitle");
        }

        /// <summary>
        /// method loads an asset
        /// </summary>
        /// <typeparam name="T"></typeparam> the type of the asset
        /// <param name="file"></param> file to load
        /// <returns></returns>
        private static T Load<T>(string file) => Content.Load<T>($"{loadPath}/{file}");
    }
}
