using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
namespace TileEngine {
    public static class TileMap {
        #region Declarations
        public const int tWidth = 32,
                         tHeight = 32,
                         mWidth = 32,
                         mHeight = 32,
                         mLayers = 3;
        private const int skyTile = 2;
        static public MapSquare[,] mCells = new MapSquare[mWidth, mHeight];
        public static bool devMode = false;
        public static SpriteFont spriteFont;
        static private Texture2D tSheet;
        #endregion

        #region Public Methods
        static public void Initialize(Texture2D tTexture)
        {
            tSheet = tTexture;
            for (int x = 0; x < mWidth; x++)
            {
                for (int y = 0; y < mHeight; y++)
                {
                    for (int z = 0; z < mLayers; z++)
                    {
                        mCells[x, y] = new MapSquare(skyTile, 0, 0, "", false);
                    }
                }
            }
        }
        #endregion

        #region Tile and tSheet Handling
        public static Rectangle TileSourceRectangle(int tileIndex)
        {
            return new Rectangle(
                (tileIndex % TilesPerRow) * tWidth,
                (tileIndex / TilesPerRow) * tHeight,
                tWidth,
                tHeight);
        }
        public static int TilesPerRow
        {
            get { return tSheet.Width / tWidth; }
        }
        #endregion

        #region Map Cells Information
        static public int GetCellByPixelX(int pixelX)
        {
            return pixelX / tWidth;
        }
        static public int GetCellByPixelY(int pixelY)
        {
            return pixelY / tHeight;
        }

        static public Vector2 GetCellByPixel(Vector2 pixelLocation)
        {
            return new Vector2(
                GetCellByPixelY((int)pixelLocation.X),
                GetCellByPixelX((int)pixelLocation.Y));
        }
        static public Vector2 GetCellCenter(int cellX, int cellY)
        {
            return new Vector2(
                (cellX * tWidth) + (tWidth / 2),
                (cellY * tHeight) + (tHeight / 2));
        }
        static public Vector2 GetCellCenter(Vector2 cell)
        {
            return GetCellCenter(
                (int)cell.X,
                (int)cell.Y);
        }
        static public Rectangle CellWorldRectangle(Vector2 cell)
        {
            return CellWorldRectangle(
                (int)cell.X,
                (int)cell.Y);
        }
        static public Rectangle CellWorldRectangle(int cellX, int cellY)
        {
            return new Rectangle(
                cellX * tWidth,
                cellY * tHeight,
                tWidth,
                tHeight);
        }

        static public Rectangle CellScreenRectangle(Vector2 cell)
        {
            return CellScreenRectangle((int)cell.X, (int)cell.Y);
        }
        static public Rectangle CellScreenRectangle(int cellX, int cellY)
        {
            return Camera.WorldToScreen(CellWorldRectangle(cellX, cellY));
        }
        static public bool CellIsPassable(int cellX, int cellY) // If false, player wont be able to reach tile.
        {

            MapSquare square = GetMapSquareAtCell(cellX, cellY);

            if (square == null)
                return false;
            else 
                return square.Passable;
        }
        static public bool CellIsPassable(Vector2 cell)
        {
            return CellIsPassable((int)cell.X, (int)cell.Y);
        }
        static public string CellCodeValue(int cellX, int cellY) // Shortcuts
        {
            MapSquare square = GetMapSquareAtCell(cellX, cellY);
            if (square == null)
                return "";
            else
                return square.CodeValue;
        }
        static public bool CellIsPassableByPixel(Vector2 pixelLocation) // Shortcuts
        {
            return CellIsPassable(GetCellByPixelX((int)pixelLocation.X),
                                  GetCellByPixelY((int)pixelLocation.Y));
        }

        static public string CellCodeValue(Vector2 cell) // Shortcuts
        {
            return CellCodeValue((int)cell.X, (int)cell.Y);
        }
        #endregion

        #region MapSquare Objects Information
        static public MapSquare GetMapSquareAtCell(int tileX, int tileY)
        {
            try
            {
                if ((tileX >= 0) && (tileX < mWidth) && (tileY >= 0) && (tileY < mHeight))
                    return mCells[tileX, tileY];
                else
                    return null;
            }
            catch { return null; }
        }
        static public void SetMapSquareAtCell(
            int tileX,
            int tileY,
            MapSquare tile)
        {
            if((tileX >= 0) && (tileX < mWidth) && (tileY >= 0) && (tileY < mHeight))
                mCells[tileX, tileY] = tile;
        }
        static public void SetTileAtCell(
            int tileX,
            int tileY,
            int layer,
            int tileIndex)
        {
            if ((tileX >= 0) && (tileX < mWidth) && (tileY >= 0) && (tileY < mHeight))
            {
                mCells[tileX, tileY].LayerTiles[layer] = tileIndex;
            }
        }
        static public MapSquare GetMapSquareAtPixel(int pixelX, int pixelY)
        {
            return GetMapSquareAtCell(
                GetCellByPixelX(pixelX),
                GetCellByPixelY(pixelY));
        }

        static public MapSquare GetMapSquareAtPixel(Vector2 pixelLocation)
        {
            return GetMapSquareAtPixel(
                (int)pixelLocation.X,
                (int)pixelLocation.Y);
        }
        #endregion

        #region Draw TileMap
        static public void Draw(SpriteBatch spriteBatch)
        {
            int startX = GetCellByPixelX((int)Camera.Position.X);
            int endX = GetCellByPixelX((int)Camera.Position.X + Camera.ViewPortWidth);
            int endY = GetCellByPixelY((int)Camera.Position.Y + Camera.ViewPortHeight);
            int startY = GetCellByPixelY((int)Camera.Position.Y);

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    for (int z = 0; z < mLayers; z++)
                    {
                        if ((x >= 0) && (y >= 0) && (x < mWidth) && (y < mHeight))
                        {
                            if (mCells[x, y].visible)
                            {
                                spriteBatch.Draw(
                                    tSheet,
                                    CellScreenRectangle(x, y),
                                    TileSourceRectangle(
                                    mCells[x, y].LayerTiles[z]),
                                    mCells[x, y].Tcolor,
                                    0.0f,
                                    Vector2.Zero,
                                    SpriteEffects.None,
                                    1f - ((float)z * 0.1f));

                                switch (mCells[x,y].CodeValue)
                                {
                                    case "Example": // te is a list of Texture2Ds
                                        //spriteBatch.Draw(te[0], CellScreenRectangle(x, y), null, mCells[x, y].Tcolor, 0f, new Vector2(0, 0), SpriteEffects.None, 0.86f);
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    if (devMode)
                        DrawEditModeItems(spriteBatch, x, y);
                }
            }
        }
        public static void DrawEditModeItems(
            SpriteBatch spriteBatch,
            int x,
            int y)
        {
            if((x < 0) || (x >= mWidth) || (y < 0) || (y >= mHeight))
                return;
            if (!CellIsPassable(x, y))
            {
                spriteBatch.Draw(
                    tSheet,
                    CellScreenRectangle(x, y),
                    TileSourceRectangle(1),
                    new Color(255, 0, 0, 1),
                    0.0f,
                    Vector2.Zero,
                SpriteEffects.None,
                0.0f);
            }
            if (mCells[x, y].CodeValue != "")
            {
                Rectangle screenRect = CellScreenRectangle(x, y);
                spriteBatch.DrawString(
                    spriteFont,
                    mCells[x, y].CodeValue,
                    new Vector2(screenRect.X, screenRect.Y),
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    1.0f,
                    SpriteEffects.None,
                    0.0f);
            }
        }
        #endregion

        #region Saving and Loading Maps
        public static void SaveMap(FileStream fileStream)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, mCells);
            fileStream.Close();
        }
        public static void ClearMap()
        {
            for(int x = 0; x < mWidth; x++)
                for(int y = 0; y < mHeight; y++)
                    for(int z = 0; z < mLayers; z++)
                    {
                        mCells[x, y] = new MapSquare(2, 0, 0, "", true);
                    }
        }
        public static void LoadMap(FileStream fileStream)
        {
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                mCells = (MapSquare[,])formatter.Deserialize(fileStream);
                fileStream.Close();
            }
            catch
            {
                ClearMap();
            }
        }
        #endregion
    }
}