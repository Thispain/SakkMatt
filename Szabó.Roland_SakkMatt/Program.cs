using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Szabó.Roland_SakkMatt
{
    class Program
    {
        /* kliens vs. szerver
         * Kliens küld: Lépés
         * Kliens fogad: érvénytelen, tábla jelenlegi állása, idő, pontszám, leütött bábúk
         * [Vakjátszma esetén csak a lépés érvényessége, és ki következik, idő, pontszám(?)]
         */
        /* Lépések:
         * k: Bábú
         * k: Bábú helye
         * sz: Helyes
         * sz: Vizsgálat
         * sz: Eredmény küldése
         * k: várni
         * */


        /*
         Dynamic:
            Játékos lép
            Ellenfél lép
         */
        /*
         Vége:
        Sakkmat/Patt
        Játékmenet mentése
        /*
         Nézetek:
            Játékos
            Ellenfél
         */
        /*
         Cél: Multiplayer
         Megvalósítás:
            Két kliens
         */


        static void Main(string[] args)
        {
            /* string[] ai = new string[2];
                 ai[1] = "abcde";
             Console.WriteLine($"{ai[1][2]}");  // Karakter sorszám lekérése tömbökben. Hihetetlen, de működik
             Console.ReadLine();*/

            

          /*  for (int i = 0; i < sor.Length; i++)
            {
                int a = sor[i];
                Console.WriteLine(a);
            }*/
            


            Console.WriteLine("Üdvözöllek a világomban!\n\nNyomj egy gombot a folytatáshoz!");
           // Console.WriteLine("> Singleplayer");
           // Console.WriteLine("> Multiplayer");
            Console.ReadKey();
            Jatszma j = new Jatszma();
         //   j.All_EllenfelStock();
            Console.WriteLine(j.Kiirat());
          //  Console.WriteLine(j.Kiirat(false));

            while (!j.JatekVege)
            {
                // Te jössz
                Program.Lepes(j, "Mivel lépsz(x:y)?: ");
                Program.Lepes(j, "Hova lépsz(x:y)/m[Mégse]?: ");
                // Fekete jön
             //   Console.ReadKey();
            }
            Console.ReadKey();
        }

        static void Lepes(Jatszma j, string sz)
        {
            string s;
            bool t = false;
            bool p = false;            
            //Kijelölés: B
            do
            {
                Console.Write(sz);
                s = Console.ReadLine();
                if (s.Split(':').Length == 2)
                {
                    int[] memory;
                    memory = Array.ConvertAll(s.Split(':'), si => int.TryParse(si, out var x) ? x - 1 : -1);
                    switch (sz[0])
                    {
                        case 'M':
                            t = j.Babu(memory[0], memory[1]); //BábúBekérés();
                            break;
                        case 'H':
                            t = j.Cel(memory[0], memory[1]);  //CelBekeres();
                            break;
                    }
                    Console.WriteLine(j.Kiirat());   //Vegrehajtas();
                }
                else if (s.Length > 0 && s[0] == 'm' && sz[0] == 'H') break;
            } while (!t);  
        }
    }
}
