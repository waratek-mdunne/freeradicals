#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FreeRadicals.Rendering;
using FreeRadicals.Gameplay;
using FreeRadicals.Gameplay.Weaponary;
using FreeRadicals.Gameplay.JointMolecules;
using FreeRadicals.Gameplay.FreeRadicals;
using FreeRadicals.Gameplay.GreenhouseGases;
using FreeRadicals.Gameplay.Atoms;
using FreeRadicals.Gameplay.Poles;
using FreeRadicals.Gameplay.RepelPoints;
#endregion

namespace FreeRadicals.Simulation
{
    /// <summary>
    /// Owns all game state and executes all game-wide logic.
    /// </summary>
    class World
    {
        #region Constants
        /// <summary>
        /// The number of seconds before the first power-up appears in a game.
        /// </summary>
        const float initialFreeRadicalsDelay = 5f;
        const float initialGreenhouseGasesDelay = 2f;

        /// <summary>
        /// The time between each power-up spawn.
        /// </summary>
        const float freeRadicalsDelay = 10f;
        const float greenhouseGasesDelay = 1f;

        /// <summary>
        /// The number of stars to generate in the starfield.
        /// </summary>
        const int atomCount = 5000;

        /// <summary>
        /// How far atmosphere should generate outside the dimensions of the game field.
        /// </summary>
        const int atmosphereBuffer = 512;
        #endregion

        #region Fields
        // Resolution Variable
        float resVar = 1.0f;

        // Oxeygen Variables
        int deadO = 0;
        int deadH = 0;
        int deadN = 0;

        // Create a Random Number
        Random random = new Random();

        // Number of Free Radicals
        int freeRadicalCount = 0;

        // Number of CFC1s
        int cFC1Count = 0;

        // Number of CFC2s
        int cFC2Count = 0;

        // Number of hydroxyls
        int hydroxylCount = 0;

        // Number of nitric oxides
        int nitricOxideCount = 0;

        // Number of Greenhouse Gases
        int greenhouseGasesCount = 0;

        // Number of Ozone molecules
        int ozoneCount = 25;

        // Number of Water molecules
        int waterCount = 0;

        // Number of nitrous oxides
        int nitrousOxideCount = 0;

        // Number of methane molecules
        int methaneCount = 0;

        // Number of carbon dioxides
        int carbonDioxideCount = 0;

        /// <summary>
        /// The dimensions of the game board.
        /// </summary>
        Vector2 dimensions;

        /// <summary>
        /// The safe dimensions of the game board.
        /// </summary>
        Rectangle safeDimensions;

        /// <summary>
        /// The timer to see if another power-up can arrive.
        /// </summary>
        float freeRadicalsTimer;
        float greenhouseGasesTimer;

        /// <summary>
        /// The audio manager that all objects in the world will use.
        /// </summary>
        private AudioManager audioManager;

        /// <summary>
        /// All nanoBots that might enter the game.
        /// </summary>
        NanoBot[] nanoBots;

        /// <summary>
        /// The Atmosphere effect behind the game-board.
        /// </summary>
        Atmosphere atmosphere;

        /// <summary>
        /// All actors in the game.
        /// </summary>
        CollectCollection<Actor> actors;

        /// <summary>
        /// All particle-systems in the game.
        /// </summary>
        CollectCollection<ParticleSystem> particleSystems;

        /// <summary>
        /// Cached list of collision results, for more optimal collision detection.
        /// </summary>
        List<CollisionResult> collisionResults = new List<CollisionResult>();
        #endregion

        #region Particle Colors
        /// <summary>
        /// Particle system colors effect for NanoBot.
        /// </summary>
        static readonly Color[] nanoBotColor = 
        { 
            Color.White,  Color.Yellow, Color.CornflowerBlue, 
            Color.DeepPink, Color.Indigo,
            Color.Gold, Color.Silver
        };
        public Color[] NanoBotColor
        {
            get { return nanoBotColor; }
        }
        /// <summary>
        /// Particle system colors effect for CFC1.
        /// </summary>
        static readonly Color[] aMBColor = 
        { 
            Color.White, Color.Gray,
            Color.Gold, Color.Silver
        };
        public Color[] AMBColor
        {
            get { return aMBColor; }
        }

        /// <summary>
        /// Particle system colors effect for CFC1.
        /// </summary>
        static readonly Color[] cFC1Color = 
        { 
            Color.White, Color.Gray, Color.Purple, 
            Color.Purple, Color.Green, Color.Green,
            Color.Gold, Color.Silver
        };
        public Color[] CFC1Color
        {
            get { return cFC1Color; }
        }
        
        /// <summary>
        /// Particle system colors effect for CFC2.
        /// </summary>
        static readonly Color[] cFC2Color = 
        { 
            Color.White, Color.Gray, Color.Purple, 
            Color.Green, Color.Green, Color.Green,
            Color.Gold, Color.Silver
        };
        public Color[] CFC2Color
        {
            get { return cFC2Color; }
        }

        /// <summary>
        /// Particle system colors effect for NO.
        /// </summary>
        static readonly Color[] nOColor = 
        { 
            Color.White, Color.Red, Color.Blue, 
            Color.Gold, Color.Silver
        };
        public Color[] NOColor
        {
            get { return nOColor; }
        }

        /// <summary>
        /// Particle system colors effect for OH.
        /// </summary>
        static readonly Color[] oHColor = 
        { 
            Color.White, Color.Red, Color.Yellow, 
            Color.Gold, Color.Silver
        };
        public Color[] OHColor
        {
            get { return oHColor; }
        }

        /// <summary>
        /// Particle system colors effect for O2.
        /// </summary>
        static readonly Color[] o2Color = 
        { 
            Color.White, Color.Red, Color.Red, 
            Color.Gold, Color.Silver
        };
        public Color[] O2Color
        {
            get { return o2Color; }
        }

        /// <summary>
        /// Particle system colors effect for N2.
        /// </summary>
        static readonly Color[] n2Color = 
        { 
            Color.White, Color.Blue, Color.Blue, 
            Color.Gold, Color.Silver
        };
        public Color[] N2Color
        {
            get { return n2Color; }
        }

        /// <summary>
        /// Particle system colors effect for CH2.
        /// </summary>
        static readonly Color[] cH2Color = 
        { 
            Color.White, Color.Gray, Color.Yellow, Color.Yellow, 
            Color.Gold, Color.Silver
        };
        public Color[] CH2Color
        {
            get { return cH2Color; }
        }

        /// <summary>
        /// Particle system colors effect for CH4.
        /// </summary>
        static readonly Color[] cH4Color = 
        { 
            Color.White, Color.Gray, Color.Yellow, Color.Yellow, 
            Color.Yellow, Color.Yellow, Color.Gold, Color.Silver
        };
        public Color[] CH4Color
        {
            get { return cH4Color; }
        }

        /// <summary>
        /// Particle system colors effect for HH.
        /// </summary>
        static readonly Color[] hHColor = 
        { 
            Color.White, Color.Yellow, Color.Yellow, 
            Color.Gold, Color.Silver
        };
        public Color[] HHColor
        {
            get { return hHColor; }
        }

        /// <summary>
        /// Particle system colors effect for H2.
        /// </summary>
        static readonly Color[] h2OColor = 
        { 
            Color.White, Color.Red, Color.Red, 
            Color.Yellow, Color.Gold, Color.Silver
        };
        public Color[] H2OColor
        {
            get { return h2OColor; }
        }

        /// <summary>
        /// Particle system colors effect for O3.
        /// </summary>
        static readonly Color[] o3Color = 
        { 
            Color.White, Color.Red, Color.Red, 
            Color.Red, Color.Gold, Color.Silver
        };
        public Color[] O3Color
        {
            get { return o3Color; }
        }

        /// <summary>
        /// Particle system colors effect for CO2.
        /// </summary>
        static readonly Color[] cO2Color = 
        { 
            Color.White, Color.Red, Color.Gray, 
            Color.Gray, Color.Gold, Color.Silver
        };
        public Color[] CO2Color
        {
            get { return cO2Color; }
        }

        /// <summary>
        /// Particle system colors effect for N2O.
        /// </summary>
        static readonly Color[] n2OColor = 
        { 
            Color.White, Color.Red, Color.Blue, 
            Color.Blue, Color.Gold, Color.Silver
        };
        public Color[] N2OColor
        {
            get { return n2OColor; }
        }
        #endregion

        #region Properties
        // Resolution Variable
        public float ResVar
        {
            get { return resVar; }
            set { resVar = value; }
        }

        public int DeadO
        {
            get { return deadO; }
        }

        public int DeadH
        {
            get { return deadH; }
        }

        public int DeadN
        {
            get { return deadN; }
        }

        // Number of Free Radicals
        public int FreeRadicalCount
        {
            get { return freeRadicalCount; }
            set { freeRadicalCount = value; }
        }

        // Number of CFC1s
        public int CFC1Count
        {
            get { return cFC1Count; }
            set { cFC1Count = value; }
        }

        // Number of CFC2s
        public int CFC2Count
        {
            get { return cFC2Count; }
            set { cFC2Count = value; }
        }

        // Number of hydroxyls
        public int HydroxylCount
        {
            get { return hydroxylCount; }
            set { hydroxylCount = value; }
        }

        // Number of nitric oxides
        public int NitricOxideCount
        {
            get { return nitricOxideCount; }
            set { nitricOxideCount = value; }
        }

        // Number of Greenhouse Gases
        public int GreenhouseGasesCount
        {
            get { return greenhouseGasesCount; }
            set { greenhouseGasesCount = value; }
        }

        // Number of Ozone molecules
        public int OzoneCount
        {
            get { return ozoneCount; }
            set { ozoneCount = value; }
        }

        // Number of Water molecules
        public int WaterCount
        {
            get { return waterCount; }
            set { waterCount = value; }
        }

        // Number of nitrous 
        public int NitrousOxideCount
        {
            get { return nitrousOxideCount; }
            set { nitrousOxideCount = value; }
        }

        // Number of methane molecules
        public int MethaneCount
        {
            get { return methaneCount; }
            set { methaneCount = value; }
        }

        // Number of carbon dioxides
        public int CarbonDioxideCount
        {
            get { return carbonDioxideCount; }
            set { carbonDioxideCount = value; }
        }

        public AudioManager AudioManager
        {
            get { return audioManager; }
            set { audioManager = value; }
        }

        public Atmosphere Atmosphere
        {
            get { return atmosphere; }
        }

        public NanoBot[] NanoBots
        {
            get { return nanoBots; }
        }

        public CollectCollection<Actor> Actors
        {
            get { return actors; }
        }

        public CollectCollection<ParticleSystem> ParticleSystems
        {
            get { return particleSystems; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Construct a new World object, holding the game simulation.
        /// </summary>
        public World(Vector2 dimensions)
        {
            this.dimensions = dimensions * resVar;
            safeDimensions = new Rectangle(
                (int)(dimensions.X * 0.05f), 
                (int)(dimensions.Y * 0.05f),
                (int)(dimensions.X * 0.90f), 
                (int)(dimensions.Y * 0.90f));

            // create the Atmosphere
            atmosphere = new Atmosphere(atomCount, new Rectangle(
                atmosphereBuffer * -1,
                atmosphereBuffer * -1,
                (int)(this.dimensions.X) 
                    + atmosphereBuffer * 2,
                (int)(this.dimensions.Y) 
                    + atmosphereBuffer * 2));

            // create a new list of actors
            actors = new CollectCollection<Actor>(this);

            // create a new list of particle systems
            particleSystems = new CollectCollection<ParticleSystem>(this);
        }
        #endregion

        #region New Game
        public void StartNewGame()
        {
            // clear out the actors list
            actors.Clear();
            // add the world actor
            WorldActor worldActor = new WorldActor(this);
            actors.Add(worldActor);

            // Spawn Screen Resolution
            // "a1920x1200", "b1680x1050", "c1440x900", "d1280x800"
            switch (WorldRules.ScreenRes)
            {
                case ScreenRes.a1920x1200:
                    resVar = 1.0f;
                    break;
                case ScreenRes.b1680x1050:
                    resVar = 0.875f;
                    break;
                case ScreenRes.c1440x900:
                    resVar = 0.75f;
                    break;
                case ScreenRes.d1280x800:
                    resVar = 0.66666f;
                    break;
            }

            // create the players
            nanoBots = new NanoBot[4];
            nanoBots[0] = new NanoBot(this, PlayerIndex.One);
            nanoBots[1] = new NanoBot(this, PlayerIndex.Two);
            nanoBots[2] = new NanoBot(this, PlayerIndex.Three);
            nanoBots[3] = new NanoBot(this, PlayerIndex.Four);

            // add the players to the actor list - they won't be removed
            for (int i = 0; i < nanoBots.Length; i++)
            {
                actors.Add(nanoBots[i]);
            }

            // spawn the poles
            SpawnPoles();

            // spawn the repel points
            SpawnRepelPointsOne();
            SpawnRepelPointsTwo();
            SpawnRepelPointsThree();
            SpawnRepelPointsFour();
            SpawnRepelPointsFive();
            SpawnRepelPointsSix();
            SpawnRepelPointsSeven();

            // spawn atoms density
            SpawnGreenHouseGases(30, 0, 0, 0, 0);

            // set up the atmosphere
            atmosphere.SetTargetPosition(dimensions);
        }
        #endregion

        #region Spawn Molecules Groups
        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        public void SpawnFreeRadicals(int NO, int CFCa, int CFCb, int HO)
        {
            // create Nitric Oxide atoms
            for (int i = 0; i < NO; ++i)
            {
                NitricOxide nitricOxide = new NitricOxide(this);
                nitricOxide.Spawn(true);
            }
            // create CFC1 atoms
            for (int i = 0; i < CFCa; ++i)
            {
                CFC1 cfc1 = new CFC1(this);
                cfc1.Spawn(true);
            }
            // create CFC2 atoms
            for (int i = 0; i < CFCb; ++i)
            {
                CFC2 cfc2 = new CFC2(this);
                cfc2.Spawn(true);
            }
            // create Hydroxyl atoms
            for (int i = 0; i < HO; ++i)
            {
                Hydroxyl hydroxyl = new Hydroxyl(this);
                hydroxyl.Spawn(true);
            }
        }


        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        public void SpawnGreenHouseGases(int O3, int H2O, int N2O, int CO2, int CH4)
        {
            // create Ozone atoms
            for (int i = 0; i < O3; ++i)
            {
                Ozone ozone = new Ozone(this);
                ozone.Spawn(true);
            }
            // create Water atoms
            for (int i = 0; i < H2O; ++i)
            {
                Water water = new Water(this);
                water.Spawn(true);
            }
            // create Nitrous Oxide atoms
            for (int i = 0; i < N2O; ++i)
            {
                NitrousOxide nitrousOxide = new NitrousOxide(this);
                nitrousOxide.Spawn(true);
            }
            // create Carbon Dioxide atoms
            for (int i = 0; i < CO2; ++i)
            {
                CarbonDioxide carbonDioxide = new CarbonDioxide(this);
                carbonDioxide.Spawn(true);
            }
            // create Methane atoms
            for (int i = 0; i < CH4; ++i)
            {
                Methane methane = new Methane(this);
                methane.Spawn(true);
            }
        }


        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        public void SpawnAtoms(int H, int C, int N, int O, int F, int Cl, int Br)
        {
            // create hydrogen atoms
            for (int i = 0; i < H; ++i)
            {
                Hydrogen hydrogen = new Hydrogen(this);
                hydrogen.Spawn(true);
            }
            // create carbon atoms
            for (int i = 0; i < C; ++i)
            {
                Carbon carbon = new Carbon(this);
                carbon.Spawn(true);
            }
            // create nitrogen atoms
            for (int i = 0; i < N; ++i)
            {
                Nitrogen nitrogen = new Nitrogen(this);
                nitrogen.Spawn(true);
            }
            // create oxygen atoms
            for (int i = 0; i < O; ++i)
            {
                Oxygen oxygen = new Oxygen(this);
                oxygen.Spawn(true);
            }
            // create fluorine atoms
            for (int i = 0; i < F; ++i)
            {
                Fluorine fluorine = new Fluorine(this);
                fluorine.Spawn(true);
            }
            // create chlorine atoms
            for (int i = 0; i < Cl; ++i)
            {
                Chlorine chlorine = new Chlorine(this);
                chlorine.Spawn(true);
            }
            // create Bromine atoms
            for (int i = 0; i < Br; ++i)
            {
                Bromine bromine = new Bromine(this);
                bromine.Spawn(true);
            }
        }

        /// <summary>
        /// Create many asteroids and add them to the game world.
        /// </summary>
        /// <param name="smallCount">The number of "small" asteroids to create.</param>
        /// <param name="mediumCount">The number of "medium" asteroids to create.
        /// </param>
        /// <param name="largeCount">The number of "large" asteroids to create.</param>
        public void SpawnJointMolecules(int O2, int N2, int HH, int CH2)
        {
            // create Oxygen Two atoms
            for (int i = 0; i < O2; ++i)
            {
                OxygenTwo oxygenTwo = new OxygenTwo(this);
                oxygenTwo.Spawn(true);
            }
            // create Nitrogen Two atoms
            for (int i = 0; i < N2; ++i)
            {
                NitrogenTwo nitrogenTwo = new NitrogenTwo(this);
                nitrogenTwo.Spawn(true);
            }
            // create Deuterium atoms
            for (int i = 0; i < HH; ++i)
            {
                Deuterium deuterium = new Deuterium(this);
                deuterium.Spawn(true);
            }
            // create Methylene atoms
            for (int i = 0; i < CH2; ++i)
            {
                Methylene methylene = new Methylene(this);
                methylene.Spawn(true);
            }
        }
        #endregion

        #region Spawning
        /// <summary>
        /// Create a new power-up in the world, if possible
        /// </summary>
        public void FreeRadicalsSpawning()
        {
            for (int i = 0; i < actors.Count; ++i)
            {
                if (freeRadicalCount >= 5)
                {
                    return;
                }
            }
            switch (random.Next(10))
            {
                case 0:
                    NitricOxide p = new NitricOxide(this);
                    p.Spawn(false);
                    p.Position = new Vector2(950f * resVar, 1500f * resVar);
                    break;
                case 1:
                    CFC1 q = new CFC1(this);
                    q.Spawn(false);
                    q.Position = new Vector2(950f * resVar, 1550f * resVar);
                    break;
                case 2:
                    CFC2 w = new CFC2(this);
                    w.Spawn(false);
                    w.Position = new Vector2(950f * resVar, 1600f * resVar);
                    break;
                case 3:
                    Hydroxyl e = new Hydroxyl(this);
                    e.Spawn(false);
                    e.Position = new Vector2(950f * resVar, 1650f * resVar);
                    break;
                case 4:
                    Ozone f = new Ozone(this);
                    f.Spawn(false);
                    f.Position = new Vector2(950f * resVar, 1650f * resVar);
                    break;
                case 5:
                    NitricOxide m = new NitricOxide(this);
                    m.Spawn(false);
                    m.Position = new Vector2(1250f * resVar, 1500f * resVar);
                    break;
                case 6:
                    CFC1 n = new CFC1(this);
                    n.Spawn(false);
                    n.Position = new Vector2(1250f * resVar, 1550f * resVar);
                    break;
                case 7:
                    CFC2 b = new CFC2(this);
                    b.Spawn(false);
                    b.Position = new Vector2(1250f * resVar, 1600f * resVar);
                    break;
                case 8:
                    Hydroxyl v = new Hydroxyl(this);
                    v.Spawn(false);
                    v.Position = new Vector2(1250f * resVar, 1650f * resVar);
                    break;
                case 9:
                    Ozone c = new Ozone(this);
                    c.Spawn(false);
                    c.Position = new Vector2(1250f * resVar, 1650f * resVar);
                    break;
            }
        }


        /// <summary>
        /// Create a new power-up in the world, if possible
        /// </summary>
        public void GreenhouseGasesSpawning()
        {
            for (int i = 0; i < actors.Count; ++i)
            {
                if (greenhouseGasesCount >= 15)
                {
                    return;
                }
            }
            switch (random.Next(5))
            {
                case 0:
                    Water p = new Water(this);
                    p.Spawn(false);
                    p.Position = new Vector2(950f * resVar, 1450f * resVar);
                    break;
                case 1:
                    CarbonDioxide q = new CarbonDioxide(this);
                    q.Spawn(false);
                    q.Position = new Vector2(950f * resVar, 1500f * resVar);
                    break;
                case 2:
                    NitrousOxide w = new NitrousOxide(this);
                    w.Spawn(false);
                    w.Position = new Vector2(950f * resVar, 1550f * resVar);
                    break;
                case 3:
                    Methane e = new Methane(this);
                    e.Spawn(false);
                    e.Position = new Vector2(950f * resVar, 1600f * resVar);
                    break;
                case 4:
                    Ozone f = new Ozone(this);
                    f.Spawn(false);
                    f.Position = new Vector2(950f * resVar, 1650f * resVar);
                    break;
                case 5:
                    Water m = new Water(this);
                    m.Spawn(false);
                    m.Position = new Vector2(1250f * resVar, 1450f * resVar);
                    break;
                case 6:
                    CarbonDioxide n = new CarbonDioxide(this);
                    n.Spawn(false);
                    n.Position = new Vector2(1250f * resVar, 1500f * resVar);
                    break;
                case 7:
                    NitrousOxide b = new NitrousOxide(this);
                    b.Spawn(false);
                    b.Position = new Vector2(1250f * resVar, 1550f * resVar);
                    break;
                case 8:
                    Methane v = new Methane(this);
                    v.Spawn(false);
                    v.Position = new Vector2(1250f * resVar, 1600f * resVar);
                    break;
                case 9:
                    Ozone c = new Ozone(this);
                    c.Spawn(false);
                    c.Position = new Vector2(1250f * resVar, 1650f * resVar);
                    break;
            }
        }
        
        #endregion

        #region Poles & Points
        /// <summary>
        /// Create a repel one points
        /// </summary>
        public void SpawnRepelPointsOne()
        {
            // Top Side
            One ts0 = new One(this);
            ts0.Spawn(false);
            ts0.Position = new Vector2(0, -200f * resVar);
            One ts1 = new One(this);
            ts1.Spawn(false);
            ts1.Position = new Vector2(250f * resVar, -200f * resVar);
            One ts2 = new One(this);
            ts2.Spawn(false);
            ts2.Position = new Vector2(500f * resVar, -200f * resVar);
            One ts3 = new One(this);
            ts3.Spawn(false);
            ts3.Position = new Vector2(750f * resVar, -200f * resVar);
            One ts4 = new One(this);
            ts4.Spawn(false);
            ts4.Position = new Vector2(1000f * resVar, -200f * resVar);
            One ts5 = new One(this);
            ts5.Spawn(false);
            ts5.Position = new Vector2(1250f * resVar, -200f * resVar);
            One ts6 = new One(this);
            ts6.Spawn(false);
            ts6.Position = new Vector2(1500f * resVar, -200f * resVar);
            One ts7 = new One(this);
            ts7.Spawn(false);
            ts7.Position = new Vector2(1750f * resVar, -200f * resVar);
            One ts8 = new One(this);
            ts8.Spawn(false);
            ts8.Position = new Vector2(2000f * resVar, -200f * resVar);

            // Bottom Side
            One bs0 = new One(this);
            bs0.Spawn(false);
            bs0.Position = new Vector2(0, 2000f * resVar);
            One bs1 = new One(this);
            bs1.Spawn(false);
            bs1.Position = new Vector2(250f * resVar, 2000f * resVar);
            One bs2 = new One(this);
            bs2.Spawn(false);
            bs2.Position = new Vector2(500f * resVar, 2000f * resVar);
            One bs3 = new One(this);
            bs3.Spawn(false);
            bs3.Position = new Vector2(750f * resVar, 2000f * resVar);
            One bs4 = new One(this);
            bs4.Spawn(false);
            bs4.Position = new Vector2(1000f * resVar, 2000f * resVar);
            One bs5 = new One(this);
            bs5.Spawn(false);
            bs5.Position = new Vector2(1250f * resVar, 2000f * resVar);
            One bs6 = new One(this);
            bs6.Spawn(false);
            bs6.Position = new Vector2(1500f * resVar, 2000f * resVar);
            One bs7 = new One(this);
            bs7.Spawn(false);
            bs7.Position = new Vector2(1750f * resVar, 2000f * resVar);
            One bs8 = new One(this);
            bs8.Spawn(false);
            bs8.Position = new Vector2(2000f * resVar, 2000f * resVar);

            // Left Side
            One ls0 = new One(this);
            ls0.Spawn(false);
            ls0.Position = new Vector2(-200f * resVar, 0);
            One ls1 = new One(this);
            ls1.Spawn(false);
            ls1.Position = new Vector2(-200f * resVar, 250f * resVar);
            One ls2 = new One(this);
            ls2.Spawn(false);
            ls2.Position = new Vector2(-200f * resVar, 500f * resVar);
            One ls3 = new One(this);
            ls3.Spawn(false);
            ls3.Position = new Vector2(-200f * resVar, 750f * resVar);
            One ls4 = new One(this);
            ls4.Spawn(false);
            ls4.Position = new Vector2(-200f * resVar, 1000f * resVar);
            One ls5 = new One(this);
            ls5.Spawn(false);
            ls5.Position = new Vector2(-200f * resVar, 1250f * resVar);

            // Right Side
            One rs0 = new One(this);
            rs0.Spawn(false);
            rs0.Position = new Vector2(2120f * resVar, 0f);
            One rs1 = new One(this);
            rs1.Spawn(false);
            rs1.Position = new Vector2(2120f * resVar, 250f * resVar);
            One rs2 = new One(this);
            rs2.Spawn(false);
            rs2.Position = new Vector2(2120f * resVar, 500f * resVar);
            One rs3 = new One(this);
            rs3.Spawn(false);
            rs3.Position = new Vector2(2120f * resVar, 750f * resVar);
            One rs4 = new One(this);
            rs4.Spawn(false);
            rs4.Position = new Vector2(2120f * resVar, 1000f * resVar);
            One rs5 = new One(this);
            rs5.Spawn(false);
            rs5.Position = new Vector2(2120f * resVar, 1250f * resVar);
        }


        /// <summary>
        /// Create a repel two points
        /// </summary>
        public void SpawnRepelPointsTwo()
        {
            // Top Side
            Two ts0 = new Two(this);
            ts0.Spawn(false);
            ts0.Position = new Vector2(0, -200f * resVar);
            Two ts1 = new Two(this);
            ts1.Spawn(false);
            ts1.Position = new Vector2(250f * resVar, -200f * resVar);
            Two ts2 = new Two(this);
            ts2.Spawn(false);
            ts2.Position = new Vector2(500f * resVar, -200f * resVar);
            Two ts3 = new Two(this);
            ts3.Spawn(false);
            ts3.Position = new Vector2(750f * resVar, -200f * resVar);
            Two ts4 = new Two(this);
            ts4.Spawn(false);
            ts4.Position = new Vector2(1000f * resVar, -200f * resVar);
            Two ts5 = new Two(this);
            ts5.Spawn(false);
            ts5.Position = new Vector2(1250f * resVar, -200f * resVar);
            Two ts6 = new Two(this);
            ts6.Spawn(false);
            ts6.Position = new Vector2(1500f * resVar, -200f * resVar);
            Two ts7 = new Two(this);
            ts7.Spawn(false);
            ts7.Position = new Vector2(1750f * resVar, -200f * resVar);
            Two ts8 = new Two(this);
            ts8.Spawn(false);
            ts8.Position = new Vector2(2000f * resVar, -200f * resVar);

            // Bottom Side
            Two bs0 = new Two(this);
            bs0.Spawn(false);
            bs0.Position = new Vector2(0, 2200f * resVar);
            Two bs1 = new Two(this);
            bs1.Spawn(false);
            bs1.Position = new Vector2(250f * resVar, 2200f * resVar);
            Two bs2 = new Two(this);
            bs2.Spawn(false);
            bs2.Position = new Vector2(500f * resVar, 2200f * resVar);
            Two bs3 = new Two(this);
            bs3.Spawn(false);
            bs3.Position = new Vector2(750f * resVar, 2200f * resVar);
            Two bs4 = new Two(this);
            bs4.Spawn(false);
            bs4.Position = new Vector2(1000f * resVar, 2200f * resVar);
            Two bs5 = new Two(this);
            bs5.Spawn(false);
            bs5.Position = new Vector2(1250f * resVar, 2200f * resVar);
            Two bs6 = new Two(this);
            bs6.Spawn(false);
            bs6.Position = new Vector2(1500f * resVar, 2200f * resVar);
            Two bs7 = new Two(this);
            bs7.Spawn(false);
            bs7.Position = new Vector2(1750f * resVar, 2200f * resVar);
            Two bs8 = new Two(this);
            bs8.Spawn(false);
            bs8.Position = new Vector2(2000f * resVar, 2200f * resVar);

            // Left Side
            Two ls0 = new Two(this);
            ls0.Spawn(false);
            ls0.Position = new Vector2(-200f * resVar, 0);
            Two ls1 = new Two(this);
            ls1.Spawn(false);
            ls1.Position = new Vector2(-200f * resVar, 250f * resVar);
            Two ls2 = new Two(this);
            ls2.Spawn(false);
            ls2.Position = new Vector2(-200f * resVar, 500f * resVar);
            Two ls3 = new Two(this);
            ls3.Spawn(false);
            ls3.Position = new Vector2(-200f * resVar, 750f * resVar);
            Two ls4 = new Two(this);
            ls4.Spawn(false);
            ls4.Position = new Vector2(-200f * resVar, 1000f * resVar);
            Two ls5 = new Two(this);
            ls5.Spawn(false);
            ls5.Position = new Vector2(-200f * resVar, 1250f * resVar);
            Two ls6 = new Two(this);
            ls6.Spawn(false);
            ls6.Position = new Vector2(-200f * resVar, 1500f * resVar);
            Two ls7 = new Two(this);
            ls7.Spawn(false);
            ls7.Position = new Vector2(-200f * resVar, 1750f * resVar);
            Two ls8 = new Two(this);
            ls8.Spawn(false);
            ls8.Position = new Vector2(-200f * resVar, 2000f * resVar);

            // Right Side
            Two rs0 = new Two(this);
            rs0.Spawn(false);
            rs0.Position = new Vector2(2120f * resVar, 0f);
            Two rs1 = new Two(this);
            rs1.Spawn(false);
            rs1.Position = new Vector2(2120f * resVar, 250f * resVar);
            Two rs2 = new Two(this);
            rs2.Spawn(false);
            rs2.Position = new Vector2(2120f * resVar, 500f * resVar);
            Two rs3 = new Two(this);
            rs3.Spawn(false);
            rs3.Position = new Vector2(2120f * resVar, 750f * resVar);
            Two rs4 = new Two(this);
            rs4.Spawn(false);
            rs4.Position = new Vector2(2120f * resVar, 1000f * resVar);
            Two rs5 = new Two(this);
            rs5.Spawn(false);
            rs5.Position = new Vector2(2120f * resVar, 1250f * resVar);
            Two rs6 = new Two(this);
            rs6.Spawn(false);
            rs6.Position = new Vector2(2120f * resVar, 1500f * resVar);
            Two rs7 = new Two(this);
            rs7.Spawn(false);
            rs7.Position = new Vector2(2120f * resVar, 1750f * resVar);
            Two rs8 = new Two(this);
            rs8.Spawn(false);
            rs8.Position = new Vector2(2120f * resVar, 2000f * resVar);
        }


        /// <summary>
        /// Create a repel three points
        /// </summary>
        public void SpawnRepelPointsThree()
        {
            // Top Side
            Three ts0 = new Three(this);
            ts0.Spawn(false);
            ts0.Position = new Vector2(0, -200f * resVar);
            Three ts1 = new Three(this);
            ts1.Spawn(false);
            ts1.Position = new Vector2(250f * resVar, -200f * resVar);
            Three ts2 = new Three(this);
            ts2.Spawn(false);
            ts2.Position = new Vector2(500f * resVar, -200f * resVar);
            Three ts3 = new Three(this);
            ts3.Spawn(false);
            ts3.Position = new Vector2(750f * resVar, -200f * resVar);
            Three ts4 = new Three(this);
            ts4.Spawn(false);
            ts4.Position = new Vector2(1000f * resVar, -200f * resVar);
            Three ts5 = new Three(this);
            ts5.Spawn(false);
            ts5.Position = new Vector2(1250f * resVar, -200f * resVar);
            Three ts6 = new Three(this);
            ts6.Spawn(false);
            ts6.Position = new Vector2(1500f * resVar, -200f * resVar);
            Three ts7 = new Three(this);
            ts7.Spawn(false);
            ts7.Position = new Vector2(1750f * resVar, -200f * resVar);
            Three ts8 = new Three(this);
            ts8.Spawn(false);
            ts8.Position = new Vector2(2000f * resVar, -200f * resVar);

            // Bottom Side
            Three bs0 = new Three(this);
            bs0.Spawn(false);
            bs0.Position = new Vector2(0, 2200f * resVar);
            Three bs1 = new Three(this);
            bs1.Spawn(false);
            bs1.Position = new Vector2(250f * resVar, 2200f * resVar);
            Three bs2 = new Three(this);
            bs2.Spawn(false);
            bs2.Position = new Vector2(500f * resVar, 2200f * resVar);
            Three bs3 = new Three(this);
            bs3.Spawn(false);
            bs3.Position = new Vector2(750f * resVar, 2200f * resVar);
            Three bs4 = new Three(this);
            bs4.Spawn(false);
            bs4.Position = new Vector2(1000f * resVar, 2200f * resVar);
            Three bs5 = new Three(this);
            bs5.Spawn(false);
            bs5.Position = new Vector2(1250f * resVar, 2200f * resVar);
            Three bs6 = new Three(this);
            bs6.Spawn(false);
            bs6.Position = new Vector2(1500f * resVar, 2200f * resVar);
            Three bs7 = new Three(this);
            bs7.Spawn(false);
            bs7.Position = new Vector2(1750f * resVar, 2200f * resVar);
            Three bs8 = new Three(this);
            bs8.Spawn(false);
            bs8.Position = new Vector2(2000f * resVar, 2200f * resVar);

            // Left Side
            Three ls0 = new Three(this);
            ls0.Spawn(false);
            ls0.Position = new Vector2(-200f * resVar, 0);
            Three ls1 = new Three(this);
            ls1.Spawn(false);
            ls1.Position = new Vector2(-200f * resVar, 250f * resVar);
            //Three ls2 = new Three(this);
            //ls2.Spawn(false);
            //ls2.Position = new Vector2(-200f * resVar, 500f * resVar);
            //Three ls3 = new Three(this);
            //ls3.Spawn(false);
            //ls3.Position = new Vector2(-200f * resVar, 750f * resVar);
            Three ls4 = new Three(this);
            ls4.Spawn(false);
            ls4.Position = new Vector2(-200f * resVar, 1000f * resVar);
            Three ls5 = new Three(this);
            ls5.Spawn(false);
            ls5.Position = new Vector2(-200f * resVar, 1250f * resVar);
            Three ls6 = new Three(this);
            ls6.Spawn(false);
            ls6.Position = new Vector2(-200f * resVar, 1500f * resVar);
            Three ls7 = new Three(this);
            ls7.Spawn(false);
            ls7.Position = new Vector2(-200f * resVar, 1750f * resVar);
            Three ls8 = new Three(this);
            ls8.Spawn(false);
            ls8.Position = new Vector2(-200f * resVar, 2000f * resVar);

            // Right Side
            Three rs0 = new Three(this);
            rs0.Spawn(false);
            rs0.Position = new Vector2(2120f * resVar, 0f);
            Three rs1 = new Three(this);
            rs1.Spawn(false);
            rs1.Position = new Vector2(2120f * resVar, 250f * resVar);
            //Three rs2 = new Three(this);
            //rs2.Spawn(false);
            //rs2.Position = new Vector2(2120f * resVar, 500f * resVar);
            //Three rs3 = new Three(this);
            //rs3.Spawn(false);
            //rs3.Position = new Vector2(2120f * resVar, 750f * resVar);
            Three rs4 = new Three(this);
            rs4.Spawn(false);
            rs4.Position = new Vector2(2120f * resVar, 1000f * resVar);
            Three rs5 = new Three(this);
            rs5.Spawn(false);
            rs5.Position = new Vector2(2120f * resVar, 1250f * resVar);
            Three rs6 = new Three(this);
            rs6.Spawn(false);
            rs6.Position = new Vector2(2120f * resVar, 1500f * resVar);
            Three rs7 = new Three(this);
            rs7.Spawn(false);
            rs7.Position = new Vector2(2120f * resVar, 1750f * resVar);
            Three rs8 = new Three(this);
            rs8.Spawn(false);
            rs8.Position = new Vector2(2120f * resVar, 2000f * resVar);
        }


        /// <summary>
        /// Create a repel four points
        /// </summary>		WorldRules.ScreenRes	d1280x800	FreeRadicals.Simulation.ScreenRes
        ///
        public void SpawnRepelPointsFour()
        {
            // Left Side
            Four ls1 = new Four(this);
            ls1.Spawn(false);
            ls1.Position = new Vector2(-300f * resVar, 250f * resVar);
            Four ls2 = new Four(this);
            ls2.Spawn(false);
            ls2.Position = new Vector2(-300f * resVar, 500f * resVar);
            Four ls3 = new Four(this);
            ls3.Spawn(false);
            ls3.Position = new Vector2(-300f * resVar, 750f * resVar);

            // Right Side
            Four rs1 = new Four(this);
            rs1.Spawn(false);
            rs1.Position = new Vector2(2220f * resVar, 250f * resVar);
            Four rs2 = new Four(this);
            rs2.Spawn(false);
            rs2.Position = new Vector2(2220f * resVar, 500f * resVar);
            Four rs3 = new Four(this);
            rs3.Spawn(false);
            rs3.Position = new Vector2(2220f * resVar, 750f * resVar);
        }


        /// <summary>
        /// Create a repel five points
        /// </summary>
        public void SpawnRepelPointsFive()
        {

            // Bottom Side
            Five bs1 = new Five(this);
            bs1.Spawn(false);
            bs1.Position = new Vector2(250f * resVar, 1750f * resVar);
            Five bs2 = new Five(this);
            bs2.Spawn(false);
            bs2.Position = new Vector2(750f * resVar, 1750f * resVar);
            Five bs3 = new Five(this);
            bs3.Spawn(false);
            bs3.Position = new Vector2(1250f * resVar, 1750f * resVar);
            Five bs4 = new Five(this);
            bs4.Spawn(false);
            bs4.Position = new Vector2(1750f * resVar, 1750f * resVar);
        }


        /// <summary>
        /// Create a repel six points
        /// </summary>
        public void SpawnRepelPointsSix()
        {
            // Top Side
            Six ts0 = new Six(this);
            ts0.Spawn(false);
            ts0.Position = new Vector2(0, -200f * resVar);
            Six ts1 = new Six(this);
            ts1.Spawn(false);
            ts1.Position = new Vector2(250f * resVar, -200f * resVar);
            //Six ts2 = new Six(this);
            //ts2.Spawn(false);
            //ts2.Position = new Vector2(500f * resVar, -200f * resVar);
            Six ts3 = new Six(this);
            ts3.Spawn(false);
            ts3.Position = new Vector2(750f * resVar, -200f * resVar);
            Six ts4 = new Six(this);
            ts4.Spawn(false);
            ts4.Position = new Vector2(1000f * resVar, -200f * resVar);
            Six ts5 = new Six(this);
            ts5.Spawn(false);
            ts5.Position = new Vector2(1250f * resVar, -200f * resVar);
            //Six ts6 = new Six(this);
            //ts6.Spawn(false);
            //ts6.Position = new Vector2(1500f * resVar, -200f * resVar);
            Six ts7 = new Six(this);
            ts7.Spawn(false);
            ts7.Position = new Vector2(1750f * resVar, -200f * resVar);
            Six ts8 = new Six(this);
            ts8.Spawn(false);
            ts8.Position = new Vector2(2000f * resVar, -200f * resVar);
        }

        public void SpawnRepelPointsSeven()
        {
            // Bottom Side
            Seven bs0 = new Seven(this);
            bs0.Spawn(false);
            bs0.Position = new Vector2(0, 1400f * resVar);
            Seven bs1 = new Seven(this);
            bs1.Spawn(false);
            bs1.Position = new Vector2(250f * resVar, 1400f * resVar);
            Seven bs2 = new Seven(this);
            bs2.Spawn(false);
            bs2.Position = new Vector2(500f * resVar, 1400f * resVar);
            Seven bs3 = new Seven(this);
            bs3.Spawn(false);
            bs3.Position = new Vector2(750f * resVar, 1400f * resVar);
            //Seven bs4 = new Seven(this);
            //bs4.Spawn(false);
            //bs4.Position = new Vector2(1000f * resVar, 1400f * resVar);
            Seven bs5 = new Seven(this);
            bs5.Spawn(false);
            bs5.Position = new Vector2(1250f * resVar, 1400f * resVar);
            Seven bs6 = new Seven(this);
            bs6.Spawn(false);
            bs6.Position = new Vector2(1500f * resVar, 1400f * resVar);
            Seven bs7 = new Seven(this);
            bs7.Spawn(false);
            bs7.Position = new Vector2(1750f * resVar, 1400f * resVar);
            Seven bs8 = new Seven(this);
            bs8.Spawn(false);
            bs8.Position = new Vector2(2000f * resVar, 1400f * resVar);
        }


        /// <summary>
        /// Create a new power-up in the world, if possible
        /// </summary>
        public void SpawnPoles()
        {
            // North Poles
            North north0 = new North(this);
            north0.Spawn(false);
            north0.Position = new Vector2(0, -7000f * resVar);
            North north1 = new North(this);
            north1.Spawn(false);
            north1.Position = new Vector2(250f * resVar, -7000f * resVar);
            North north2 = new North(this);
            north2.Spawn(false);
            north2.Position = new Vector2(500f * resVar, -7000f * resVar);
            North north3 = new North(this);
            north3.Spawn(false);
            north3.Position = new Vector2(750f * resVar, -7000f * resVar);
            North north4 = new North(this);
            north4.Spawn(false);
            north4.Position = new Vector2(1000f * resVar, -7000f * resVar);
            North north5 = new North(this);
            north5.Spawn(false);
            north5.Position = new Vector2(1250f * resVar, -7000f * resVar);
            North north6 = new North(this);
            north6.Spawn(false);
            north6.Position = new Vector2(1500f * resVar, -7000f * resVar);
            North north7 = new North(this);
            north7.Spawn(false);
            north7.Position = new Vector2(1750f * resVar, -7000f * resVar);
            North north8 = new North(this);
            north8.Spawn(false);
            north8.Position = new Vector2(2000f * resVar, -1000f * resVar);

            // South Poles
            South south1 = new South(this);
            south1.Spawn(true);
            south1.Position = new Vector2(500f * resVar, 1900f * resVar);
            South south2 = new South(this);
            south2.Spawn(true);
            south2.Position = new Vector2(1000f * resVar, 2000f * resVar);
            South south3 = new South(this);
            south3.Spawn(true);
            south3.Position = new Vector2(1500f * resVar, 1900f * resVar);

            // West Pole
            West west = new West(this);
            west.Spawn(true);
            west.Position = new Vector2(-500f * resVar, 500f * resVar);

            // East Pole
            East east = new East(this);
            east.Spawn(true);
            east.Position = new Vector2(2420f * resVar, 500f * resVar);
        }        
        #endregion

        #region Update and Draw
        /// <summary>
        /// Update the world simulation.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        public void Update(float elapsedTime)
        {
            // update all actors
            for (int i = 0; i < actors.Count; i++)
            {
                actors[i].Update(elapsedTime);
            }

            // update collision
            MoveWorld(elapsedTime);

            // update particle systems
            for (int i = 0; i < particleSystems.Count; i++)
            {
                particleSystems[i].Update(elapsedTime);
                if (particleSystems[i].IsActive == false)
                {
                    particleSystems.Garbage.Add(particleSystems[i]);
                }
            }

            // update the starfield
            Vector2 starfieldTarget = Vector2.Zero;
            int playingPlayers = 0;
            for (int i = 0; i < nanoBots.Length; i++)
            {
                if (nanoBots[i].Playing)
                {
                    starfieldTarget += nanoBots[i].Position;
                    playingPlayers++;
                }
            }
            if (playingPlayers > 0)
            {
                atmosphere.SetTargetPosition(starfieldTarget / playingPlayers);
            }
            atmosphere.Update(elapsedTime);

            // check if we can create a free radicals yet
            if (freeRadicalsTimer > 0f)
            {
                freeRadicalsTimer = Math.Max(freeRadicalsTimer - elapsedTime, 0f);
            }
            if (freeRadicalsTimer <= 0.0f)
            {
                FreeRadicalsSpawning();
                freeRadicalsTimer = freeRadicalsDelay;
            }

            // check if we can create a greenhouse gas yet
            if (greenhouseGasesTimer > 0f)
            {
                greenhouseGasesTimer = Math.Max(greenhouseGasesTimer - elapsedTime, 0f);
            }
            if (greenhouseGasesTimer <= 0.0f)
            {
                GreenhouseGasesSpawning();
                greenhouseGasesTimer = greenhouseGasesDelay;
            }

            UpdateGameLevels();

            // clean up the lists
            actors.Collect();
            particleSystems.Collect();
        }




        /// <summary>
        /// Place this ship in the world.
        /// </summary>
        /// <param name="findSpawnPoint">
        /// If true, the actor's position is changed to a valid, non-colliding point.
        /// </param>
        public void UpdateGameLevels()
        {
            // Reset count values
            freeRadicalCount = 0;
            cFC1Count = 0;
            cFC2Count = 0;
            hydroxylCount = 0;
            nitricOxideCount = 0;
            greenhouseGasesCount = 0;
            ozoneCount = 0;
            nitrousOxideCount = 0;
            methaneCount = 0;
            waterCount = 0;
            carbonDioxideCount = 0;
            // Recount molecules
            for (int i = 0; i < Actors.Count; ++i)
            {
                if ((Actors[i] is CFC1) == true)
                {
                    freeRadicalCount += 1;
                    cFC1Count += 1;
                }
                else if ((Actors[i] is CFC2) == true)
                {
                    freeRadicalCount += 1;
                    cFC2Count += 1;
                }
                else if ((Actors[i] is Hydroxyl) == true)
                {
                    freeRadicalCount += 1;
                    hydroxylCount += 1;
                }
                else if ((Actors[i] is NitricOxide) == true)
                {
                    freeRadicalCount += 1;
                    nitricOxideCount += 1;
                }
                else if ((Actors[i] is Ozone) == true)
                {
                    ozoneCount += 1;
                }
                else if ((Actors[i] is NitrousOxide) == true)
                {
                    greenhouseGasesCount += 1;
                    nitrousOxideCount += 1;
                }
                else if ((Actors[i] is Methane) == true)
                {
                    greenhouseGasesCount += 1;
                    methaneCount += 1;
                }
                else if ((Actors[i] is Water) == true)
                {
                    greenhouseGasesCount += 1;
                    waterCount += 1;
                }
                else if ((Actors[i] is CarbonDioxide) == true)
                {
                    greenhouseGasesCount += 1;
                    carbonDioxideCount += 1;
                }
                else if (actors.Count == i)
                {
                    return;
                }
            }
        }
        #endregion

        #region Collision
        /// <summary>
        /// Move all of the actors in the world.
        /// </summary>
        /// <param name="elapsedTime">The amount of elapsed time, in seconds.</param>
        private void MoveWorld(float elapsedTime)
        {
            Vector2 point = Vector2.Zero;
            // move each actor
            for (int i = 0; i < actors.Count; ++i)
            {
                if (actors[i].Dead)
                {
                    continue;
                }
                // determine how far they are going to move
                Vector2 movement = actors[i].Velocity * elapsedTime;
                // only allow actors that have not collided yet this frame to collide
                // -- otherwise, objects can "double-hit" and trade their momentum fast
                if (actors[i].CollidedThisFrame == false)
                {
                    movement = MoveAndCollide(actors[i], movement);
                }
                // determine the new position
                actors[i].Position += movement;
            }
        }


        /// <summary>
        /// Move the given actor by the given movement, colliding and adjusting
        /// as necessary.
        /// </summary>
        /// <param name="actor">The actor who is moving.</param>
        /// <param name="movement">The desired movement vector for this update.</param>
        /// <returns>The movement vector after considering all collisions.</returns>
        private Vector2 MoveAndCollide(Actor actor, Vector2 movement)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }
            // make sure we care about where this actor goes
            if (actor.Dead || (actor.Collidable == false))
            {
                return movement;
            }
            // make sure the movement is significant
            if (movement.LengthSquared() <= 0f)
            {
                return movement;
            }

            // generate the list of collisions
            Collide(actor, movement);

            // determine if we had any collisions
            if (collisionResults.Count > 0)
            {
                collisionResults.Sort(CollisionResult.Compare);
                foreach (CollisionResult collision in collisionResults)
                {
                    // let the two actors touch each other, and see what happens
                    if (actor.Touch(collision.Actor) && collision.Actor.Touch(actor))
                    {
                        actor.CollidedThisFrame = 
                            collision.Actor.CollidedThisFrame = true;
                        // they should react to the other, even if they just died
                        AdjustVelocities(actor, collision.Actor);
                        return Vector2.Zero;
                    }
                }
            }

            return movement;
        }


        /// <summary>
        /// Determine all collisions that will happen as the given actor moves.
        /// </summary>
        /// <param name="actor">The actor that is moving.</param>
        /// <param name="movement">The actor's movement vector this update.</param>
        /// <remarks>The results are stored in the cached list.</remarks>
        public void Collide(Actor actor, Vector2 movement)
        {
            collisionResults.Clear();

            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }
            if (actor.Dead || (actor.Collidable == false))
            {
                return;
            }

            // determine the movement direction and scalar
            float movementLength = movement.Length();
            if (movementLength <= 0f)
            {
                return;
            }

            // check each actor
            foreach (Actor checkActor in actors)
            {
                if ((actor == checkActor) || checkActor.Dead || !checkActor.Collidable)
                {
                    continue;
                }

                // calculate the target vector
                Vector2 checkVector = checkActor.Position - actor.Position;
                float distanceBetween = checkVector.Length() - 
                    (checkActor.Radius + actor.Radius);

                // check if they could possibly touch no matter the direction
                if (movementLength < distanceBetween)
                {
                    continue;
                }

                // determine how much of the movement is bringing the two together
                float movementTowards = Vector2.Dot(movement, checkVector);

                // check to see if the movement is away from each other
                if (movementTowards < 0f)
                {
                    continue;
                }

                if (movementTowards < distanceBetween)
                {
                    continue;
                }

                CollisionResult result = new CollisionResult();
                result.Distance = distanceBetween;
                result.Normal = Vector2.Normalize(checkVector);
                result.Actor = checkActor;

                collisionResults.Add(result);
            }
        }


        /// <summary>
        /// Adjust the velocities of the two actors as if they have collided,
        /// distributing their velocities according to their masses.
        /// </summary>
        /// <param name="actor1">The first actor.</param>
        /// <param name="actor2">The second actor.</param>
        private static void AdjustVelocities(Actor actor1, Actor actor2)
        {
            // don't adjust velocities if at least one has negative mass
            if ((actor1.Mass <= 0f) || (actor2.Mass <= 0f))
            {
                return;
            }

            // determine the vectors normal and tangent to the collision
            Vector2 collisionNormal = Vector2.Normalize(
                actor2.Position - actor1.Position);
            Vector2 collisionTangent = new Vector2(
                -collisionNormal.Y, collisionNormal.X);

            // determine the velocity components along the normal and tangent vectors
            float velocityNormal1 = Vector2.Dot(actor1.Velocity, collisionNormal);
            float velocityTangent1 = Vector2.Dot(actor1.Velocity, collisionTangent);
            float velocityNormal2 = Vector2.Dot(actor2.Velocity, collisionNormal);
            float velocityTangent2 = Vector2.Dot(actor2.Velocity, collisionTangent);

            // determine the new velocities along the normal
            float velocityNormal1New = ((velocityNormal1 * (actor1.Mass - actor2.Mass))
                + (2f * actor2.Mass * velocityNormal2)) / (actor1.Mass + actor2.Mass);
            float velocityNormal2New = ((velocityNormal2 * (actor2.Mass - actor1.Mass))
                + (2f * actor1.Mass * velocityNormal1)) / (actor1.Mass + actor2.Mass);

            // determine the new total velocities
            actor1.Velocity = (velocityNormal1New * collisionNormal) + 
                (velocityTangent1 * collisionTangent);
            actor2.Velocity = (velocityNormal2New * collisionNormal) + 
                (velocityTangent2 * collisionTangent);
        }
        
        
        /// <summary>
        /// Find a valid point for the actor to spawn.
        /// </summary>
        /// <param name="actor">The actor to find a location for.</param>
        /// <remarks>This query is not bounded, which would be needed in a more complex
        /// game with a likelihood of no valid spawn locations.</remarks>
        /// <returns>A valid location for the user to spawn.</returns>
        public Vector2 FindSpawnPoint(Actor actor)
        {
            if (actor == null)
            {
                throw new ArgumentNullException("actor");
            }

            Vector2 spawnPoint;
            float radius = actor.Radius;

            // fudge the radius slightly so we're not right on top of another actor
            if (actor is NanoBot)
            {
                radius *= 2f;
            }
            else
            {
                radius *= 1.1f;
            }
            radius = (float)Math.Ceiling(radius);

            Vector2 spawnMinimum = new Vector2(
                safeDimensions.X + radius, 
                safeDimensions.Y + radius);
            Vector2 spawnDimensions = new Vector2(
                (float)Math.Floor(safeDimensions.Width - 2f * radius),
                (float)Math.Floor(safeDimensions.Height - 2f * radius));
            Vector2 spawnMaximum = spawnMinimum + spawnDimensions;

            //Collision.CircleLineCollisionResult result = 
            //    new Collision.CircleLineCollisionResult();
            bool valid = true;
            while (true)
            {
                valid = true;
                // generate a new spawn point
                spawnPoint = new Vector2(
                    spawnMinimum.X + spawnDimensions.X * (float)random.NextDouble(),
                    spawnMinimum.Y + spawnDimensions.Y * (float)random.NextDouble());
                if ((spawnPoint.X < spawnMinimum.X) ||
                    (spawnPoint.Y < spawnMinimum.Y) ||
                    (spawnPoint.X > spawnMaximum.X) ||
                    (spawnPoint.Y > spawnMaximum.Y))
                {
                    continue;
                }
                // if we don't collide, then one is good enough
                if (actor.Collidable == false)
                {
                    break; 
                }
                // check against all other actors
                if (valid == true)
                {
                    foreach (Actor checkActor in actors)
                    {
                        if ((actor == checkActor) || checkActor.Dead)
                        {
                            continue;
                        }
                        if (Collision.CircleCircleIntersect(checkActor.Position,
                            checkActor.Radius, spawnPoint, radius))
                        {
                            valid = false;
                            break;
                        }
                    }
                }
                // if we have gotten this far, then the spawn point is good
                if (valid == true)
                {
                    break;
                }
            }
            return spawnPoint;
        }
        #endregion

        #region Molecular Bonds

        /// <summary>
        /// Defines the interaction between the actors when they touch.
        /// </summary>
        /// <param name="target">The actor that is bonding this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public void BondOxygenTwo(Actor oxygen1, Actor oxygen2, int O)
        {
            deadO = deadO + O;
            if (deadO == 2)
            {
                oxygen1.Die(oxygen1);
                oxygen2.Die(oxygen2);
                Vector2 pos = (oxygen1.Position + oxygen2.Position) / 2;
                Vector2 vel = (oxygen1.Velocity + oxygen2.Velocity) / 2;
                Vector2 dir = (oxygen1.Direction + oxygen2.Direction) / 2;
                OxygenTwo oxygenTwo = new OxygenTwo(this);
                oxygenTwo.Spawn(false);
                oxygenTwo.Position = pos;
                oxygenTwo.Velocity = vel;
                oxygenTwo.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 36, 64f, 128f, 2f, 0.05f, Color.Red));
                deadO = 0;
            }
        }
        public void UnbondOxygenTwo(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Oxygen oxygen1 = new Oxygen(this);
            oxygen1.Spawn(false);
            oxygen1.Position = pos;
            oxygen1.Velocity = vel * 0.5f;
            oxygen1.Direction = dir * 0.5f;
            Oxygen oxygen2 = new Oxygen(this);
            oxygen2.Spawn(false);
            Vector2 newPos = new Vector2(45f * resVar, 0);
            oxygen2.Position = pos + newPos;
            oxygen2.Velocity = vel * 2f;
            oxygen2.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, Color.Red));
        }
        public void BondDeuterium(Actor hydrogen1, Actor hydrogen2, int H)
        {
            deadH = deadH + H;
            if (deadH == 2)
            {
                hydrogen1.Die(hydrogen1);
                hydrogen2.Die(hydrogen2);
                Vector2 pos = (hydrogen1.Position + hydrogen2.Position) / 2;
                Vector2 vel = (hydrogen1.Velocity + hydrogen2.Velocity) / 2;
                Vector2 dir = (hydrogen1.Direction + hydrogen2.Direction) / 2;
                Deuterium deuterium = new Deuterium(this);
                deuterium.Spawn(false);
                deuterium.Position = pos;
                deuterium.Velocity = vel;
                deuterium.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 16, 32f, 64f, 2f, 0.05f, hHColor));
                deadH = 0;
            }
        }
        public void UnbondDeuterium(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Hydrogen H1 = new Hydrogen(this);
            H1.Spawn(false);
            H1.Position = pos;
            H1.Velocity = vel * 0.5f;
            H1.Direction = dir * 0.5f;
            Hydrogen H2 = new Hydrogen(this);
            H2.Spawn(false);
            Vector2 newPos = new Vector2(10f * resVar, 0);
            H2.Position = pos + newPos;
            H2.Velocity = vel * 2f;
            H2.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 16, 32f, 64f, 2f, 0.05f, hHColor));
        }
        public void BondNitrogenTwo(Actor nitrogen1, Actor nitrogen2, int N)
        {
            deadN = deadN + N;
            if (deadN == 2)
            {
                nitrogen1.Die(nitrogen1);
                nitrogen2.Die(nitrogen2);
                Vector2 pos = (nitrogen1.Position + nitrogen2.Position) / 2;
                Vector2 vel = (nitrogen1.Velocity + nitrogen2.Velocity) / 2;
                Vector2 dir = (nitrogen1.Direction + nitrogen2.Direction) / 2;
                NitrogenTwo nitrogenTwo = new NitrogenTwo(this);
                nitrogenTwo.Spawn(false);
                nitrogenTwo.Position = pos;
                nitrogenTwo.Velocity = vel;
                nitrogenTwo.Direction = dir;
                ParticleSystems.Add(new ParticleSystem(pos,
                    dir, 36, 64f, 128f, 2f, 0.05f, n2Color));
                deadN = 0;
            }
        }
        public void UnbondNitrogenTwo(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Nitrogen N1 = new Nitrogen(this);
            N1.Spawn(false);
            N1.Position = pos;
            N1.Velocity = vel * 0.5f;
            N1.Direction = dir * 0.5f;
            Nitrogen N2 = new Nitrogen(this);
            N2.Spawn(false);
            Vector2 newPos = new Vector2(30f * resVar, 0);
            N2.Position = pos + newPos;
            N2.Velocity = vel * 2f;
            N2.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, n2Color));
        }
        public void BondOzone(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, o3Color));
            Ozone ozone = new Ozone(this);
            ozone.Spawn(false);
            ozone.Position = pos;
            ozone.Velocity = vel;
            ozone.Direction = dir;
        }
        public void UnbondOzone(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, o3Color));
            OxygenTwo oxygenTwo = new OxygenTwo(this);
            oxygenTwo.Spawn(false);
            oxygenTwo.Position = pos;
            oxygenTwo.Velocity = vel * 0.5f;
            oxygenTwo.Direction = dir * 0.5f;
            Oxygen oxygen = new Oxygen(this);
            oxygen.Spawn(false);
            Vector2 newPos = new Vector2(70f * resVar, 0);
            oxygen.Position = pos + newPos;
            oxygen.Velocity = vel * 0.5f;
            oxygen.Direction = dir * 0.5f;
        }
        public void BondCarbonDioxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            CarbonDioxide CO2 = new CarbonDioxide(this);
            CO2.Spawn(false);
            CO2.Position = pos;
            CO2.Velocity = vel;
            CO2.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cO2Color));
        }
        public void UnbondCarbonDioxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            OxygenTwo O2 = new OxygenTwo(this);
            O2.Spawn(false);
            O2.Position = pos;
            O2.Velocity = vel * 0.5f;
            O2.Direction = dir * 0.5f;
            Carbon C = new Carbon(this);
            C.Spawn(false);
            Vector2 newPos = new Vector2(70f * resVar, 0);
            C.Position = pos + newPos;
            C.Velocity = vel * 2f;
            C.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cO2Color));
        }
        public void BondHydroxyl(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Hydroxyl OH = new Hydroxyl(this);
            OH.Spawn(false);
            OH.Position = pos;
            OH.Velocity = vel;
            OH.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, oHColor));
        }
        public void UnbondHydroxyl(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Oxygen O = new Oxygen(this);
            O.Spawn(false);
            O.Position = pos;
            O.Velocity = vel * 0.5f;
            O.Direction = dir * 0.5f;
            Hydrogen H = new Hydrogen(this);
            H.Spawn(false);
            Vector2 newPos = new Vector2(50f * resVar, 0);
            H.Position = pos + newPos;
            H.Velocity = vel * 2f;
            H.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, oHColor));
        }
        public void BondNitricOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            NitricOxide NO = new NitricOxide(this);
            NO.Spawn(false);
            NO.Position = pos;
            NO.Velocity = vel;
            NO.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, nOColor));
        }
        public void UnbondNitricOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Oxygen O = new Oxygen(this);
            O.Spawn(false);
            O.Position = pos;
            O.Velocity = vel * 0.5f;
            O.Direction = dir * 0.5f;
            Nitrogen N = new Nitrogen(this);
            N.Spawn(false);
            Vector2 newPos = new Vector2(55f * resVar, 0);
            N.Position = pos + newPos;
            N.Velocity = vel * 2f;
            N.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, nOColor));
        }
        public void BondWater(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Water H2O = new Water(this);
            H2O.Spawn(false);
            H2O.Position = pos;
            H2O.Velocity = vel;
            H2O.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, h2OColor));
        }
        public void UnbondWater(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Oxygen O = new Oxygen(this);
            O.Spawn(false);
            O.Position = pos;
            O.Velocity = vel * 0.5f;
            O.Direction = dir * 0.5f;
            Deuterium D = new Deuterium(this);
            D.Spawn(false);
            Vector2 newPos = new Vector2(30f * resVar, 0);
            D.Position = pos + newPos;
            D.Velocity = vel * 2f;
            D.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, h2OColor));
        }
        public void BondMethylene(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Methylene CH2 = new Methylene(this);
            CH2.Spawn(false);
            CH2.Position = pos;
            CH2.Velocity = vel;
            CH2.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cH2Color));
        }
        public void UnbondMethylene(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Carbon C = new Carbon(this);
            C.Spawn(false);
            C.Position = pos;
            C.Velocity = vel * 0.5f;
            C.Direction = dir * 0.5f;
            Deuterium D = new Deuterium(this);
            D.Spawn(false);
            Vector2 newPos = new Vector2(25f * resVar, 0);
            D.Position = pos + newPos;
            D.Velocity = vel * 2f;
            D.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cH2Color));
        }
        public void BondMethane(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Methane CH4 = new Methane(this);
            CH4.Spawn(false);
            CH4.Position = pos;
            CH4.Velocity = vel;
            CH4.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cH4Color));
        }
        public void UnbondMethane(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Carbon C = new Carbon(this);
            C.Spawn(false);
            C.Position = pos;
            C.Velocity = vel * 0.5f;
            C.Direction = dir * 0.5f;
            Deuterium D1 = new Deuterium(this);
            D1.Spawn(false);
            Vector2 newPos1 = new Vector2(25f * resVar, 0);
            D1.Position = pos + newPos1;
            D1.Velocity = vel;
            D1.Direction = dir;
            Deuterium D2 = new Deuterium(this);
            D2.Spawn(false);
            Vector2 newPos2 = new Vector2(40f * resVar, 0);
            D2.Position = pos + newPos2;
            D2.Velocity = vel * 2f;
            D2.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, cH4Color));
        }
        public void BondNitrousOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            NitrousOxide N2O = new NitrousOxide(this);
            N2O.Spawn(false);
            N2O.Position = pos;
            N2O.Velocity = vel;
            N2O.Direction = dir;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, n2OColor));
        }
        public void UnbondNitrousOxide(Vector2 pos, Vector2 vel, Vector2 dir)
        {
            Oxygen O = new Oxygen(this);
            O.Spawn(false);
            O.Position = pos;
            O.Velocity = vel * 0.5f;
            O.Direction = dir * 0.5f;
            NitrogenTwo N2 = new NitrogenTwo(this);
            N2.Spawn(false);
            Vector2 newPos = new Vector2(45f * resVar, 0);
            N2.Position = pos + newPos;
            N2.Velocity = vel * 2f;
            N2.Direction = dir * 2f;
            ParticleSystems.Add(new ParticleSystem(pos,
                dir, 36, 64f, 128f, 2f, 0.05f, n2OColor));
        }
        #endregion
    }
}
