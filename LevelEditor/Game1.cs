using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace Level_Editor {
    public class Game1:Microsoft.Xna.Framework.Game {
        public int paintLayer = 0;
        public int paintTile = 0;
        public bool codeEditing = false;
        public string CurrentCodeValue = "";
        public string hCodeVal = "";
        public MouseState lastMouseState;
        
        System.Windows.Forms.HScrollBar horizontalScroll;
        System.Windows.Forms.VScrollBar verticalScroll;
        System.Windows.Forms.Form parentForm;
        System.Windows.Forms.PictureBox pictureBox;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        IntPtr drawSurface;
        System.Windows.Forms.Control gameForm;

        public Game1(IntPtr drawSurface, System.Windows.Forms.Form parentForm,
                                         System.Windows.Forms.PictureBox surfacePictureBox)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            this.parentForm = parentForm;
            this.drawSurface = drawSurface;
            this.pictureBox = surfacePictureBox;
            verticalScroll = (System.Windows.Forms.VScrollBar)parentForm.Controls["verticalScrollBar1"];
            horizontalScroll = (System.Windows.Forms.HScrollBar)parentForm.Controls["horizontalScrollBar1"];
            graphics.PreparingDeviceSettings +=
                new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);

            Mouse.WindowHandle = drawSurface;
            gameForm = System.Windows.Forms.Control.FromHandle(this.Window.Handle);
            gameForm.VisibleChanged += new EventHandler(gameForm_VisibleChanged);
            gameForm.SizeChanged += new EventHandler(pictureBox_sizeChanged);
        }
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = drawSurface;
        }

        protected override void Initialize() {
            base.Initialize();
        }
        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Camera.ViewPortWidth = pictureBox.Width;
            Camera.ViewPortHeight = pictureBox.Height;
            Camera.WorldRectangle = new Rectangle(0, 0, 200 * 64, 200 * 64);

            TileMap.Initialize(Content.Load<Texture2D>(@"Textures\PlatformTiles"));
            TileMap.spriteFont = Content.Load<SpriteFont>(@"Fonts\Pericles8");
            lastMouseState = Mouse.GetState();
            pictureBox_sizeChanged(null, null);
        }
        protected override void UnloadContent() {

        }
        protected override void Update(GameTime gameTime) 
        {
            Camera.Position = new Vector2(horizontalScroll.Value, verticalScroll.Value);
            MouseState ms = Mouse.GetState();
                if ((ms.X > 0) && (ms.Y > 0) && 
                   (ms.X < Camera.ViewPortWidth) && 
                   (ms.Y < Camera.ViewPortHeight)) 
             { 
                   Vector2 mouseLoc = Camera.ScreenToWorld( 
                        new Vector2(ms.X, ms.Y)); 

                   if (Camera.WorldRectangle.Contains( 
                        (int)mouseLoc.X, (int)mouseLoc.Y)) 
                   { 
                     if (ms.LeftButton == ButtonState.Pressed) 
                     { 
                        TileMap.SetTileAtCell( 
                          TileMap.GetCellByPixelX((int)mouseLoc.X), 
                          TileMap.GetCellByPixelY((int)mouseLoc.Y), 
                          paintLayer, 
                          paintTile); 
                     } 

                     if ((ms.RightButton == ButtonState.Pressed) && 
                          (lastMouseState.RightButton == 
                               ButtonState.Released)) 
                     { 
                        if (codeEditing) 
                        { 
                            TileMap.GetMapSquareAtCell(TileMap.GetCellByPixelX((int)mouseLoc.X),
                                TileMap.GetCellByPixelY((int)mouseLoc.Y)).CodeValue = CurrentCodeValue;
                        }
                        else
                        {
                            TileMap.GetMapSquareAtCell(TileMap.GetCellByPixelX((int)mouseLoc.X),
                                                       TileMap.GetCellByPixelY((int)mouseLoc.Y)).TogglePassable();
                        }
                    }
                    hCodeVal = TileMap.GetMapSquareAtCell(TileMap.GetCellByPixelX((int)mouseLoc.X), TileMap.GetCellByPixelY((int)mouseLoc.Y)).CodeValue;
                }
            }
            lastMouseState = ms;
            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            TileMap.Draw(spriteBatch);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #region Event Handlers
        private void gameForm_VisibleChanged(object sender, EventArgs e)
        {
            if(gameForm.Visible == true)
                gameForm.Visible = false;
        }
        void pictureBox_sizeChanged(object sender, EventArgs e)
        {
            if(parentForm.WindowState != System.Windows.Forms.FormWindowState.Minimized)
            {
                graphics.PreferredBackBufferWidth = pictureBox.Width;
                graphics.PreferredBackBufferHeight = pictureBox.Height;
                Camera.ViewPortWidth = pictureBox.Width;
                Camera.ViewPortHeight = pictureBox.Height;
                graphics.ApplyChanges();
            }
        }
        #endregion
    }
}
