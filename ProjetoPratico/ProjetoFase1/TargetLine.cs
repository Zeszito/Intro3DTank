using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFase1
{
    class TargetLine
    {
        BasicEffect effect;
        Matrix worldMatrix;

        VertexPositionColor[] vertices;
        int vertexCount = 2;
        public Vector3 position_new;
        private Vector3 position_old;
        float timeStamp;
        float speed;
        Vector3 direction;
        public bool isAlive;

        public TargetLine(GraphicsDevice device, Vector3 intialPosition, float speed, Vector3 direction, int lifeSpan)
        {
            worldMatrix = Matrix.Identity;
            effect = new BasicEffect(device);

            position_new = intialPosition;
            this.speed = speed;
            this.direction = direction;
            timeStamp = lifeSpan;
            isAlive = true;
        }

        public void Update()
        {
            if (isAlive == true)
            {
                //Movimento da particula
                //A partícula é desenha através de uma linha com dois vertices: a posição antiga e a nova. Estas posições são atualizados a cada frame
                vertices = new VertexPositionColor[vertexCount];
                position_old = position_new;
                vertices[0] = new VertexPositionColor(position_old, Color.Red);
                position_new += speed * direction;
                vertices[1] = new VertexPositionColor(position_new, Color.Red);
                //A cada frame as partículas perdem tempo de vida. Chegando a 0 a particula é considerada morta.
                timeStamp -= 2.5f;

                if (position_new.Y < -1)
                {
                    isAlive = false;
                }

                if (timeStamp < 0)
                {
                    isAlive = false;
                }
            }
        }

        //A particula é desenhada no ecrâ
        public void Draw(GraphicsDevice device, Matrix projection, Matrix view)
        {
            if (isAlive == true)
            {
                effect.World = worldMatrix;
                effect.View = view;
                effect.Projection = projection;

                effect.CurrentTechnique.Passes[0].Apply();
                device.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, vertexCount - 1);
            }
        }
    }
}

