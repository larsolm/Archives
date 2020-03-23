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
%x.setSize("1024.000000 335.200012");
%x.setPosition("375.200012 -326.000000");
%x.setImageMap(Wall28);
%x.imap = "Wall28";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("512.000000 686.400024");
%x.setPosition("-196.800003 -21.600000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("916.799988 300.000000");
%x.setPosition("448.799988 263.200012");
%x.setImageMap(Wall29);
%x.imap = "Wall29";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1024.000000 284.000000");
%x.setPosition("437.600006 -22.400000");
%x.setImageMap(Background6);
%x.imap = "Background6";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("41.400002 37.200001");
%x.setPosition("159.600006 -162.000000");
%x.setImageMap(Mailbox);
%x.imap = "Mailbox";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("45.400002 39.599998");
%x.setPosition("777.599976 -160.000000");
%x.setImageMap(Mailbox);
%x.imap = "Mailbox";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("31.000000 26.799999");
%x.setPosition("520.000000 101.599998");
%x.setImageMap(Trashcan);
%x.imap = "Trashcan";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("27.799999 29.200001");
%x.setPosition("72.800003 96.400002");
%x.setImageMap(Trashcan);
%x.imap = "Trashcan";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("512.000000 512.000000");
%x.setPosition("781.599976 -408.799988");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("512.000000 690.400024");
%x.setPosition("1136.000000 87.599998");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "370.399994 -139.199997", 10, 30);

CreateEnemy("Pete", "217.600006 -138.399994", 8, 20);

CreateEnemy("Pete", "511.200012 -140.000000", 10, 10);

CreateEnemy("Pete", "818.400024 -1.600000", 8, 40);

CreateEnemy("Pete", "820.799988 -72.800003", 6, 30);

CreateEnemy("Pete", "585.599976 56.799999", 7, 20);

CreateEnemy("Pete", "588.799988 -47.200001", 6, 40);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("83.599998 61.200001");
%x.setPosition("588.000000 -158.399994");
%x.setImageMap(Hotdog);
%x.imap = "Hotdog";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

CreateTrigger("LoadLevelLobby();", "292.399994 -166.800003", "84.800003 41.599998");
