using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace TileEngine
{
    [Serializable]
    public class MapSquare
    {
        #region Declarations
        public bool Passable = true; // Determines if player can move into the square or not.
        public bool visible = true;
        public Color Tcolor = Color.White;
        public string CodeValue = "";
        public short CVCount = 0;
        public int[] LayerTiles = new int[3];
        
        #endregion
        #region Constructor
        public MapSquare(
            int background,
            int interactive,
            int foreground,
            string code,
            bool passable)
        {
            LayerTiles[0] = background;
            LayerTiles[1] = interactive;
            LayerTiles[2] = foreground;
            Passable = passable;
            CodeValue = code;

        }
        #endregion
        #region Public Methods
        public void CVUpdate()
        {
            if (CVCount > 0)
                CVCount--;       
            else
                CodeValue = "";

            
        }
        public void TogglePassable()
        {
            Passable = !Passable;
        }
        #endregion
    }
}
