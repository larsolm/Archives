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
%x.setSize("54.000000 338.000000");
%x.setPosition("-114.400002 88.000000");
%x.setImageMap(Wall1);
%x.imap = "Wall1";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("351.200012 38.799999");
%x.setPosition("80.000000 236.000000");
%x.setImageMap(Wall2);
%x.imap = "Wall2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("58.000000 165.199997");
%x.setPosition("276.799988 173.600006");
%x.setImageMap(Wall3);
%x.imap = "Wall3";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("158.000000 189.199997");
%x.setPosition("327.200012 8.800000");
%x.setImageMap(Wall4);
%x.imap = "Wall4";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("314.000000 132.000000");
%x.setPosition("247.199997 -142.399994");
%x.setImageMap(Wall5);
%x.imap = "Wall5";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("146.000000 134.000000");
%x.setPosition("-68.000000 -140.399994");
%x.setImageMap(Wall6);
%x.imap = "Wall6";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("53.200001 114.800003");
%x.setPosition("-114.000000 -253.600006");
%x.setImageMap(Wall7);
%x.imap = "Wall7";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("730.000000 74.000000");
%x.setPosition("226.399994 -336.799988");
%x.setImageMap(Wall8);
%x.imap = "Wall8";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("502.000000 69.199997");
%x.setPosition("649.599976 -174.000000");
%x.setImageMap(Wall9);
%x.imap = "Wall9";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("161.600006 72.400002");
%x.setPosition("508.799988 -406.000000");
%x.setImageMap(Wall10);
%x.imap = "Wall10";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("226.000000 248.000000");
%x.setPosition("325.600006 -494.399994");
%x.setImageMap(Wall11);
%x.imap = "Wall11";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("58.000000 218.000000");
%x.setPosition("409.600006 -718.400024");
%x.setImageMap(Wall12);
%x.imap = "Wall12";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("350.399994 48.400002");
%x.setPosition("604.400024 -804.799988");
%x.setImageMap(Wall13);
%x.imap = "Wall13";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("53.200001 394.799988");
%x.setPosition("798.000000 -631.200012");
%x.setImageMap(Wall14);
%x.imap = "Wall14";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("146.000000 140.000000");
%x.setPosition("752.000000 -372.399994");
%x.setImageMap(Wall15);
%x.imap = "Wall15";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("136.800003 77.199997");
%x.setPosition("883.599976 -340.799988");
%x.setImageMap(Wall16);
%x.imap = "Wall16";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("60.400002 286.000000");
%x.setPosition("921.599976 -65.599998");
%x.setImageMap(Wall17);
%x.imap = "Wall17";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("50.000000 142.399994");
%x.setPosition("1261.599976 -400.399994");
%x.setImageMap(Wall18);
%x.imap = "Wall18";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("40.000000 140.000000");
%x.setPosition("932.000000 -438.799988");
%x.setImageMap(Wall19);
%x.imap = "Wall19";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("342.000000 44.000000");
%x.setPosition("1113.599976 -486.799988");
%x.setImageMap(Wall20);
%x.imap = "Wall20";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("50.000000 256.399994");
%x.setPosition("1262.400024 -95.199997");
%x.setImageMap(Wall21);
%x.imap = "Wall21";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("342.799988 53.599998");
%x.setPosition("1114.000000 50.799999");
%x.setImageMap(Wall22);
%x.imap = "Wall22";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("103.599998 74.800003");
%x.setPosition("1100.400024 -316.399994");
%x.setImageMap(Table2);
%x.imap = "Table2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("31.799999 45.400002");
%x.setPosition("1064.000000 -380.000000");
%x.setImageMap(Chair2);
%x.imap = "Chair2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("30.799999 46.000000");
%x.setPosition("1135.199951 -378.000000");
%x.setImageMap(Chair2);
%x.imap = "Chair2";
%x.type = "boundary";
%x.setGroup(9);
%x.setLayer(9);
%x.setCollisionActive(false, true);
%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));
%x.setCollisionCallback(true);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("474.799988 169.199997");
%x.setPosition("134.800003 -8.400000");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "212.800003 -20.000000", 10, 100);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("382.799988 169.199997");
%x.setPosition("80.000000 148.399994");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("578.000000 406.000000");
%x.setPosition("520.000000 -395.600006");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("370.799988 226.000000");
%x.setPosition("612.400024 -690.400024");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("352.399994 528.400024");
%x.setPosition("1108.000000 -230.399994");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("469.200012 247.600006");
%x.setPosition("139.199997 -192.399994");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("192.399994 135.600006");
%x.setPosition("878.400024 -255.600006");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "276.000000 -259.200012", 10, 100);

CreateEnemy("Pete", "1007.200012 -121.599998", 10, 100);

CreateEnemy("Pete", "1189.199951 -121.199997", 10, 100);

CreateEnemy("Pete", "1005.599976 -25.600000", 10, 100);

CreateEnemy("Pete", "720.000000 -546.400024", 10, 100);

CreateEnemy("Pete", "518.400024 -540.799988", 10, 100);

CreateTrigger("LoadLevelStreets1();", "1270.400024 -276.399994", "29.600000 112.000000");
