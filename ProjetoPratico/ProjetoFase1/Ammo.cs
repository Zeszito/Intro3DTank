using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFase1
{
    class Ammo
    {
        Vector3 direction;
        Matrix rotationMatrix;
        float rotationHorizontal;
        float rotationVertical;
        float speed;
        Model myModel;
        float scale;
        Vector3 position;
        Matrix viewWorld;
        BasicEffect effect;
        Matrix[] bonetransforms;


        public Ammo(Vector3 position, Vector3 direction, Matrix rotationMatrix, float rotationHorizontal, float rotationVertical, GraphicsDevice device)
        {
            this.position = new Vector3(position.X,position.Y, position.Z);
            this.direction = direction;
            this.rotationHorizontal = rotationHorizontal;
            this.rotationVertical = rotationVertical;
            this.rotationMatrix = rotationMatrix;
            speed = 0.25f;
            myModel = Game1.content.Load<Model>("bala");
            viewWorld = Matrix.Identity;
            scale = 0.006f;
            effect = new BasicEffect(device);
            bonetransforms = new Matrix[myModel.Bones.Count];
            myModel.Root.Transform = Matrix.CreateTranslation(position);

        }

        public void Update()
        {
            position += speed * -direction;
            Matrix translacao = Matrix.CreateTranslation(position);


            myModel.Root.Transform = (rotationMatrix * Matrix.CreateFromYawPitchRoll(rotationHorizontal, 0f, rotationVertical)) * Matrix.CreateScale(scale) * translacao;
            myModel.CopyAbsoluteBoneTransformsTo(bonetransforms);
        }

        public void Draw(Matrix viewMatrix, Matrix projection)
        {
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = bonetransforms[mesh.ParentBone.Index];
                    effect.View = viewMatrix;
                    effect.Projection = projection;

                    effect.EnableDefaultLighting();
                }
                mesh.Draw();
            }
        }
    }
}
