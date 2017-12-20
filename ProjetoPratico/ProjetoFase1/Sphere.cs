using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetoFase1
{
    public class Sphere
    {
        float radius;
        public Vector3 center;

        public Sphere(float radius, Vector3 centro)
        {
            this.radius = radius;
            center = centro;
        }

        public bool SphereOnSphere(Sphere other)
        {
            float distanceCenters = Vector3.Distance(center, other.center);
            if (distanceCenters < radius + other.radius)
            {
                return true;
            }

            return false;
        }
    }
}
