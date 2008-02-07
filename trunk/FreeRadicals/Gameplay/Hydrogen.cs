#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace FreeRadicals.Gameplay
{
    /// <summary>
    /// Atoms that fill the game simulation
    /// </summary>
    class Hydrogen : Actor, IBaseAgent
    {
        #region Constants
        /// <summary>
        /// The ratio between the mass and the radius of an oxygen.
        /// </summary>
        const float massRadiusRatio = 4f;

        /// <summary>
        /// The amount of drag applied to velocity per second, 
        /// as a percentage of velocity.
        /// </summary>
        const float dragPerSecond = 0.20f;

        /// <summary>
        /// Scalar for calculated damage values that asteroids apply to players.
        /// </summary>
        const float damageScalar = 0.001f;

        /// <summary>
        /// Scalar to convert the velocity / mass ratio into a "nice" rotational value.
        /// </summary>
        const float velocityMassRatioToRotationScalar = -0.005f;
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new oxygen.
        /// </summary>
        /// <param name="world">The world that this oxygen belongs to.</param>
        /// <param name="radius">The size of the oxygen.</param>
        public Hydrogen(World world)
            : base(world)
        {
            // Hydrogen Radius
            this.radius = 4.0f;//(1.00794)
            // Collision Radius (Radius * 40)
            this.collisionRadius = this.radius * 40;
            // all atoms are coloured according to which type they are
            this.color = Color.Yellow;
            // create the polygon
            this.polygon = VectorPolygon.CreateCircle(Vector2.Zero, radius, 100);
            // the atom polygon might not be as big as the original radius, 
            // so find out how big it really is
            for (int i = 0; i < this.polygon.Points.Length; i++)
            {
                float length = this.polygon.Points[i].Length();
                if (length > this.radius)
                {
                    this.radius = length;
                }
            }
            // calculate the mass
            this.mass = radius * massRadiusRatio;
        }
        #endregion

        #region Update
        /// <summary>
        /// Update the actor.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public override void Update(float elapsedTime)
        {
            // spin the oxygen based on the size and velocity
            this.rotation += (this.velocity.LengthSquared() / this.mass) * elapsedTime *
                velocityMassRatioToRotationScalar;

            // apply some drag so the asteroids settle down
            velocity -= velocity * (elapsedTime * dragPerSecond);

            // check if there is an Hydrogen
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Hydrogen) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.01f;
                        return;
                    }
                }
            }

            // check if there is an Hydroxyl
            for (int i = 0; i < world.Actors.Count; ++i)
            {
                if ((world.Actors[i] is Hydroxyl) == true)
                {
                    Vector2 distance = this.position - world.Actors[i].Position;
                    if (distance.Length() <= this.collisionRadius)
                    {
                        world.Actors[i].Velocity -= -distance * 0.01f;
                        return;
                    }
                }
            }

            base.Update(elapsedTime);
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Defines the interaction between the oxygen and a target actor
        /// when they touch.
        /// </summary>
        /// <param name="target">The actor that is touching this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public override bool Touch(Actor target)
        {
            // if the oxygen has touched a player, then damage it
            NanoBot player = target as NanoBot;
            if (player != null)
            {
                // calculate damage as a function of how much the two actor's
                // velocities were going towards one another
                Vector2 playerAsteroidVector =
                    Vector2.Normalize(this.position - player.Position);
                float rammingSpeed =
                    Vector2.Dot(playerAsteroidVector, player.Velocity) -
                    Vector2.Dot(playerAsteroidVector, this.velocity);
                //player.Damage(this, this.mass * rammingSpeed * damageScalar);

            }
            // if the oxygen didn't hit a projectile, play the oxygen-touch cue
            if ((target is Projectile) == false)
            {
                this.world.AudioManager.PlayCue("asteroidTouch");
            }
            // if the Hydrogen hit an Hydrogen, Bond them 2H
            if ((target is Hydrogen) == true)
            {
                int H = 1;
                world.BondDeuterium(this, target, H);

                //this.Die(this);
                //target.Die(target);
                Vector2 pos = (this.position + target.Position) / 2;
                Vector2 vel = (this.velocity + target.Velocity) / 2;
                Vector2 dir = (this.direction + target.Direction) / 2;
                //Gameplay.Deuterium deuterium = new Gameplay.Deuterium(world);
                //deuterium.Spawn(true);
                //deuterium.Position = pos;
                //deuterium.Velocity = vel;
                //deuterium.Direction = dir;
                world.ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 18, 32f, 64f, 1.5f, 0.05f, Color.Yellow));
                //world.AudioManager.PlayCue("asteroidTouch");
            }
            return base.Touch(target);
        }


        /// <summary>
        /// Damage this oxygen by the amount provided.
        /// </summary>
        /// <remarks>
        /// This function is provided in lieu of a Life mutation property to allow 
        /// classes of objects to restrict which kinds of objects may damage them,
        /// and under what circumstances they may be damaged.
        /// </remarks>
        /// <param name="source">The actor responsible for the damage.</param>
        /// <param name="damageAmount">The amount of damage.</param>
        /// <returns>If true, this object was damaged.</returns>
        public override bool Damage(Actor source, float damageAmount)
        {
            // nothing hurst asteroids, nothing!
            return false;
        }
        #endregion
        #region Finite State Machine Methods
        /// <summary>
        /// Method called to patrol
        /// </summary>
        public void Patrol()
        {
            //// wander works just like the mouse's.
            //Wander(position, ref direction, ref orientation,
            //    turnSpeed);
            //currentSpeed = .25f * maxSpeed;
            Move(position, new Vector2(0.5f, 0.5f), currentSpeed);
        }
        /// <summary>
        /// Yes, you guessed it my fight method.
        /// </summary>
        public void Bond()
        {
            //// This is where you would put all your lovely shoot the Player and dive for cover stuff
            //// and maybe some more path finding to get a better shot on your target.

            //// Simulate that I am taking damage. 
            //HP -= .5f;
            //// Stop and fight.
            //speed = 0;
            //// Set to a threatenign color.
            //AmbientColor = Color.Red.ToVector4();

            //// Look at the Player with an angry face.... or in this case prop.
            //LookAt(Camera.myPosition, .1f);
        }
        /// <summary>
        /// Guess what this method is for??
        /// </summary>
        public void Flee()
        {
            //// Run really fast!
            //speed = .02f;

            //// Yellow belly!
            //AmbientColor = Color.Yellow.ToVector4();

            //// Probably be better to again pathfind your way out, but this is a simple tut, so just run!
            //velocity.Z -= .25f;

            //Move();
        }

        /// <summary>
        /// Simply, am I in any danger, or have I come into contact with the player in this case.
        /// </summary>
        /// <returns>true if safe else false.</returns>
        public bool isSafe()
        {
            //bool retVal = true;

            //// Am I in range of the player or are my hits too low?
            //if (Vector3.Distance(myPosition, Camera.myPosition) <= 10 || HP < 70)
            //    retVal = false;

            //return retVal;
            return false;
        }

        /// <summary>
        /// Do I think I should do a runner??
        /// </summary>
        /// <returns>true, "yes I should leave.". false "naa I am OK"</returns>
        public bool runAway()
        {
            //// If I have lost half or more of my hits, I want to leave...
            //if (HP <= 50)
            //    return true;
            //else
            //    return false;
            return false;
        }

        /// <summary>
        /// Once a certain target is found chase until it is bound.
        /// </summary>
        /// <returns>true if safe else false.</returns>
        public bool Chase()
        {
            //bool retVal = true;

            //// Am I in range of the player or are my hits too low?
            //if (Vector3.Distance(position, Camera.myPosition) <= 10 || HP < 70)
            //    retVal = false;

            //return retVal;
            return false;
        }

        /// <summary>
        /// Seek a certain target??
        /// </summary>
        /// <returns>true, "yes I should leave.". false "naa I am OK"</returns>
        public bool Seek()
        {
            //// If I have lost half or more of my hits, I want to leave...
            //if (HP <= 50)
            //    return true;
            //else
            //    return false;
            return false;
        }
        #endregion
    }
}