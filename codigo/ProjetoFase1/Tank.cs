using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjetoFase1
{
    class Tank
    {
        Model myModel;

        Matrix world;
        Matrix mundo;
        Matrix view;
        Matrix projection;
        BasicEffect effect;
        float scale;

        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone right_front_wheelbone, left_front_wheelbone, right_back_wheelbone, left_back_wheelbone;
        ModelBone right_steerBone, left_steerBone;

        Matrix cannonTransform;
        Matrix turretTransform;
        Matrix right_front_wheelTransform, left_front_wheelTransform, right_back_wheelTransform, left_back_wheelTransform;
        Matrix right_steerTransform, left_steerTransform;

        float turretAngle = 0.0f;
        float cannonAngle = 0f;
        float yaw = 0.00f;
        float vel = 0.1f;
        float wheelRotation = 0.0f;
        float wheelSideRotation = 0.0f;
        Vector3 posicao = new Vector3(64, 10, 64);
        Vector3 normal = Vector3.Up;
        Vector3 direcaoBase = Vector3.UnitX;
        //Matrix rotacao = Matrix.Identity;

        Matrix[] bonetransforms;

        public Tank(GraphicsDevice device, ContentManager content, Vector3 PosicaoInicial, Matrix projection)
        {
            this.projection = projection;
            posicao = PosicaoInicial;
            myModel = content.Load<Model>("tank");
            effect = new BasicEffect(device);
            //float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            view = Matrix.CreateLookAt(new Vector3(0.5f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
          
            effect.LightingEnabled = true;
            myModel.Root.Transform = Matrix.CreateTranslation(posicao);

            scale = 0.003f;

            turretBone = myModel.Bones["turret_geo"];
            cannonBone = myModel.Bones["canon_geo"];

            right_front_wheelbone = myModel.Bones["r_front_wheel_geo"];
            left_front_wheelbone = myModel.Bones["l_front_wheel_geo"];
            right_back_wheelbone = myModel.Bones["r_back_wheel_geo"];
            left_back_wheelbone = myModel.Bones["l_back_wheel_geo"];

            right_steerBone = myModel.Bones["r_steer_geo"];
            left_steerBone = myModel.Bones["l_steer_geo"];

            turretTransform = turretBone.Transform;
            cannonTransform = cannonBone.Transform;

            right_front_wheelTransform = right_front_wheelbone.Transform;
            left_front_wheelTransform = left_front_wheelbone.Transform;
            right_back_wheelTransform = right_back_wheelbone.Transform;
            left_back_wheelTransform = left_back_wheelbone.Transform;

            right_steerTransform = right_steerBone.Transform;
            left_steerTransform = left_steerBone.Transform;

            bonetransforms = new Matrix[myModel.Bones.Count];

            effect.LightingEnabled = true; // turn on the lighting subsystem.
                                           //effect.EnableDefaultLighting();

            effect.DirectionalLight0.DiffuseColor = new Vector3(2f, 2f, 2f); // a red light
            effect.DirectionalLight0.Direction = new Vector3(1, 0f, 0);  // coming along the x-axis
            //effect.DirectionalLight0.SpecularColor = new Vector3(0, 150, 0); // with green highlights
            effect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            //effect.EmissiveColor = new Vector3(150f, 150, 150);
        }

        public void Update(KeyboardState kb, Terrain terrain, Keys up, Keys down, Keys left, Keys rigth, Keys torreRight, Keys torreLeft, Keys torreUp, Keys torreDown)
        {
            Matrix rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);
            Vector3 direcaoHorizontal = Vector3.Transform(direcaoBase, rotacao);

            posicao.Y = terrain.CalculateInterpolation(posicao.X, posicao.Z, 0f);
            normal = terrain.CalculateInterpolationNormal(posicao.X, posicao.Z);

            Matrix translacao = Matrix.CreateTranslation(posicao);

            rotacao = Matrix.Identity;

            Vector3 tankNormal = Vector3.Normalize(normal);
            rotacao.Up = tankNormal;

            Vector3 tankRight = Vector3.Normalize(Vector3.Cross(direcaoHorizontal, tankNormal));
            rotacao.Right = tankRight;

            Vector3 tankDir = Vector3.Normalize(Vector3.Cross(tankNormal, tankRight));
            rotacao.Forward = tankDir;


            if (kb.IsKeyDown(up))
            {
                posicao -= vel * tankDir;
                wheelRotation += 0.5f;
            }
            if (kb.IsKeyDown(down))
            {
                posicao += vel * tankDir;
                wheelRotation -= 0.5f;
            }

            if (kb.IsKeyDown(rigth))
            {
                yaw -= MathHelper.ToRadians(1f);
                wheelSideRotation -= MathHelper.ToRadians(1f);
                wheelRotation += 0.25f;
            }
            if (kb.IsKeyDown(left))
            {
                yaw += MathHelper.ToRadians(1f);
                wheelSideRotation += MathHelper.ToRadians(1f);
                wheelRotation += 0.25f;
            }
            if (kb.IsKeyUp(left) && kb.IsKeyUp(rigth))
            {
                if (wheelSideRotation != 0)
                {
                    if (wheelSideRotation > 0)
                        wheelSideRotation -= 0.05f;
                    if (wheelSideRotation < 0)
                        wheelSideRotation += 0.05f;
                }
                //wheelSideRotation -= wheelSideRotation;
            }

            if (kb.IsKeyDown(torreLeft))
            {
                turretAngle -= MathHelper.ToRadians(1f);
            }
            if (kb.IsKeyDown(torreRight))
            {
                turretAngle += MathHelper.ToRadians(1f);
            }

            if (kb.IsKeyDown(torreUp))
            {
                cannonAngle -= MathHelper.ToRadians(1f);
            }
            if (kb.IsKeyDown(torreDown))
            {
                cannonAngle += MathHelper.ToRadians(1f);
            }

            //Controlo das rotações
            if (wheelSideRotation > 0.8f)
                wheelSideRotation = 0.8f;
            if (wheelSideRotation < -0.8f)
                wheelSideRotation = -0.8f;


            myModel.Root.Transform = rotacao * Matrix.CreateScale(scale) * translacao;
            turretBone.Transform = Matrix.CreateRotationY(turretAngle) * turretTransform;
            cannonBone.Transform = Matrix.CreateRotationX(cannonAngle) * cannonTransform;

            left_back_wheelbone.Transform = Matrix.CreateRotationX(wheelRotation) * left_back_wheelTransform;
            right_back_wheelbone.Transform = Matrix.CreateRotationX(wheelRotation) * right_back_wheelTransform;
            left_front_wheelbone.Transform = Matrix.CreateRotationX(wheelRotation) * left_front_wheelTransform;
            right_front_wheelbone.Transform = Matrix.CreateRotationX(wheelRotation) * right_front_wheelTransform;

            right_steerBone.Transform = Matrix.CreateRotationY(wheelSideRotation) * right_steerTransform;
            left_steerBone.Transform = Matrix.CreateRotationY(wheelSideRotation) * left_steerTransform;

            myModel.CopyAbsoluteBoneTransformsTo(bonetransforms);
            //mundo = Matrix.CreateWorld(posicao, rotacao.Forward, rotacao.Up);

        }



        public void Draw(Matrix view2)
        {
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = bonetransforms[mesh.ParentBone.Index];
                    effect.View = view2;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
