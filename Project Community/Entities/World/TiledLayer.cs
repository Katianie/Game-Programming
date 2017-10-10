using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Entities.World
{
    /// <summary>
    /// Defines a tile with an index to a corresponding texture and a color for variation and debugging.
    /// </summary>
    public struct Tile
    {
        public String texIndex;
        public Color color;
    }

    /// <summary>
    /// This represents the tiled background layer of a world
    /// </summary>
    /// <Owner>Ed Linero</Owner>
   public class TiledLayer
    {
        public int width;
        public int height;
        public int tileWidth = 64;
        private List<String> textureStrings;
        private String[] textures;
        private Texture2D[] tileTextures;
        private Tile[] fullMap;
        private GameWorld gameWorld;
        private Game game;

       /// <summary>
       /// 
       /// </summary>
       /// <param name="_gameWorld"></param>
        public TiledLayer(GameWorld _gameWorld)
        {
            gameWorld = _gameWorld;
            game = gameWorld.getGame();
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="tileFile"></param>
       /// <param name="mapFile"></param>
       /// <param name="_width"></param>
       /// <param name="_height"></param>
        public void load(String tileFile, String mapFile, int _width, int _height)
        {
            width = _width;
            height = _height;
            StreamReader mapReader = new StreamReader(mapFile);
            XmlTextReader tileFileReader = new XmlTextReader(tileFile);

            //Load Tiles
            tileFileReader.ReadToFollowing("Tile");
            List<Texture2D> tiles = new List<Texture2D>();
            List<bool> colBool = new List<bool>();
            textureStrings = new List<String>();
            
            do
            {
                bool collidable = bool.Parse(tileFileReader.GetAttribute("collidable"));
                String tileLocation = tileFileReader.ReadElementContentAsString();
                textureStrings.Add(tileLocation);
                tiles.Add(game.Content.Load<Texture2D>(tileLocation));
                colBool.Add(collidable);                    
            } while (tileFileReader.ReadToNextSibling("Tile"));
            bool[] colArray = colBool.ToArray();
            tileTextures = tiles.ToArray();


            //Load map
            String[] strs = mapReader.ReadToEnd().Split(',');
            List<Tile> tempList = new List<Tile>();
            int periodPos = 0;
            String final = "";
            
            foreach (String s in strs)
            {

               
            Tile temp = new Tile();
            if (s.Equals(string.Empty))
                break;

                    
                    periodPos = s.IndexOf('.');
                    if(periodPos != -1)
                    {
                        final = "GameTiles\\" + s.Substring(0, periodPos).Trim();
                
                
                        temp.texIndex = final;
                        temp.color = Color.White;
                        tempList.Add(temp);
                    }
            }
            fullMap = tempList.ToArray();
            textures = textureStrings.ToArray();



            tileFileReader.Close();
            mapReader.Close();
        }

        /// <summary>
        /// starting row
        /// starting collumn
        /// ending row
        /// ending collumn
        /// </summary>
        /// <returns></returns>
        public int[] getCollumnAndRowBounds()
        {
            int[] bounds = new int[4];
            bounds[0] = gameWorld.viewport.X/tileWidth;
            bounds[1] = gameWorld.viewport.Y/tileWidth;
            bounds[2] = (gameWorld.viewport.X + gameWorld.viewport.Width)/tileWidth;
            if(bounds[2] < 32)
            bounds[2]++;
            bounds[3] = (gameWorld.viewport.Y + gameWorld.viewport.Height)/tileWidth;
            if (bounds[3] < 32)
            bounds[3]++;

            return bounds;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="row"></param>
       /// <param name="collumn"></param>
       /// <returns></returns>
        public int tileIndex(int row, int collumn)
        {
            return (row * width) + collumn;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="index"></param>
       /// <returns></returns>
        public Vector2 getPositionFromIndex(int index)
        {
            Vector2 pos;
            pos.X = index % width;
            pos.Y = index / width;
            return pos;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="row"></param>
       /// <param name="collumn"></param>
       /// <returns></returns>
       /* public Texture2D getTextureFromIndex(int row, int collumn)
        {
            return tileTextures[fullMap[(row * width) + collumn].texIndex];
        }*/


        public int[] compareTiles(String[] textures, Tile[] tiles)
        {


            List<int> intList = new List<int>();
            int[] array = null;
            
            String str1 = "";
            String str2 = "";
            int count = 0;



            for (int i = 0; i < tiles.Length; i++)
            {

                str1 = tiles[i].texIndex;

                for (int j = 0; j < textures.Length; j++)
                {
                    
                    
                    str2 = textures[j];

                    if (str1 == str2)
                    {

                        intList.Add(j);
                        count++;

                    }
                }
            }



            array = intList.ToArray();
            

            return array;
        }

       /// <summary>
       /// 
       /// </summary>
       /// <param name="sb"></param>
        public void draw(SpriteBatch sb)
        {

            int[] compare = compareTiles(textures, fullMap);

            Vector2 viewPos = new Vector2(gameWorld.viewport.X,gameWorld.viewport.Y);
            int[] bounds = getCollumnAndRowBounds();
            int i = bounds[0];
            int j = bounds[1];
            while (i < bounds[2] + 1)
            {
                j = bounds[1];
                while (j < bounds[3] + 1)
                {

                    
                    int index = tileIndex(j,i);
                    Vector2 adjustedPos = new Vector2(i*tileWidth, j*tileWidth);
                    adjustedPos -= viewPos;
                    if (compare != null)
                    {
                        sb.Draw(tileTextures[compare[index]], adjustedPos, fullMap[index].color);
                    }
                    if (!fullMap[index].color.Equals( Color.White))
                        fullMap[index].color = Color.White;
                    j++;
                }
                i++;
            }
        }


    }

}
