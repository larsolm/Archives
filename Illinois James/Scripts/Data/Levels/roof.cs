new SimSet(Mission) { xVel = 0; yVel = 0; };
new SimSet(Enemies) { };

datablock fxImageMapDatablock2D(Trigger) {
   mode = full;
   textureName = "Data/Images/trigger.png";
};

datablock fxImageMapDatablock2D(Enemy) {
   mode = full;
   textureName = "Data/Images/Enemy.png";
};

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("609.200012 194.600006");
%x.setPosition("-75.599998 168.000000");
%x.setImageMap(Wall36);
%x.imap = "Wall36";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("155.000000 140.600006");
%x.setPosition("151.199997 4.400000");
%x.setImageMap(Wall37);
%x.imap = "Wall37";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("459.600006 13.200000");
%x.setPosition("-151.199997 -48.400002");
%x.setImageMap(Wall38);
%x.imap = "Wall38";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("12.400000 120.000000");
%x.setPosition("-374.799988 14.400000");
%x.setImageMap(Wall39);
%x.imap = "Wall39";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("455.600006 119.599998");
%x.setPosition("-143.600006 13.200000");
%x.setImageMap(Background8);
%x.imap = "Background8";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("876.400024 334.799988");
%x.setPosition("-193.199997 -224.800003");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("288.399994 150.800003");
%x.setPosition("-513.599976 16.000000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("782.000000 50.000000");
%x.setPosition("-263.600006 -70.400002");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1240.400024 363.600006");
%x.setPosition("-148.800003 256.000000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("293.200012 589.200012");
%x.setPosition("342.399994 -58.400002");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Jergens", "-344.000000 16.000000", 5, 200);

CreateEnemy("Pig", "-277.600006 -16.000000", 10, 100);

CreateEnemy("Pig", "-277.600006 36.000000", 10, 100);
