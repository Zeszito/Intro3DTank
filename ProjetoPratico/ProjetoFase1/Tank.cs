using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProjetoFase1
{
    public class Tank
    {
        Model myModel;

        Matrix world;
        Matrix mundo;
        Matrix view;
        Matrix projection;
        BasicEffect effect;
        float scale;

        //Bones do tanque
        ModelBone turretBone;
        ModelBone cannonBone;
        ModelBone right_front_wheelbone, left_front_wheelbone, right_back_wheelbone, left_back_wheelbone;
        ModelBone right_steerBone, left_steerBone;

        //Matrizes dos bones
        Matrix cannonTransform;
        Matrix turretTransform;
        Matrix right_front_wheelTransform, left_front_wheelTransform, right_back_wheelTransform, left_back_wheelTransform;
        Matrix right_steerTransform, left_steerTransform;

        //Variáveis
        float turretAngle = 0.0f;
        float cannonAngle = 0f;
        float yaw = 0.00f;
        float vel = 0.1f;
        float wheelRotation = 0.0f;
        float wheelSideRotation = 0.0f;

        public Vector3 posicao = new Vector3(64, 10, 64);
        private Vector3 positionPrev;

        Vector3 normal = Vector3.Up;
        Vector3 direcaoBase = Vector3.UnitX;
        public Vector3 tankDir;
        public Vector3 ammoDir;
        Matrix[] bonetransforms;

        //Collisions
        public Sphere colSphere;
        public Sphere staticSphere;

        //Ammo
        List<Ammo> ammunition;
        float recharge = 0;
        bool canShoot = true;
        GraphicsDevice device;

        List<TargetLine> scopeLine;
        public bool ismoving = false;

        public Tank(GraphicsDevice device, ContentManager content, Vector3 PosicaoInicial, Matrix projection)
        {
            this.projection = projection;
            posicao = PosicaoInicial;
            myModel = content.Load<Model>("tank");
            effect = new BasicEffect(device);
            this.device = device;
            //float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
            view = Matrix.CreateLookAt(new Vector3(0.5f, 2.0f, 2.0f), Vector3.Zero, Vector3.Up);
          
            effect.LightingEnabled = true;
            myModel.Root.Transform = Matrix.CreateTranslation(posicao);

            scale = 0.003f;
            colSphere = new Sphere(1f, positionPrev);
            staticSphere = new Sphere(1f, posicao);
            ammunition = new List<Ammo>();
            scopeLine = new List<TargetLine>();

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

        public void Update(KeyboardState kb, Terrain terrain, Keys up, Keys down, Keys left, Keys right, Keys torreRight, Keys torreLeft, 
            Keys torreUp, Keys torreDown, Keys shootKey, List<Sphere> other)
        {
            //Esta parte do codigo vai calcular a direção do tanque
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

            tankDir = Vector3.Normalize(Vector3.Cross(tankNormal, tankRight));
            rotacao.Forward = tankDir;

          
            //#region Boids
            //if (presa != null)
            //{
            //    boidChase(presa);

               
            //}
         
               // #endregion

                #region Movimentos
                positionPrev = posicao;
                if (kb.IsKeyDown(up))
                {
                    positionPrev -= vel * tankDir;
                    colSphere.center = positionPrev;
                    foreach(Sphere sp in other)
                    {
                        if (colSphere.SphereOnSphere(sp) == false && (!(colSphere.Equals(sp))))
                        {
                            posicao -= vel * tankDir;
                            wheelRotation += 0.5f;
                            ismoving = true;
                        }
                    }
                  
                    colSphere.center = posicao;
                }
                if (kb.IsKeyDown(down))
                { 
                        positionPrev += vel * tankDir;
                        colSphere.center = positionPrev;
                    foreach (Sphere sp in other)
                    {
                        if (colSphere.SphereOnSphere(sp) == false && (!(colSphere.Equals(sp))))
                        {
                            posicao += vel * tankDir;
                            wheelRotation -= 0.5f;
                            ismoving = true;
                        }
                    }
                    colSphere.center = posicao;
                }


                if (kb.IsKeyDown(right))
                {
                    yaw -= MathHelper.ToRadians(1f);
                    wheelSideRotation -= MathHelper.ToRadians(1f);
                }
                if (kb.IsKeyDown(left))
                {
                    yaw += MathHelper.ToRadians(1f);
                    wheelSideRotation += MathHelper.ToRadians(1f);
                }
                if (kb.IsKeyUp(left) && kb.IsKeyUp(right))
                {
                    if (wheelSideRotation != 0)
                    {
                        if (wheelSideRotation > 0)
                            wheelSideRotation -= 0.05f;
                        if (wheelSideRotation < 0)
                            wheelSideRotation += 0.05f;
                    }
                }

                if (kb.IsKeyDown(torreLeft))
                {
                    turretAngle += MathHelper.ToRadians(1f);
                }
                if (kb.IsKeyDown(torreRight))
                {
                    turretAngle -= MathHelper.ToRadians(1f);
                }

                if (kb.IsKeyDown(torreUp))
                {
                    cannonAngle -= MathHelper.ToRadians(1f);
                }

                if (kb.IsKeyDown(torreDown))
                {
                    cannonAngle += MathHelper.ToRadians(1f);
                }
                ismoving = false;

      

                if (!canShoot)
            {
                recharge++;
            }

            if (recharge >= 100)
            {
                recharge = 0;
                canShoot = true;
            }

            foreach(Ammo ammo in ammunition)
            {
                ammo.Update();
            }


            staticSphere.center = posicao;

            //Controlo das rotações
            if (wheelSideRotation > 0.8f)
                wheelSideRotation = 0.8f;
            if (wheelSideRotation < -0.8f)
                wheelSideRotation = -0.8f;
            #endregion

            //Rotação e/ou movimentação das várias partes do tanque
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

            #region CannonRotation
            Vector3 torreDir = Vector3.Normalize(Vector3.Transform(tankDir, Matrix.CreateFromAxisAngle(tankNormal, turretAngle)));
            Vector3 torreR = Vector3.Normalize(Vector3.Cross(torreDir, tankNormal));
            Vector3 cannonDir = Vector3.Normalize(Vector3.Transform(torreDir, Matrix.CreateFromAxisAngle(torreR, cannonAngle)));
            #endregion

            TargetLine p = new TargetLine(device, new Vector3(posicao.X + 0.04f, posicao.Y, posicao.Z - 0.05f) + new Vector3(0, 1f, 0f), 15, -cannonDir, 200);
            scopeLine.Add(p);

            if (kb.IsKeyDown(shootKey))
            {
                if (recharge == 0 && canShoot)
                {
                    //Vector3 ammoDirection = Vector3.Transform(tankDir, Matrix.CreateRotationY(turretAngle));
                    Ammo ammo = new Ammo(new Vector3(posicao.X + 0.08f, posicao.Y, posicao.Z - 0.09f) + new Vector3(0f, 1f, 0), cannonDir, rotacao, turretAngle, cannonAngle, device);
                    ammunition.Add(ammo);
                    canShoot = false;
                }
            }

            for (int i = scopeLine.Count() - 1; i > -1; i--)
            {
                if (scopeLine[i].isAlive == false)
                {
                    scopeLine[i] = null;
                    scopeLine.RemoveAt(i);
                }
            }

            foreach (TargetLine t in scopeLine)
            {
                t.Update();
            }

        }

    //public void boidChase(Tank tankAPresseguir)
    //    {
    //        Vector3 distancia;
    //        Vector3 velocidade;
    //        Vector3 pontoIntercepcao;
    //        float tempoColisao;


    //        //posicao -= vel * tankDir;
    //        wheelRotation += 0.5f;

    //        if (tankAPresseguir.ismoving)
    //        {
    //            velocidade = (tankAPresseguir.vel * tankAPresseguir.tankDir) - (vel * tankDir); //Velocidde
    //            distancia = tankAPresseguir.posicao - posicao;
    //            tempoColisao = distancia.Length() / velocidade.Length();
    //            pontoIntercepcao = tankAPresseguir.posicao + ((tankAPresseguir.posicao * tankAPresseguir.vel) * tempoColisao);

    //            //double angloFormado = Math.Acos(Vector2.Dot((new Vector2(posicao.X, posicao.Z)), (new Vector2(pontoIntercepcao.X, pontoIntercepcao.Y))));
    //            //double angloFormado = Math.Atan2(pontoIntercepcao.Z - posicao.Z, pontoIntercepcao.X - posicao.X);
    //            yaw = (float)Math.Acos((float)(pontoIntercepcao.X * tankDir.X + pontoIntercepcao.Z * tankDir.Z)
    //              /((float)Math.Sqrt((pontoIntercepcao.X * pontoIntercepcao.X) + (pontoIntercepcao.Z * pontoIntercepcao.Z)) * (float)Math.Sqrt((tankDir.Z * tankDir.Z) + (tankDir.X * tankDir.X))));
    //            Matrix rotacao = Matrix.CreateFromYawPitchRoll(yaw, 0, 0);

    //            rotacao = Matrix.Identity;

    //            Vector3 direcaoHorizontal = Vector3.Transform(direcaoBase, rotacao);
    //            Vector3 tankNormal = Vector3.Normalize(normal);
    //            rotacao.Up = tankNormal;

    //            Vector3 tankRight = Vector3.Normalize(Vector3.Cross(direcaoHorizontal, tankNormal));
    //            rotacao.Right = tankRight;

    //            tankDir = Vector3.Normalize(Vector3.Cross(tankNormal, tankRight));
    //            rotacao.Forward = tankDir;
    //            //tankDir = -pontoIntercepcao;
    //            // posicao -= vel * tankDir;
    //        }
  
        
    //        //criar um vector tank a ponto, comprar coma direccao e se for maior direita se menor esquerda, ver o angulo entre eles
    //        //Vector3 vectorTankPonto = pontoIntercepcao - posicao;
    //        //double angloFormado = Math.Atan2(pontoIntercepcao.Z - posicao.Z , pontoIntercepcao.X - posicao.X);
    //        //istop podia se passar como YAW
           
    //        //double normli = Math.Sqrt(pontoIntercepcao.X * pontoIntercepcao.X + pontoIntercepcao.Z * pontoIntercepcao.Z);
    //        //tankDir.X = (float)(posicao.X - pontoIntercepcao.X) / (float)(normli);
    //        //tankDir.Z = (float)(posicao.Z - pontoIntercepcao.Z) / (float)(normli);
      


    //    }

        public void updateCacador(KeyboardState kb, Terrain terrain, Keys up, Keys down, Keys left, Keys right, Keys torreRight, Keys torreLeft,
            Keys torreUp, Keys torreDown, Keys shootKey, List<Sphere> other, Tank tankAPresseguir)
        {
            //Esta parte do codigo vai calcular a direção do tanque
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

            tankDir = Vector3.Normalize(Vector3.Cross(tankNormal, tankRight));
            rotacao.Forward = tankDir;

            Vector3 distancia;
            Vector3 velocidade;
            Vector3 pontoIntercepcao;
            float tempoColisao;

            velocidade = (tankAPresseguir.vel * tankAPresseguir.tankDir) - (vel * tankDir); //Velocidde
            distancia = tankAPresseguir.posicao - posicao;
            tempoColisao = 0.2f;
            pontoIntercepcao = tankAPresseguir.posicao + ((tankAPresseguir.posicao * tankAPresseguir.vel) *tempoColisao );

            //double angloFormado = Math.Acos(Vector2.Dot((new Vector2(posicao.X, posicao.Z)), (new Vector2(pontoIntercepcao.X, pontoIntercepcao.Y))));
            //double angloFormado = Math.Atan2(pontoIntercepcao.Z - posicao.Z, pontoIntercepcao.X - posicao.X);
           

            double normli = Math.Sqrt(pontoIntercepcao.X * pontoIntercepcao.X + pontoIntercepcao.Z * pontoIntercepcao.Z);
            //yaw *=  (float)normli;
            //tankDir.X = (float)((posicao.X - pontoIntercepcao.X) * (float)(normli) * 2*Math.PI);
            //tankDir.Z = (float)((posicao.Z - pontoIntercepcao.Z) * (float)(normli) * 2*Math.PI);
            //tankDir.Y = 0;
            yaw = -(float) Math.Atan2(((posicao.Z - pontoIntercepcao.Z) * (float)(normli) * 2 * Math.PI) - posicao.Z, (float)((posicao.X - pontoIntercepcao.X) * (float)(normli) * 2 * Math.PI) - posicao.X);


            if(Math.Abs(tankAPresseguir.posicao.X- posicao.X) >   3) //distancia de segurancao
            {
                posicao -= vel * tankDir;
            }
       

            //Rotação e/ou movimentação das várias partes do tanque
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

            #region CannonRotation
            Vector3 torreDir = Vector3.Normalize(Vector3.Transform(tankDir, Matrix.CreateFromAxisAngle(tankNormal, turretAngle)));
            Vector3 torreR = Vector3.Normalize(Vector3.Cross(torreDir, tankNormal));
            Vector3 cannonDir = Vector3.Normalize(Vector3.Transform(torreDir, Matrix.CreateFromAxisAngle(torreR, cannonAngle)));
            #endregion

            TargetLine p = new TargetLine(device, new Vector3(posicao.X + 0.04f, posicao.Y, posicao.Z - 0.05f) + new Vector3(0, 1f, 0f), 15, -cannonDir, 200);
            scopeLine.Add(p);

            if (kb.IsKeyDown(shootKey))
            {
                if (recharge == 0 && canShoot)
                {
                    //Vector3 ammoDirection = Vector3.Transform(tankDir, Matrix.CreateRotationY(turretAngle));
                    Ammo ammo = new Ammo(new Vector3(posicao.X + 0.08f, posicao.Y, posicao.Z - 0.09f) + new Vector3(0f, 1f, 0), cannonDir, rotacao, turretAngle, cannonAngle, device);
                    ammunition.Add(ammo);
                    canShoot = false;
                }
            }

            for (int i = scopeLine.Count() - 1; i > -1; i--)
            {
                if (scopeLine[i].isAlive == false)
                {
                    scopeLine[i] = null;
                    scopeLine.RemoveAt(i);
                }
            }

            foreach (TargetLine t in scopeLine)
            {
                t.Update();
            }

        }
        public void Draw(Matrix view2, Matrix proj)
        {
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = bonetransforms[mesh.ParentBone.Index];
                    effect.View = view2;
                    effect.Projection = proj;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }

            foreach (Ammo ammo in ammunition)
            {
                ammo.Draw(view2,proj);
            }

            foreach (TargetLine t in scopeLine)
            {
                t.Draw(device,proj,view2);
            }
        }
    }
}
