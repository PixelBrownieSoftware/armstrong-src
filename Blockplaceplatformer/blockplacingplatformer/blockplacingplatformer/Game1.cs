using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using System.Threading;
using BrownieEngine;
using BrownieContentImporter;


namespace blockplacingplatformer
{
    using anim_size_offset = List<Tuple<Vector2, Vector2>>;
    public class s_levelloader
    {

    }

    public class s_renderer
    {

    }

    public struct s_shape
    {
        public s_shape(List<Vector2> points, Vector2 position)
        {
            this.points = points;
            this.position = position;
        }
        public Vector2 position;

        public float GetOverlap()
        {
            float small = float.MaxValue;
            
            return small;
        }

        public List<Vector2> GetNormals()
        {
            List<Vector2> pts = new List<Vector2>();
               Vector2 lastv = points[0];
            foreach (Vector2 v in points)
            {
                Vector2 ve = GetNormal(lastv - v);
                lastv = v;
                pts.Add(ve);
            }
            return pts;
        }

        public Tuple<Vector2,float> FindMTV(s_shape shape2)
        {
            List<Vector2> axis1 = GetAxis();
            List<Vector2> axis2 = shape2.GetAxis();
            Vector2 smallest = new Vector2(0,0);
            float overlap = float.MaxValue;
            Vector2 lastv = shape2.points[0];
            foreach (Vector2 v in shape2.points)
            {
                Vector2 ve = GetNormal(lastv-v);

                if (IsIntersecting(shape2, ve))
                {
                    Tuple<float, float> sh = GetIntersectAxis(ve);
                    Tuple<float, float> sh1 = shape2.GetIntersectAxis(ve);

                    float[] a = new float[4];
                    a[0] = sh1.Item2 - sh.Item1;
                    a[1] = sh1.Item1 - sh.Item1;
                    a[2] = sh1.Item2 - sh.Item2;
                    a[3] = sh1.Item1 - sh.Item2;

                    for (int i = 0; i < 3; i++)
                    {
                        float ab = Math.Abs(a[i]);
                        if (ab == 0)
                            continue;
                        if (ab < overlap)
                        {
                            smallest = ve;
                            smallest.Y = smallest.Y * -1;
                            overlap = ab;
                        }
                    }
                }
                lastv = v;
            }
            return new Tuple<Vector2, float>(smallest, overlap);
        }

        public List<Vector2> GetAxis()
        {
            List<Vector2> axis = new List<Vector2>();
            foreach (Vector2 ax in points)
            {
                Vector2 v = ax;
                ax.Normalize();
                axis.Add(ax);
            }
            return axis;
        }

        public Vector2 GetNormal(Vector2 point)
        {
            Vector2 v = new Vector2(-point.Y, point.X);
            v.Normalize();
            return v;
        }

        public bool Interesect(s_shape shape2)
        {
            foreach (Vector2 axis in Game1.axis)
            {
                if (!IsIntersecting(shape2, axis))
                    return false;
            }
            return true;
        }

        public bool Interesect(s_shape shape2, List<Vector2> axis)
        {
            foreach (Vector2 ax in axis)
            {
                if (!IsIntersecting(shape2, ax))
                    return false;
            }
            return true;
        }
        
        public Tuple<float, float> GetIntersectAxis( Vector2 axis)
        {
            float max = 0, min = float.MaxValue;
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 pt = points[i];
                float f = Vector2.Dot(axis, pt + position);
                if (f < min)
                    min = f;
                else if (f > max)
                    max = f;
            }
            return new Tuple<float, float>(max,min);
        }

        public bool IsIntersecting(s_shape shape2, Vector2 axis)
        {
            float max1 = 0, min1 = float.MaxValue;
            float max2 = 0, min2 = float.MaxValue;
            
            for (int i = 0; i < points.Count; i++)
            {
                Vector2 pt = points[i];
                float f = Vector2.Dot(axis, pt + position);
                if (f < min1)
                    min1 = f;
                else if (f > max1)
                    max1 = f;
            }
            for (int i = 0; i < shape2.points.Count; i++)
            {
                Vector2 pt2 = shape2.points[i];
                float f = Vector2.Dot(axis, pt2 + shape2.position);
                if (f < min2)
                    min2 = f;
                else if(f > max2)
                    max2 = f;
            }

            if (min1 < max2 && max1 > min2)
                return true;
            return false;
        }
        public List<Vector2> points;
    }

    public static class s_gui
    {
        public static bool IfButton(Rectangle rect)
        {
            Game1.buttons.Add(new Tuple<Rectangle, string>(rect, ""));
            if (Game1.mousestate.LeftButton == ButtonState.Pressed)
            {
                if (rect.Intersects(new Rectangle(Game1.mouseposition2.ToPoint(), new Point(2, 2))))
                    return true;
            }
            return false;
        }
        public static bool IfButton(Rectangle rect, string text)
        {
            Game1.buttons.Add(new Tuple<Rectangle, string>(rect, text));
            if (Game1.game.GetMouseDown())
            {
                if (rect.Intersects(new Rectangle(Game1.scrnMsPos.ToPoint(), new Point(2, 2))))
                    return true;
            }
            return false;
        }
    }

    public struct o_button
    {
        public Rectangle rect;
        public string text;
        public o_button(string text, Rectangle rect)
        {
            this.rect = rect;
            this.text = text;
        }
    }

    public struct s_font
    {
        public s_font(Vector2 fontSize, Vector2 fontGlyphs, List<char> characters, Texture2D fontTexture)
        {
            this.fontSize = fontSize;
            this.characters = characters;
            this.fontTexture = fontTexture;
            this.fontGlyphs = fontGlyphs;
        }
        public Vector2 fontGlyphs;
        public Vector2 fontSize;
        public List<char> characters;
        public Texture2D fontTexture;
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game1 game;
        public static Dictionary<string, List<ushort>> layers = new Dictionary<string, List<ushort>>();
        bool enableDebug = false;
        bool isPaused = false;
        int levelPageNumber = 0;

        int endingNumber = 0;
        float endingTimer = 1f;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static RenderTarget2D VirtScreen;
        s_font font;
        bool introShow = false;

        Color BG_COLOUR;
        public enum GAME_MODE
        {
            INTRO,
            MENU,
            GAME,
            LEVEL_SELECT,
            INSTRUCTIONS,
            ENDING
        }
        GAME_MODE gameMode;

        List<List<Vector2>> normals = new List<List<Vector2>>();

        bool shapeIntersect = false;

        public static Vector2 scrnMsPos;
        public static Vector2 mouseposition;
        public static Vector2 mouseposition2;

        public static s_map[] levels;
        public static s_map[] StaticLevels;
        public s_map level;
        public static int currentLevel = 0;

        uint thrust = 1;

        public static float deltaTime;

        float introTimer = 1f;
        float introDelay = 1f;

        public static MouseState mousestate;
        MouseState prevmousestate;

        KeyboardState previousKeyboardState;
        KeyboardState currentKeyboardState;
        bool debuginfo = false;
        
        enum DEBUG_CONTROL
        {
            PLATFORM,
            MOVE
        }
        DEBUG_CONTROL dbg = DEBUG_CONTROL.PLATFORM;

        static Vector2 campos = new Vector2(0, 0);

        Random ra = new Random();

        public static Vector2[] axis = new Vector2[8] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(-1, 0), new Vector2(0, -1), new Vector2(0, -0.5f), new Vector2(0.5f, 0), new Vector2(-1f, -1f) };

        public static Dictionary<string, Tuple<Texture2D, anim_size_offset>> textures = new Dictionary<string, Tuple<Texture2D, anim_size_offset>>();
        public static Texture2D blank;
        Texture2D titleImg;
        Texture2D logoImg;
        Texture2D endImg;

        o_plcharacter pl;
        s_object testO;
        Vector2 player_onScreenPosition;

        s_shape shape;
        s_shape shape2;
        Tuple<Vector2, float> dist;
        Tuple<float, float> Penetration1;
        Tuple<float, float> Penetration2;

        Vector2 screensize = new Vector2(33, 720);

        public static List<Tuple<Rectangle, string>> buttons = new List<Tuple<Rectangle, string>>();
        public static List<o_platformControler> chara = new List<o_platformControler>();
        public static List<o_block> blocks = new List<o_block>();
        public static List<s_object> objects = new List<s_object>();
        public static List<SoundEffect> sounds = new List<SoundEffect>();

        public Game1()
        {
            game = this;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1152;
            graphics.PreferredBackBufferHeight = 648;
            graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            VirtScreen = new RenderTarget2D(GraphicsDevice, 384, 216);
            blank = new Texture2D(GraphicsDevice, 1, 1);
            mouseposition = new Vector2(graphics.GraphicsDevice.Viewport.
                       Width / 2,
                                    graphics.GraphicsDevice.Viewport.
                                    Height / 2);
            Mouse.SetPosition(graphics.GraphicsDevice.Viewport.
                                  Width / 2,
                    graphics.GraphicsDevice.Viewport.Height / 2);
            IsMouseVisible = true;
            SetLayers();
            BG_COLOUR = Color.White;
            base.Initialize();
        }

        public static void PlaySound(int sfx_num)
        {
            SoundEffect se = sounds[sfx_num];
            se.Play();
        }
        public static void PlaySound(int sfx_num, Vector2 position)
        {
            float hyp = HypotenuseVector(position - (campos + centreOfScreen));

            if (hyp > 145)
                return;
            SoundEffect se = sounds[sfx_num];
            se.Play();
        }

        void SetLayers()
        {
            string solid = "solid";
            List<ushort> solidBlocks = new List<ushort> {1, 2};

            string liquid = "liquid";
            List<ushort> liquidBlocks = new List<ushort> {4};

            string air = "air";
            List<ushort> airBlocks = new List<ushort> {0};
            
            string solidFall = "solidFall";
            List<ushort> solidFallBlocks = new List<ushort> { 5 };

            layers.Add(solid, solidBlocks);
            layers.Add(liquid, liquidBlocks);
            layers.Add(air, airBlocks);
            layers.Add(solidFall, solidFallBlocks);
        }

        public static bool IsSameLayer(ushort num, string layr)
        {
            for (int i = 0; i < layers[layr].Count; i++)
            {
                if (num == layers[layr][i])
                    return true;
            }
            return false;
        }
        public static bool IsSameLayer(ushort num, List<string> layr)
        {
            foreach (string st in layr)
            {
                for (int i = 0; i < layers[st].Count; i++)
                {
                    if (num == layers[st][i])
                        return true;
                }
            }
            return false;
        }
        
        public bool GetMouseDown()
        {
            if (prevmousestate.LeftButton == ButtonState.Released 
                && mousestate.LeftButton == ButtonState.Pressed
                )
            {
                return true;
            }
            else if(prevmousestate.LeftButton == ButtonState.Pressed
                && mousestate.LeftButton == ButtonState.Pressed
                )
            {
                return false;
            }
            return false;
        }

        public void CreateSpriteSheets()
        {
            anim_size_offset aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(23, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(30, 0), new Vector2(60, 60)));
            textures.Add("bomb", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("explosion"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 25)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 25)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(40, 0), new Vector2(20, 25)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(60, 0), new Vector2(20, 25)));
            textures.Add("spring", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("spring"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            textures.Add("ball", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("ball"), aso) );

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 40)));
            textures.Add("door", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("door"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            textures.Add("bound", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("boundary"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(36, 46), new Vector2(25, 42)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(1, 47), new Vector2(29, 41)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(36, 2), new Vector2(28, 41)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(1, 0), new Vector2(29, 41)));
            textures.Add("annie", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("annie_sprite"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            textures.Add("spikes", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("spikes"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
            textures.Add("switch", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("switch"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(40, 0), new Vector2(20, 20)));
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(60, 0), new Vector2(20, 20)));
            textures.Add("tiles", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("tiles"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
            textures.Add("bomber", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("bomber"), aso));

            aso = new anim_size_offset();
            aso.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(10, 10)));
            textures.Add("font", new Tuple<Texture2D, anim_size_offset>(Content.Load<Texture2D>("font"), aso));
        }

        protected override void LoadContent()
        {
            currentLevel = 0;
            sounds = new List<SoundEffect>();

            blank = Content.Load<Texture2D>("pixel");
            titleImg = Content.Load<Texture2D>("logo");
            logoImg = Content.Load<Texture2D>("PBRS_LOGO_2019");
            endImg = Content.Load<Texture2D>("ending");
            spriteBatch = new SpriteBatch(GraphicsDevice);

            CreateSpriteSheets();

            sounds.Add(Content.Load<SoundEffect>("Sound/step"));
            sounds.Add(Content.Load<SoundEffect>("Sound/dive"));
            sounds.Add(Content.Load<SoundEffect>("Sound/op_jingle"));
            sounds.Add(Content.Load<SoundEffect>("Sound/error"));
            sounds.Add(Content.Load<SoundEffect>("Sound/bash2"));
            sounds.Add(Content.Load<SoundEffect>("Sound/land2"));
            sounds.Add(Content.Load<SoundEffect>("Sound/land"));
            sounds.Add(Content.Load<SoundEffect>("Sound/explosion2"));
            sounds.Add(Content.Load<SoundEffect>("Sound/bash2"));
            sounds.Add(Content.Load<SoundEffect>("Sound/bounce2"));
            sounds.Add(Content.Load<SoundEffect>("Sound/outofwater"));

            int mapLeng = 27;

            string[] levelNames = new string[mapLeng];
            levels = new s_map[mapLeng];
            StaticLevels = new s_map[mapLeng];

            levelNames[0] = "level1";
            levelNames[1] = "map2";
            levelNames[2] = "map3";
            levelNames[3] = "map4";
            levelNames[4] = "map6";
            levelNames[5] = "map7";
            levelNames[6] = "map9";
            levelNames[7] = "map11";
            levelNames[8] = "map10";
            levelNames[9] = "map8";
            levelNames[10] = "map12";
            levelNames[11] = "map22";
            levelNames[12] = "map23";
            levelNames[13] = "map13";
            levelNames[14] = "map18";
            levelNames[15] = "map16";
            levelNames[16] = "map25";
            levelNames[17] = "map29";
            levelNames[18] = "map33";
            levelNames[19] = "map30";
            levelNames[20] = "map26";
            levelNames[21] = "map27";
            levelNames[22] = "map28";
            levelNames[23] = "map34";
            levelNames[24] = "map32";
            levelNames[25] = "map20";
            levelNames[26] = "map35";

            /*
            17
            22
            19
            24
            16
            20
            21
            25
            23
            18
            26
            
            levelNames[23] = "map31";
            levels[0] = Content.Load<s_map>("level1");
            levels[1] = Content.Load<s_map>("map2");
            levels[2] = Content.Load<s_map>("map3");
            levels[3] = Content.Load<s_map>("map4");
            levels[4] = Content.Load<s_map>("map6");
            levels[5] = Content.Load<s_map>("map7");
            levels[6] = Content.Load<s_map>("map8");
            levels[7] = Content.Load<s_map>("map9");
            levels[8] = Content.Load<s_map>("map10");
            levels[9] = Content.Load<s_map>("map11");
            levels[10] = Content.Load<s_map>("map12");
            levels[11] = Content.Load<s_map>("map18");
            levels[12] = Content.Load<s_map>("map13");
            levels[13] = Content.Load<s_map>("map22");
            levels[14] = Content.Load<s_map>("map16");
            levels[15] = Content.Load<s_map>("map23");
            */

            for (int i = 0; i < levelNames.Length; i++)
            {
                string st = levelNames[i];
                levels[i] = Content.Load<s_map>(st);
                StaticLevels[i] = Content.Load<s_map>(st);
            }
            //levels[15] = Content.Load<s_map>("map21");
            
            level = levels[0];
            AddFont();

            //normals.Add(shape2.GetNormals());
            //normals.Add(shape.GetNormals());
        }

        public void AddFont()
        {
            Vector2 fontsize = new Vector2(10, 10);
            Vector2 fontglyph = new Vector2(12, 7);
            List<char> characters = new List<char>();

            string fontstr = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789: .0><?,!-()+[]=_#/" +
                " " + ";%*|^abcdefghijklmnopqrstuvwxyz";
            for (int i = 0; i < fontstr.Length; i++)
            {
                characters.Add(fontstr[i]);
                char c = fontstr[i];
            }
            font = new s_font(fontsize, fontglyph, characters, textures["font"].Item1);
        }
        public void AddFontOld()
        {
            List<Rectangle> glyp = new List<Rectangle>();
            List<Rectangle> bound = new List<Rectangle>();
            List<Vector3> kern = new List<Vector3>();
            List<char> characters = new List<char>();

            for (int i = 0; i < 66; i++)
            {
                kern.Add(new Vector3(0, 0, 0));
                glyp.Add(new Rectangle(0, 0, 10, 10));
            }
            for (int x = 0; x < 11; x++)
            {
                for (int y = 0; y < 6; y++)
                {
                    bound.Add(new Rectangle(x * 10, y * 10, 10, 10));
                }
            }
            string fontstr = "abcdefghijklmnopqrstuvwxyz123456789: .0><?,!";
            for (int i = 0; i < fontstr.Length; i++)
            {
                characters.Add(fontstr[i]);
            }

            //font = new SpriteFont(fontTexture,glyp, bound,characters,0,0, kern,'a');
        }

        public void IncrementLevel()
        {
            currentLevel++;
            if (currentLevel == 16) //ENDING OF ORIGINAL
            {
                endingNumber = 0;
                gameMode = GAME_MODE.ENDING;
                return;
            }
            if (currentLevel == 27) //ENDING OF EP 1
            {
                endingNumber =1;
                gameMode = GAME_MODE.ENDING;
                return;
            }
            objects.Clear();
            pl.heldItem = o_item.ITEM_TY.None;
            objects.Add(pl);
            
            LoadLevel(StaticLevels[currentLevel]);
        }

        protected override void UnloadContent()
        {
        }
        
        public void LoadLevel(s_map map)
        {
            s_map newMap = new s_map();

            ushort sx = map.mapSizeX, sy = map.mapSizeY;
            ushort tx = map.tileSizeX, ty = map.tileSizeY;

            newMap.mapSizeX = sx;
            newMap.mapSizeY = sy;
            newMap.tileSizeX = (byte)tx;
            newMap.tileSizeY = (byte)ty;

            newMap.tiles = new ushort[map.tiles.Length];
            newMap.entities = new List<o_entity>();
            for (ushort i = 0; i < map.tiles.Length; i++)
            {
                newMap.tiles[i] = map.tiles[i];
            }
            for (int i = 0; i < map.entities.Count; i++)
            {
                o_entity ent = map.entities[i];
                newMap.entities.Add(map.entities[i]);

            }
            level = newMap;

            #region CREATE TILES
            int x = 0, y = 0;
            for (ushort i = 0; i < map.tiles.Length; i++)
            {
                anim_size_offset anim = new anim_size_offset();
                if (x == (sx * tx))
                {
                    y += ty;
                    x = 0;
                }
                ushort tex = map.tiles[i];
                if (tex == 0)
                {
                    x += tx;
                    continue;
                }
                else
                    tex--;
                Texture2D texTu = textures["tiles"].Item1;
                
                int tilX = tex % (texTu.Width / map.tileSizeX);
                int tilY = ((tex * map.tileSizeX) / texTu.Width);

                o_block bl = null;
                Vector2 pos = new Vector2(x, y);
                int intPos = 0;

                switch (tex)
                {
                    default:
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20 * tilX, 20 * tilY), new Vector2(20, 20)));
                        bl = AddObject<o_block>(new Vector2(x, y), "bl_" + i, anim, textures["tiles"].Item1);
                        bl.issolid = true;
                        break;

                    case 1:

                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
                        bl = AddObject<o_block>(pos, "breakable block", anim, textures["tiles"].Item1);
                        intPos = TwoDToOneDArray(pos);
                        //level.tiles[intPos] = 2;
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.BREAKABLE;
                        bl.issolid = true;
                        break;
                    case 8:

                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
                        bl = AddObject<o_block>(pos, "breakable block", anim, textures["tiles"].Item1);
                        intPos = TwoDToOneDArray(pos);
                        //level.tiles[intPos] = 2;
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.BREAKABLE;
                        bl.issolid = true;
                        break;
                }
                x += tx;
            }
            #endregion
            
            #region CREATE ENTITIES
            int leng = map.entities.Count;
            for (int i = 0; i < map.entities.Count; i++)
            {
                o_entity ent = map.entities[i];
                ushort id = ent.id;
                int a = ent.position.X;
                Vector2 pos = new Vector2(ent.position.X, ent.position.Y);
                ushort label = ent.labelToCall;
                anim_size_offset anim = new anim_size_offset();
                o_block bl = null;
                int intPos = 0;
                switch (id)
                {
                    case 0:
                        if (pl == null)
                        {
                            pl = AddObject<o_plcharacter>(pos, "player", textures["annie"].Item2, textures["annie"].Item1);
                            pl.spawnpoint = pos;
                            pl.velocityLimiter = 0.85f;
                            pl.collisionBox.Height = 40;
                            pl.height = 2;
                        }
                        else
                        {
                            pl.spawnpoint = pos;
                            pl.position = pos;
                        }

                        break;

                    case 1:
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 40)));
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 40)));
                        bl = AddObject<o_block>(pos - new Vector2(0, 20), "Door", anim, textures["door"].Item1);
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.DOOR;
                        bl.flag = "0";
                        bl.flag = ent.GetFlag("locked");
                        bl.name = ent.GetFlag("name");
                        bl.issolid = false;
                        //AddBlock(new o_block(pos), 1, o_block.BLOCK_TYPE.DOOR, "Door");
                        break;

                    case 2:
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
                        bl = AddObject<o_block>(pos, "switch", anim, textures["switch"].Item1 );
                        bl.flag = ent.GetFlag("doorFlag");
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.SWITCH;
                        bl.issolid = false;
                        break;

                    case 3:
                        bl = AddObject<o_block>(pos + new Vector2(0,4), "spikes", textures["spikes"].Item2, textures["spikes"].Item1 );
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.SPIKES;
                        bl.collisionBox.Height = 10;
                        bl.collisionBox.Width = 10;
                        bl.collisionOffset = new Vector2(5, 5);
                        bl.renderer.CentreOffset = new Point(-5, -5);
                        bl.issolid = false;
                        break;

                    case 4:
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(20, 0), new Vector2(20, 20)));
                        bl = AddObject<o_block>(pos, "breakable block", anim, textures["tiles"].Item1);
                        intPos = TwoDToOneDArray(pos);
                        bl.TileNum = GetTile(pos);
                        level.tiles[intPos] = 1;
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.BREAKABLE;
                        bl.issolid = true;
                        break;

                    case 5:
                        continue;
                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
                        o_movableSolid bla = AddObject<o_movableSolid>(pos, "boulder", anim, textures["testtexture2"].Item1);
                        bla.issolid = true;
                        break;

                    case 6:
                        
                        o_item b = AddObject<o_item>(pos, "bomb", textures["bomb"].Item2, textures["bomb"].Item1);
                        b.spawnpoint = pos;
                        b.itType = o_item.ITEM_TY.Throwing;
                        switch (ent.GetFlag("itemType"))
                        {
                            case "0":
                                b.collisionOffset = new Vector2(-10, 0);
                                b.collisionBox.Width = 10;
                                b.itType = o_item.ITEM_TY.Bomb;
                                break;

                            case "1":
                                b.renderer.SetTexture(textures["ball"].Item2, textures["ball"].Item1);
                                b.itType = o_item.ITEM_TY.Throwing;
                                break;
                        }
                        break;

                    case 7:

                        anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
                        bl = AddObject<o_block>(pos, "switch", anim, textures["bound"].Item1);
                        bl.name = ent.GetFlag("name");
                        intPos = TwoDToOneDArray(pos);
                        level.tiles[intPos] = 1;
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.SWITCH_DOOR;
                        break;

                    case 8:
                        
                        bl = AddObject<o_block>(pos, "bomber", textures["bomber"].Item2, textures["bomber"].Item1);
                        bl.max_bomb_timer = 1.5f;
                        bl.direction = new Vector2(0f, 1f);
                        /*
                        int dir = (int)ent.GetFlagFloat("dir");
                        switch (dir)
                        {
                            case 0:
                                bl.direction = new Vector2(0, -1);
                                break;

                            case 1:
                                bl.direction = new Vector2(0.5f, -0.5f);
                                break;

                            case 2:
                                bl.direction = new Vector2(1, 0);
                                break;

                            case 3:
                                bl.direction = new Vector2(0.5f, 0.5f);
                                break;

                            case 4:
                                bl.direction = new Vector2(0f, 1f);
                                break;

                            case 5:
                                bl.direction = new Vector2(-0.5f, 0.5f);
                                break;

                            case 6:
                                bl.direction = new Vector2(-1, 0);
                                break;

                            case 7:
                                bl.direction = new Vector2(-0.5f, -0.5f);
                                break;

                        }
                        */
                        intPos = TwoDToOneDArray(pos);
                        level.tiles[intPos] = 1;
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.BOMBER;
                        break;

                    case 9:

                        bl = AddObject<o_block>(pos, "bouncer", textures["spring"].Item2, textures["spring"].Item1);
                        bl.collisionBox.Height = 10;
                        bl.collisionBox.Width = 15;
                        bl.collisionOffset = new Vector2(3, 5);
                        bl.renderer.CentreOffset = new Point(-3, -10);

                        bl.direction = new Vector2(0, -1);
                        intPos = TwoDToOneDArray(pos);
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.BOUNCER;
                        bl.issolid = false;
                        break;

                    case 10:

                        bl = AddObject<o_block>(pos, "portal", textures["bomber"].Item2, textures["bomber"].Item1);
                        bl.flag = ent.GetFlag("portalFlag");
                        bl.name = ent.GetFlag("name");
                        bl.TYPEOFBLOCK = o_block.BLOCK_TYPE.PORTAL;
                        break;
                }
                
            }
            #endregion
        }

        public T AddObject<T>(Vector2 pos, string name) where T : s_object, new()
        {
            if (chara.Count > 20)
                return null;
            T cha = new T();
            cha.position = pos;
            cha.SetCollisonSize(new Vector2(20, 20));
            cha.name = name;
            objects.Add(cha);
            //chara.Add(cha);
            return cha;
        }
        public static T AddObject<T>(Vector2 pos, string name, List<Tuple<Vector2, Vector2>> sizesAndOffsets, Texture2D texture) where T : s_object, new()
        {
            if (chara.Count > 20)
                return null;
            T cha = new T();
            cha.position = pos;
            cha.SetCollisonSize(new Vector2(20, 20));
            cha.renderer.SetSpriteOffsetsAndSizes(sizesAndOffsets);
            cha.renderer.texture = texture;
            cha.name = name;
            objects.Add(cha);
            //chara.Add(cha);
            return cha;
        }
        public static void RemoveObject(s_object obj)
        {
            s_object obja = objects.Find(x => x == obj);
            if (obja != null)
                objects.Remove(obja);
        }

        public static T FindCharacter<T>(string nam) where T : s_object
        {
            for (int i = 0; i < objects.Count; i++)
            {
                T obj = objects[i].GetComponent<T>();
                if (obj != null)
                {
                    if (obj.name != nam)
                        continue;
                    return obj;
                }
            }
            return null;
        }
        public static T[] FindCharacters<T>(string nam) where T : s_object
        {
            List<T> objec = new List<T>();
            for (int i = 0; i < objects.Count; i++)
            {
                T obj = objects[i].GetComponent<T>();
                if (obj != null)
                {
                    if (obj.name != nam)
                        continue;
                    objec.Add(obj);
                }
            }
            return objec.ToArray();
        }

        static float Square(float number, int power)
        {
            float result = number;
            for (int i = power; i != 1; i--)
            {
                result *= number;
            }
            return result;
        }

        static float SquareRoot(float num)
        {
            return (float)Math.Sqrt(num);
        }

        static float HypotenuseVector(Vector2 a)
        {
            float add = Square(a.X, 2) + Square(a.Y, 2);
            float hyp = SquareRoot(add);
            return Math.Abs(hyp);
        }

        public void DrawShape(s_shape sh, GameTime gt, SpriteBatch sb, Vector2 position)
        {
            if (sh.points == null)
                return;
            if (sh.points.Count == 0)
                return;
            Vector2 point = sh.points[0];
            for(int i = 1; i < sh.points.Count; i++) 
            {
                Vector2 pt = sh.points[i];
                DrawLine(pt + position, point + position, gt);
                point = pt;
            }
        }
        public void DrawShape(s_shape sh, GameTime gt, SpriteBatch sb)
        {
            if (sh.points == null)
                return;
            if (sh.points.Count == 0)
                return;
            Vector2 point = sh.points[0] + sh.position;
            for (int i = 1; i < sh.points.Count; i++)
            {
                Vector2 pt = sh.points[i] + sh.position;
                DrawLine(pt, point, gt);
                point = pt;
            }
        }

        public void DrawLine(Vector2 direction,Vector2 origin,float length,GameTime gt)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;
            direction.Normalize();

            Vector2 pos = origin;
            pos = new Vector2(pos.X, pos.Y);

            for (float i = 0; i < length; i += delta)
            {
                pos += direction;
                spriteBatch.Draw(blank, pos, Color.White);
            }
        }
        public void DrawLine(Vector2 end, Vector2 origin, GameTime gt)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;

            Vector2 pos = origin;
            Vector2 dist = end - origin;
            float hypotenuse = HypotenuseVector(dist);

            dist.Normalize();

            for (float i = 0; i < hypotenuse; i += delta)
            {
                pos += dist * delta;
                spriteBatch.Draw(blank, pos, Color.Black);
            }
        }
        public void DrawLine(Vector2 direction, Vector2 origin, float length, GameTime gt, Color colour)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;
            direction.Normalize();

            Vector2 pos = origin;
            pos = new Vector2(pos.X, pos.Y);

            for (float i = 0; i < length; i += delta)
            {
                pos += direction;
                spriteBatch.Draw(blank, pos, colour);
            }
        }
        public void DrawLine(Vector2 end, Vector2 origin, GameTime gt, Color colour)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;

            Vector2 pos = origin;
            Vector2 dist = end - origin;
            float hypotenuse = HypotenuseVector(dist);

            dist.Normalize();

            for (float i = 0; i < hypotenuse; i += delta)
            {
                pos += dist * delta;
                spriteBatch.Draw(blank, pos, colour);
            }
        }

        public float CalculateLine(Vector2 end, Vector2 origin, GameTime gt)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;

            Vector2 pos = origin;
            Vector2 dist = end - origin;
            float hypotenuse = HypotenuseVector(dist);
            return hypotenuse;
        }
        public void DrawThrowDirection(GameTime gt, float length)
        {
            var delta = (float)gt.ElapsedGameTime.TotalSeconds;
            Vector2 pos = pl.centre - campos;
            pl.throwDir = scrnMsPos - pos;

            Vector2 m = scrnMsPos - pl.centre;

            pl.throwDir.Normalize();

           // DrawLine(pl.centre, scrnMsPos, HypotenuseVector(m), gt);
            Vector2 vel = pl.throwDir * pl.throwMultiplier;


            for (float i = 0; i < 2.5f; i += delta)
            {
                vel.X *= 0.97f;
                vel.Y += 0.1f;
                //Vector2 newpos = pos + vel;
                pos += vel;
                //Point posInWorld = pos.ToPoint() - campos.ToPoint()- centreOfScreen.ToPoint();
                /*
                    foreach (s_object o in objects)
                    {
                        if (o == pl)
                            continue;
                        if (o.collisionBox.Intersects(new Rectangle(posInWorld, new Point(1, 1))))
                        {
                            sb.Draw(blank, new Rectangle(o.collisionBox.Location - campos.ToPoint() - centreOfScreen.ToPoint(), o.collisionBox.Size) , Color.White);
                            return;
                        }
                    }
                */
                spriteBatch.Draw(blank, pos, Color.Gray);
            }
        }
        public void DrawText(string text, s_font fnt, Vector2 pos, SpriteBatch sb)
        {
            Vector2 initalVe = pos;
            if (text == null)
                return;
            int sizeX = (int)fnt.fontSize.X, sizeY = (int)fnt.fontSize.Y;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    pos.X = initalVe.X;
                    pos.Y += 10;
                    continue;
                }

                int ind = fnt.characters.FindIndex(chr => chr == c) ;
                int x = (int)(ind % fnt.fontGlyphs.X),
                    y = (int)(ind / fnt.fontGlyphs.X);
                pos.X += sizeX;

                Rectangle dst = new Rectangle(new Point((int)pos.X , (int)pos.Y), fnt.fontSize.ToPoint());
                Rectangle src = new Rectangle(new Point((x * sizeX), (y * sizeY)), fnt.fontSize.ToPoint());

                sb.Draw(fnt.fontTexture, dst, src, Color.White);
            }
        }
        public void DrawText(string text, s_font fnt, Vector2 pos, SpriteBatch sb, byte alpha)
        {
            if (text == null)
                return;
            int sizeX = (int)fnt.fontSize.X, sizeY = (int)fnt.fontSize.Y;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (c == '\n')
                {
                    pos.X = 0;
                    pos.Y += 10;
                }

                int ind = fnt.characters.FindIndex(chr => chr == c);
                int x = (int)(ind % fnt.fontGlyphs.X),
                    y = (int)(ind / fnt.fontGlyphs.X);
                pos.X += sizeX;

                Rectangle dst = new Rectangle(new Point((int)pos.X, (int)pos.Y), fnt.fontSize.ToPoint());
                Rectangle src = new Rectangle(new Point((x * sizeX), (y * sizeY)), fnt.fontSize.ToPoint());

                sb.Draw(fnt.fontTexture, dst, src, new Color(Color.White.R, Color.White.G, Color.White.B, alpha));
            }
        }

        public void ResetLevel()
        {
            objects.Clear();
            objects.Add(pl);
            pl.heldItem = o_item.ITEM_TY.None;
            pl.position = pl.spawnpoint;
            pl.velocity = Vector2.Zero;
            pl.oyxgenMetre = 1;
            LoadLevel(levels[currentLevel]);
        }
        
        Vector2 NormalizeVector(Vector2 vec)
        {
            return vec;
        }
        static Vector2 centreOfScreen
        {
            get
            {
                return new Vector2(VirtScreen.Width / 2, VirtScreen.Height / 2);
            }
        }
        Vector2 screenSize
        {
            get
            {
                return new Vector2(VirtScreen.Width, VirtScreen.Height);
            }
        }
        Vector2 ScreenToVirtualScreen(Vector2 pos)
        {
            Vector2 realScreen = new Vector2(graphics.GraphicsDevice.Viewport.Width, graphics.GraphicsDevice.Viewport.Height);

            float posx = pos.X / realScreen.X;
            float posy = pos.Y / realScreen.Y;

            return new Vector2(VirtScreen.Width * posx, VirtScreen.Height * posy);
        }

        public Vector2 getTruncLevel(Vector2 pos)
        {
            Vector2 lv = new Vector2();

            lv.X = pos.X / level.tileSizeX;
            lv.Y = pos.Y / level.tileSizeY;

            lv.X = (int)Math.Truncate(lv.X);
            lv.Y = (int)Math.Truncate(lv.Y);

            //lv.Y *= level.tileSizeX;
            //lv.X *= level.tileSizeY;

            return lv;
        }
        public Vector2 getTruncLevel(Point pos)
        {
            Vector2 lv = new Vector2();

            lv.X = pos.X / level.tileSizeX;
            lv.Y = pos.Y / level.tileSizeY;

            lv.X = (int)Math.Truncate(lv.X);
            lv.Y = (int)Math.Truncate(lv.Y);

            //lv.Y *= level.tileSizeX;
            //lv.X *= level.tileSizeY;

            return lv;
        }

        public ushort GetTile(Vector2 smallPos)
        {
            int pos = TwoDToOneDArray(smallPos);
            if (pos > level.tiles.Length - 1 || pos < 0)
                return 0;
            return level.tiles[pos];
        }

        public void ChangeTile(Vector2 realpos, ushort into)
        {
            int a = TwoDToOneDArray(realpos);
            level.tiles[a] = into;
        }

        public int TwoDToOneDArray(Vector2 smallPos)
        {
            smallPos = getTruncLevel(smallPos);
            int pos = (int)(smallPos.X + (level.mapSizeX * smallPos.Y));
            return pos;
        }

        protected override void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            buttons.Clear();
            mousestate = Mouse.GetState();
            mouseposition = new Vector2(mousestate.X, mousestate.Y);

            float width = VirtScreen.Width / 2, height = VirtScreen.Height / 2;

            switch (gameMode)
            {
                case GAME_MODE.INTRO:

                    if (introDelay <= 0)
                    {
                        if (!introShow)
                        {
                            PlaySound(2);
                            introShow = true;
                        }
                        introTimer -= deltaTime;
                    }
                    else
                    {
                        introDelay -= deltaTime;
                    }

                    if (introTimer <= 0)
                    {
                        BG_COLOUR = Color.White;
                        gameMode = GAME_MODE.MENU;
                    }
                    break;

                case GAME_MODE.GAME:

                    if (isPaused)
                    {
                        if (s_gui.IfButton(new Rectangle(20, 50, 70, 20), "Yes"))
                        {
                            isPaused = false;
                            objects.Clear();
                            pl = null;
                            gameMode = GAME_MODE.MENU;
                        }
                        if (s_gui.IfButton(new Rectangle(20, 80, 120, 20), "No"))
                        {
                            isPaused = false;
                        }
                    }
                    else
                    {
                        if (pl != null)
                        {
                            if (pl.position != pl.spawnpoint)
                            {
                                if (currentKeyboardState.IsKeyDown(Keys.R))
                                {
                                    ResetLevel();
                                }
                            }

                            pl.throwMultiplier = CalculateLine(scrnMsPos, pl.position - campos, gameTime) / 24;
                        }

                        for (int i = 0; i < objects.Count; i++)
                        {
                            s_object obje = objects[i];
                            if (obje != null)
                            {
                                obje.contact_points.Clear();
                                obje.Update(gameTime);
                                obje.ForceCollisionBoxUpdate();
                            }
                        }
                        if (pl != null)
                            player_onScreenPosition = campos - pl.position;

                        if (currentKeyboardState.IsKeyDown(Keys.Q))
                        {
                            isPaused = true;
                        }
                    }
                    break;

                case GAME_MODE.INSTRUCTIONS:

                    if (s_gui.IfButton(new Rectangle(20, 150, 120, 20), "Back"))
                    {
                        gameMode = GAME_MODE.MENU;
                    }
                    break;

                case GAME_MODE.MENU:
                    if (s_gui.IfButton(new Rectangle(20,80, 70, 20), "Start"))
                    {
                        LoadLevel(level);
                        gameMode = GAME_MODE.GAME;
                    }
                    if (s_gui.IfButton(new Rectangle(20, 110, 120, 20), "Level Select"))
                    {
                        gameMode = GAME_MODE.LEVEL_SELECT;
                    }
                    if (s_gui.IfButton(new Rectangle(20, 130, 120, 20), "Instructions"))
                    {
                        gameMode = GAME_MODE.INSTRUCTIONS;
                    }
                    break;

                case GAME_MODE.LEVEL_SELECT:

                    if (s_gui.IfButton(new Rectangle(0, 0, 80, 20), "Back"))
                    {
                        gameMode = GAME_MODE.MENU;
                    }
                    int maxLimit = 4;
                    int y = 1;
                    int x = 0;
                    for (int i = 0; i < 16; i++)
                    {
                        if (x % maxLimit == 0)
                        {
                            y++;
                            x = 0;
                        }
                        switch (levelPageNumber)
                        {
                            case 0:

                                if (s_gui.IfButton(new Rectangle(90 * x, 30 * y, 70, 20), "Level " + (i + 1)))
                                {
                                    currentLevel = i;
                                    level = levels[currentLevel];
                                    LoadLevel(level);
                                    gameMode = GAME_MODE.GAME;
                                }
                                if (s_gui.IfButton(new Rectangle(115, 170, 30, 20), ">"))
                                {
                                    levelPageNumber = 1;
                                    buttons.Clear();
                                    return;
                                }
                                break;

                            case 1:

                                if (i + 17 < levels.Length + 1)
                                {
                                    if (s_gui.IfButton(new Rectangle(90 * x, 30 * y, 70, 20), "Level " + (i + 17)))
                                    {
                                        currentLevel = i + 16;
                                        level = levels[currentLevel];
                                        LoadLevel(level);
                                        gameMode = GAME_MODE.GAME;
                                    }
                                }

                                if (s_gui.IfButton(new Rectangle(155, 170, 30, 20), "<"))
                                {
                                    levelPageNumber = 0;
                                    buttons.Clear();
                                    return;
                                }
                                break;
                        }
                        x++;
                    }
                    if (enableDebug)
                    {
                        if (s_gui.IfButton(new Rectangle(150, 170, 80, 20), "Play Ending "))
                        {
                            gameMode = GAME_MODE.ENDING;
                        }
                    }
                    break;

                case GAME_MODE.ENDING:

                    if (s_gui.IfButton(new Rectangle(150, 170, 80, 20), "Back"))
                    {
                        gameMode = GAME_MODE.MENU;
                    }
                    break;
            }

            if (enableDebug)
            {
                if (currentKeyboardState.IsKeyDown(Keys.Tab) && !previousKeyboardState.IsKeyDown(Keys.Tab))
                    debuginfo = debuginfo ? false : true;
            }

            if (pl != null)
            {
                if (debuginfo)
                    pl.isdebug = true;
                else
                    pl.isdebug = false;
            }

            if (pl != null)
            {
                pl.ForceCollisionBoxUpdate();
                campos = pl.position - new Vector2(centreOfScreen.X, centreOfScreen.Y);
                campos.X = MathHelper.Clamp(campos.X,0 , (level.mapSizeX - 19.15f) * level.tileSizeX);
                campos.Y = MathHelper.Clamp(campos.Y, 0, (level.mapSizeY - 10.2f) * level.tileSizeY);
            }
            else
                campos = new Vector2(0, 0) - new Vector2(centreOfScreen.X, centreOfScreen.Y);
            scrnMsPos = ScreenToVirtualScreen(mouseposition);
            mouseposition2 = scrnMsPos - campos - centreOfScreen;

            prevmousestate = mousestate;
            previousKeyboardState = Keyboard.GetState();

            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            base.Update(gameTime);
        }
        
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.SetRenderTarget(VirtScreen);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            GraphicsDevice.Clear(BG_COLOUR);
            switch (gameMode)
            {
                case GAME_MODE.INTRO:
                    BG_COLOUR = Color.Black;
                    if (introDelay <= 0)
                    {
                        spriteBatch.Draw(logoImg, new Rectangle((int)(screenSize.X / 3), (int)(screenSize.Y / 4), logoImg.Width, logoImg.Height), Color.White);
                    }
                    break;

                case GAME_MODE.MENU:
                    spriteBatch.Draw(titleImg, new Rectangle((int)(screenSize.X / 4), -50, titleImg.Width, titleImg.Height), Color.White);

                    DrawText("Pixel Brownie Software 2019", font, new Vector2(0, 180), spriteBatch);
                    break;

                case GAME_MODE.LEVEL_SELECT:
                    switch (levelPageNumber)
                    {
                        case 0:
                            DrawText("Original", font, new Vector2(0, 40), spriteBatch);
                            break;

                        case 1:
                            DrawText("Episode 1", font, new Vector2(0, 40), spriteBatch);
                            break;
                    }
                    break;

                case GAME_MODE.INSTRUCTIONS:
                    DrawText("WASD Keys - Move", font, new Vector2(20, 40), spriteBatch);
                    DrawText("Left click - Throw", font, new Vector2(20, 60), spriteBatch);
                    DrawText("Space - High jump", font, new Vector2(20, 80), spriteBatch);
                    DrawText("WASD + Space - Small jump", font, new Vector2(20, 100), spriteBatch);
                    DrawText("Shift - Swim faster", font, new Vector2(20, 120), spriteBatch);
                    break;

                case GAME_MODE.GAME:

                    int ind = 0;
                    for (int i = 0; i < objects.Count; i++)
                    {
                        s_object obje = objects[i];
                        if (obje == null)
                            continue;
                        float hyp = HypotenuseVector(obje.position - (campos + centreOfScreen));

                        if (hyp > 240)
                            continue;

                        if (obje != pl)  //Player gets drawn first
                            obje.renderer.Draw(spriteBatch, campos);

                        if (debuginfo)
                        {
                            DrawText(obje.DrawDebugInfo(), font, obje.position - campos + new Vector2(-20, -20), spriteBatch);
                            if (obje != null && obje.name == "bomb" || obje.name == "spikes" || obje.name == "player" || obje.name == "bouncer")
                                spriteBatch.Draw(blank, new Rectangle(obje.collisionBox.Location - new Point(0, 0) - campos.ToPoint(), obje.collisionBox.Size), new Color(99, 42, 42));
                        }

                    }
                    if (pl != null)
                        pl.renderer.Draw(spriteBatch, campos);

                    for (int i = 0; i < objects.Count; i++)
                    {
                        s_object obje = objects[i];
                        if (obje == null)
                            continue;
                        float hyp = HypotenuseVector(obje.position - (campos + centreOfScreen));
                        if (hyp > 240)
                            continue;

                        if (debuginfo)
                        {
                            DrawText(obje.DrawDebugInfo(), font, obje.position - campos + new Vector2(-20, -20), spriteBatch);
                        }

                        DrawText(obje.DrawInfo(), font, obje.position - campos + new Vector2(-20, -20), spriteBatch);
                    }
                    if (debuginfo)
                    {
                        if (!isPaused)
                            if (pl != null)
                            {
                                if (pl.position.Y < 3 * level.tileSizeY)
                                {
                                    DrawText("Map size: " + new Vector2(level.mapSizeX, level.mapSizeY), font, new Vector2(0, 3), spriteBatch, 90);
                                    DrawText("Trunc position: " + getTruncLevel(pl.centre), font, new Vector2(0, 13), spriteBatch, 90);
                                    DrawText("Tile: " + GetTile(pl.position), font, new Vector2(0, 23), spriteBatch, 90);
                                    DrawText("Velocity: " + pl.velocity, font, new Vector2(0, 33), spriteBatch, 90);
                                    DrawText("1D arrya position: " + TwoDToOneDArray(pl.centre) + "/" + level.tiles.Length, font, new Vector2(0, 43), spriteBatch, 90);
                                    DrawText("Level height: " + (level.mapSizeY + 3) * level.tileSizeY, font, new Vector2(0, 55), spriteBatch, 90);

                                    DrawText("Throwing power: " + HypotenuseVector(pl.throwDir * pl.throwMultiplier), font, new Vector2(0, 65), spriteBatch, 90);
                                    DrawText("Player Position: " + new Vector2((int)pl.position.X, (int)pl.position.Y), font, new Vector2(0, 65), spriteBatch, 90);

                                }
                                else
                                {
                                    DrawText("Map size: " + new Vector2(level.mapSizeX, level.mapSizeY), font, new Vector2(0, 3), spriteBatch);
                                    DrawText("Trunc position: " + getTruncLevel(pl.centre), font, new Vector2(0, 13), spriteBatch);
                                    DrawText("Tile: " + GetTile(pl.position), font, new Vector2(0, 23), spriteBatch);
                                    DrawText("Velocity: " + pl.velocity, font, new Vector2(0, 33), spriteBatch);
                                    DrawText("1D arrya position: " + TwoDToOneDArray(pl.centre) + "/" + level.tiles.Length, font, new Vector2(0, 43), spriteBatch);
                                    DrawText("Level height: " + (level.mapSizeY + 3) * level.tileSizeY, font, new Vector2(0, 55), spriteBatch);

                                    DrawText("Throwing power: " + HypotenuseVector(pl.throwDir * pl.throwMultiplier), font, new Vector2(0, 65), spriteBatch);
                                    DrawText("Player Position: " + new Vector2((int)pl.position.X, (int)pl.position.Y), font, new Vector2(0, 65), spriteBatch);
                                }
                            }
                    }
                    else
                    {
                        if (!isPaused)
                            if (pl != null)
                            {
                                if (pl.position.Y < 3 * level.tileSizeY)
                                {
                                    DrawText("Level " + (currentLevel + 1), font, new Vector2(0, 0), spriteBatch, 90);
                                    DrawText("Item: " + pl.heldItem.ToString(), font, new Vector2(0, 10), spriteBatch, 90);
                                    DrawText("Press R to restart Level", font, new Vector2(0, 20), spriteBatch, 90);
                                    DrawText("Press Q to pause", font, new Vector2(0, 30), spriteBatch, 90);
                                }
                                else
                                {
                                    DrawText("Level " + (currentLevel + 1), font, new Vector2(0, 0), spriteBatch);
                                    DrawText("Item: " + pl.heldItem.ToString(), font, new Vector2(0, 10), spriteBatch);
                                    DrawText("Press R to restart Level", font, new Vector2(0, 20), spriteBatch);
                                    DrawText("Press Q to pause", font, new Vector2(0, 30), spriteBatch);
                                }
                            }
                        //DrawText("Press R to restart Level" + HypotenuseVector(pl.throwDir * pl.throwMultiplier), font, new Vector2(0, 20), spriteBatch);
                    }
                    if (pl != null)
                        if (pl.heldItem != o_item.ITEM_TY.None)
                            DrawThrowDirection(gameTime, pl.throwMultiplier);

                    if (isPaused)
                        DrawText("Back to title screen?", font, new Vector2(20, 30), spriteBatch);
                    break;

                case GAME_MODE.ENDING:
                    spriteBatch.Draw(endImg, new Rectangle(0, 0, endImg.Width, endImg.Height), Color.White);
                    string endingTxt = "Brownie Systems Inc\n\n" +
                        "SUBJECT: Annie Fedorov\n\n" +
                        "DATE: 15/09/20XX.\n\n" +
                        "REPORT: Finished chamb\n" +
                        "er challenges.\n\n";

                    switch (endingNumber)
                    {
                        case 0:
                            endingTxt = "Brownie Systems Inc\n\n" +
                        "SUBJECT: Annie Fedorov\n\n" +
                        "DATE: 15/09/20XX.\n\n" +
                        "REPORT: Finished chamb\n" +
                        "er challenges.\n\n";
                            break;

                        case 1:
                            endingTxt = "Brownie Systems Inc\n\n" +
                        "SUBJECT: Annie Fedorov\n\n" +
                        "DATE: 20/10/20XX.\n\n" +
                        "REPORT: Finished 2nd \nchamb" +
                        "er challenges.\n\n";
                            break;
                    }
                    DrawText(endingTxt, font, new Vector2(71, 55), spriteBatch);

                    DrawText(
                        "STATUS: Awaiting furth\ner instruction.\n", font, new Vector2(71, 140), spriteBatch);
                    break;
            }
            foreach (Tuple<Rectangle, string> rect in buttons)
            {
                //spriteBatch.Draw(blank, rect.Item1, Color.White);
                Vector2 v = rect.Item1.Location.ToVector2();
                DrawText(rect.Item2, font, v, spriteBatch);
            }


            //DrawLine(new Vector2(1,0), new Vector2(Penetration1.Item1,15), Penetration1.Item2 / 7 , gameTime, spriteBatch);
            //DrawLine(new Vector2(1, 0), new Vector2(Penetration2.Item1, 19), Penetration2.Item2 / 7, gameTime, spriteBatch);
            foreach (Vector2 v in axis)
            {
                //DrawLine(v, new Vector2(15, 17), 40, gameTime, spriteBatch);
            }

           // DrawDebugPenetration(gameTime, spriteBatch,DebugPoints());
           
            spriteBatch.Draw(blank, new Rectangle(scrnMsPos.ToPoint() + new Point(-2,-2), new Point(4, 4)), Color.White);

            GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Draw(VirtScreen, new Rectangle(0, 0, VirtScreen.Width, VirtScreen.Height), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        Tuple<List<Tuple<float, float>>, List<Tuple<float, float>>> DebugPoints()
        {
            List<Tuple<float, float>> pts = new List<Tuple<float, float>>();
            List<Tuple<float, float>> pts2 = new List<Tuple<float, float>>();
            foreach (Vector2 v in axis)
            {
                pts.Add(shape.GetIntersectAxis(v));
                pts2.Add(shape2.GetIntersectAxis(v));
            }
            return new Tuple<List<Tuple<float, float>>, List<Tuple<float, float>>>(pts, pts2);
        }

        void DrawDebugPenetration(GameTime gt, SpriteBatch sb, Tuple<List<Tuple<float, float>>, List<Tuple<float, float>>> fl)
        {
            List<Tuple<float, float>> pts = fl.Item1;
            List<Tuple<float, float>> pts2 = fl.Item2;
            float st = 150;
            float leng = 2;

            for (int i = 0; i < pts2.Count; i++)
            {
                Tuple<float, float> pt = pts2[i];
                DrawLine(
                    new Vector2(pt.Item1, st + (leng * i)),
                    new Vector2(pt.Item2, st + (leng * i)),
                    gt,
                    Color.Red);
            }
            for (int i = 0; i < pts.Count; i++)
            {
                Tuple<float, float> pt = pts[i];
                DrawLine(
                    new Vector2(pt.Item1, st + (leng * i)),
                    new Vector2(pt.Item2, st + (leng * i)),
                    gt,
                    Color.Blue);
            }

        }

    }
}
