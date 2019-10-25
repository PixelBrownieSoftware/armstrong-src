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
    public class o_item : o_platformControler
    {
        public enum ITEM_TY
        {
            Throwing,
            BREAKABLE,
            BLOCK,
            Bomb,
            BOMB_INSTA,
            None
        }
        public ITEM_TY itType;
        enum FUSE_STATE
        {
            INACTIVE,
            THROWN,
            BLOWING_UP
        }
        FUSE_STATE fs= FUSE_STATE.INACTIVE;

        float timer = 1.45f;
        public float catchDel = 0.5f;
        public bool iscatchable = true;
        bool isthrown = false;

        public float bombTimer = 3f;
        float explosionTimer = 0.2f;
        public o_block parent; //For the bomber

        const float startCatchDel = 0.5f;

        public o_item()
        {
            catchDel = 0f;
        }
        public o_item(Vector2 _position) : base(_position)
        {
            catchDel = 0.5f;
        }

        public o_item(Vector2 _position, float velocityLimiter) : base(_position, velocityLimiter)
        {
            catchDel = 0.5f;
        }
        public override string DrawDebugInfo()
        {
            /*
            float x = (float)Math.Round(velocity.X, 3);
            float y = (float)Math.Round(velocity.Y, 3);
            return "ThrDir: " + x + ", " + y;
            */
            return "Spawn " + spawnpoint;
        }

        public override void OnContact(o_block obj)
        {
            switch (obj.TYPEOFBLOCK)
            {
                case o_block.BLOCK_TYPE.BREAKABLE:
                    
                    break;
                case o_block.BLOCK_TYPE.SWITCH:

                    obj.ButtonChange();
                    break;
                    
            }
        }

        public void Throw(Vector2 dir)
        {
            fs = FUSE_STATE.THROWN;
            catchDel = 0.5f;
            velocity += dir;
            if (itType == ITEM_TY.Bomb)
            {
                fs = FUSE_STATE.THROWN;
                bombTimer = 1f;
            }
            else {
                renderer.SetTexture(Game1.textures["ball"].Item2,Game1.textures["ball"].Item1);
            }
        }

        public override string DrawInfo()
        {
            switch (itType)
            {
                case ITEM_TY.Bomb:
                    if (parent == null)
                        return "" + Math.Round(bombTimer);
                    else
                        return "";

            }
            return "";
        }

        public override void OnGround()
        {
            if(parent != null)
            {
                Game1.PlaySound(7, position);
                bombTimer = 0.5f;
                fs = FUSE_STATE.BLOWING_UP;
                Destroy();
            }
        }

        public void Destroy()
        {
            collisionOffset = new Vector2(-20, -27);
            isKenematic = false;
            renderer.animNum = 1;
            renderer.ForceUpdate();
            ForceCollisionBoxUpdate();
            collisionBox.Width = 50;
            collisionBox.Height = 50;
            o_block[] blocks = IntersectBoxAll<o_block>(new Vector2(0, 0));
            o_plcharacter play = IntersectBox<o_plcharacter>(new Vector2(0, 0));

            if (blocks != null)
            {
                if (play != null)
                {
                    if (parent != null)
                        parent.bomb_ItemToFocusOn = null;

                    TPPlayer(ref play);

                    //Game1.RemoveObject(play);
                    //Game1.game.ResetLevel();
                    return;
                }
                for (int i = 0; i < blocks.Length; i++)
                {
                    s_object bl = blocks[i];
                    o_block blok = bl.GetComponent<o_block>();
                    

                    if (blok.TYPEOFBLOCK == o_block.BLOCK_TYPE.BREAKABLE)
                    {
                        Game1.game.ChangeTile(bl.position, blok.TileNum);
                        Game1.RemoveObject(bl);
                    }
                }
            }
            if (parent != null)
                parent.bomb_ItemToFocusOn = null;
        }

        /// <summary>
        /// This was originally a bug, but I decided to make this a feature since it would be frustrating to repeat all the puzzles of the level.
        /// </summary>
        /// <param name="play"></param>
        public void TPPlayer(ref o_plcharacter play)
        {
            play.position.Y = play.spawnpoint.Y - 1;
            play.position.X = play.spawnpoint.X;
            Game1.PlaySound(3);
            play.velocity = Vector2.Zero;
        }

        public override void Update(GameTime gametime)
        {
            if (CheckPoint(new Vector2(0, 0)))
            {
                if (catchDel < 0)
                    Game1.RemoveObject(this);
            }

            if (itType == ITEM_TY.Bomb)
            {
                switch (fs)
                {

                    case FUSE_STATE.THROWN:
                        if (parent == null)
                        {
                            bombTimer -= (float)gametime.ElapsedGameTime.TotalSeconds;
                            if (bombTimer <= 0)
                            {
                                isKenematic = false;
                                collisionBox.Size = (new Vector2(20, 20) * 2f).ToPoint();
                                position = new Vector2(collisionBox.Location.X + 10, collisionBox.Location.Y - 10);
                                ForceCollisionBoxUpdate();
                                bombTimer = 0.5f;
                                fs = FUSE_STATE.BLOWING_UP;
                            }
                        }
                        else
                        {
                            o_plcharacter play = IntersectBox<o_plcharacter>(new Vector2(0, 0));
                            if (play != null)
                            {
                                if (parent != null)
                                    parent.bomb_ItemToFocusOn = null;
                                TPPlayer(ref play);
                                return;
                            }
                        }
                        break;

                    case FUSE_STATE.BLOWING_UP:

                        bombTimer -= (float)gametime.ElapsedGameTime.TotalSeconds;
                        if (bombTimer >= 0)
                        {
                            Destroy();
                        }
                        else
                        {
                            Game1.PlaySound(7, position);
                            if (parent != null)
                                parent.bomb_ItemToFocusOn = null;
                            else
                            {
                                o_item it = Game1.AddObject<o_item>(spawnpoint, "bomb", Game1.textures["bomb"].Item2, Game1.textures["bomb"].Item1);
                                if (it != null)
                                {
                                    it.itType = o_item.ITEM_TY.Bomb;
                                    it.spawnpoint = spawnpoint;
                                }
                            }

                            Game1.RemoveObject(this);
                        }
                        break;
                }
            }

            //timer -= (float)gametime.ElapsedGameTime.TotalSeconds;
            if (catchDel > 0)
                catchDel -= (float)gametime.ElapsedGameTime.TotalSeconds;

            o_plcharacter pl = CheckPointEnt<o_plcharacter>(new Vector2(10,10));
            if (pl != null)
            {
                if (catchDel <= 0 && iscatchable)
                {
                    pl.heldItem = itType;
                    pl.itemSpawn = spawnpoint;
                    Game1.RemoveObject(this);
                }
            }

            /*
            if (timer < 0)
                Game1.RemoveObject(this);
             
            o_movableSolid item = IntersectBox<o_movableSolid>(new Vector2(0, 0));
            if (item != null)
            {
                item.velocity.X += velocity.X / 10;
                velocity = Vector2.Zero;
            }
            */

            o_block block = null;
            block = IntersectBox<o_block>(new Vector2(0, 0));

            if (block != null)
            {
                switch (block.TYPEOFBLOCK)
                {

                    case o_block.BLOCK_TYPE.SWITCH:

                        block.ButtonChange();
                        Game1.RemoveObject(this);
                        break;
                    case o_block.BLOCK_TYPE.SPIKES:

                        if (itType != ITEM_TY.Bomb)
                        {
                            position = spawnpoint;
                            velocity = Vector2.Zero;
                        }
                        else
                            Destroy();
                        break;
                }
            }

            base.Update(gametime);
        }
    }
}
