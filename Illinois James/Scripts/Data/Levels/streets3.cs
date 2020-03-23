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
%x.setSize("1024.000000 284.000000");
%x.setPosition("428.000000 -235.199997");
%x.setImageMap(Wall27);
%x.imap = "Wall27";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1033.599976 200.000000");
%x.setPosition("401.600006 195.199997");
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
%x.setSize("1024.800049 200.000000");
%x.setPosition("440.399994 0.000000");
%x.setImageMap(Background4);
%x.imap = "Background4";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("98.000000 42.000000");
%x.setPosition("668.000000 -25.600000");
%x.setImageMap(Car7);
%x.imap = "Car7";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("98.000000 45.000000");
%x.setPosition("617.599976 25.600000");
%x.setImageMap(Car3);
%x.imap = "Car3";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("98.000000 42.000000");
%x.setPosition("165.600006 31.200001");
%x.setImageMap(Car5);
%x.imap = "Car5";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("25.400000 26.799999");
%x.setPosition("538.799988 -95.199997");
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
%x.setSize("98.000000 48.200001");
%x.setPosition("375.200012 -31.200001");
%x.setImageMap(Car3);
%x.imap = "Car3";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("395.200012 1032.800049");
%x.setPosition("-242.000000 54.400002");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("374.399994 814.400024");
%x.setPosition("1080.400024 -33.200001");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("44.000000 63.000000");
%x.setPosition("105.599998 -93.599998");
%x.setImageMap(Sign2);
%x.imap = "Sign2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("29.200001 25.200001");
%x.setPosition("88.000000 -104.400002");
%x.setImageMap(Bush2);
%x.imap = "Bush2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("36.400002 26.799999");
%x.setPosition("-52.400002 -104.400002");
%x.setImageMap(Bush2);
%x.imap = "Bush2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

CreateEnemy("Pete", "784.000000 36.400002", 8, 50);

CreateEnemy("Pete", "782.400024 -57.599998", 10, 20);

CreateEnemy("Pete", "849.599976 -8.800000", 10, 10);

CreateEnemy("Pete", "510.399994 -28.400000", 18, 45);

CreateEnemy("Pete", "159.199997 -32.000000", 8, 15);

CreateEnemy("Pete", "371.200012 38.400002", 5, 35);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("38.599998 30.000000");
%x.setPosition("444.399994 31.200001");
%x.setImageMap(Manhole);
%x.imap = "Manhole";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("50.000000 50.000000");
%x.setPosition("704.000000 60.000000");
%x.setImageMap(Bush1);
%x.imap = "Bush1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

CreateTrigger("LoadLevelFreeway();", "901.200012 6.000000", "38.399899 243.199997");
