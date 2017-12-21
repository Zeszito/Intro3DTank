//Código desenvolvido pelos alunos Moisés Moreira, nº 13676 e Vasco Figueiredo, nº 13222, do curso EDJD 2º ano
//2017-2018 Introdução à Programação 3D
//1ª Fase do trabalho prático: Terreno e Câmera Surface Follow
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace ProjetoFase1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //Terreno e Câmera
        public Terrain terrain;
        Camera cam_surfaceFollow;
        Camera cam_freeview;
        Camera cam_thirdperson;

        Camera cameraMundo;
        Camera cameraMundo2;

        Viewport viewPort1;
        Viewport viewPort2;
        Viewport defaultViewport;
        Matrix projectionMatrix;
        Matrix halfprojectionMatrix;

        Tank tank;
        Tank tank2;
        List<Sphere> sphere;
        public static ContentManager content;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            content = Content;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            //viewPort1 = new Viewport();
            //viewPort1.X = 0;
            //viewPort1.Y = 0;
            //viewPort1.Width = GraphicsDevice.Viewport.Width;
            //viewPort1.Height = GraphicsDevice.Viewport.Height / 2;
            //viewPort1.MinDepth = 0;
            //viewPort1.MaxDepth = 1;

            //viewPort2 = new Viewport();
            //viewPort2.X = 0;
            //viewPort2.Y = GraphicsDevice.Viewport.Height / 2;
            //viewPort2.Width = GraphicsDevice.Viewport.Width;
            //viewPort2.Height = GraphicsDevice.Viewport.Height;
            //viewPort2.MinDepth = 0;
            //viewPort2.MaxDepth = 1;

            defaultViewport = graphics.GraphicsDevice.Viewport;
            viewPort1 = defaultViewport;
            viewPort2 = defaultViewport;
            viewPort1.Width = viewPort1.Width / 2;
            viewPort2.Width = viewPort2.Width / 2;
            viewPort2.X = viewPort1.Width + 1;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 4.0f / 3.0f, 1.0f, 10000f);
            halfprojectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 2.0f / 3.0f, 1.0f, 10000f);

        
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            cam_surfaceFollow = new Camera(GraphicsDevice, CameraType.SurfaceFollow);
            cam_freeview = new Camera(GraphicsDevice, CameraType.FreeView);
            cam_thirdperson = new Camera(GraphicsDevice, CameraType.ThirdPerson);
            cameraMundo = cam_surfaceFollow;
            cameraMundo2 = new Camera(GraphicsDevice, CameraType.ThirdPerson);
            terrain = new Terrain(GraphicsDevice, content);
            tank = new Tank(GraphicsDevice, content, new Vector3(64, 0, 64), cameraMundo.projection);
            tank2 = new Tank(GraphicsDevice, content, new Vector3(60, 0, 60), cameraMundo.projection);
            sphere = new List<Sphere>();
            sphere.Add(tank.staticSphere);
            sphere.Add(tank2.staticSphere);
            // TODO: use this.Content to load your game content here
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            //Lê ps inputs do teclado e do rato e manda-os para a câmera
            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            #region Cameras
            if (keyState.IsKeyDown(Keys.NumPad1))
            {
                cameraMundo = cam_surfaceFollow;
            }

            if (keyState.IsKeyDown(Keys.NumPad2))
            {
                cameraMundo = cam_freeview;
            }

            if (keyState.IsKeyDown(Keys.NumPad3))
            {
                cameraMundo = cam_thirdperson;
            }
            cameraMundo.UpdateMove(GraphicsDevice, keyState, mouseState, terrain, tank, tank2);
            cameraMundo2.UpdateMove(GraphicsDevice, keyState, mouseState, terrain, tank2, tank);

            #endregion

            #region Tanques
            tank.Update(keyState, terrain, Keys.W, Keys.S, Keys.A,Keys.D, Keys.Right, Keys.Left, Keys.Up, Keys.Down, Keys.Space, sphere,null);
            tank2.Update(keyState, terrain, Keys.I, Keys.K, Keys.J, Keys.L, Keys.O, Keys.P, Keys.U,Keys.H, Keys.Enter, sphere, null);
            #endregion

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
          
            //Desenha o terreno

            if (cameraMundo.cameraType == CameraType.ThirdPerson)
            {
                Viewport original = graphics.GraphicsDevice.Viewport;

                graphics.GraphicsDevice.Viewport = viewPort1;
                tank.Draw(cameraMundo.viewMatrix, halfprojectionMatrix);
                tank2.Draw(cameraMundo.viewMatrix, halfprojectionMatrix);
                terrain.Draw(GraphicsDevice, cameraMundo.viewMatrix, halfprojectionMatrix);

                graphics.GraphicsDevice.Viewport = viewPort2;
                tank2.Draw(cameraMundo2.viewMatrix, halfprojectionMatrix);
                tank.Draw(cameraMundo2.viewMatrix, halfprojectionMatrix);
                terrain.Draw(GraphicsDevice, cameraMundo2.viewMatrix, halfprojectionMatrix);

                //GraphicsDevice.Viewport = defaultViewport;
                //tank.Draw(cameraMundo.viewMatrix, cameraMundo.projection);
                //tank2.Draw(cameraMundo.viewMatrix, cameraMundo.projection);

                //terrain.Draw(GraphicsDevice, cameraMundo.viewMatrix);
            }
            else
            {
                GraphicsDevice.Viewport = defaultViewport;
                terrain.Draw(GraphicsDevice, cameraMundo.viewMatrix, cameraMundo.projection);
                tank.Draw(cameraMundo.viewMatrix, cameraMundo.projection);
                tank2.Draw(cameraMundo.viewMatrix, cameraMundo.projection);
            }
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
