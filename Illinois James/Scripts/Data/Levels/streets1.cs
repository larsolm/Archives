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
%x.setSize("1022.400024 288.399994");
%x.setPosition("100.000000 -199.199997");
%x.setImageMap(Wall23);
%x.imap = "Wall23";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1028.000000 293.600006");
%x.setPosition("144.399994 274.000000");
%x.setImageMap(Wall24);
%x.imap = "Wall24";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("310.000000 548.400024");
%x.setPosition("-430.000000 8.400000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("101.800003 52.000000");
%x.setPosition("-185.600006 60.799999");
%x.setImageMap(Car1);
%x.imap = "Car1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("113.199997 54.799999");
%x.setPosition("-155.600006 -0.799999");
%x.setImageMap(Car6);
%x.imap = "Car6";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("25.000000 31.400000");
%x.setPosition("-157.199997 108.800003");
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
%x.setSize("27.000000 27.600000");
%x.setPosition("-103.599998 -54.799999");
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
%x.setSize("26.199800 26.799999");
%x.setPosition("455.200012 -65.599998");
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
%x.setSize("21.400000 29.200001");
%x.setPosition("226.399994 -60.400002");
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
%x.setSize("112.400002 53.200001");
%x.setPosition("204.399994 62.799999");
%x.setImageMap(Car2);
%x.imap = "Car2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("380.399994 719.599976");
%x.setPosition("753.200012 12.400000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1024.000000 200.000000");
%x.setPosition("98.400002 30.400000");
%x.setImageMap(Background2);
%x.imap = "Background2";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "418.799988 9.600000", 7, 30);

CreateEnemy("Pete", "-2.400000 71.199997", 6, 80);

CreateEnemy("Pete", "500.799988 61.599998", 5, 80);

CreateEnemy("Pete", "115.199997 -9.600000", 10, 50);

CreateEnemy("Pete", "334.399994 69.599998", 8, 60);

CreateTrigger("LoadLevelStreets2();", "596.000000 30.000000", "55.200001 284.799988");
