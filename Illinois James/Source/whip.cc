//-----------------------------------------------------------------------------
// Melvyn May
// 2D Static Sprite.
//-----------------------------------------------------------------------------

#include "dgl/dgl.h"
#include "console/consoleTypes.h"
#include "core/bitStream.h"
#include "./whip.h"


//------------------------------------------------------------------------------

IMPLEMENT_CO_DATABLOCK_V1(WhipDatablock);
IMPLEMENT_CONOBJECT(Whip);

//------------------------------------------------------------------------------

WhipDatablock::WhipDatablock() :	INITIALISE_FXDATABLOCK2D(ID_WhipDatablock),
															mImageMapName(StringTable->insert("")),
															mFrame(0)
{
}

//------------------------------------------------------------------------------

WhipDatablock::~WhipDatablock()
{
}

//------------------------------------------------------------------------------

void WhipDatablock::initPersistFields()
{
	Parent::initPersistFields();

	// Fields.
	addField("imageMap",	TypeString,				Offset(mImageMapName,		WhipDatablock));
	addField("frame",		TypeS32,				Offset(mFrame,				WhipDatablock));
}

//------------------------------------------------------------------------------

bool WhipDatablock::onAdd()
{
	// Eventually, we'll need to deal with Server/Client functionality!

	// Call Parent.
	if(!Parent::onAdd())
		return false;

	// Validate Datablock.
	mValid = true;

	// Return Okay.
	return true;
}

//------------------------------------------------------------------------------

void WhipDatablock::packData(BitStream* stream)
{
	// Parent packing.
	Parent::packData(stream);

	// Write Datablock.
	//stream->write( mFrame );
}

//------------------------------------------------------------------------------

void WhipDatablock::unpackData(BitStream* stream)
{
	// Parent unpacking.
	Parent::unpackData(stream);

	// Read Datablock.
	//stream->read( &mFrame );
}




//-----------------------------------------------------------------------------
// Constructor.
//-----------------------------------------------------------------------------
Whip::Whip() :	T2D_Stream_HeaderID(makeFourCCTag('2','D','S','S')),
										T2D_Stream_Version(0X00000003),
										mImageMapDataBlock(NULL),
										mFrame(0),
                              mouseMove(false)
{
   whip = new RopeSimulation(32, 0.05f, 10000.0f, 0.10f, 0.5f, Vector3D(0.0f, 0.0f, 15.0f), 0.1f, 100.0f, 0.2f, 2.0f, -1000.0f);
}

//-----------------------------------------------------------------------------
// Destructor.
//-----------------------------------------------------------------------------
Whip::~Whip()
{
   whip->release();
   delete(whip);
   whip = NULL;
}

//-----------------------------------------------------------------------------
// OnAdd
//-----------------------------------------------------------------------------
bool Whip::onAdd()
{
	// Eventually, we'll need to deal with Server/Client functionality!

	// Call Parent.
	if(!Parent::onAdd())
		return false;

	// Cast the Datablock.
	mConfigDataBlock = dynamic_cast<WhipDatablock*>(Parent::mConfigDataBlock);

	// Transfer Datablock (if we've got one).
	if ( checkFXDatablock2D( mConfigDataBlock, fxBaseDatablock2D::ID_fxStaticSpriteDatablock2D ) )
	{
		// Set ImageMap/Frame.
		setImageMap( mConfigDataBlock->mImageMapName, mConfigDataBlock->mFrame );
	}
	else if ( mConfigDataBlock )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::onAdd() - fxStaticSprite2D Datablock is invalid! (%s)", getIdString());
	}


	// Return Okay.
	return true;
}

//-----------------------------------------------------------------------------
// OnRemove.
//-----------------------------------------------------------------------------
void Whip::onRemove()
{
	// Call Parent.
	Parent::onRemove();
}


//-----------------------------------------------------------------------------
// Set ImageMap.
//-----------------------------------------------------------------------------
ConsoleMethod(Whip, setImageMap, bool, 3, 4, "(imageMapName$, [int frame]) - Sets imageMap/Frame.")
{
	// Calculate Frame.
	U32 frame = argc >= 4 ? dAtoi(argv[3]) : 0;

	// Set ImageMap.
	return object->setImageMap( argv[2], frame );
}   
// Set ImageMap/Frame.
bool Whip::setImageMap( const char* imageMapName, U32 frame )
{
	// Invalid ImageMap Name.
	if ( imageMapName == StringTable->insert("") )
		return false;

	// Find ImageMap Datablock.
	fxImageMapDatablock2D* pImageMapDataBlock = dynamic_cast<fxImageMapDatablock2D*>(Sim::findObject( imageMapName ));

	// Set Datablock.
	if ( !checkFXDatablock2D( pImageMapDataBlock, fxBaseDatablock2D::ID_fxImageMapDatablock2D ) )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::setImageMap() - fxImageMapDatablock2D Datablock is invalid! (%s)", imageMapName);
		// Return Here.
		return false;
	}

	// Check Frame Validity.
	if ( frame >= pImageMapDataBlock->getImageMapRegionCount() )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::setImageMap() - Invalid Frame #%d for fxImageMapDatablock2D Datablock! (%s)", frame, imageMapName);
		// Return Here.
		return false;
	}

	// Set ImageMap Datablock.
	mImageMapDataBlock = pImageMapDataBlock;

	// Set Frame.
	mFrame = frame;

	// Return Okay.
	return true;
}




//-----------------------------------------------------------------------------
// Set ImageMap Frame.
//-----------------------------------------------------------------------------
ConsoleMethod(Whip, setFrame, bool, 3, 3, "(frame) - Sets imageMap frame.")
{
	// Set ImageMap Frame.
	return object->setFrame( dAtoi(argv[2]) );
}   
// Set ImageMap/Frame.
bool Whip::setFrame( U32 frame )
{
	// Check Existing ImageMap.
	if ( !mImageMapDataBlock )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::setFrame() - Cannot set Frame without existing fxImageMapDatablock2D Datablock!");
		// Return Here.
		return false;
	}

	// Check Frame Validity.
	if ( frame >= mImageMapDataBlock->getImageMapRegionCount() )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::setFrame() - Invalid Frame #%d for fxImageMapDatablock2D Datablock! (%s)", frame, getIdString());
		// Return Here.
		return false;
	}

	// Set Frame.
	mFrame = frame;

	// Return Okay.
	return true;
}


//-----------------------------------------------------------------------------
// Get ImageMap Name.
//-----------------------------------------------------------------------------
ConsoleMethod(Whip, getImageMap, const char*, 2, 2, "Gets current imageMap name.")
{
	// Get ImageMap Name.
	return object->getImageMapName();
}   


//-----------------------------------------------------------------------------
// Get ImageMap Frame.
//-----------------------------------------------------------------------------
ConsoleMethod(Whip, getFrame, S32, 2, 2, "Gets current imageMap Frame.")
{
	// Get ImageMap Frame.
	return object->getFrame();
}   


//-----------------------------------------------------------------------------
// Load From File Stream.
//-----------------------------------------------------------------------------
bool Whip::loadStream( FileStream& fileStream, fxSceneGraph2D* pSceneGraph2D, Vector<fxSceneObject2D*>& ObjReferenceList, bool ignoreLayerOrder )
{
	// Handle Parent Load First.
	if ( !Parent::loadStream( fileStream, pSceneGraph2D, ObjReferenceList, ignoreLayerOrder ) )
		return false;

	// Read Object-Stream Header/Version.
	U32 streamHeaderID;
	U32 streamVersion;
	if (	!fileStream.read( &streamHeaderID ) ||
			!fileStream.read( &streamVersion ) )
		return false;
	// Is this the correct Stream Header?
	if ( streamHeaderID != T2D_Stream_HeaderID )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::loadStream() - Invalid Stream Header ID!");
		return false;
	}
	// Check Stream Version.
	else if ( streamVersion != T2D_Stream_Version )
	{
		// Warn.
		Con::warnf("fxStaticSprite2D::loadStream() - Invalid Stream Version!");
		return false;
	}


	// *********************************************************
	// Read Object Information.
	// *********************************************************

	U32						frame;
	bool					imageMapFlag;
	char					imageMapName[256];

	// Read Ad-Hoc Info.
	if ( !fileStream.read( &imageMapFlag ) )
		return false;

	// Do we have an imageMap?
	if ( imageMapFlag )
	{
		// Yes, so read ImageMap Name.
		fileStream.readString( imageMapName );

		// Read Frame.
		if  ( !fileStream.read( &frame ) )
			return false;

		// Set ImageMap/Frame.
		setImageMap( imageMapName, frame );
	}

	// Return Okay.
	return true;
}


//-----------------------------------------------------------------------------
// Save to File Stream.
//-----------------------------------------------------------------------------
bool Whip::saveStream( FileStream& fileStream, U32 serialiseID, U32 serialiseKey )
{
	// Finish if already saved to this stream.
	if ( serialiseID == mLocalSerialiseID )
		// Return Okay.
		return true;

	// Handle Parent Save First.
	if ( !Parent::saveStream( fileStream, serialiseID, serialiseKey ) )
		return false;

	// Write Stream Header.
	if (	!fileStream.write( T2D_Stream_HeaderID ) ||
			!fileStream.write( T2D_Stream_Version ) )
		return false;


	// *********************************************************
	// Write Object Information.
	// *********************************************************

	// Ad-Hoc Info.
	if ( mImageMapDataBlock )
	{
		// Write ImageMap Datablock Name.
		if ( !fileStream.write( true ) )
			return false;

		// Write ImageMap Datablock Name.
		fileStream.writeString( mImageMapDataBlock->getName() );

		// Write Frame.
		if  ( !fileStream.write( mFrame ) )
			return false;
	}
	else
	{
		// Write "No ImageMap Datablock".
		if ( !fileStream.write( false ) )
			return false;
	}

	// Return Okay.
	return true;
}

//-----------------------------------------------------------------------------
// Integrate Object.
//-----------------------------------------------------------------------------
void Whip::integrateObject( F32 sceneTime, F32 elapsedTime, CDebugStats* pDebugStats )
{
   F32 dt = elapsedTime;

   F32 maxdt = 0.002f;

   int numIterations = (int)(dt / maxdt) + 1;
   if (numIterations != 0) dt = dt / numIterations;

   for (int i = 0; i < numIterations; i++) whip->operate(dt);

	// Call Parent.
	Parent::integrateObject( sceneTime, elapsedTime, pDebugStats );
}

ConsoleMethod(Whip, GetMassPosition, const char*, 3, 3, "Whip.GetMassPosition(%index)") {
   fxVector2D pos = object->getMassPosition(dAtoi(argv[2]));
	// Create Returnable Buffer.
	char* pBuffer = Con::getReturnBuffer(256);
	// Format Buffer.
	dSprintf(pBuffer, 256, "%f %f", pos.mX, pos.mY);
	// Return Velocity.
	return pBuffer;
}

fxVector2D Whip::getMassPosition(S32 index) {
   return fxVector2D(whip->getMass(index)->pos.x * 10.0, whip->getMass(index)->pos.y * 10.0);
}

ConsoleMethod(Whip, GetMassVelocity, const char*, 3, 3, "Whip.GetMassPosition(%index)") {
   fxVector2D vel = object->getMassVelocity(dAtoi(argv[2]));
	// Create Returnable Buffer.
	char* pBuffer = Con::getReturnBuffer(256);
	// Format Buffer.
	dSprintf(pBuffer, 256, "%f %f", vel.mX, vel.mY);
	// Return Velocity.
	return pBuffer;
}

fxVector2D Whip::getMassVelocity(S32 index) {
   return fxVector2D(whip->getMass(index)->vel.x, whip->getMass(index)->vel.y);
}

ConsoleMethod(Whip, GetMassSpeed, F32, 3, 3, "Whip.GetMassSpeed(%index)") {
   return object->getMassSpeed(dAtoi(argv[2]));
}

F32 Whip::getMassSpeed(S32 index) {
   fxVector2D speed(whip->getMass(index)->vel.x, whip->getMass(index)->vel.y);
   return speed.len();
}

ConsoleMethod(Whip, MoveX, void, 3, 3, "Whip.MoveX(1.0)") {
   object->MoveX(dAtof(argv[2]));
}

ConsoleMethod(Whip, MoveY, void, 3, 3, "Whip.MoveY(1.0)") {
   object->MoveY(dAtof(argv[2]));
}

ConsoleMethod(Whip, GetMoveX, F32, 2, 2, "Whip.GetMoveX()") {
   return object->whip->ropeConnectionVel.x;
}

ConsoleMethod(Whip, GetMoveY, F32, 2, 2, "Whip.GetMoveY()") {
   return object->whip->ropeConnectionVel.y;
}

void Whip::MoveX(F32 val) {
   mouseMove = true;
   // Bind the rope to the middle of the screen
   // Grab the distance from the center
   F32 distance = whip->ropeConnectionPos.length();
   // Only allow movement within a certain radius
   // This has the side effect of disallowing erattic movements
   if (distance < 2.5) whip->ropeConnectionVel.x = val;
}

void Whip::MoveY(F32 val) {
   mouseMove = true;
   // Bind the rope to the middle of the screen
   // Grab the distance from the center
   F32 distance = whip->ropeConnectionPos.length();
   // Only allow movement within a certain radius
   // This has the side effect of disallowing erattic movements
   if (distance < 2.5) whip->ropeConnectionVel.y = val;
}

//-----------------------------------------------------------------------------
// Render Object.
//-----------------------------------------------------------------------------
void Whip::renderObject( const RectF& viewPort, const RectF& viewIntersection )
{
   // Draw the dudes arm/hand
   // Outline
   glColor3ub(0, 0, 0);
   glLineWidth(10);
   fxVector2D pos1(0.0f, 0.0f);
   fxVector2D pos2(whip->getMass(0)->pos.x * 10.0f, whip->getMass(0)->pos.y * 10.0f);
   glBegin(GL_LINES);
      glVertex2fv((GLfloat*)&pos1);
      glVertex2fv((GLfloat*)&pos2);
   glEnd();
   // Arm
   glColor3ub(249, 210, 166);
   glLineWidth(6);
   glBegin(GL_LINES);
      glVertex2fv((GLfloat*)&pos1);
      glVertex2fv((GLfloat*)&pos2);
   glEnd();

   // Draw the whip
   glColor3ub(0, 0, 0);
   glLineWidth(4);
   glBegin(GL_LINES);
   for (int i = 0; i < whip->numOfMasses - 1; i++) {
      Mass* mass1 = whip->getMass(i);
      fxVector2D pos1(mass1->pos.x * 10.0f, mass1->pos.y * 10.0f);
      Mass* mass2 = whip->getMass(i + 1);
      fxVector2D pos2(mass2->pos.x * 10.0f, mass2->pos.y * 10.0f);

      glVertex2fv((GLfloat*)&pos1);
      glVertex2fv((GLfloat*)&pos2);
   }
   glEnd();

   // Move the whip slowly back to the center
   if (!mouseMove) {
      if (whip->ropeConnectionPos.x > 0.1) whip->ropeConnectionPos.x -= 0.1;
      else if (whip->ropeConnectionPos.x < -0.1) whip->ropeConnectionPos.x += 0.1;
      else whip->ropeConnectionPos.x = 0.0;
      if (whip->ropeConnectionPos.y > 0.1) whip->ropeConnectionPos.y -= 0.1;
      else if (whip->ropeConnectionPos.y < -0.1) whip->ropeConnectionPos.y += 0.1;
      else whip->ropeConnectionPos.y = 0.0;
   }

   // Only allow the whip to move when the mouse is moving
   whip->ropeConnectionVel.x = whip->ropeConnectionVel.y = 0.0;
   mouseMove = false;

	// Call Parent.
	Parent::renderObject( viewPort, viewIntersection );	// Always use for Debug Support!
}
