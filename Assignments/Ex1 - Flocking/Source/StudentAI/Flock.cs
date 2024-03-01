using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using AI.SteeringBehaviors.Core;

namespace AI.SteeringBehaviors.StudentAI
{
    public class Flock
    {
        public float AlignmentStrength { get; set; }
        public float CohesionStrength { get; set; }
        public float SeparationStrength { get; set; }
        public List<MovingObject> Boids { get; protected set; }
        public Vector3 AveragePosition { get; set; }
        protected Vector3 AverageForward { get; set; }
        public float FlockRadius { get; set; }

        #region TODO
        public Flock()
        {
            //this.AverageForward = getAvgForward(Boids);
            //this.AveragePosition = getAvgPosition(Boids);

            this.AlignmentStrength = 1.0f;
            this.CohesionStrength = 1.0f;
            this.SeparationStrength = 1.0f;
            this.FlockRadius = 10.0f;
        }

        public virtual void Update(float deltaTime)
        {
            this.AverageForward = getAvgForward(Boids);
            this.AveragePosition = getAvgPosition(Boids);

            Vector3 acceleration = new Vector3();

            foreach (MovingObject boid in Boids)
            {
                acceleration = getAlignmentAccel(boid);
                acceleration += getCohesionAccel(boid);
                acceleration += getSeparationAccel(boid);
                acceleration *= (boid.MaxSpeed * deltaTime);
                boid.Velocity += acceleration;

                if (boid.Velocity.Length > boid.MaxSpeed)
                {
                    boid.Velocity.Normalize();
                    boid.Velocity *= boid.MaxSpeed;
                }
                boid.Update(deltaTime);
            }
        }

        private Vector3 getAvgForward(List<MovingObject> _boids)
        {
            Vector3 totalForwards = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 avgForward;
            foreach (MovingObject boid in _boids)
            {
                totalForwards += boid.Velocity;
            }
            avgForward = totalForwards / _boids.Count;
            return avgForward;
        }

        private Vector3 getAvgPosition(List<MovingObject> _boids)
        {
            Vector3 totalPositions = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 avgPosition;
            foreach (MovingObject boid in _boids)
            {
                totalPositions += boid.Position;
            }
            avgPosition = totalPositions / _boids.Count;
            return avgPosition;
        }

        private Vector3 getAlignmentAccel(MovingObject _boid) 
        {
            Vector3 acceleration = this.AverageForward / _boid.MaxSpeed;

            if (acceleration.Length > 1.0f)
            {
                acceleration.Normalize();
            }

            return acceleration * this.AlignmentStrength;
        }

        private Vector3 getCohesionAccel(MovingObject _boid)
        {
            Vector3 acceleration = this.AveragePosition - _boid.Position;
            float distance = acceleration.Length;
            acceleration.Normalize();

            if (distance < this.FlockRadius)
            {
                acceleration *= (distance / this.FlockRadius);
            }
            return acceleration * this.CohesionStrength;
        }

        private Vector3 getSeparationAccel(MovingObject _boid)
        {
            Vector3 sum = new Vector3(0.0f, 0.0f, 0.0f);
            Vector3 acceleration = new Vector3();

            foreach (MovingObject sibling in this.Boids)
            {
                if (_boid != sibling)
                {
                    acceleration = _boid.Position - sibling.Position;
                    float distance = acceleration.Length;
                    float safeDistance = _boid.SafeRadius + sibling.SafeRadius;

                    if (distance < safeDistance)
                    {
                        acceleration.Normalize();
                        acceleration *= (safeDistance - distance) / safeDistance;
                        sum += acceleration;
                    }
                }
            }

            if (sum.Length > 1.0f)
            {
                sum.Normalize();
            }

            return sum * this.SeparationStrength;
        }
    }
}
#endregion