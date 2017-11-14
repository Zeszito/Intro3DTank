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
    public class Camera
    {
        public Matrix viewMatrix;//Matriz final a passar ao terreno para a sua effect.view 
        public Vector3 camPosition; //Posicao da Camera nos 3 eixos
        Vector3 direction;
        private float aspectRatio;

        float yaw, roll, vel; // yaw e pitch vão adquirir valores de acordo com o rato, Vel e apenas para ajustar a velocidade da camera
        Matrix mDirecao;// corresponde à direção da camera

        //Normais e receptores de normais, que são guias para entender qual lado a camera está virado e assim mudar
        //o comportmanto dos inputs de forma a ajustar a direção do das teclas com a direção da camera
        Vector3 NormalZ = Vector3.Backward;
        Vector3 NormalX = Vector3.Right;
        Vector3 turnX; Vector3 turnZ;

        public Camera(GraphicsDevice device)
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

        }
        public void UpdateMove(KeyboardState key, MouseState state, Terrain terrain) //Recebe os inputs do teclado, rato e recebe um terreno
        {
            //Recebe inputs do teclado e reage de acordo

            if (key.IsKeyDown(Keys.NumPad8)) camPosition += vel * turnX; //Frente
            if (key.IsKeyDown(Keys.NumPad2)) camPosition -= vel * turnX; //Trás
            if (key.IsKeyDown(Keys.NumPad4)) camPosition += vel * turnZ; //Esquerda
            if (key.IsKeyDown(Keys.NumPad6)) camPosition -= vel * turnZ; //Direita

            //Recebe os inputs do rato e processa-os
            yaw = MathHelper.ToRadians(-state.Position.X);// ve a posição do rato e transforma em radianos
            roll = MathHelper.ToRadians(state.Position.Y);
            mDirecao = Matrix.CreateFromYawPitchRoll(yaw, 0.0f, roll);//Usa os inputs do rato e criar uma matriz que vai rodar um vector para uma certa posição
            direction = Vector3.Transform(NormalX, mDirecao); //direção final, que vamos somar a posição da camera para termos o Target

            //Cálculo final para o eixo do X e Z (movimentos horizontais)
            turnX = Vector3.Transform(NormalX, mDirecao);
            turnZ = Vector3.Transform(NormalZ, mDirecao);


            //A posição Y da câmera vai ser adquirida atravéz de uma função do terreno que cálcula o Y de acordo com a posição da câmera no eixo XZ. Esta função é explicada mais na class Terrain
            camPosition.Y = terrain.CalculateInterpolation(camPosition.X, camPosition.Z,2);

            //actualiza a Camera com os novos paramtros
            UpdateView();
        }

        public void UpdateView() //Atualiza a viewMatrix
        {
            viewMatrix = Matrix.CreateLookAt(
            camPosition,
            camPosition + direction, Vector3.Up);

        }
    }
}
