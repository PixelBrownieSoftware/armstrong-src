
/*
switch (dir2)
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
/*
    AddBlock(new o_block(new Vector2(0, 110)));
    AddBlock(new o_block(new Vector2(0, 130)));
    AddBlock(new o_block(new Vector2(0, 150)));
    AddBlock(new o_block(new Vector2(20, 150)));
    AddBlock(new o_block(new Vector2(40, 150)));
    AddBlock(new o_block(new Vector2(60, 150)));
    AddBlock(new o_block(new Vector2(80, 150)));
    AddBlock(new o_block(new Vector2(100, 150)));
    AddBlock(new o_block(new Vector2(120, 150)));
    AddBlock(new o_block(new Vector2(140, 150)));
    AddBlock(new o_block(new Vector2(160, 120)));
    AddBlock(new o_block(new Vector2(180, 150)));
    AddBlock(new o_block(new Vector2(200, 150)));
    AddBlock(new o_block(new Vector2(220, 150)));
    AddBlock(new o_block(new Vector2(240, 150)));
    AddBlock(new o_block(new Vector2(240, 130)));
    AddBlock(new o_block(new Vector2(240, 110)));
    */
/*
DrawLine(scrnMsPos,
   pl.centre,
    gameTime,
    spriteBatch
    );
*/
/*
obje.renderer.colour = Color.White;
for (int a = 0; a < level.tiles.Length; a++) {
    int p = TwoDToOneDArray(pl.centre);

    if (a == p)
    {
        if (testO == null)
        {
            testO = obje;
            if (TwoDToOneDArray(testO.position) == p)
            {
                if (testO != pl)
                {
                    testO.renderer.colour = Color.Red;
                    break;
                }
            }
        }
    }
}
testO = null;
*/
/*
if (obje != null && obje ==pl)
    spriteBatch.Draw(blank, new Rectangle(obje.collisionBox.Location - new Point(0,0) - campos.ToPoint(), new Point(4,4)), Color.White);

for (int p = 0; p < obje.contact_points.Count; p++)
{
    Vector2 pts = obje.contact_points[p];
    spriteBatch.Draw(blank, new Rectangle(pts.ToPoint() - campos.ToPoint(), new Point(1, 1)), Color.White);
}
*/
/*
public void AddBlock(o_block cha, ushort texturePos)
{
    cha.AssignSpriteToRenderer(testtexture2, texturePos);
    objects.Add(cha);
}
public void AddBlock(o_block cha, ushort texturePos, o_block.BLOCK_TYPE typ)
{
    cha.AssignSpriteToRenderer(testtexture2, texturePos);
    cha.TYPEOFBLOCK = typ;
    objects.Add(cha<o_block>);
}
public void AddBlock(o_block cha, ushort texturePos, o_block.BLOCK_TYPE typ, string name)
{
    cha.AssignSpriteToRenderer(testtexture2, texturePos);
    cha.name = name;
    cha.TYPEOFBLOCK = typ;
    objects.Add(cha);
}

public object AddCharacter(o_platformControler cha, string name)
{
    if (chara.Count > 20)
        return null;
    cha.AssignSpriteToRenderer(testtexture);
    cha.name = name;
    objects.Add(cha);
    return cha;
}
*/
/*
public object AddCharacter(o_plcharacter cha, string name)
{
    if (chara.Count > 20)
        return null;
    cha.AssignSpriteToRenderer(textures["testtexture"].Item2, textures["testtexture"].Item1);
    cha.name = name;
    objects.Add(cha);
    //chara.Add(cha);
    return cha;
}
*/
/*
public object AddCharacter(o_platformControler cha, Texture2D texturerend, string name)
{
    if (chara.Count > 20)
        return null;
    cha.AssignSpriteToRenderer(texturerend);
    cha.name = name;
    objects.Add(cha);
    return cha;
}
*/
/*
        #region CAPITAL TO SMALL
        switch (c)
        {
            case 'A':
                c = 'a';
                break;
            case 'B':
                c = 'b';
                break;
            case 'C':
                c = 'c';
                break;
            case 'D':
                c = 'd';
                break;
            case 'E':
                c = 'e';
                break;
            case 'F':
                c = 'f';
                break;
            case 'G':
                c = 'g';
                break;
            case 'H':
                c = 'h';
                break;
            case 'I':
                c = 'i';
                break;
            case 'J':
                c = 'j';
                break;
            case 'K':
                c = 'k';
                break;
            case 'L':
                c = 'l';
                break;
            case 'M':
                c = 'm';
                break;
            case 'N':
                c = 'n';
                break;
            case 'O':
                c = 'o';
                break;
            case 'P':
                c = 'p';
                break;
            case 'Q':
                c = 'q';
                break;
            case 'R':
                c = 'r';
                break;
            case 'S':
                c = 's';
                break;
            case 'T':
                c = 't';
                break;
            case 'U':
                c = 'u';
                break;
            case 'V':
                c = 'v';
                break;
            case 'W':
                c = 'w';
                break;
            case 'X':
                c = 'x';
                break;
            case 'Y':
                c = 'y';
                break;
            case 'Z':
                c = 'z';
                break;
        }
        #endregion
 */
/*
anim_size_offset anim = new anim_size_offset();
anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
pl = AddObject<o_plcharacter>(new Vector2(0,0), "player", anim, testtexture);
pl.spawnpoint = new Vector2(0, 0);
pl.velocityLimiter = 0.85f;

shape = new s_shape(
    new List<Vector2>()
    {
        new Vector2(24, 3),
        new Vector2(1, 7),
        new Vector2(1, 11),
        new Vector2(27, 13),
        new Vector2(24, 3)
    }
    ,pl.position);

shape2 = new s_shape(
    new List<Vector2>()
    {
        new Vector2(75, 3),
        new Vector2(26, 3),
        new Vector2(1, 55),
        new Vector2(90, 33),
        new Vector2(75, 3)
    }
    , new Vector2(20,40));
*/
/*
if (currentKeyboardState.IsKeyDown(Keys.D))
    shape2.position.X += 1;
if (currentKeyboardState.IsKeyDown(Keys.A))
    shape2.position.X -= 1;
if (currentKeyboardState.IsKeyDown(Keys.S))
    shape2.position.Y += 1;
if (currentKeyboardState.IsKeyDown(Keys.W))
    shape2.position.Y -= 1;

if (testO != null)
{
    if (Keyboard.GetState().IsKeyDown(Keys.E) && !previousKeyboardState.IsKeyDown(Keys.E))
        if ((testO.collisionBox.Right - pl.collisionBox.Left) > 0)
            pl.position.X += (testO.collisionBox.Right - pl.collisionBox.Left) + 1;

    if (Keyboard.GetState().IsKeyDown(Keys.Q) && !previousKeyboardState.IsKeyDown(Keys.Q))
        if ((testO.collisionBox.Right - pl.collisionBox.Left) > 0)
            pl.position.X += (testO.collisionBox.Left - pl.collisionBox.Right) - 1;
}
*/
/*
if (mousestate.RightButton == ButtonState.Pressed)
{
    if (shape.points == null)
        shape.points = new List<Vector2>();
    shape.points.Add(scrnMsPos);
}

shape.position = pl.position;
shapeIntersect = shape.Interesect(shape2);
if (shapeIntersect)
{
    Tuple<Vector2, float> pen = shape.FindMTV(shape2);
    dist = new Tuple<Vector2, float>(pen.Item1, Math.Abs(pen.Item2));
    pl.position += pen.Item1 * pen.Item2;
}
Penetration1 = shape.GetIntersectAxis(new Vector2(1, 0));
Penetration2 = shape2.GetIntersectAxis(new Vector2(1, 0));
*/
/*
DrawText("Interesect " + shapeIntersect, font, new Vector2(0, 0), spriteBatch);
if (dist != null)
{
    DrawText("Vector " + dist.Item1 , font, new Vector2(0, 13), spriteBatch);
    DrawText("Distance: "+ dist.Item2, font, new Vector2(0, 23), spriteBatch);
}
*/
/*
DrawLine(pl.throwDir, 
    centreOfScreen,
    0.7f,
    gameTime, 
    spriteBatch
    );
*/
/*
int ind = 0;
foreach (List<Vector2> ve in normals)
{
    foreach (Vector2 v in ve)
    {
        if (ind == 0)
            DrawLine(shape2.position + (v * 40), shape2.position, gameTime, spriteBatch, Color.Orange);
        if (ind == 1)
            DrawLine(shape.position + (v * 40), shape.position, gameTime, spriteBatch, Color.Orange);
    }
    ind++;
}

if (debuginfo)
{
    DrawText("Position: " + pl.position.X + "," + pl.position.Y, font, new Vector2(0, 0), spriteBatch);
    DrawText("Position: " + pl.position.X + "," + pl.position.Y, font, new Vector2(0, 0), spriteBatch);
    DrawText("Velocity: " + pl.velocity.X + "," + pl.velocity.Y, font, new Vector2(0, 10), spriteBatch);
    DrawText("Throw direction: " + pl.throwDir.X + "," + pl.throwDir.Y, font, new Vector2(0, 20), spriteBatch);
    DrawText("Is Grounded: " + pl.isgrounded, font, new Vector2(0, 30), spriteBatch);

    DrawText("Vitrtual screen: " + VirtScreen.Width + ", " + VirtScreen.Height, font, new Vector2(0, 40), spriteBatch);
    DrawText("Spawn", font, pl.spawnpoint - campos + new Vector2(-20, -20), spriteBatch);

}
else
{
    DrawText("Press tab for debug", font, new Vector2(0, 0), spriteBatch);
    if(pl.heldItem)
        DrawText("Left click to throw", font, new Vector2(0, 10), spriteBatch);
    else
        DrawText("Collect item", font, new Vector2(0, 10), spriteBatch);
    DrawText("Throwing power: " + HypotenuseVector(pl.throwDir * pl.throwMultiplier), font, new Vector2(0, 20), spriteBatch);

}
DrawText("Box position (L,R,T,B): " + 
    (pl.collisionBox.Width + pl.collisionBox.Right) + ", " +
    pl.collisionBox.Right + ", " +
    pl.collisionBox.Top + ", " +
    pl.collisionBox.Bottom, font, new Vector2(0, 20), spriteBatch);

DrawShape(shape, gameTime, spriteBatch);
DrawShape(shape2, gameTime, spriteBatch);
*/

/*
public void CollisionDetectionOld()
{
    o_block leftUp = CheckPointEnt<o_block>(new Vector2(-1, 0), x => x.issolid);
    o_block leftDown = CheckPointEnt<o_block>(new Vector2(-1, 19), x => x.issolid);
    o_block rightUp = CheckPointEnt<o_block>(new Vector2(20, 0), x => x.issolid);
    o_block rightDown = CheckPointEnt<o_block>(new Vector2(20, 19), x => x.issolid);
    o_block upLeft = CheckPointEnt<o_block>(new Vector2(0, -1), x => x.issolid);
    o_block upRight = CheckPointEnt<o_block>(new Vector2(19, -1), x => x.issolid);
    o_block downLeft = CheckPointEnt<o_block>(new Vector2(0, 20), x => x.issolid);
    o_block downRight = CheckPointEnt<o_block>(new Vector2(19, 20), x => x.issolid);

    if (downLeft != null || downRight != null)
    {
        if (velocity.Y > 0)
        {
            if (downLeft != null)
            {
                velocity.Y = 0;
                position.Y = downLeft.position.Y - downLeft.collisionBox.Height;//downLeft.collisionBox.Top - downLeft.collisionBox.Height;
            }
            else
            {
                velocity.Y = 0;
                position.Y = downRight.position.Y - downRight.collisionBox.Height;//downRight.collisionBox.Top - downRight.collisionBox.Height;
            }
            isgrounded = true;
        }

    }
    else
    {
        isgrounded = false;
    }
    if (leftUp != null|| leftDown != null)
    {
        if (velocity.X < 0)
        {
            if (leftUp != null)
            {
                velocity.X = 0;
                position.X = leftUp.position.X + leftUp.collisionBox.Width + 1;
            }
            else if (leftDown != null)
            {
                velocity.X = 0;
                position.X = leftDown.position.X + leftDown.collisionBox.Width + 1;
            }
        }

    }
    if (rightUp != null || rightDown != null)
    {
        if (velocity.X > 0)
        {
            if (rightUp != null)
            {
                velocity.X = 0;
                position.X = rightUp.position.X - rightUp.collisionBox.Width + 1;//rightUp.collisionBox.Left - rightUp.collisionBox.Width;
                ForceCollisionBoxUpdate();
            }
            else if (rightDown != null)
            {
                velocity.X = 0;
                position.X = rightDown.position.X - rightDown.collisionBox.Width + 1;//rightDown.collisionBox.Left - rightDown.collisionBox.Width;
                ForceCollisionBoxUpdate();
            }
        }
    }
    if (upLeft != null || upRight != null)
    {
        if (velocity.Y < 0)
        {
            if (upLeft != null)
            {
                velocity.Y = 0;
                position.Y = upLeft.collisionBox.Bottom + 1;
            }
            else
            {
                velocity.Y= 0;
                position.Y = upRight.collisionBox.Bottom + 1;
            }
        }

    }
    ForceCollisionBoxUpdate();
    //collisionObj();
}
*/
/*
void collisionObj()
{
    o_movableSolid leftUp = CheckPointEnt<o_movableSolid>(new Vector2(-1, 0), x => x.issolid);
    o_movableSolid leftDown = CheckPointEnt<o_movableSolid>(new Vector2(-1, 20), x => x.issolid);
    o_movableSolid rightUp = CheckPointEnt<o_movableSolid>(new Vector2(20, 0), x => x.issolid);
    o_movableSolid rightDown = CheckPointEnt<o_movableSolid>(new Vector2(20, 20), x => x.issolid);
    o_movableSolid upLeft = CheckPointEnt<o_movableSolid>(new Vector2(0, -1), x => x.issolid);
    o_movableSolid upRight = CheckPointEnt<o_movableSolid>(new Vector2(19, -1), x => x.issolid);
    o_movableSolid downLeft = CheckPointEnt<o_movableSolid>(new Vector2(0, 21), x => x.issolid);
    o_movableSolid downRight = CheckPointEnt<o_movableSolid>(new Vector2(19, 21), x => x.issolid);

    if (downLeft != null || downRight != null)
    {
        if (velocity.Y > 0)
        {
            if (downLeft != null)
            {
                velocity.Y = (downLeft.collisionBox.Top - collisionBox.Bottom);
            }
            else
            {
                velocity.Y = (downRight.collisionBox.Top - collisionBox.Bottom);
            }
        }

    }
    if (leftUp != null || leftDown != null)
    {
        if (velocity.X < 0)
        {
            if (leftUp != null)
            {
                velocity.X = (leftUp.collisionBox.Right - collisionBox.Left) + 1;
            }
            else if (leftDown != null)
            {
                velocity.X = (leftDown.collisionBox.Right - collisionBox.Left) + 1;
            }
        }

    }
    if (rightUp != null || rightDown != null)
    {
        if (velocity.X > 0)
        {
            if (rightUp != null)
            {
                velocity.X = (rightUp.collisionBox.Left - collisionBox.Right) + 1;
            }
            else if (rightDown != null)
            {
                velocity.X = (rightDown.collisionBox.Left - collisionBox.Right) + 1;
            }
        }
    }
    if (upLeft != null || upRight != null)
    {
        if (velocity.Y < 0)
        {
            if (upLeft != null)
            {
                velocity.Y = (upLeft.collisionBox.Bottom - collisionBox.Top) + 1;
            }
            else
            {
                velocity.Y = (upRight.collisionBox.Bottom - collisionBox.Top) + 1;
            }
            velocity.Y = 0;
        }

    }
}
    */
/*
if (heldItem != null)
{

    o_item it = (o_item)AddCharacter(new o_item(new Vector2(0, 0), 0.97f), testtexture2, "thing");
    if (it != null)
    {
        it.position = position + new Vector2(0, -10);
        it.velocity = Vector2.Zero;
        it.Throw(new Vector2(dir.X * throwMultiplier, dir.Y * throwMultiplier));
        heldItem = null;
    }
}
*/
/*
o_item it = CheckPointCh2<o_item>(position);
if (it != null)
{
    int a = 0;
}
*/
/*
Vector2 dir = Game1.mouseposition - position;
dir.Normalize();
throwDir = dir;
*/
/*
if (Keyboard.GetState().IsKeyDown(Keys.C))
{
    float x = ra.Next(-5, 5);
    float y = ra.Next(-5, -1);

    o_item it = (o_item)AddCharacter(new o_item(new Vector2(0, 0), 0.97f), testtexture2, "thing");
    if (it != null)
    {
        it.position = c.position + new Vector2(0, -10);
        it.velocity = Vector2.Zero;
        it.Throw(new Vector2(x, y));
    }
}
*/
/*
if (!Keyboard.GetState().IsKeyDown(Keys.Left) && !Keyboard.GetState().IsKeyDown(Keys.Right))
    velocity.X = 0;
if (isdebug)
{
    if (!Keyboard.GetState().IsKeyDown(Keys.Up) && !Keyboard.GetState().IsKeyDown(Keys.Down))
        velocity.Y = 0;
}
*/
/*
public static s_object FindC(string name)
{
    return (s_object)Game1.FindCharacter(name);
}
public static s_object FindB(string name)
{
    return (s_object)Game1.FindBlock(name);
}
public static s_object Find(string name)
{
    return (s_object)Game1.FindBlock(name);
}
*/
/*
public T CreateObject<T>(string name) where T: s_object, new ()
{
    anim_size_offset anim = new anim_size_offset();
    anim.Add(new Tuple<Vector2, Vector2>(new Vector2(0, 0), new Vector2(20, 20)));
    T obj = Game1.game.AddObject<T>(position, name, anim, renderer.texture);
    return obj;
}
*/
/*
public static object FindBlock(string nam)
{
    foreach (o_block c in blocks)
    {
        if (c.name == nam)
            return c;
    }
    return null;
}
public static object FindCharacter(string nam)
{
    foreach (o_platformControler c in chara)
    {
        if (c.name == nam)
            return c;
    }
    return null;
}
public static object FindCharacter(string nam, string type)
{
    string t = type;
    string add = "blockplacingplatformer.";
    type = add + t;
    foreach (o_platformControler c in chara)
    {
        string typ = c.GetType().ToString();
        if (c.name == nam && typ == type)
            return c;
    }
    return null;
}
*/