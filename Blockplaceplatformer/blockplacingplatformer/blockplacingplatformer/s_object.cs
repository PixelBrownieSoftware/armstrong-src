using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace blockplacingplatformer
{
    using anim_size_offset = List<Tuple<Vector2, Vector2>>;

    public abstract class s_generic
    {
        public virtual void Start()
        {

        }

        public virtual void Update(GameTime gametime)
        {

        }
    }

    public class s_spriteSheet
    {
        public anim_size_offset sprites = new anim_size_offset();
    }

    public class s_spriterend : s_generic
    {
        public SpriteEffects spriteRotation;
        public Texture2D texture;
        public Point spriteOffset = new Point(0,0);
        public Point spriteSize = new Point(0, 0);
        public Point CentreOffset = new Point(0,0);

        Rectangle pos_size;
        Rectangle sprite_rect;
        public Color colour;
        s_object host;

        Texture2D blank;

        public int animNum = 0;
        List<Tuple<Vector2, Vector2>> offsetsAndSizes = new List<Tuple<Vector2, Vector2>>();

        public s_spriterend(s_object _host)
        {
            blank = Game1.blank;
            colour = Color.White;
            host = _host;
        }

        public void ForceUpdate()
        {
            if (offsetsAndSizes.Count > animNum)
                if (offsetsAndSizes[animNum] != null)
                {
                    spriteSize = offsetsAndSizes[animNum].Item2.ToPoint();
                    sprite_rect = new Rectangle(
                                   offsetsAndSizes[animNum].Item1.ToPoint(),
                                   offsetsAndSizes[animNum].Item2.ToPoint());
                }
        }

        public void SetSpriteOffsetsAndSizes(List<Tuple<Vector2, Vector2>> offsetsAndSizes)
        {
            this.offsetsAndSizes = offsetsAndSizes;
        }

        public void SetTexture(anim_size_offset ani, Texture2D tex)
        {
            texture = tex;
        }

        public void Draw(SpriteBatch sb, Vector2 campos, Color col)
        {
            spriteSize = offsetsAndSizes[animNum].Item2.ToPoint();
            pos_size = new Rectangle(
                    (int)(host.position.X + host.collisionOffset.X) - (int)campos.X,
                    (int)(host.position.Y + host.collisionOffset.Y) - (int)campos.Y,
                    spriteSize.X,
                    spriteSize.Y);

            sprite_rect = new Rectangle(
                    offsetsAndSizes[animNum].Item1.ToPoint(),
                    offsetsAndSizes[animNum].Item2.ToPoint());
            /*
             sprite_rect = new Rectangle(
                     new Point(0, 0),
                     new Point(20,20)); 
             */
            if (texture != null)
                sb.Draw(texture,
                    new Rectangle(pos_size.X + CentreOffset.X, pos_size.Y + CentreOffset.Y, pos_size.Width, pos_size.Height),
                    new Rectangle(sprite_rect.X + CentreOffset.X, sprite_rect.Y + CentreOffset.Y, sprite_rect.Width, sprite_rect.Height), 
                    colour);
            else
                sb.Draw(blank, new Rectangle(pos_size.X, pos_size.Y, pos_size.Width, pos_size.Height), sprite_rect, colour);
        }
        public void Draw(SpriteBatch sb, Vector2 campos)
        {
            if (animNum > offsetsAndSizes.Count - 1)
                animNum = offsetsAndSizes.Count - 1;

            if (offsetsAndSizes.Count > 0)
                spriteSize = offsetsAndSizes[animNum].Item2.ToPoint();
            pos_size = new Rectangle(
                    (int)(host.position.X + host.collisionOffset.X) - (int)campos.X,
                    (int)(host.position.Y + host.collisionOffset.Y) - (int)campos.Y,
                    spriteSize.X,
                    spriteSize.Y);

            if (offsetsAndSizes.Count > 0)
            {
                sprite_rect = new Rectangle(
                        offsetsAndSizes[animNum].Item1.ToPoint(),
                        offsetsAndSizes[animNum].Item2.ToPoint());
            }
            else
            {
                sprite_rect = new Rectangle(
                        new Point(0, 0),
                        new Point(20, 20));
            }
             if(texture != null)
                sb.Draw(texture,
                    new Rectangle(pos_size.X + CentreOffset.X, pos_size.Y + CentreOffset.Y, pos_size.Width, pos_size.Height),
                    sprite_rect
                    //new Rectangle(sprite_rect.X + CentreOffset.X, sprite_rect.Y + CentreOffset.Y, sprite_rect.Width, sprite_rect.Height)
                    , 
                    colour, 
                    0f,
                    new Vector2(0,0),
                    spriteRotation,
                    0f);
            else
                sb.Draw(blank, pos_size, sprite_rect, colour);
        }
    }

    public class s_anim
    {
        public s_anim(string name)
        {
            this.name = name;
        }
        public void AddAnimation(int pos, float duration, ANIM_TYPE type)
        {
            animations.Add(new Tuple<int, float, ANIM_TYPE>(pos, duration, type));
        }
        public void AddAnimation(int pos, float duration)
        {
            animations.Add(new Tuple<int, float, ANIM_TYPE>(pos, duration, ANIM_TYPE.SPRITE));
        }
        public string name;
        public enum ANIM_TYPE { SPRITE, SOUND };
        public ANIM_TYPE type;
        public List<Tuple<int, float, ANIM_TYPE>> animations = new List<Tuple<int, float, ANIM_TYPE>>();
    }

    public class s_animhandler : s_generic
    {
        List<s_anim> animations;
        public s_anim currentAnimation;
        string lastAnimation;
        float animTimer = 0f;
        public int currentFrameNumber = 0;
        bool islooping = true;
        Tuple<int, float, s_anim.ANIM_TYPE> currentFrame;
        
        public void SetAnimation(string animName, bool loop)
        {
            islooping = loop;
            s_anim an = animations.Find(x=> x.name == animName);
            if (an != null)
            {
                if (lastAnimation != animName)
                {
                    currentAnimation = an;
                    currentFrameNumber = 0;
                    currentFrame = currentAnimation.animations[0];
                    animTimer = currentAnimation.animations[0].Item2;
                    lastAnimation = animName;
                }
            }
        }

        public void AddAnimation(s_anim anima)
        {
            if (animations != null)
                animations.Add(anima);
            else
            {
                animations = new List<s_anim>();
                animations.Add(anima);
            }
        }

        public override void Update(GameTime gametime)
        {
            if (currentAnimation != null)
            {
                int currentANIMLENG = currentAnimation.animations.Count - 1;
                if (currentANIMLENG > 0)
                {
                    if (animTimer <= 0)
                    {
                        if (currentFrame.Item3 == s_anim.ANIM_TYPE.SOUND)
                        {
                            Game1.PlaySound(currentFrame.Item1);
                        }
                        if (currentFrameNumber == currentANIMLENG)
                        {
                            if (islooping)
                                currentFrameNumber = 0;
                            else
                                currentFrameNumber = currentANIMLENG;
                        }
                        else
                            currentFrameNumber++;

                        animTimer = currentFrame.Item2;
                        currentFrame = currentAnimation.animations[currentFrameNumber];
                    }
                    else
                        animTimer -= Game1.deltaTime;
                }
                else
                {
                    currentFrame = currentAnimation.animations[currentFrameNumber];
                    animTimer = currentFrame.Item2;
                }
            }
        }
    }

    public class s_object : s_generic
    {
        public Vector2 collisionOffset;
        public bool issolid = true;
        delegate void OnTouch();
        public bool isKenematic = true;
        public string name;
        public Vector2 position;
        public Vector2 velocity;
        public s_spriterend renderer;
        public Vector2 points;
        public Rectangle collisionBox;
        public List<Vector2> contact_points = new List<Vector2>();
        public Vector2 centre
        {
            get
            {
                return new Vector2(
                    collisionBox.Location.X + (collisionBox.Width / 2),
                    collisionBox.Location.Y + (collisionBox.Height / 2));
            }
        }
        public void SetPos(Vector2 position) {
            collisionBox.X = (int)position.X + (int)collisionOffset.X;
            collisionBox.Y = (int)position.Y + (int)collisionOffset.Y;
        }

        public s_object()
        {
            renderer = new s_spriterend(this);
        }

        public virtual string DrawDebugInfo()
        {
            return null;
        }
        public virtual string DrawInfo()
        {
            return null;
        }

        public void SetCollisonSize(Vector2 size)
        {
            collisionBox = new Rectangle(position.ToPoint() + collisionOffset.ToPoint(), size.ToPoint());
        }
        public bool CheckPointCh(Vector2 point)
        {
            foreach (o_platformControler pl in Game1.chara)
            {
                points = point + position;
                if (pl.collisionBox.Intersects(
                    new Rectangle(
                        point.ToPoint() +
                        position.ToPoint(),
                        new Point(1, 1))
                        )
                        )
                    return true;
            }
            return false;
        }
        public bool CheckPoint(Vector2 point)
        {
            foreach (o_block bl in Game1.blocks)
            {
                if (bl.TYPEOFBLOCK != o_block.BLOCK_TYPE.BLOCK)
                    continue;
                points = point + position;
                if (bl.collisionBox.Intersects(
                    new Rectangle(
                        point.ToPoint() +
                        position.ToPoint(),
                        new Point(1, 1))
                        )
                        )
                    return true;
            }
            return false;
        }

        public void AssignSpriteToRenderer(anim_size_offset anim ,Texture2D tex)
        {
            renderer.SetTexture(anim, tex);
            renderer.spriteSize = new Point(20, 20);
            renderer.spriteOffset = new Point(0, 0);
        }
        public void AssignSpriteToRenderer(anim_size_offset anim ,Texture2D tex, ushort texturePos)
        {
            renderer.SetTexture(anim,tex);
            renderer.spriteSize = new Point(20, 20);
            renderer.spriteOffset = new Point(20 * texturePos, 0);
        }

        public T GetComponent<T>() where T : s_object
        {
            if (typeof(T).ToString() != ToString())
                return null;
            return (T)this;
        }

        public bool CheckPoint<T>(Vector2 point) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null)
                    continue;
                if (obje.collisionBox.Intersects(
                    new Rectangle(points.ToPoint() + new Point(-1,-1), new Point(1, 1))))
                    return true;
            }
            return false;
        }
        public T CheckPointEnt<T>(Vector2 point) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null)
                    continue;
                if (obje.collisionBox.Intersects(
                    new Rectangle(points.ToPoint() + new Point(-1, -1), new Point(1, 1))))
                    return obje;
            }
            return null;
        }
        public T CheckPointEnt<T>(Vector2 point, Predicate<T> condition) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null)
                    continue;
                if (!condition.Invoke(obje))
                    continue;
                if (obje.collisionBox.Intersects(
                    new Rectangle(points.ToPoint() + new Point(-1, -1), new Point(1, 1))))
                    return obje;
            }
            return null;
        }
        public T IntersectBox<T>(Vector2 point) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null || obje == this)
                    continue;
                if (obje.collisionBox.Intersects(collisionBox))
                    return obje;
            }
            return null;
        }
        public T IntersectBox<T>(Vector2 point, float mult) where T : s_object
        {
            contact_points.Add(point + position + collisionOffset);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null || obje == this)
                    continue;
                Rectangle r = collisionBox;
                r.Size = new Point((int)(r.Size.X * mult), (int)(r.Size.Y * mult));
                if (obje.collisionBox.Intersects(r))
                    return obje;
            }
            return null;
        }
        public T[] IntersectBoxAll<T>(Vector2 point) where T : s_object
        {
            contact_points.Add(point + position + collisionOffset);
            points = point + position;
            List<T> intersectedBoxes = new List<T>();
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null || obje == this)
                    continue;
                if (obje.collisionBox.Intersects(collisionBox))
                    intersectedBoxes.Add(obje);
            }
            return intersectedBoxes.ToArray();
        }
        public T[] IntersectBoxAll<T>(Vector2 point, float mult) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            List<T> intersectedBoxes = new List<T>();
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null || obje == this)
                    continue;
                Rectangle r = collisionBox;
                r.Size = new Point((int)(r.Size.X * mult), (int)(r.Size.Y * mult));
          
                if (obje.collisionBox.Intersects(collisionBox))
                    intersectedBoxes.Add(obje);
            }
            return intersectedBoxes.ToArray();
        }
        public T IntersectBox<T>(Vector2 point, Predicate<T> condition) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                if (Game1.objects[i] == null)
                    continue;
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null)
                    continue;
                if (!condition.Invoke(obje))
                    continue;
                if (obje.collisionBox.Intersects(collisionBox))
                    return obje;
            }
            return null;
        }
        public T IntersectBox<T>(Vector2 point, float mult, Predicate<T> condition) where T : s_object
        {
            contact_points.Add(point + position);
            points = point + position;
            for (int i = 0; i < Game1.objects.Count; i++)
            {
                T obje = Game1.objects[i].GetComponent<T>();
                if (obje == null)
                    continue;
                if (!condition.Invoke(obje))
                    continue;
                Rectangle r = collisionBox;
                r.Size = new Point((int)(r.Size.X * mult), (int)(r.Size.Y * mult));
                if (obje.collisionBox.Intersects(r))
                    return obje;
            }
            return null;
        }

        public s_object(Vector2 _position)
        {
            renderer = new s_spriterend(this);
            position = _position;
            collisionBox = new Rectangle(position.ToPoint(), new Point (20,20));
        }


        public void ForceCollisionBoxUpdate()
        {
            collisionBox.Location = position.ToPoint() + collisionOffset.ToPoint();
        }

        public override void Update(GameTime gametime)
        {
            collisionBox.Location = position.ToPoint() + collisionOffset.ToPoint();
        }

    }
}
