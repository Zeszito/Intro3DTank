//Código desenvolvido pelos alunos Moisés Moreira, nº 13676 e Vasco Figueiredo, nº 13222, do curso EDJD 2º ano
//2017-2018 Introdução à Programação 3D
//1ª Fase do trabalho prático: Terreno e Câmera Surface Follow
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        Camera camSurfaceFollow;
        Tank tank;
        Tank tank2;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            camSurfaceFollow = new Camera(GraphicsDevice);
            terrain = new Terrain(GraphicsDevice, Content);
            tank = new Tank(GraphicsDevice, Content);
            tank2 = new Tank(GraphicsDevice, Content);
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
            tank.Update(keyState, terrain, Keys.W, Keys.S, Keys.A,Keys.D);
            tank2.Update(keyState, terrain, Keys.I, Keys.K, Keys.J, Keys.L);
            camSurfaceFollow.UpdateMove(keyState, mouseState, terrain);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
          
            //Desenha o terreno
            terrain.Draw(GraphicsDevice, camSurfaceFollow.viewMatrix);
            tank.Draw(camSurfaceFollow.viewMatrix);
            tank2.Draw(camSurfaceFollow.viewMatrix);
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
