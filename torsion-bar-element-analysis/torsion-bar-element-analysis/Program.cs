using System;
using System.IO;

namespace torsion_bar_element_analysis
{
    class Program
    {
        // FINITE ELEMENT METHOD - TORSION BAR ELEMENT ALAYSIS ALGORITHM

        static void Main(string[] args)
        {
            //TOPOLOJI MATRISININ BOYUTLARI
            var topology = new int[4, 2]
            {
                { 1,   2 },
                { 2,   3 },
                { 3,   4 },
                { 4,   5 }
            };

            decimal pi = 3.14159265m;
            decimal L = 1200;


            var KGSize = 5; // TOPOLOJI MATRISININ BOYUTU (12x12)
            var matrisSize = 2;

            // ELEMAN SAYISI
            int eleman = 4; // 2 serbestlik

            var matrises = new decimal[KGSize, matrisSize, matrisSize]; // STIFFNESS MATRIS BOYUTU

            var index = 0;
            for (int z = 0; z <= L; z += (int)L / eleman)
            {
                decimal D = 60;
                decimal d = 32;
                decimal dh1 = 45;
                decimal dh2 = 24;
                decimal dz1 = 0;
                decimal dz2 = 0;
                decimal G = 0;
                decimal Ip = 0;
                decimal Lz = 0;

                Lz = z;
                if (z == 0)
                {
                    dz1 = D;
                    dz2 = dh1;
                    G = 77;
                    Lz = 0;
                    Ip = (pi * ((dz1 * dz1 * dz1 * dz1) - (dz2 * dz2 * dz2 * dz2)) / 32);
                }
                else if (z < 300)
                {
                    dz1 = D - ((D - d) / L) * z;
                    dz2 = dh1 - ((dh1 - dh2) / L) * z;
                    Ip = (pi * ((dz1 * dz1 * dz1 * dz1) - (dz2 * dz2 * dz2 * dz2)) / 32);
                    G = 77;
                }
                else if (z >= 300 && z < 800)
                {
                    dz1 = D - ((D - d) / L) * z;
                    Ip = (pi * (dz1 * dz1 * dz1 * dz1) / 32);
                    G = 77;
                }
                else
                {
                    dz1 = D - ((D - d) / L) * z;
                    Ip = (pi * (dz1 * dz1 * dz1 * dz1) / 32);
                    G = 32;
                }

                var formuleArray = new decimal[2, 2]
                {
                    { 1, -1 },
                    { -1, 1 }
                };

                for (int k = 0; k < matrisSize; k++)
                    for (int l = 0; l < matrisSize; l++)
                        matrises[index, k, l] =
                            Lz == 0 ?
                            0 :
                            G * Ip / (Lz) * formuleArray[k, l];

                index += 1;

            }

            var KG = new decimal[KGSize, KGSize];
            for (int k = 0; k < eleman; k++)
            {
                for (int l = 0; l < matrisSize; l++)
                {
                    for (int m = 0; m < matrisSize; m++)
                    {
                        KG[(topology[k, l] - 1), (topology[k, m] - 1)] += matrises[k, l, m];
                    }
                }
            }

            for (int j = 0; j < KGSize; j++)
            {
                for (int k = 0; k < KGSize; k++)
                {
                    Console.Write(KG[j, k].ToString("0.000") + "\t");
                }
                Console.WriteLine();
            }

            Console.Write("\n\nSonuç matrisini kaydetmek istiyor musunuz (y/n): ");
            var save = Console.ReadLine();
            if (save.ToLower() == "y")
            {
                var fileName = @"D:\matris";
                using (StreamWriter file = new StreamWriter($"{fileName}.xls"))
                {
                    for (int i = 0; i < KGSize; i++)
                    {
                        for (int j = 0; j < KGSize; j++)
                        {
                            file.Write($"{KG[i, j].ToString("0.000")}\t");
                        }
                        file.Write(Environment.NewLine);
                    }
                }

                Console.WriteLine($"\n\nKG matrisi \"{fileName}.xls\" kaydedildi.\n\n");
            }

            Console.WriteLine("Kapatmak için bir tuşa bas");

            Console.ReadKey();
        }
    }
}
