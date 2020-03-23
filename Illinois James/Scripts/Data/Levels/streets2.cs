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
%x.setSize("5.799990 46.599998");
%x.setPosition("313.200012 164.399994");
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
%x.setSize("1048.000000 16.000000");
%x.setPosition("532.799988 188.000000");
%x.setImageMap(Background2);
%x.imap = "Background2";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("143.399994 49.599998");
%x.setPosition("277.600006 78.800003");
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
%x.setSize("24.400000 26.799999");
%x.setPosition("295.200012 87.599998");
%x.setImageMap(Bush1);
%x.imap = "Bush1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1024.000000 285.200012");
%x.setPosition("360.799988 227.600006");
%x.setImageMap(Wall25);
%x.imap = "Wall25";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1024.000000 302.000000");
%x.setPosition("432.000000 -252.000000");
%x.setImageMap(Wall26);
%x.imap = "Wall26";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("1024.000000 194.000000");
%x.setPosition("418.399994 -9.600000");
%x.setImageMap(Background3);
%x.imap = "Background3";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("342.799988 658.000000");
%x.setPosition("1027.199951 -12.000000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("22.000000 41.799999");
%x.setPosition("533.599976 78.000000");
%x.setImageMap(Skateboard);
%x.imap = "Skateboard";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("29.400000 30.799999");
%x.setPosition("392.799988 -118.800003");
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
%x.setSize("44.000000 63.000000");
%x.setPosition("551.200012 -99.199997");
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
%x.setSize("90.800003 41.200001");
%x.setPosition("104.000000 21.600000");
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
%x.setSize("98.000000 45.000000");
%x.setPosition("403.200012 -40.799999");
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
%x.setSize("121.000000 49.000000");
%x.setPosition("777.599976 21.600000");
%x.setImageMap(Car4);
%x.imap = "Car4";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("28.400000 29.200001");
%x.setPosition("496.399994 -116.000000");
%x.setImageMap(Bush1);
%x.imap = "Bush1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("26.799999 29.200001");
%x.setPosition("618.799988 87.199997");
%x.setImageMap(Bush1);
%x.imap = "Bush1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("27.600000 22.799999");
%x.setPosition("175.199997 -110.400002");
%x.setImageMap(Bush1);
%x.imap = "Bush1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("43.599998 34.799999");
%x.setPosition("663.200012 -45.200001");
%x.setImageMap(Manhole);
%x.imap = "Manhole";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "725.599976 -41.599998", 5, 100);

CreateEnemy("Pete", "560.000000 -22.400000", 5, 40);

CreateEnemy("Pete", "798.000000 -41.599998", 10, 30);

CreateEnemy("Pete", "396.000000 40.000000", 7, 40);

CreateEnemy("Pete", "109.599998 -50.400002", 8, 50);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("455.200012 329.600006");
%x.setPosition("-290.399994 -70.000000");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("500.000000 500.000000");
%x.setPosition("-296.000000 -350.399994");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("500.000000 500.000000");
%x.setPosition("-318.399994 265.600006");
%x.setImageMap(Empty);
%x.imap = "Empty";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateTrigger("LoadLevelStreets3();", "877.200012 -14.400000", "44.799999 253.600006");
