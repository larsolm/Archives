//-----------------------------------------------------------------------------
// Melvyn May
// 2D Static Sprite.
//-----------------------------------------------------------------------------

#ifndef _WHIP_H_
#define _WHIP_H_

#ifndef _FXSCENEOBJECT2D_H_
#include "../T2D/fxSceneObject2D.h"
#endif

#ifndef _FXIMAGEMAPDATABLOCK2D_H_
#include "../T2D/fxImageMapDatablock2D.h"
#endif

#include "physics2.h"


//-----------------------------------------------------------------------------
// Static Sprite Datablock 2D.
//-----------------------------------------------------------------------------
class WhipDatablock : public fxSceneObjectDatablock2D
{
public:
	typedef fxSceneObjectDatablock2D Parent;

	WhipDatablock();
	virtual ~WhipDatablock();

	static void  initPersistFields();
	virtual bool onAdd();
	virtual void packData(BitStream* stream);
	virtual void unpackData(BitStream* stream);

	StringTableEntry		mImageMapName;
	U32						mFrame;

	// Declare FX Datablock.
	DECLARE_FXDATABLOCK2D();

	// Declare Console Object.
	DECLARE_CONOBJECT(WhipDatablock);
};

//-----------------------------------------------------------------------------
// Static Object 2D.
//-----------------------------------------------------------------------------
class Whip : public fxSceneObject2D
{
	typedef fxSceneObject2D			Parent;
	WhipDatablock*		            mConfigDataBlock;
	fxImageMapDatablock2D*			mImageMapDataBlock;
	U32							   	mFrame;

   bool mouseMove;

	const U32 T2D_Stream_HeaderID;
	const U32 T2D_Stream_Version;

public:
   RopeSimulation* whip;
	Whip();
	virtual ~Whip();

   fxVector2D getMassPosition(S32 index);
   fxVector2D getMassVelocity(S32 index);
   F32 getMassSpeed(S32 index);

   void MoveX(F32 val);
   void MoveY(F32 val);

	bool setImageMap( const char* imageMapName, U32 frame );
	bool setFrame( U32 frame );

	const char* getImageMapName( void )	{ if (mImageMapDataBlock) return mImageMapDataBlock->getName(); else return NULL; };
	U32 getFrame( void )				{ return mFrame; };

	virtual bool onAdd();
	virtual void onRemove();
	virtual bool loadStream( FileStream& fileStream, fxSceneGraph2D* pSceneGraph2D, Vector<fxSceneObject2D*>& ObjReferenceList, bool ignoreLayerOrder );
	virtual bool saveStream( FileStream& fileStream, U32 serialiseID, U32 serialiseKey );
	virtual void integrateObject( F32 sceneTime, F32 elapsedTime, CDebugStats* pDebugStats );
	virtual void renderObject( const RectF& viewPort, const RectF& viewIntersection );

	DECLARE_CONOBJECT(Whip);
};

#endif // _FXSTATICSPRITE2D_H_
