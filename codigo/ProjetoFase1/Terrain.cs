//Código desenvolvido pelos alunos Moisés Moreira, nº 13676 e Vasco Figueiredo, nº 13222, do curso EDJD 2º ano
//2017-2018 Introdução à Programação 3D
//1ª Fase do trabalho prático: Terreno e Câmera Surface Follow
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFase1
{
    public class Terrain
    {
        VertexBuffer vertexBuffer;
        IndexBuffer indexBuffer;
        public Matrix worldMatrix;
        BasicEffect effect;
        VertexPositionNormalTexture[] vertices;

        Texture2D texture, texture2;
        Color[] texels;

        int x;
        int z;
        float y;
        float scale = 0.02f;

        int vertexCount;
        int indexCount;
        short[] indices;
        float[] listHeights;
        Vector3[] listNormals;


        public Terrain(GraphicsDevice device, ContentManager content)
        {
            effect = new BasicEffect(device);
            texture = content.Load<Texture2D>("ground");
            texture2 = content.Load<Texture2D>("grass");
            worldMatrix = Matrix.Identity;//cam.effect.World;



            float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;

            //Multiplicar as Matrizes
            //effect.View = Matrix.CreateLookAt(new Vector3(100f, 5f, 20f), Vector3.Zero, Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.001f, 1000.0f);

            effect.LightingEnabled = true; // turn on the lighting subsystem.
                                           //effect.EnableDefaultLighting();

            effect.DirectionalLight0.DiffuseColor = new Vector3(2f, 2f, 2f); // a red light
            effect.DirectionalLight0.Direction = new Vector3(1, 0f, 0);  // coming along the x-axis
            //effect.DirectionalLight0.SpecularColor = new Vector3(0, 150, 0); // with green highlights
            effect.AmbientLightColor = new Vector3(1f, 1f, 1f);
            //effect.EmissiveColor = new Vector3(150f, 150, 150);



            effect.TextureEnabled = true;
            effect.Texture = texture;
            //effect.AmbientLightColor = new Vector3(2f, 3f, 2f);



            x = texture.Width;
            z = texture.Height;
            texels = new Color[x * z];

            // Cria os eixos 3D
            CreateTerrain(device);
        }

        public void CreateTerrain(GraphicsDevice device)
        {
            texture.GetData<Color>(texels);
            vertexCount = z * x;

            vertices = new VertexPositionNormalTexture[vertexCount];
            listHeights = new float[z * x];
            listNormals = new Vector3[z * x];
            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    y = texels[j * x + i].G * scale;
                    vertices[j * x + i] = new VertexPositionNormalTexture(new Vector3(i, y, j), Vector3.Up, new Vector2(i % 2, j % 2));
                    listHeights[j * x + i] = y;
                }
            }

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < z; j++)
                {
                    if (i == 0 && j != 0 && j != z - 1) //Lado Esquerdo
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position));
                        //normal /= 4;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (i == x - 1 && j != 0 && j != z - 1) //Lado Direito
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position));
                        //normal /= 4;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (j == 0 && i != 0 && i != x - 1) //Lado Cima
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position));
                        //normal /= 4;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (j == z - 1 && i != 0 && i != x - 1) //Lado Baixo
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position));
                        //normal /= 4;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else if (i != 0 && i != x - 1 && j != 0 && j != z - 1) //Centro
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position));
                        //normal /= 8;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (i == 0 && j == 0) //Canto superior Esquerdo
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position));
                        //normal /= 2;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (i == x - 1 && j == z - 1) //Canto inferior Direito
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position));
                        //normal /= 2;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    else
                    if (i == x - 1 && j == 0) //Canto superior Direito
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j + 1) * x + (i - 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j + 1) * x + i].Position - vertices[j * x + i].Position));
                        //normal /= 2;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                    if (i == 0 && j == z - 1) //Canto inferior Esquerdo
                    {
                        Vector3 normal = Vector3.Zero;
                        normal += Vector3.Cross(Vector3.Normalize(vertices[j * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position));
                        normal += Vector3.Cross(Vector3.Normalize(vertices[(j - 1) * x + (i + 1)].Position - vertices[j * x + i].Position), Vector3.Normalize(vertices[(j - 1) * x + i].Position - vertices[j * x + i].Position));
                        //normal /= 2;
                        normal = Vector3.Normalize(normal);
                        vertices[j * x + i].Normal = normal;
                        listNormals[j * x + i] = normal;
                    }
                }
            }


            vertexBuffer = new VertexBuffer(device, typeof(VertexPositionNormalTexture), vertices.Length, BufferUsage.None);
            vertexBuffer.SetData<VertexPositionNormalTexture>(vertices);

            indexCount = (z * 2) * (x - 1);
            indices = new short[indexCount];
            for (int i = 0; i < z - 1; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    indices[2 * j + 0 + i * 2 * z] = (short)(j * x + i);
                    indices[2 * j + 1 + i * 2 * z] = (short)(j * x + 1 + i);
                }
            }
            indexBuffer = new IndexBuffer(device, typeof(short), indices.Length, BufferUsage.None);
            indexBuffer.SetData<short>(indices);

        }


        //Este método é responsável por cálcular a altura da câmera em relação ao terreno
        //Este método é responsável por cálcular a altura da câmera em relação ao terreno
        public float CalculateInterpolation(float cameraX, float cameraZ, float floatingOverGround)
        {
            float overGround = floatingOverGround;
            float outsideBounderiesHeight = 10f;
            //No entanto, para prevenir que dê erro quando a câmera sair do terreno, criamos um if para confirmar se esta se encontra dentro do terreno
            if (cameraX > 0 && cameraZ > 0 && cameraX < x - 1 && cameraZ < z - 1)
            {
                //Para cálcular a altura da câmera são criados os 4 vértices que estão à volta da câmera. 
                //A posição dos vértices é a seguinte:
                /* vertex1                             vertex2
                 * 
                 *            Posição da Câmera
                 * 
                 * vertex3                             vertex4  
                */
                VertexPositionNormalTexture vertex1, vertex2, vertex3, vertex4;
                vertex1 = vertices[(int)(cameraZ) * x + (int)(cameraX)];
                vertex2 = vertices[(int)(cameraZ) * x + (int)(cameraX + 1)];
                vertex3 = vertices[(int)(cameraZ + 1) * x + (int)(cameraX)];
                vertex4 = vertices[(int)(cameraZ + 1) * x + (int)(cameraX + 1)];
                //Também são criadas as variáveis dos pesos, da altura em cada interpolação e da variável que vai adicionar altura à câmera, para esta não ficar dentro do chão
                float peso1, peso2;
                float inter1, inter2, interFinal;

                //O peso serve para ver de quais vértices a câmera está mais próxima.

                //Interpolação 1: eixo dos X parte de cima
                peso1 = cameraX - vertex1.Position.X;
                peso2 = 1 - peso1;

                inter1 = listHeights[(int)vertex1.Position.Z * x + (int)vertex1.Position.X] * peso2 + listHeights[(int)vertex2.Position.Z * x + (int)vertex2.Position.X] * peso1;

                //Interpolação 2: eixo do X parte de baixo
                inter2 = listHeights[(int)vertex3.Position.Z * x + (int)vertex3.Position.X] * peso2 + listHeights[(int)vertex4.Position.Z * x + (int)vertex4.Position.X] * peso1;

                //Interpolação 3: eixo dos Z entre as duas ultimas interpolações
                peso1 = cameraZ - vertex1.Position.Z;
                peso2 = 1 - peso1;

                interFinal = inter1 * peso2 + inter2 * peso1;

                //Retorna a altura da câmera
                return interFinal + overGround;
            }
            //retorna uma altura da câmera pré-definida caso esta não esteja dentro do terreno
            return outsideBounderiesHeight;

        }

        public Vector3 CalculateInterpolationNormal(float cameraX, float cameraZ)
        {
           
            //No entanto, para prevenir que dê erro quando a câmera sair do terreno, criamos um if para confirmar se esta se encontra dentro do terreno
            if (cameraX > 0 && cameraZ > 0 && cameraX < x - 1 && cameraZ < z - 1)
            {
                //Para cálcular a altura da câmera são criados os 4 vértices que estão à volta da câmera. 
                //A posição dos vértices é a seguinte:
                /* vertex1                             vertex2
                 * 
                 *            Posição da Câmera
                 * 
                 * vertex3                             vertex4  
                */
                VertexPositionNormalTexture vertex1, vertex2, vertex3, vertex4;
                vertex1 = vertices[(int)(cameraZ) * x + (int)(cameraX)];
                vertex2 = vertices[(int)(cameraZ) * x + (int)(cameraX + 1)];
                vertex3 = vertices[(int)(cameraZ + 1) * x + (int)(cameraX)];
                vertex4 = vertices[(int)(cameraZ + 1) * x + (int)(cameraX + 1)];
                //Também são criadas as variáveis dos pesos, da altura em cada interpolação e da variável que vai adicionar altura à câmera, para esta não ficar dentro do chão
                float peso1, peso2;
                Vector3 inter1, inter2, interFinal;


                //O peso serve para ver de quais vértices a câmera está mais próxima.

                //Interpolação 1: eixo dos X parte de cima
                peso1 = cameraX - vertex1.Position.X;
                peso2 = 1 - peso1;

                inter1 = listNormals[(int)vertex1.Position.Z * x + (int)vertex1.Position.X] * peso2 + listNormals[(int)vertex2.Position.Z * x + (int)vertex2.Position.X] * peso1;

                //Interpolação 2: eixo do X parte de baixo
                inter2 = listNormals[(int)vertex3.Position.Z * x + (int)vertex3.Position.X] * peso2 + listNormals[(int)vertex4.Position.Z * x + (int)vertex4.Position.X] * peso1;

                //Interpolação 3: eixo dos Z entre as duas ultimas interpolações
                peso1 = cameraZ - vertex1.Position.Z;
                peso2 = 1 - peso1;

                interFinal = inter1 * peso2+ inter2 * peso1;

                //Retorna a altura da câmera
                return interFinal;
            }
            //retorna uma altura da câmera pré-definida caso esta não esteja dentro do terreno
            return Vector3.Up;

        }

        public void Draw(GraphicsDevice device, Matrix view)
        {
            effect.World = worldMatrix;
            //cam.effect.World *= worldMatrix;
            effect.Texture = texture2;
            effect.View = view;
            //effect.World = worldMatrix;
            device.Indices = indexBuffer;
            device.SetVertexBuffer(vertexBuffer);
            int Strips = texture.Width;
            effect.CurrentTechnique.Passes[0].Apply();
            for (int i = 0; i < Strips - 1; i++)
            {
                device.DrawIndexedPrimitives(PrimitiveType.TriangleStrip, 0, z * 2 * i, 2 * z - 2);
            }

        }
    }
}
