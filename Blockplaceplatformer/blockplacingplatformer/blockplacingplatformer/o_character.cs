using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace blockplacingplatformer
{
    using anim_size_offset = List<Tuple<Vector2, Vector2>>;
    public class o_block: s_object
    {
        int rand = 0;
        public enum BLOCK_TYPE
        {
            BLOCK,
            BREAKABLE,
            DOOR,
            SWITCH,
            SPIKES,
            TRAP_DOOR,
            SWITCH_DOOR,
            BOMBER,
            BOUNCER,
            PORTAL
        }
        public Vector2 direction;
        public float max_bomb_timer = 0f;
        float bomb_timer = 0f;
        public string flag = "";
        public BLOCK_TYPE TYPEOFBLOCK;
        public o_item bomb_ItemToFocusOn = null;
        s_animhandler anima = new s_animhandler();
        public ushort TileNum = 0;  //The tile that this block is under

        public o_block()
        {
            anima = new s_animhandler();
            s_anim animjump = new s_anim("spring_bounce");
            animjump.AddAnimation(0, 0.1f);
            animjump.AddAnimation(1, 0.1f);
            animjump.AddAnimation(2, 0.1f);
            animjump.AddAnimation(3, 0.1f);
            anima.AddAnimation(animjump);

            s_anim animidle = new s_anim("spring_idle");
            animidle.AddAnimation(3, 0.1f);
            animidle.AddAnimation(3, 0.1f);
            anima.AddAnimation(animidle);
            anima.currentFrameNumber = 3;
        }
        public o_block(Vector2 _position) : base(_position)
        {
            Random ran = new Random();
            rand = ran.Next(0,3);
        }

        public void OnBounce()
        {
            Game1.PlaySound(9, position);
            anima.SetAnimation("spring_idle", false);
            anima.SetAnimation("spring_bounce", false);
        }

        public override string DrawDebugInfo()
        {
            if (flag != "")
                return "Flag: " + flag;
            else
                return "";
        }

        public void ButtonChange()
        {
            o_block[] bl = Game1.FindCharacters<o_block>(flag);
            if (bl != null)
            {
                Game1.PlaySound(8);
                foreach (o_block b in bl)
                {
                    if (b.TYPEOFBLOCK == BLOCK_TYPE.DOOR || b.TYPEOFBLOCK == BLOCK_TYPE.SWITCH_DOOR)
                    {
                        b.flag = "0";
                    }
                }
            }
            renderer.animNum = 1;
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
            anima.Update(gametime);
            renderer.animNum = anima.currentFrameNumber;
            o_platformControler contact = null;
            switch (TYPEOFBLOCK)
            {
                case BLOCK_TYPE.DOOR:
                    if (flag == "0")
                    {
                        renderer.animNum = 1;
                        if (IntersectBox<o_plcharacter>(new Vector2(0, 0)) != null)
                        {
                            Game1.game.IncrementLevel();
                        }
                    }
                    else
                    {
                        renderer.animNum = 0;
                    }
                    break;

                case BLOCK_TYPE.PORTAL:
                    contact = IntersectBox<o_platformControler>(new Vector2(0, 0));
                    if (contact != null)
                    {
                        o_block f = Game1.FindCharacter<o_block>(flag);
                        contact.position = f.position + new Vector2(20,0);
                    }
                    break;

                case BLOCK_TYPE.SWITCH_DOOR:

                    if (flag == "0")
                    {
                        Game1.game.ChangeTile(position, 0);
                           Game1.RemoveObject(this);
                    }
                    break;

                case BLOCK_TYPE.BOMBER:
                    
                    if (bomb_timer <= 0)
                    {
                        if (bomb_ItemToFocusOn == null)
                        {
                            o_item it = Game1.AddObject<o_item>(position + new Vector2(0, 20), "bomb", Game1.textures["bomb"].Item2, Game1.textures["bomb"].Item1);
                            if (it != null)
                            {
                                it.itType = o_item.ITEM_TY.Bomb;
                                it.iscatchable = false;
                                it.velocityLimiter = 0.97f;
                                it.Throw(direction);
                                it.bombTimer = 2.5f;
                                it.collisionOffset = new Vector2(6, 0);
                                it.collisionBox.Width = 12;
                                bomb_ItemToFocusOn = it;
                                it.parent = this;
                                bomb_timer = max_bomb_timer;
                            }
                        }
                    }
                    else
                        if (bomb_ItemToFocusOn == null)
                            bomb_timer -= Game1.deltaTime;
                    break;
            }
        }

    }
    
    public class o_plcharacter : o_platformControler
    {
        public o_item.ITEM_TY heldItem = o_item.ITEM_TY.None;
        public Vector2 itemSpawn { get; set; }
        bool fallkeyPressed = false;

        public Vector2 throwDir;
        public float throwMultiplier = 1;
        float throwDel = 0.5f;
        int itemCount = 0;
        float speed = 1.75f;
        const float speedOrigin = 1.75f;
        s_shape shape;
        public float oyxgenMetre = 1f;
        s_animhandler anima = new s_animhandler();


        public o_plcharacter()
        {
            isdebug = false;
            anima = new s_animhandler();
            renderer.CentreOffset = new Point(-3, 0);

            s_anim animjump = new s_anim("jump");
            animjump.AddAnimation(2, 0.0f);
            animjump.AddAnimation(2, 0.0f);
            anima.AddAnimation(animjump);

            s_anim animidle = new s_anim("idle");
            animidle.AddAnimation(0, 0.4f);
            anima.AddAnimation(animidle);

            s_anim aniwalk = new s_anim("walk");
            aniwalk.AddAnimation(0, 0.07f);
            aniwalk.AddAnimation(0, 0.0f, s_anim.ANIM_TYPE.SOUND);
            aniwalk.AddAnimation(1, 0.07f);
            anima.AddAnimation(aniwalk);
        }
        public o_plcharacter(Vector2 _position) : base(_position)
        {
        }
        public o_plcharacter(Vector2 _position, float velocityLimiter) : base(_position)
        {
            this.velocityLimiter = velocityLimiter;
        }

        public override string DrawInfo()
        {
            if (oyxgenMetre < 1)
            {
                return "Air: " + Math.Round(oyxgenMetre * 100) + "%";
            }

            return "";
        }

        public override void Update(GameTime gametime)
        {
            anima.Update(gametime);
            ushort plTile = Game1.game.GetTile(collisionBox.Center.ToVector2());

            oyxgenMetre = MathHelper.Clamp(oyxgenMetre, 0,0.99f);

            if (!Keyboard.GetState().IsKeyDown(Keys.S))
                if(fallkeyPressed)
                    fallkeyPressed = false;

            switch (STATE)
            {
                case CHARA_STATE.MOVING:
                    //isdebug = false;
                    if (isgrounded)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
                            anima.SetAnimation("walk", true);
                        else
                            anima.SetAnimation("idle", false);
                    }
                    else
                        anima.SetAnimation("jump", false);

                    oyxgenMetre += 0.01f;
                    fallVel = gravity;

                    if (isgrounded)
                        speed = speedOrigin;

                    if (Game1.IsSameLayer(plTile, "liquid"))
                    {
                        speed = speedOrigin;
                        Game1.PlaySound(1);
                        STATE = CHARA_STATE.SWIMMING;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        renderer.spriteRotation = SpriteEffects.None;
                        velocity.X = speed;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        renderer.spriteRotation = SpriteEffects.FlipHorizontally;
                        velocity.X = -speed;
                    }

                    if (isdebug)
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.S))
                        {
                            velocity.Y = speed;
                        }
                        if (Keyboard.GetState().IsKeyDown(Keys.W))
                        {
                            velocity.Y = -speed;
                        }
                    }
                    else
                    {
                        if (Keyboard.GetState().IsKeyDown(Keys.Space))
                        {
                            if (isgrounded == true)
                            {
                                anima.SetAnimation("walk", false);
                                position.Y -= 3;
                                if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.D))
                                {
                                    Game1.PlaySound(6);
                                    velocity.Y = -2.60f;
                                }
                                else
                                {
                                    Game1.PlaySound(6);
                                    speed = speedOrigin / 1.4f;
                                    velocity.Y = -3.31f;
                                }
                            }
                        }
                    }
                    break;

                case CHARA_STATE.SWIMMING:
                    //isdebug = true;
                    oyxgenMetre -= 0.0005f;
                    velocity.Y *= velocityLimiter;
                    fallVel = 0.04f;

                    anima.SetAnimation("jump", false);
                    if (Keyboard.GetState().IsKeyDown(Keys.W))
                    {
                        if (Game1.IsSameLayer(plTile, "air"))
                            velocity.Y = -speed * 1.35f;
                        else
                            velocity.Y = -speed;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.LeftShift))
                    {
                        speed = speedOrigin * 0.93f;
                        oyxgenMetre -= 0.0007f;
                    }
                    else
                        speed = speedOrigin / 2.5f;

                    if (Game1.IsSameLayer(plTile, "air"))
                    {
                        Game1.PlaySound(10, collisionBox.Location.ToVector2());
                        STATE = CHARA_STATE.MOVING;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.D))
                    {
                        renderer.spriteRotation = SpriteEffects.None;
                        velocity.X = speed;
                    }

                    if (Keyboard.GetState().IsKeyDown(Keys.A))
                    {
                        renderer.spriteRotation = SpriteEffects.FlipHorizontally;
                        velocity.X = -speed;
                    }
                    
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                        velocity.Y = speed;

                    if (oyxgenMetre <= 0.00)
                        Game1.game.ResetLevel();

                    break;
            }

            o_block detectedSpike = IntersectBox<o_block>(new Vector2(0, 0), x => x.TYPEOFBLOCK == o_block.BLOCK_TYPE.SPIKES);

            if (detectedSpike != null)
            {
                position.Y = spawnpoint.Y - 1;
                position.X = spawnpoint.X;
                Game1.PlaySound(3);
                velocity = Vector2.Zero;
            }
            if (throwDel > 0)
                throwDel -= Game1.deltaTime;
            
            o_item item = CheckPointEnt<o_item>(centre);
            if (item != null)
            {
                if (item.catchDel > 0)
                {
                    if (heldItem == o_item.ITEM_TY.None)
                    {
                        itemSpawn = item.spawnpoint;
                        heldItem = item.itType;
                        Game1.RemoveObject(item);
                    }
                }
            }
            
            if (Game1.mousestate.LeftButton == ButtonState.Pressed)
            {
                if (heldItem != o_item.ITEM_TY.None)
                {
                    o_item it = null;
                    switch (heldItem)
                    {
                        case o_item.ITEM_TY.Bomb:
                            it = Game1.AddObject<o_item>(centre + new Vector2(0, -15), "bomb", Game1.textures["bomb"].Item2, Game1.textures["bomb"].Item1);
                            break;

                        case o_item.ITEM_TY.Throwing:
                            it = Game1.AddObject<o_item>(centre + new Vector2(0, -15), "Thing", Game1.textures["ball"].Item2, Game1.textures["ball"].Item1);
                            break;
                    }
                    it.catchDel = 0.2f;
                    if (it != null)
                    {
                        it.spawnpoint = itemSpawn;
                        it.itType = heldItem;
                        it.velocityLimiter = 0.97f;
                        it.Throw(new Vector2(throwDir.X * throwMultiplier, throwDir.Y * throwMultiplier));
                        heldItem = o_item.ITEM_TY.None;
                    }
                }
            }
            renderer.animNum = anima.currentFrameNumber;
            base.Update(gametime);
        }

        public override void TouchingGround(ushort b)
        {
            if (b == 5)
                if(!fallkeyPressed)
                    if (Keyboard.GetState().IsKeyDown(Keys.S))
                    {
                        fallkeyPressed = true;
                        velocity.Y = 0;
                        position.Y += 20;
                        position.Y += 20;
                    }
        }
    }

    public class o_platformControler : s_object
    {
        public bool isgrounded;
        public float velocityLimiter = 0.85f;
        public float fallVel = 0.1f;
        public const float gravity = 0.1f;

        public int height = 1;

        public bool isdebug = false;
        public Vector2 spawnpoint;
        public enum CHARA_STATE
        {
            MOVING,
            SWIMMING
        }
        protected CHARA_STATE STATE = CHARA_STATE.MOVING;

        public o_platformControler()
        {
        }
        public o_platformControler(Vector2 _position) : base(_position)
        {
        }
        
        public o_platformControler(Vector2 _position, float velocityLimiter) : base(_position)
        {
            this.velocityLimiter = velocityLimiter;
        }

        public virtual void OnContact(o_block obj)
        {

        }
        public virtual void TouchingGround(ushort blocktype)
        {

        }

        public virtual void OnGround()
        {

        }
        public virtual void OnGround(ushort blocktype)
        {

        }

        public void CollisionDetectionY()
        {
            ForceCollisionBoxUpdate();
            Vector2 f = collisionBox.Location.ToVector2();
            ushort topR = Game1.game.GetTile(f + new Vector2(0, -1));
            ushort topL = Game1.game.GetTile(f + new Vector2(collisionBox.Width - 1, -1));

            ushort bottomR = Game1.game.GetTile(f + new Vector2(0, collisionBox.Height));
            ushort bottomL = Game1.game.GetTile(f + new Vector2(collisionBox.Width - 1, collisionBox.Height));

            ushort midR = Game1.game.GetTile(f + new Vector2(-1, collisionBox.Height/2));
            ushort midL = Game1.game.GetTile(f + new Vector2(collisionBox.Width + 1, collisionBox.Height/2));

            ushort leftT = Game1.game.GetTile(f + new Vector2(collisionBox.Width, collisionBox.Height - 2));
            ushort leftB = Game1.game.GetTile(f + new Vector2(collisionBox.Width, 0));

            ushort rightT = Game1.game.GetTile(f + new Vector2(-1, collisionBox.Height - 2));
            ushort rightB = Game1.game.GetTile(f + new Vector2(-1, 1));
            
            //Down
            CheckPointEnt<o_block>(new Vector2(0, collisionBox.Height), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, collisionBox.Height), x => x.issolid);

            CheckPointEnt<o_block>(new Vector2(0, collisionBox.Height/2), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, collisionBox.Height/2), x => x.issolid);
            //Up
            CheckPointEnt<o_block>(new Vector2(0, -1), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, -1), x => x.issolid);

            //Left
            CheckPointEnt<o_block>(new Vector2(-1, 0), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(-1, collisionBox.Height - 1), x => x.issolid);

            CheckPointEnt<o_block>(new Vector2(collisionBox.Width, 0), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width, collisionBox.Height - 1), x => x.issolid);

            if (Game1.IsSameLayer(bottomL, "solid") || Game1.IsSameLayer(bottomR, "solid") || 
                Game1.IsSameLayer(bottomL, "solidFall") || Game1.IsSameLayer(bottomR, "solidFall"))
            {
                if (Game1.IsSameLayer(bottomL, "solidFall") && Game1.IsSameLayer(bottomR, "solidFall"))
                    TouchingGround(bottomR);
                if (velocity.Y > 0)
                {
                    Vector2 v = Game1.game.getTruncLevel(centre + new Vector2(0, collisionBox.Height)) - new Vector2(0, height);
                    velocity.Y = 0;
                    v.Y *= Game1.game.level.tileSizeY ;
                    position.Y = v.Y - (collisionBox.Height - Game1.game.level.tileSizeY);
                    isgrounded = true;
                    OnGround();
                    if (STATE != CHARA_STATE.SWIMMING)
                        Game1.PlaySound(5, position);
                    return;
                }
            }
            else
            {
                isgrounded = false;
            }
            if (Game1.IsSameLayer(topR, "solid") || Game1.IsSameLayer(topL, "solid"))
            {
                if (velocity.Y < 0)
                {
                    velocity.Y = 0;
                    //Vector2 v = Game1.game.getTruncLevel(collisionBox.Location.ToVector2() - new Vector2(0, collisionBox.Height/2)) - new Vector2(0, height);
                    Vector2 v = Game1.game.getTruncLevel(position - new Vector2(0,1));// + new Vector2(0, height/2);
                    v.Y *= Game1.game.level.tileSizeY;
                    //position.Y = v.Y; + collisionBox.Height
                    position.Y = v.Y + Game1.game.level.tileSizeY;
                    return;
                }
            }
        }

        public void CollisionDetectionX()
        {
            ForceCollisionBoxUpdate();
            Vector2 f = collisionBox.Location.ToVector2();
            ushort topR = Game1.game.GetTile(f + new Vector2(0, -1));
            ushort topL = Game1.game.GetTile(f + new Vector2(collisionBox.Width - 1, -1));

            ushort bottomR = Game1.game.GetTile(f + new Vector2(0, collisionBox.Height));
            ushort bottomL = Game1.game.GetTile(f + new Vector2(collisionBox.Width - 1, collisionBox.Height));

            ushort midR = Game1.game.GetTile(f + new Vector2(-1, collisionBox.Height / 2));
            ushort midL = Game1.game.GetTile(f + new Vector2(collisionBox.Width + 1, collisionBox.Height / 2));

            ushort leftT = Game1.game.GetTile(f + new Vector2(collisionBox.Width, collisionBox.Height - 2));
            ushort leftB = Game1.game.GetTile(f + new Vector2(collisionBox.Width, 0));

            ushort rightT = Game1.game.GetTile(f + new Vector2(-1, collisionBox.Height - 2));
            ushort rightB = Game1.game.GetTile(f + new Vector2(-1, 1));

            //Down
            CheckPointEnt<o_block>(new Vector2(0, collisionBox.Height), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, collisionBox.Height), x => x.issolid);

            CheckPointEnt<o_block>(new Vector2(0, collisionBox.Height / 2), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, collisionBox.Height / 2), x => x.issolid);
            //Up
            CheckPointEnt<o_block>(new Vector2(0, -1), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width - 1, -1), x => x.issolid);

            //Left
            CheckPointEnt<o_block>(new Vector2(-1, 0), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(-1, collisionBox.Height - 1), x => x.issolid);

            CheckPointEnt<o_block>(new Vector2(collisionBox.Width, 0), x => x.issolid);
            CheckPointEnt<o_block>(new Vector2(collisionBox.Width, collisionBox.Height - 1), x => x.issolid);
            
            if (Game1.IsSameLayer(leftT, "solid") || Game1.IsSameLayer(leftB, "solid") || Game1.IsSameLayer(midL, "solid"))
            {
                if (velocity.X > 0)
                {
                    velocity.X = 0;
                    Vector2 v = Game1.game.getTruncLevel(centre + new Vector2(20, 0)) + new Vector2(-1, 0);
                    v.X *= Game1.game.level.tileSizeX;
                    position.X = v.X;
                    return;
                }
            }
            if (Game1.IsSameLayer(rightT, "solid") || Game1.IsSameLayer(rightB, "solid") || Game1.IsSameLayer(midR, "solid"))
            {
                if (velocity.X < 0)
                {
                    velocity.X = 0;
                    Vector2 v = Game1.game.getTruncLevel(centre - new Vector2(20, 0)) + new Vector2(1, 0);
                    v.X *= Game1.game.level.tileSizeX;
                    position.X = v.X;
                    return;
                }
            }
        }
        
        public override void Start()
        {

        }

        public override void Update(GameTime gametime)
        {
            if (isKenematic)
            {
                if (!isdebug)
                {
                    if (!isgrounded)
                        if (velocity.Y < 4.6f)
                            velocity.Y += fallVel;
                    if (position.Y > Game1.game.level.mapSizeY * 20)
                        position = spawnpoint;
                }
                if(isdebug)
                    velocity.Y *= velocityLimiter;
                velocity.X *= velocityLimiter;
                if (Math.Abs(velocity.X) < 0.001f)
                    velocity.X = 0;
                
                ForceCollisionBoxUpdate();
                position.Y += velocity.Y;
                CollisionDetectionY();

                ForceCollisionBoxUpdate();
                position.X += velocity.X;
                CollisionDetectionX();

                o_block bl = IntersectBox<o_block>(new Vector2(0, 0), x => x.TYPEOFBLOCK == o_block.BLOCK_TYPE.BOUNCER);
                if (bl != null) {
                    bl.OnBounce();
                    velocity.Y = 6.1f * -1;
                }

            }
            base.Update(gametime);
        }

    }

    public class o_movableSolid : o_platformControler
    {

        public override void Update(GameTime gametime)
        {
            renderer.colour = Color.LightGreen;
            base.Update(gametime);
        }
}
}
