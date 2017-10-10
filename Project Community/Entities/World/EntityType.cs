using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
namespace Entities.World
{
    public enum TypeOfThing { PLAYER, BUILDING, TILE, ITEM, ITEM2, NPC, ENEMY,BUILDING_ENEMY, WALL, PROJECTILE, CHOPTREE, CANNON, BAR, EXPLOSIVE }

    /// <summary>
    /// Represents a type of entity. 
    ///  When entities are added their data is loaded from here.
    /// </summary>
    /// <Owner>Justin Dale</Owner>>
    public class EntityType
    {
        public readonly float scale;
        public TypeOfThing type;
        public Dictionary<string,List<short>> animations;
        public List<Texture2D> images;
        public float restitution { get; set; }
        public float friction { get; set; }
        public Vector2 size { get; set; }
        public float density { get; set; }
        private BodyType bodyType;
        public bool fixedRotation = true;
        public Shape shape {get; set;}
        private readonly List<Shape> shapes = new List<Shape>();
        public String fileName {get; set;}


        /// <summary>
        /// Constructor.  Loads the entity type from file.
        /// </summary>
        /// <param name="filename">EntityType file location</param>
        /// <param name="game">Game reference</param>
        public EntityType(String filename, Game game)
        {
            scale = 64.0f;
            Load(filename, game);
        }

        /// <summary>
        /// This should only be used for walls
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="game"></param>
        public EntityType(Game game, TypeOfThing _type)
        {
            scale = 64.0f;
            type = _type;
        }


        /// <summary>
        /// Returns the shapes that make up the body of an entity of this type.
        /// </summary>
        /// <returns>Returns list of Shapes</returns>
        public List<Shape> getShapes()
        {
            return shapes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BodyType getBodyType()
        {
            return bodyType;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="game"></param>
        public void Load(String filename, Game game)
        {
            fileName = filename;
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToFollowing("Data");
            restitution = float.Parse(reader.GetAttribute("Restitution"));
            density = float.Parse(reader.GetAttribute("Density"));
            friction = float.Parse(reader.GetAttribute("Friction"));
            String fr = reader.GetAttribute("fixedRotation");
            if(fr!=null)
                fixedRotation = bool.Parse(fr);
            //Load images
            images = new List<Texture2D>();
            reader.ReadToFollowing("Image");
            int x;
            int y;
            do
            {
                x = int.Parse(reader.GetAttribute("width"));
                y = int.Parse(reader.GetAttribute("height"));
                String fileToLoad = reader.ReadElementContentAsString();
                images.Add(game.Content.Load<Texture2D>(fileToLoad));
            }
            while (reader.ReadToNextSibling("Image"));
            size = new Vector2(x,y);

            //Load animations
            animations = new Dictionary<string, List<short>>();
            reader.ReadToFollowing("State");
            do
            {
                String name = reader.GetAttribute("name");
                String[] strs = reader.ReadElementContentAsString().Split(',');

                animations.Add(name, new List<short>());
                List<short> tempList;
                animations.TryGetValue(name, out tempList);
                foreach (String s in strs)
                {
                    tempList.Add(short.Parse(s));
                }
            } while (reader.ReadToNextSibling("State"));

            reader.ReadToFollowing("Shape");
            do{
            string shapeType = reader.GetAttribute("ShapeType");
            string  type = reader.GetAttribute("type");
            switch (type)
            {
                case "Static":
                    {
                        bodyType = BodyType.Static;
                        break;
                    }
                case "Dynamic":
                    {
                        bodyType = BodyType.Dynamic;
                        break;
                    }
                case "Kinematic":
                    {
                        bodyType = BodyType.Kinematic;
                        break;
                    }


            }
            switch(shapeType)
            {
                case "Circle":
                    {
                        float rad =  reader.ReadElementContentAsFloat();
                        shape = new CircleShape();
                        shape._radius = rad;
                        break;
                    }
                case "Polygon":
                    {
                        PolygonShape pShape = new PolygonShape();
                        List<Vector2> verts = new List<Vector2>();
                        float multiX = x / scale;
                        float multiY = y / scale;
                        reader.ReadToFollowing("Vertex");
                        do
                        {
                            String str = reader.ReadElementContentAsString();
                            String[] strs = str.Split(',');
                            Vector2 vert = new Vector2(float.Parse(strs[0]) * multiX, float.Parse(strs[1]) * multiY);
                            verts.Add(vert);
                        } while (reader.ReadToNextSibling("Vertex"));
                        pShape.Set(verts.ToArray(),verts.Count);
                        shape = pShape;
                        break;

                    }


            }
            shapes.Add(shape);
            if (shapes.Count > 1)
                Console.Write(" ");
            }while(reader.ReadToNextSibling("Shape"));

        }
    }
}
