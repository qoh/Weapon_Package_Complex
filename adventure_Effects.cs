//
// common particle effects for the adventurer pack
//

 // small bullet trails and fire particles
 /////////////////////////////////////////////////////

datablock ParticleData(advSmallBulletFireParticle)
{
    dragCoefficient      = 0;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 50;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/star1";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "1.0 0.5 0 0.9";
    colors[1]     = "0.9 0.4 0 0.8";
    colors[2]     = "1 0.5 0.2 0.6";
    colors[3]     = "1 0.5 0.2 0.4";

    sizes[0]      = 1.15;
   sizes[1]      = 0.4;
    sizes[2]      = 0.10;
    sizes[3]      = 0.0;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advSmallBulletFireEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 54.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advSmallBulletFireParticle";
};

datablock ParticleData(advSmallBulletSmokeParticle)
{
    dragCoefficient      = 3;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 100;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/cloud";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "0.6 0.6 0.6 0.0";
    colors[1]     = "0.7 0.7 0.7 0.3";
    colors[2]     = "1 1 1 0.2";
    colors[3]     = "1 1 1 0";

    sizes[0]      = 0.0;
   sizes[1]      = 0.4;
    sizes[2]      = 0.10;
    sizes[3]      = 0.05;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advSmallBulletSmokeEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 32.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advSmallBulletSmokeParticle";
};

datablock ParticleData(advSmallBulletTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 80;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "1 1 0.5 0.1";
	colors[1]	= "1 1 0.7 0.08";
	colors[2]	= "0.9 0.9 0.9 0";
	sizes[0]	= 0.2;
	sizes[1]	= 0.15;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(advSmallBulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;
   ejectionOffset = 0;
   thetaMin         = 0.0;
   thetaMax         = 90.0;

   particles = advSmallBulletTrailParticle;
};

 // big bullet trails and fire particles
 /////////////////////////////////////////////////////

datablock ParticleData(advBigBulletFireParticle)
{
    dragCoefficient      = 0;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 50;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/star1";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "1.0 0.5 0 0.9";
    colors[1]     = "0.9 0.4 0 0.8";
    colors[2]     = "1 0.5 0.2 0.6";
    colors[3]     = "1 0.5 0.2 0.4";

    sizes[0]      = 1.75;
   sizes[1]      = 0.4;
    sizes[2]      = 0.10;
    sizes[3]      = 0.0;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advBigBulletFireEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 64.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advBigBulletFireParticle";
};

datablock ParticleData(advBigBulletSmokeParticle)
{
    dragCoefficient      = 3;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 100;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/cloud";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "0.6 0.6 0.6 0.0";
    colors[1]     = "0.7 0.7 0.7 0.3";
    colors[2]     = "1 1 1 0.2";
    colors[3]     = "1 1 1 0";

    sizes[0]      = 0.0;
   sizes[1]      = 0.4;
    sizes[2]      = 0.10;
    sizes[3]      = 0.05;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advBigBulletSmokeEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 32.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advBigBulletSmokeParticle";
};

datablock ParticleData(advBigBulletTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 190;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "1 1 0.5 0.1";
	colors[1]	= "1 1 0.7 0.08";
	colors[2]	= "0.9 0.9 0.9 0";
	sizes[0]	= 0.3;
	sizes[1]	= 0.15;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(advBigBulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;
   ejectionOffset = 0;
   thetaMin         = 0.0;
   thetaMax         = 90.0;

   particles = advBigBulletTrailParticle;
};

 // huge bullet trails and fire particles
 /////////////////////////////////////////////////////

datablock ParticleData(advHugeBulletFireParticle)
{
    dragCoefficient      = 0;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 50;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/star1";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "1.0 0.5 0 0.9";
    colors[1]     = "0.9 0.4 0 0.8";
    colors[2]     = "1 0.5 0.2 0.6";
    colors[3]     = "1 0.5 0.2 0.4";

    sizes[0]      = 2.75;
   sizes[1]      = 1.3;
    sizes[2]      = 0.10;
    sizes[3]      = 0.0;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advHugeBulletFireEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 96.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advHugeulletFireParticle";
};

datablock ParticleData(advHugeBulletSmokeParticle)
{
    dragCoefficient      = 3;
    gravityCoefficient   = 0;
    inheritedVelFactor   = 0;
    constantAcceleration = 0.0;
    lifetimeMS           = 100;
    lifetimeVarianceMS   = 0;
    textureName          = "base/data/particles/cloud";
    spinSpeed        = 9000.0;
    spinRandomMin        = -5000.0;
    spinRandomMax        = 5000.0;

    colors[0]     = "0.6 0.6 0.6 0.0";
    colors[1]     = "0.7 0.7 0.7 0.1";
    colors[2]     = "1 1 1 0.1";
    colors[3]     = "1 1 1 0";

    sizes[0]      = 0.0;
   sizes[1]      = 0.7;
    sizes[2]      = 0.10;
    sizes[3]      = 0.05;

   times[0] = 0.0;
   times[1] = 0.1;
   times[2] = 0.5;
   times[3] = 1.0;

    useInvAlpha = false;
};
datablock ParticleEmitterData(advHugeBulletSmokeEmitter)
{
   ejectionPeriodMS = 4;
   periodVarianceMS = 0;
   ejectionVelocity = 32.0;
   velocityVariance = 0.0;
   ejectionOffset   = 0.0;
   thetaMin         = 0;
   thetaMax         = 1;
   phiReferenceVel  = 0;
   phiVariance      = 360;
   overrideAdvance = false;

   particles = "advHugeBulletSmokeParticle";
};

datablock ParticleData(advHugeBulletTrailParticle)
{
	dragCoefficient		= 3.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= 0.0;
	inheritedVelFactor	= 0.0;
	constantAcceleration	= 0.0;
	lifetimeMS		= 190;
	lifetimeVarianceMS	= 0;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= false;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/dot";

	// Interpolation variables
	colors[0]	= "1 1 0.5 0.1";
	colors[1]	= "1 1 0.7 0.08";
	colors[2]	= "0.9 0.9 0.9 0";
	sizes[0]	= 0.9;
	sizes[1]	= 0.55;
	sizes[2]	= 0.0;
	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};
datablock ParticleEmitterData(advHugeBulletTrailEmitter)
{
   ejectionPeriodMS = 1;
   periodVarianceMS = 0;
   ejectionVelocity = 0; //0.25;
   velocityVariance = 0; //0.10;
   ejectionOffset = 0;
   thetaMin         = 0.0;
   thetaMax         = 90.0;

   particles = advHugeBulletTrailParticle;
};
