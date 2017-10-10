using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
namespace GUI
{
    public class FontManager
    {
        private static FontManager fontManager;
        private Dictionary<String, SpriteFont> fontDict;
        private ContentManager content;

        private FontManager(ContentManager _content)
        {
            fontDict = new Dictionary<String, SpriteFont>();
            content = _content;
        }

        public static FontManager getFontManager(ContentManager _content)
        {
            if(fontManager == null)
                fontManager = new FontManager(_content);
            return fontManager;
        }

        public void loadFonts(String filename)
        {
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToFollowing("Font");
            do
            {
                String name = reader.GetAttribute("name");
                String fontName = reader.ReadElementContentAsString();
                SpriteFont sf = content.Load<SpriteFont>(fontName);
                fontDict.Add(name,sf);
                
            } while (reader.ReadToNextSibling("Font"));
        }

        public SpriteFont getFont(String key)
        {
            if (fontDict.ContainsKey(key))
                return fontDict[key];
            else
                return fontDict["Default"];
        }

    }
}
