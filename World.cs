using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Linq.Expressions;

namespace ZPG_Server
{
    public class World
    {
        //Проврка существования файлов

        public static int chunkCounter = 0;
        public static void TotalLoader()
        {
            TestFilesUpgraded();
            LoadPreGen();
        }
        //----------------------------------------------------------
        private static void TestFilesUpgraded()
        {
            if (System.IO.File.Exists("config.json"))
            {
                Console.WriteLine("Config file loaded");
                var fileReader = new System.IO.StreamReader("config.json");
                var allLines = fileReader.ReadToEnd().Split('\n');
                foreach (var line in allLines)
                {
                    if (line != "_Files_" && !line.Contains(".json"))
                    {
                        System.Threading.Thread.Sleep(500);
                        Console.WriteLine(Directory.Exists(line) ? "Directory checked" : "WTF?");
                    }
                    else Console.WriteLine($"Loaded {line}");
                }
                Console.WriteLine("File loading completed");
                fileReader.Close();
            }
            else Console.WriteLine("Config file cannot be found!!! ERROR");
        }
        //----------------------------------------------------------



        //Общяя структура чанка

        public class Chunk
        {
            private int ID;
            private int PosX;
            private int PosY;
            public const int StaticNum = 64;
            private Cord[,] CordsXY = new Cord[StaticNum, StaticNum];
            private struct Cord
            {
                // public int Temp;
                // public int Wet;
                public double Z;
            }
            private BiomeType Biome;
            private string BiomeName;



            //Функция генерирующяя чанк

            public static Boolean GenerateChunk(int PosX, int PosY)
            {
                string ChMain = "", Ch1 = "", Ch2 = "", Ch3 = "", Ch4 = "", Ch5 = "", Ch6 = "", Ch7 = "", Ch8 = "";
                double RT = 0, LT = 0, RB = 0, LB = 0;
                int R1 = 0, R2 = 0, R3 = 0, R4 = 0;
                Chunk NewChunk = new Chunk();
                Chunk Addional = new Chunk();
                //+00
                //0X0
                //000
                if (CheckChunkExist(PosX - 1, PosY - 1))
                {
                    Addional = loadChunk(PosX - 1, PosY - 1);
                    Ch1 = Addional.BiomeName;
                    RT += Addional.CordsXY[StaticNum-1, StaticNum-1].Z;
                    R1++;
                }
                //0+0
                //0X0
                //000
                if (CheckChunkExist(PosX, PosY + 1))
                {
                    Addional = loadChunk(PosX, PosY + 1);
                    Ch8 = Addional.BiomeName;
                    for (int i = 1; i < StaticNum - 1; i++)
                        NewChunk.CordsXY[0, i].Z = Addional.CordsXY[StaticNum-1,i].Z;
                    RT += Addional.CordsXY[0, StaticNum - 1].Z;
                    R1++;
                    LT += Addional.CordsXY[StaticNum - 1, StaticNum - 1].Z;
                    R2++;
                }
                //00+
                //0X0
                //000
                if (CheckChunkExist(PosX + 1, PosY + 1))
                {
                    Addional = loadChunk(PosX + 1, PosY + 1);
                    Ch7 = Addional.BiomeName;
                    LT += Addional.CordsXY[0, StaticNum - 1].Z;
                    R2++;
                }
                //000
                //+X0
                //000
                if (CheckChunkExist(PosX - 1, PosY))
                {
                    Addional = loadChunk(PosX - 1, PosY);
                    Ch2 = Addional.BiomeName;
                    for (int i = 1; i < StaticNum - 1; i++)
                        NewChunk.CordsXY[i, 0].Z = Addional.CordsXY[i, StaticNum-1].Z;
                    RT += Addional.CordsXY[StaticNum - 1, 0].Z;
                    R1++;
                    RB += Addional.CordsXY[StaticNum - 1, StaticNum - 1].Z;
                    R3++;
                }
                //000
                //0X+
                //000
                if (CheckChunkExist(PosX + 1, PosY))
                {
                    Addional = loadChunk(PosX + 1, PosY);
                    Ch6 = Addional.BiomeName;
                    for (int i = 1; i < StaticNum - 1; i++)
                        NewChunk.CordsXY[i, StaticNum-1].Z = Addional.CordsXY[i, 0].Z;
                    LT += Addional.CordsXY[0, 0].Z;
                    R2++;
                    LB += Addional.CordsXY[0, StaticNum - 1].Z;
                    R4++;
                }
                //000
                //0X0
                //+00
                if (CheckChunkExist(PosX - 1, PosY + 1))
                {
                    Addional = loadChunk(PosX - 1, PosY + 1);
                    Ch3 = Addional.BiomeName;
                    RB += Addional.CordsXY[StaticNum - 1, 0].Z;
                    R3++;
                }
                //000
                //0X0
                //0+0
                if (CheckChunkExist(PosX, PosY - 1))
                {
                    Addional = loadChunk(PosX, PosY - 1);
                    Ch4 = Addional.BiomeName;
                    for (int i = 1; i < StaticNum - 1; i++)
                        NewChunk.CordsXY[StaticNum-1, i].Z = Addional.CordsXY[0, i].Z;
                    RB += Addional.CordsXY[0,0].Z;
                    R3++;
                    LB += Addional.CordsXY[StaticNum - 1, 0].Z;
                    R4++;
                }
                //000
                //0X0
                //00+
                if (CheckChunkExist(PosX + 1, PosY - 1))
                {
                    Addional = loadChunk(PosX + 1, PosY - 1);
                    Ch5 = Addional.BiomeName;
                    LB += Addional.CordsXY[0, 0].Z;
                    R4++;
                }

                Console.WriteLine(RT + " " + LT);
                Console.WriteLine(RB + " " + LB);

                if (LB != 0 && R4 != 0) NewChunk.CordsXY[StaticNum - 1, 0].Z = LB / R4;
                if (LT != 0 && R3 != 0) NewChunk.CordsXY[0, 0].Z = LT / R2; 
                if (RT != 0 && R1 != 0) NewChunk.CordsXY[0, StaticNum - 1].Z = RT / R1;
                if (RB != 0 && R3 != 0) NewChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z = RB / R3; 

                Random Rand = new Random();
                while (ChMain == "") {
                int randy = Rand.Next(0, 999);
                if (randy < 125 && Ch1 != "") ChMain = Ch1;
                if (randy < 250 && Ch2 != "") ChMain = Ch2;
                if (randy < 375 && Ch3 != "") ChMain = Ch3;
                if (randy < 500 && Ch4 != "") ChMain = Ch4;
                if (randy < 625 && Ch5 != "") ChMain = Ch5;
                if (randy < 750 && Ch6 != "") ChMain = Ch6;
                if (randy < 875 && Ch7 != "") ChMain = Ch7;
                if (randy < 1000 && Ch8 != "") ChMain = Ch8;
                 }
                NewChunk.BiomeName = ChMain;
                NewChunk.Biome = BiomeWork(NewChunk.BiomeName);
                NewChunk.PosX = PosX;
                NewChunk.PosY = PosY;
                chunkCounter = 0;
                CountChunks();
                NewChunk.ID = chunkCounter;
                NewChunk = ChunkGen(NewChunk);
                Chunks.Add(NewChunk);
                Console.WriteLine("Chunk generated " + NewChunk.ID);
                SaveChunk(NewChunk);
                return true;
            }
            private static void CountChunks()
            {
                foreach (var File in Directory.EnumerateFiles("Chunks")) if (File.Contains(".json")) chunkCounter++;
            }
            public static void SpawnCreate()
            {
                if(!CheckChunkExist(0,0))
                    {
                    Chunk Spawn = new Chunk();
                    Spawn.ID = 0;
                    Spawn.CordsXY[0, 0].Z = 50;
                    Spawn.CordsXY[0, StaticNum - 1].Z = 40;
                    Spawn.CordsXY[StaticNum - 1, 0].Z = 30;
                    Spawn.CordsXY[StaticNum - 1, StaticNum - 1].Z = 40;
                    Spawn.Biome = BiomeType.Grassland;
                    Spawn.BiomeName = "Jarkovia";
                    //---------------------------------------------------------
                    Spawn.PosX = 0;
                    Spawn.PosY = 0;
                    chunkCounter = 0;
                    CountChunks();
                    Spawn.ID = chunkCounter;
                    Spawn = ChunkGen(Spawn);
                    Chunks.Add(Spawn);
                    Console.WriteLine("Chunk generated " + Spawn.ID);
                    SaveChunk(Spawn);
                    //---------------------------------------------------------
                    Chunks.Add(Spawn);
                    SaveChunk(Chunks[0]);
                }
            }
            private static BiomeType BiomeWork(string myBiome)
            { 
                BiomeType mainBiome = BiomeType.Edge;
                foreach (var ourBiome in BiomesList)
                {
                    mainBiome = ourBiome.Name == myBiome ? ourBiome.Type : BiomeType.Edge ;
                }
                return mainBiome;
            }
            private static Chunk ChunkGen(Chunk myChunk)
            {
               Random X = new Random();

               if (myChunk.CordsXY[ 0 , 0 ].Z == 0)
                   myChunk.CordsXY[ 0 , 0 ].Z = X.Next(0, 10000) /100;

               if (myChunk.CordsXY[StaticNum - 1 , 0 ].Z == 0)
                   myChunk.CordsXY[StaticNum - 1 , 0 ].Z = X.Next(0, 10000) / 100;

               if (myChunk.CordsXY[ 0 , StaticNum - 1].Z == 0)
                   myChunk.CordsXY[ 0 , StaticNum - 1].Z = X.Next(0, 10000) / 100;

               if (myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z == 0)
                   myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z = X.Next(0, 10000) / 100;

                //На вход в Генератор должен поступать чанк с крайними точками [0,0];[0,StaticNum - 1];[StaticNum - 1,0];[StaticNum - 1,StaticNum - 1];
                myChunk = ChunkEdges(myChunk);
                myChunk = ChunkXGen(myChunk);
                //Первая строка
                //myChunk = ChunkAddFirstLine(myChunk);
                //Последняя строка
                //myChunk = ChunkAddLastLine(myChunk);
                //Столбики между первой и последней строкой
                //myChunk = ChunkAddOtherLines(myChunk);
                //Шум
                //myChunk = ChunkAddNoise(myChunk, 0, 100);
                //Алгаритм DiamondSquare
                //myChunk = ChunkDS.Generate(myChunk);

                return myChunk;
            }
            /*
            private static Chunk ChunkAddFirstLine(Chunk myChunk)
            {
                for (int i = 1; i < StaticNum - 1; i++)
                    myChunk.CordsXY[0, i].Z = myChunk.CordsXY[0, 0].Z + (myChunk.CordsXY[0, StaticNum - 1].Z - myChunk.CordsXY[0, 0].Z) / (StaticNum - 1) * i;
                return myChunk;
            }

            private static Chunk ChunkAddLastLine(Chunk myChunk)
            {
                for (int i = 1; i < StaticNum - 1; i++)
                    myChunk.CordsXY[StaticNum - 1, i].Z = myChunk.CordsXY[StaticNum - 1, 0].Z + (myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z - myChunk.CordsXY[StaticNum - 1, 0].Z) / (StaticNum - 1) * i;
                return myChunk;
            }

            private static Chunk ChunkAddOtherLines(Chunk myChunk)
            {
                for (int j = 0; j < StaticNum; j++)
                    for (int i = 1; i < StaticNum - 1; i++)
                        myChunk.CordsXY[i, j].Z = myChunk.CordsXY[0, j].Z + (myChunk.CordsXY[StaticNum - 1, j].Z - myChunk.CordsXY[0, j].Z) / (StaticNum - 1) * i;
                return myChunk;
            }
            */
            private static Chunk ChunkAddNoise(Chunk myChunk, int NoiseBot, int NoiseTop)
            {
                Random MyRand = new Random();
                for (int i = 1; i <= StaticNum - 2; i++)
                    for (int j = 1; j <= StaticNum - 2; j++)
                        myChunk.CordsXY[i, j].Z += Convert.ToDouble(MyRand.Next(NoiseBot, NoiseTop)) / 100;
                return myChunk;
            }
            //Генератор через иксы
            private static Chunk ChunkXGen(Chunk myChunk)
            {
                for (int i = 1; i < StaticNum - 1; i++)
                {
                    myChunk.CordsXY[i, i].Z = myChunk.CordsXY[i-1, i-1].Z + (myChunk.CordsXY[0, 0].Z - myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z) / (StaticNum - i - 1);
                    myChunk.CordsXY[StaticNum - i - 1, i].Z = myChunk.CordsXY[StaticNum - i, i + 1].Z + (myChunk.CordsXY[StaticNum - 1, 0].Z - myChunk.CordsXY[StaticNum - 1, 0].Z) / (StaticNum - i - 1);
                }
                /*
                for (int i = 2; i < StaticNum / 2; i++)
                    for (int j = 1; j < i; j++)
                    {
                        myChunk.CordsXY[i, j].Z = (myChunk.CordsXY[i - 1, j].Z + myChunk.CordsXY[i,i].Z) / 2;
                        //myChunk.CordsXY[StaticNum - i, j].Z = (myChunk.CordsXY[i + 1,j].Z + myChunk.CordsXY[StaticNum - i,StaticNum - i].Z) / 2;
                    }*/
                return myChunk;
            }

            private static Chunk ChunkEdges(Chunk myChunk)
            {
                for (int i=0; i < StaticNum - 1; i++)
                {
                    myChunk.CordsXY[0, i].Z = myChunk.CordsXY[0, 0].Z + (myChunk.CordsXY[0, StaticNum - 1].Z - myChunk.CordsXY[0, 0].Z) / (StaticNum - 1) * i;
                    myChunk.CordsXY[StaticNum - 1, i].Z = myChunk.CordsXY[StaticNum - 1, 0].Z + (myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z - myChunk.CordsXY[StaticNum - 1, 0].Z) / (StaticNum - 1) * i;
                    myChunk.CordsXY[i, StaticNum-1].Z = myChunk.CordsXY[0, StaticNum - 1].Z + (myChunk.CordsXY[StaticNum - 1, StaticNum - 1].Z - myChunk.CordsXY[0, StaticNum - 1].Z) / (StaticNum - 1) * i; ;
                    myChunk.CordsXY[i, 0].Z = myChunk.CordsXY[0, 0].Z + (myChunk.CordsXY[StaticNum - 1, 0].Z - myChunk.CordsXY[0, 0].Z) / (StaticNum - 1) * i;
                }
                return myChunk;
            }

            //Сохронялка чанков

            private static bool CheckChunkExist(int PosX, int PosY)
            {
                if (System.IO.File.Exists("Chunks/Chunk (" + PosX + "," + PosY + ").json"))
                    return true;
                else
                    return false;
            }

            private static void SaveChunk(Chunk myChunk)
            {
                System.IO.StreamWriter MyWriter = new System.IO.StreamWriter("Chunks/Chunk (" + myChunk.PosX + "," + myChunk.PosY + ").json");
                MyWriter.WriteLine("Id: " + myChunk.ID);
                MyWriter.WriteLine("PosX: " + myChunk.PosX);
                MyWriter.WriteLine("PosY: " + myChunk.PosY);
                // MyWriter.WriteLine("ChunkN: " + 0);
                // MyWriter.WriteLine("ChunkE: " + 0);
                // MyWriter.WriteLine("ChunkS: " + 0);
                // MyWriter.WriteLine("ChunkW: " + 0);
                MyWriter.WriteLine("Biom: " + myChunk.Biome);
                MyWriter.WriteLine("BiomName: " + myChunk.BiomeName);
                MyWriter.WriteLine("Cords: ");
                for (int i = 0; i < World.Chunk.StaticNum; i++)
                {
                    for (int j = 0; j < World.Chunk.StaticNum; j++)
                    {
                        MyWriter.Write(Math.Round(myChunk.CordsXY[i, j].Z, 2) + " ");
                    }
                    MyWriter.WriteLine();
                }
                Console.WriteLine("Chunk (" + myChunk.PosX + "," + myChunk.PosY + ") saved.");
                MyWriter.Close();
                return;
            }

            //Сохраняет файл конфигураций

            private static void SaveFilesInfo()
            {
                System.IO.StreamWriter MyWriter = new System.IO.StreamWriter("config.json");
                MyWriter.WriteLine("Chunks");
                MyWriter.WriteLine("_Files_");
                MyWriter.WriteLine("ChunksInfo.json");
                DirectoryInfo Dir = new DirectoryInfo("Chunks");
                List<FileInfo> Files = new List<FileInfo>();
                foreach (var File in Dir.EnumerateFiles()) MyWriter.WriteLine(File.Name);
                MyWriter.Close();
            }

            //Загрузчик чанков

            private static Chunk loadChunk(int PosX, int PosY)
            {
                Chunk newChunk = new Chunk();

                if (CheckChunkExist(PosX, PosY))
                {
                    System.IO.StreamReader FileReader = new System.IO.StreamReader("Chunks/Chunk (" + PosX + "," + PosY + ").json");

                    string strLine = FileReader.ReadLine();
                    int d = 0;

                    if (strLine.Contains("Id: "))
                        for (int i = 0; i < strLine.Length; i++)
                            if (strLine[i] == ':') d = i;
                    for (int i = d + 2; i < strLine.Length; i++)
                        newChunk.ID = newChunk.ID * 10 + Convert.ToInt16(strLine[i]);
                   
                    strLine = FileReader.ReadLine();

                    if (strLine.Contains("PosX: "))
                        for (int i = 0; i < strLine.Length; i++)
                            if (strLine[i] == ':') d = i;
                    for (int i = d + 2; i < strLine.Length; i++)
                        newChunk.PosX = newChunk.PosX * 10 + Convert.ToInt16(strLine[i]);

                    strLine = FileReader.ReadLine();

                    if (strLine.Contains("PosY: "))
                        for (int i = 0; i < strLine.Length; i++)
                            if (strLine[i] == ':') d = i;
                    for (int i = d + 2; i < strLine.Length; i++)
                        newChunk.PosY = newChunk.PosY * 10 + Convert.ToInt16(strLine[i]);
                    strLine = FileReader.ReadLine();
                    strLine = FileReader.ReadLine();

                    if (strLine.Contains("BiomName: "))
                        for (int i = 0; i < strLine.Length; i++)
                            if (strLine[i] == ':') d = i;
                    for (int i = d + 2; i < strLine.Length; i++)
                        newChunk.BiomeName = newChunk.BiomeName + Convert.ToChar(strLine[i]);

                    strLine = FileReader.ReadLine();

                    if (strLine.Contains("Cords: "))
                        while (!FileReader.EndOfStream)
                        {
                            int a = 0, b = 0;
                            string S = "";
                            strLine = FileReader.ReadLine();
                            for (int i = 0; i < strLine.Length; i++)
                                if (strLine[i] != ' ')
                                    S += strLine[i];
                                else
                                {
                                    newChunk.CordsXY[a, b].Z = Convert.ToDouble(S);
                                    b++;
                                    S = "";
                                }
                            a++;
                        }
                    Console.WriteLine("Loaded! (" + PosY + "," + PosX + ")");
                    return newChunk;
                }
                else Console.WriteLine("WTF Where is this?");
                return newChunk;
            }

            //DiamondSquare Class

            private class ChunkDS
            {
                private static int ysize = World.Chunk.StaticNum, xsize = World.Chunk.StaticNum;
                private static float[,] heighmap = new float[xsize, ysize];
                private static int roughness = 2;
                private static void Square(int lx, int ly, int rx, int ry)
                {
                    Random Randy = new Random();
                    int l = (rx - lx) / 2;
                    float a = heighmap[lx, ly];              //  B--------C
                    float b = heighmap[lx, ry];              //  |        |
                    float c = heighmap[rx, ry];              //  |   ce   |
                    float d = heighmap[rx, ly];              //  |        |        
                    int cex = lx + l;                        //  A--------D
                    int cey = ly + l;
                    while (cex > 255) cex /= 2;
                    while (cey > 255) cey /= 2;
                    heighmap[cex, cey] = (a + b + c + d) / 4 + Randy.Next(-l * 2 * roughness / ysize, l * 2 * roughness / ysize);
                }
                private static bool lrflag = false;
                private static void Diamond(int tgx, int tgy, int l)
                {
                    Random Randy = new Random();
                    float a, b, c, d;
                    if (tgy - l >= 0)
                        a = heighmap[tgx, tgy - l];                        //      C--------
                    else                                                   //      |        |
                        a = heighmap[tgx, ysize - l];                      // B---t g----D  |
                                                                           //      |        |
                                                                           //      A--------
                    if (tgx - l >= 0)
                        b = heighmap[tgx - l, tgy];
                    else
                        if (lrflag)
                        b = heighmap[xsize - l, tgy];
                    else
                        b = heighmap[ysize - l, tgy];
                    if (tgy + l < ysize)
                        c = heighmap[tgx, tgy + l];
                    else
                        c = heighmap[tgx, l];
                    if (lrflag)
                        if (tgx + l < xsize)
                            d = heighmap[tgx + l, tgy];
                        else
                            d = heighmap[l, tgy];
                    else
                        if (tgx + l < ysize)
                        d = heighmap[tgx + l, tgy];
                    else
                        d = heighmap[l, tgy];
                    heighmap[tgx, tgy] = (a + b + c + d) / 4 + Randy.Next(-l * 2 * roughness / ysize, l * 2 * roughness / ysize);
                }

                public static World.Chunk Generate(World.Chunk MyChunk)
                {
                    for (int i = 0; i < xsize; i++)
                        for (int j = 0; j < ysize; j++)
                            heighmap[i, j] = (float)MyChunk.CordsXY[i, j].Z;
                    for (int l = (ysize - 1) / 2; l > 0; l /= 2)
                        for (int x = 0; x < xsize - l; x += l)
                        {
                            if (x >= ysize - l) lrflag = true;
                            else lrflag = false;
                            for (int y = 0; y < ysize - l; y += l) DiamondSquare(x, y, x + l, y + l);
                        }
                    for (int i = 0; i < xsize; i++)
                        for (int j = 0; j < ysize; j++)
                            MyChunk.CordsXY[i, j].Z = heighmap[i, j];
                    return MyChunk;
                }
                private static void DiamondSquare(int lx, int ly, int rx, int ry)
                {
                    int l = (rx - lx) / 2;
                    Square(lx, ly, rx, ry);
                    Diamond(lx, ly + l, l);
                    Diamond(rx, ry - l, l);
                    Diamond(rx - l, ry, l);
                    Diamond(lx + l, ly, l);
                }
            }
        }

        private class Biomes
        {
            public string Name;
            public int Chunks;
            public BiomeType Type;
        }

        private static void LoadPreGen()
        {
            Biomes myBiom = new Biomes();
            StreamReader MyReader = new StreamReader("WorldPreGen.json");
            string Type="",Lines = MyReader.ReadLine();
            int a = 0, z = 0, v =0;
            while (!MyReader.EndOfStream)
            {
                for (int i = 1; i < Lines.Length; i++) if (Lines[i] == ',' || Lines[i] == ';') if (z == 0) z = i; else if (a == 0) a = i; else v = i;
                for (int i = 0; i <= Lines.Length - 2; i++)
                    if (i < z) myBiom.Name += Lines[i]; 
                    else if (i > z && i < a) Type += Lines[i]; 
                    else if (i > a && i < v) myBiom.Chunks = myBiom.Chunks * 10 + Lines[i];

                       // Console.WriteLine(myBiom.Chunks);
                switch (Type)
                {
                    case "Desert": myBiom.Type = BiomeType.Desert; break;
                    case "Savanna": myBiom.Type = BiomeType.Savanna; break;
                    case "Tropic": myBiom.Type = BiomeType.Tropic; break;
                    case "Grassland": myBiom.Type = BiomeType.Grassland; break;
                    case "Wood": myBiom.Type = BiomeType.Wood; break;
                    case "Forest": myBiom.Type = BiomeType.Forest; break;
                    case "Tundra": myBiom.Type = BiomeType.Tundra; break;
                    case "Ice": myBiom.Type = BiomeType.Ice; break;
                    case "Edge": myBiom.Type = BiomeType.Edge; break;
                    default: break;
                }
                BiomesList.Add(myBiom);
                Lines = MyReader.ReadLine();
            }
            MyReader.Close(); 
        }
        private enum BiomeType
        {
            Desert,//Пустыня
            Savanna,//Савана
            Tropic,//Тропики
            Grassland,//Лужайка
            Wood,//Рощя
            Forest,//Лес
            Tundra, //Тундра
            Ice, //Лёд
            Edge
        }
        //Временное хранилище кусков данных
        private static List<Biomes> BiomesList = new List<Biomes>();
        private static List<Chunk> Chunks = new List<Chunk>();
    }
}
