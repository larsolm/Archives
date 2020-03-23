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
%x.setPosition("-146.399994 8.000000");
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
%x.setPosition("48.000000 156.000000");
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
%x.setPosition("244.800003 93.599998");
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
%x.setPosition("295.200012 -71.199997");
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
%x.setPosition("215.199997 -222.399994");
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
%x.setPosition("-100.000000 -220.399994");
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
%x.setPosition("-146.000000 -333.600006");
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
%x.setPosition("194.399994 -416.799988");
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
%x.setPosition("617.599976 -254.000000");
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
%x.setPosition("476.799988 -486.000000");
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
%x.setPosition("293.600006 -574.400024");
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
%x.setPosition("377.600006 -798.400024");
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
%x.setPosition("572.400024 -884.799988");
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
%x.setPosition("766.000000 -711.200012");
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
%x.setPosition("720.000000 -452.399994");
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
%x.setPosition("851.599976 -420.799988");
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
%x.setPosition("889.599976 -145.600006");
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
%x.setPosition("1229.599976 -480.399994");
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
%x.setPosition("900.000000 -518.799988");
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
%x.setPosition("1081.599976 -566.799988");
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
%x.setPosition("1230.400024 -175.199997");
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
%x.setPosition("1082.000000 -29.200001");
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
%x.setPosition("1062.800049 -450.000000");
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
%x.setPosition("1028.800049 -506.399994");
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
%x.setSize("30.000000 46.000000");
%x.setPosition("1090.800049 -506.799988");
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
%x.setPosition("102.800003 -88.400002");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "180.800003 -100.000000", 5, 50);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("382.799988 169.199997");
%x.setPosition("48.000000 68.400002");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("578.000000 406.000000");
%x.setPosition("488.000000 -475.600006");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("370.799988 226.000000");
%x.setPosition("580.400024 -770.400024");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("352.399994 528.400024");
%x.setPosition("1076.000000 -310.399994");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("469.200012 247.600006");
%x.setPosition("107.199997 -272.399994");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
%x.setSize("192.399994 135.600006");
%x.setPosition("846.400024 -335.600006");
%x.setImageMap(Background1);
%x.imap = "Background1";
%x.type = "background";
%x.setGroup(31);
%x.setLayer(31);
Mission.add(%x);

CreateEnemy("Pete", "244.000000 -339.200012", 10, 10);

CreateTrigger("LoadLevelStreets1();", "1238.400024 -356.399994", "29.600000 112.000000");

CreateTrigger("CreateHouseEnemies1();", "438.399994 -335.600006", "50.000000 103.599998");

CreateTrigger("CreateHouseEnemies2();", "702.400024 -334.799988", "50.000000 105.199997");
