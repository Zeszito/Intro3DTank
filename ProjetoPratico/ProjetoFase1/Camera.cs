//Código desenvolvido pelos alunos Moisés Moreira, nº 13676 e Vasco Figueiredo, nº 13222, do curso EDJD 2º ano
//2017-2018 Introdução à Programação 3D
//1ª Fase do trabalho prático: Terreno e Câmera Surface Follow
using Microsoft.Xna.Framework;
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
    public enum CameraType { SurfaceFollow, FreeView, ThirdPerson }; //Aqui estão definidos os vários tipos de câmera
    public class Camera
    {
        public Matrix viewMatrix;//Matriz final a passar ao terreno para a sua effect.view 
        public Matrix viewMatrix2;
        public Vector3 camPosition; //Posicao da Camera nos 3 eixos
        Vector3 direction;
        private float aspectRatio;
        public Matrix projection;
        public Matrix projection2;
        public CameraType cameraType;

        float yaw, roll, vel; // yaw e pitch vão adquirir valores de acordo com o rato, Vel e apenas para ajustar a velocidade da camera
        Matrix mDirecao;// corresponde à direção da camera

        //Normais e receptores de normais, que são guias para entender qual lado a camera está virado e assim mudar
        //o comportmanto dos inputs de forma a ajustar a direção do das teclas com a direção da camera
        Vector3 NormalZ = Vector3.Backward;
        Vector3 NormalX = Vector3.Right;
        Vector3 turnX; Vector3 turnZ;

       
        public Camera(GraphicsDevice device, CameraType cType)
        {
            aspectRatio = (float)device.Viewport.Width /
              device.Viewport.Height; //fixo

            //-------------------------//
            viewMatrix = Matrix.Identity;
            camPosition = new Vector3(64f, 5f, 64f);
            direction = new Vector3(1, 0, 0);
            vel = 0.3f;
            //-------------------------//
            viewMatrix = Matrix.CreateLookAt(
            camPosition, //este varia conforme percorremos o terreno
            camPosition + direction, //Da a orientação da camera
            Vector3.Up);
            projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 0.001f, 1000.0f);
            cameraType = cType;

        }
        public void UpdateMove(GraphicsDevice device, KeyboardState key, MouseState state, Terrain terrain, Tank tank, Tank tank2) //Recebe os inputs do teclado, rato e recebe um terreno
        {
            //Recebe inputs do teclado e reage de acordo
            //Vector3 tankPosition = tank.posicao;
            if (!(cameraType == CameraType.ThirdPerson))
            {
                if (key.IsKeyDown(Keys.NumPad8)) camPosition += vel * turnX; //Frente
                if (key.IsKeyDown(Keys.NumPad5)) camPosition -= vel * turnX; //Trás
                if (key.IsKeyDown(Keys.NumPad4)) camPosition -= vel * turnZ; //Esquerda
                if (key.IsKeyDown(Keys.NumPad6)) camPosition += vel * turnZ; //Direita

                //Recebe os inputs do rato e processa-os
                yaw = MathHelper.ToRadians(-state.Position.X);// ve a posição do rato e transforma em radianos
                roll = MathHelper.ToRadians(-state.Position.Y);
                if (roll > MathHelper.ToRadians(89f))
                    roll = MathHelper.ToRadians(88f);

                if (roll < MathHelper.ToRadians(-89f))
                    roll = MathHelper.ToRadians(-88f);

                mDirecao = Matrix.CreateFromYawPitchRoll(yaw, 0.0f, roll);//Usa os inputs do rato e criar uma matriz que vai rodar um vector para uma certa posição
                direction = Vector3.Transform(NormalX, mDirecao); //direção final, que vamos somar a posição da camera para termos o Target

                //Cálculo final para o eixo do X e Z (movimentos horizontais)
                turnX = Vector3.Transform(NormalX, mDirecao);
                turnZ = Vector3.Transform(NormalZ, mDirecao);

                if (turnX.X > device.Viewport.Width)
                {
                    Mouse.SetPosition(0, 0);
                }

                //A posição Y da câmera vai ser adquirida atravéz de uma função do terreno que cálcula o Y de acordo com a posição da câmera no eixo XZ. Esta função é explicada mais na class Terrain
                if (cameraType == CameraType.SurfaceFollow)
                    camPosition.Y = terrain.CalculateInterpolation(camPosition.X, camPosition.Z, 2);

                //actualiza a Camera com os novos paramtros
                UpdateView(tank, tank2, device);
            }
            else
            {
                UpdateView(tank, tank2, device);
            }
        }

        public void UpdateView(Tank tank, Tank tank2, GraphicsDevice device) //Atualiza a viewMatrix
        {
            if (!(cameraType == CameraType.ThirdPerson))
            {
                viewMatrix = Matrix.CreateLookAt(
                camPosition,
                camPosition + direction, Vector3.Up);
            }
            else //Caso a câmera for a câmera em 3ª pessoa
            {
                aspectRatio = (float)(device.Viewport.Width) /
              (device.Viewport.Height);

                //Tank 1
                Vector3 tankPosition = tank.posicao;
                Vector3 tankDirection = tank.tankDir;
                viewMatrix = Matrix.CreateLookAt(new Vector3(0, tankPosition.Y / 2, 0) + new Vector3(tankPosition.X, tankPosition.Y + 4, tankPosition.Z) + tankDirection * 10,
                new Vector3(tankPosition.X, tankPosition.Y + 4, tankPosition.Z) - tankDirection - new Vector3(0, tankPosition.Y / 3, 0), Vector3.Up);

                //Tank 2
                Vector3 tankPosition2 = tank2.posicao;
                Vector3 tankDirection2 = tank2.tankDir;
                viewMatrix2 = Matrix.CreateLookAt(new Vector3(0, tankPosition2.Y / 2, 0) + new Vector3(tankPosition2.X, tankPosition2.Y + 4, tankPosition2.Z) + tankDirection2 * 10,
                new Vector3(tankPosition2.X, tankPosition2.Y + 4, tankPosition2.Z) - tankDirection2 - new Vector3(0, tankPosition2.Y / 3, 0), Vector3.Up);
            }

        }
    }
}

