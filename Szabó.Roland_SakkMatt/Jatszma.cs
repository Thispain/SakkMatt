using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Szabó.Roland_SakkMatt
{
    class Jatszma
    {
        public string[,,] table { get; } // Még gondolkodom
        // 0.: Négyzet színezés: "x": Fehér - "+": Fekete
        // 1.: Foglalás: #
        // 2.: Bábú:szín
        private bool[] ArrayCycles; // Azt gondoltam, ide fogom gyűjteni a FOR ciklusokban lévő bool változók publikus értékeit. Végül nagyon nem így lett. Törlendő!
        private int[] memory = new int[2]; // Ez a megfogott bábú lokációjának eltárolásáért felel.
        private int Next = 1; // Mindig a fehér kezd. 1=fehér 2=fekete (maybe -1=fekete)
        Stack<string> lepesek = new Stack<string>(); // 1. xx: "1(memory[0]:memory[1]=>x:y)" 2. xx: 1:memory[0]:memory[1]=>x:y

        bool Tervez = false; // Ez egy globális változó, ami a SzinCheck metódusnak határozza meg, melyik kimenzióra tegye rá a keresztet [Ilyet sose csináljatok! Ez ideiglenes megoldás.]. 
        /*
         * Szükséges információk:
         * Enpassan-t:
         *  Előző lépés az ellenfél részéről (Ha az elpőző lépést egy gyaloggal tette, ami kettőt ugrott az ellenfél gyalogja mellé, az EnPassant végrehajtható)
         *  
         *  Sánc:
         *   Király vagy bástya elmozdulása
         */



        // string EnpassantAndSanc;
        // string gyoztes;

        // 1A (Bal sarok, fekete mező)
        // Konstruktor
        public Jatszma()
        {
            table = new string[8, 8, 4];
            Restart(true);
            Kezdoallas();
        }

        private void Restart(bool kezdes)
        {
            for (int i = 0; i < table.GetLength(0); i++)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    switch (kezdes)
                    {
                        case true:
                            table[i, j, 2] = "0";
                            goto case false;
                        case false:
                            table[i, j, 0] = i % 2 == 1 && j % 2 == 0 || i % 2 == 0 && j % 2 == 1 ? "x" : "+"; // szín
                            table[i, j, 1] = "0"; // Foglalás jelzése
                            table[i, j, 3] = "0"; // 
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void Kezdoallas()
        {
            int a = 0;
            int bi = 1;

            for (int i = 0; i < table.GetLength(0); i++)
            { 
                // Gyalogok felrakása
                table[6, i, 2] = "1:2" /*+ a*/;
                table[1, i, 2] = "1:1" /*+ Math.Abs(a-1)*/; 
            }
            for (int i = 0, b = 2; i < table.GetLength(0); i++, b += bi)
            {
                table[7, i, 2] = $"{b}:2";
                table[0, i, 2] = $"{b}:1";
                if (i == 4)
                {
                    b -= 1;
                    bi = -1;
                }
            }
             // table[4, 5, 2] = "6:1";
        }
        public bool Babu(int x, int y)
        {
            //string szint = szin ? "1" : "2";
            bool t = table[x, y, 2] != "0" && SzinCode(table[x, y, 2]) == Next.ToString();

            if (t)
            {
                Restart(false);
                memory[0] = x;
                memory[1] = y;
                Szabalyok(table[memory[0], memory[1], 2].Split(':'));
                table[x, y, 1] = "Z";
            }
            return t;
        }
        public bool Cel(int x, int y)
        {
            bool t = table[x, y, 1] == "#";
            if (t)
            {
                table[x, y, 2] = table[memory[0], memory[1], 2]; //A bábú helye ??????
                table[memory[0], memory[1], 2] = "0";
                lepesek.Push($"{Next}-{memory[0]}:{memory[1]}-{x}:{y}-{table[x, y, 2]}");
                Restart(false);
                //*Next = Next == 1 ? 2 : 1;
                // table[x, y, 2] = "0";
                //include SakkMatt/Patt fuggveny
            }
            return t;
        }
        public void Szabalyok(string[] babu)
        {
            int index = int.Parse(babu[0]);
            switch (index)
            {
                case 1: // Paraszt
                    Gyalog();
                    // 2 Előre
                    
                    EnPassant();
                    break;
                case 2: // Bástya
                    Egyenes();
                    break;
                case 3: // Huszár
                    // L
                    Huszar();
                    break;
                case 4: // Futár
                    Atlo();
                    break;
                case 5: // Királyné
                    Atlo();
                    Egyenes(); //??
                    break;
                case 6: // Király
                    // 1
                    // Sánc [Hiányzik]
                    Kiraly();
                    break;
                default:
                    Console.WriteLine("Ez nem igy működik." +
                                      "\nSemmiféleképpen nem így működik!");
                    break;
            }
        }
        // Megjelenítés
        public string Kiirat()
        {
            string sor = "abcdefghijklmnopqrstuvwxyz";
            int a = Next == 1 ? 0 : table.GetLength(0) - 1; // Képlettfordítás
            string s = "";
            string babu = " GBHFYK";
            // s = "  " + shKar("-", table.GetLength(0)); 
            /*
                Bábú jelölés:
                Y= Királynő
                K=Király
            */
            for (int i = table.GetLength(0) - 1 - a; i >= 0 - a; i--)
            {
                s += $"\n{sor[Math.Abs(i)].ToString().ToUpper()}| ";
                for (int j = 0 - a; j < table.GetLength(1) - a; j++)
                {
                    int ij = Math.Abs(i);
                    int ji = Math.Abs(j);

                    if ((table[ij, ji, 1] == "#" || table[ij, ji, 1] == "B") && table[ij, ji, 2] == "0") s += $"{table[ij, ji, 1]} ";
                    else if (table[ij, ji, 2] != "0") s += $"{babu.Substring(int.Parse(table[ij, ji, 2].Split(':')[0]), 1)} ";
                    else s += $"{table[ij, ji, 0]} ";
                }
                s += "|";
            }
            s += Next == 1 ? $" {1}\n" : $" {2}\n";
            s += "  " + shKar($"-", table.GetLength(0));
            //if(Sakkmatt || Patt) Cw($"{Next == 1 ? "Fehér" : "Fekete"} nyert.
            //")
            return s;
        }

        public string shKar(string ch, int darab)
        {
            string s = ch;
            for (int i = 0; i < darab; i++)
            {
                s += $"{i}" + ch;
            }
            return s;
        }
        public string KiiratT(int dimi)
        {
            int a = table.GetLength(0); // Képlettfordítás
            string s = "";
            for (int i = table.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < table.GetLength(1); j++)
                {
                    s += $"{table[i, j, dimi]} ";
                }
                s += "\n";
            }
            return s;
        }
        public IEnumerable<int> EnTrue(int max)
        { //FS
            for (int i = 0; i < 100; ++i)
            {
                yield return i;
            }
            yield break;
        }

        //Befoglalások
        private void Egyenes()
        { // szín?
            int i = memory[0], j = memory[1];
            int la = 10000; // Az 1 utáni 4 nulla 1-1 információt jelöl

            //Egyenes
            for (int k = 0; k < table.GetLength(0); k++) // Csak felfele [k: Mező az adott oszlopban, i: A bábú sorhelye, j: A bábú oszlophelye]
            {
                if (j + k < table.GetLength(1) && la.ToString()[4] == '0')
                {
                    SzinCheck(i, j + k);
                    la += k > 0 && table[i, j + k, 2] != "0" ? 4 : 0;
                }
                if (j - k >= 0 && la.ToString()[3] == '0')
                {
                    SzinCheck(i, j - k);
                    la += k > 0 && table[i, j - k, 2] != "0" ? 40 : 0;
                }
                if (i + k < table.GetLength(0) && la.ToString()[2] == '0')
                {
                    SzinCheck(i + k, j); //Helyes
                    la += k > 0 && table[i + k, j, 2] != "0" ? 400 : 0;
                }
                if (i - k >= 0 && la.ToString()[1] == '0')
                {
                    SzinCheck(i - k, j); //Helyes
                    la += k > 0 && table[i - k, j, 2] != "0" ? 4000 : 0;
                }
                //  if()
                // Console.WriteLine(la);
            }
            // Console.WriteLine(KiiratT(3));
        } //Kész
        private void Atlo()
        { // Kész
            int i = memory[0], j = memory[1];
            int la = 10000;
            //Átló
            for (int k = 0; k < table.GetLength(0) || k < table.GetLength(1); k++)
            {
                if (i - k >= 0 && j + k < table.GetLength(1) && la.ToString()[4] == '0')
                {
                    SzinCheck(i - k, j + k); // Mellék átló [le]
                    la += k > 0 && table[i - k, j + k, 2] != "0" ? 4 : 0;
                }
                if (i + k < table.GetLength(0) && j - k >= 0 && la.ToString()[3] == '0')
                {
                    SzinCheck(i + k, j - k); // Mellék átló [fel]
                    la += k > 0 && table[i + k, j - k, 2] != "0" ? 40 : 0;
                }
                if (i + k < table.GetLength(0) && j + k < table.GetLength(1) && la.ToString()[2] == '0')
                {
                    SzinCheck(i + k, j + k); // Főátló [fel]
                    la += k > 0 && table[i + k, j + k, 2] != "0" ? 400 : 0;
                }
                if (i - k >= 0 && j - k >= 0 && la.ToString()[1] == '0')
                {
                    SzinCheck(i - k, j - k);  // Főátló [le]
                    la += k > 0 && table[i - k, j - k, 2] != "0" ? 4000 : 0;
                }
            }
        }  //Kész
        private void Huszar()
        {
            int i = memory[0], j = memory[1];
            //2 fel i jobbra
            SzinCheck(i + 1, j + 2);
            SzinCheck(i + -1, j + 2);
            SzinCheck(i + 1, j + -2);
            SzinCheck(i + -1, j + -2);

            SzinCheck(i + 2, j + 1);
            SzinCheck(i + -2, j + 1);
            SzinCheck(i + 2, j + -1);
            SzinCheck(i + -2, j + -1);
        }
        private void Gyalog()
        {
            int i = memory[0], j = memory[1];
            // table[i + 1, j, 2] = i < table.GetLength(0) && table[i + 1, j, 2] == "0" && table[i, j, 2].Split(':')[1] == "1" ? "#" : "0";
            // table[, 2] = i < table.GetLength(0) && table[i + 1, j, 2] == "0" && table[i, j, 2].Split(':')[1] == "1" ? "#" : "0";
            int dimi = Tervez ? 3 : 1;
            int a = SzinCode(table[i, j, 2]) == "1" ? 1 : -1;
            for (int k = -1; k <= 1; k++)
            {
                bool t = k == 0 ? EmptyCheck(i + a, j + k) : SzinCheck(i + a, j + k); //Színtévesztés
            }
            if (i == 1) EmptyCheck(i + 2, j);
            if (i == 6) EmptyCheck(i + -2, j);
            // EnPassant();
            // Kezdes();
        }
        private void EnPassant()
        {
            int a = SzinCode(table[memory[0], memory[1], 2]) == "1" ? 1 : -1;
            if (memory[0] == 5)
            {
                string[] id = lepesek.Peek().Split('-');
                if (memory[0] == 1 && LepesFuggveny(int.Parse(id[0]), id[1].Split(':'), id[2].Split(':'), int.Parse(id[3])))
                {
                    int i = memory[0], j = memory[1];
                    SzinCheck(i + a, j + 1);
                    SzinCheck(i + a, j + -1);
                }
            }
        }
        private bool LepesFuggveny(int ellenfel, string[] rajt, string[] cel, int babu)
        {
            return ellenfel != Next && babu == 1 && Math.Abs(int.Parse(rajt[0]) - int.Parse(cel[0])) == 2 ? true : false;
        }

        private void Kiraly()
        { //Kivételkezelés
            this.All_EllenfelStock();
            int i = memory[0], j = memory[1];
            for (int k = -1; k < 2; k++) {
                for (int l = -1; l < 2; l++) {
                    if (VizsgaCheck(i + k, j + l, 3) != "x") SzinCheck(i+k, j+l);
                }
            }
            //Sanc
            // Királyok közötti távolságtartás
         /*   int hatokor = 2;
            for (int l = hatokor*-1; l <= hatokor; l++) {
                BabuCheck(i + hatokor, j + l, 6);
                BabuCheck(i + l, j + hatokor, 6);
                BabuCheck(i + -hatokor, j + l, 6);
                BabuCheck(i + l, j + -hatokor, 6);  
                // #KiralyokatNemLehetLeutni(Csak Sakk)
            }
            // Patt Vizsgalat
            hatokor = 1;
            for (int l = hatokor * -1; l <= hatokor; l++)
            {
                BabuCheck(i + hatokor, j + l, 6);
                BabuCheck(i + l, j + hatokor, 6);
                BabuCheck(i + -hatokor, j + l, 6);
                BabuCheck(i + l, j + -hatokor, 6);
                // #KiralyokatNemLehetLeutni(Csak Sakk)
            }*/


        }
        private void Sanc()
        {
            bool t = true;
            string[] id;
            foreach (var item in lepesek)
            {
                id = item.Split('-');
                if (SancFuggveny(int.Parse(id[0]), id[1].Split(':'), id[2].Split(':'), int.Parse(id[3])))
                {
                    t = false;
                    break;
                }
            }
            if (t)
            {
                
            }
        }
        private bool SancFuggveny(int ellenfel, string[] rajt, string[] cel, int babu)
        {
            return ellenfel != Next && babu == 1 && Math.Abs(int.Parse(rajt[0]) - int.Parse(cel[0])) == 2 ? true : false;
        }

        // Könnyítő függvények

        // Ezzel a kiváló függvénnyel adom vissza a táblán lévő bábúk szín értékét       
        string SzinCode(string tableD)
        {
            string[] s = tableD.Split(':');
            return s.Length == 2 ? s[1] : "0";
        }
        /// <summary>
        /// Pelda1.
        /// </summary>
        /// <param name="celX">Ehe</param>
        /// <param name="celY">Ahae</param>
        /// <returns>...</returns>
        bool SzinCheck(int celX, int celY)
        {
            int dimi = Tervez ? 3: 1;
            bool t = false;
            if (celX >= 0 && celY >= 0 && celX < table.GetLength(0) && celY < table.GetLength(1))
            {
                table[celX, celY, dimi] = (SzinCode(table[celX, celY, 2]) != SzinCode(table[memory[0], memory[1], 2])) ? "#" : "0";
                t = true;
            }
            return t;
        }
        bool EmptyCheck(int celX, int celY)
        {
            int dimi = Tervez ? 3 : 1;
            bool t = false;
            if (celX >= 0 && celY >= 0 && celX < table.GetLength(0) && celY < table.GetLength(1))
            {
                table[celX, celY, dimi] = VizsgaCheck(celX, celY, 2)=="0" ? "#" : "0";
                t = true;
            }
            return t;
        }

        bool BabuCheck(int celX, int celY, int Babu)
        {
            bool t = false;
            if (celX >= 0 && celY >= 0 && celX < table.GetLength(0) && celY < table.GetLength(1)) {
                t = SzinCode(table[celX, celY, 2]) != SzinCode(table[memory[0], memory[1], 2]) && int.Parse(table[celX,celY,2]) == Babu ? true : false;
            }
            return t;
        }

        string VizsgaCheck(int celX, int celY, int dimension)
        {
            return celX >= 0 && celY >= 0 && celX < table.GetLength(0) && celY < table.GetLength(1) ? table[celX, celY, dimension] : null;
        }

        public bool JatekVege {
            get => false;
        }

        public string Nyertes
        {
            // if(JatekVege)
            get => "Senki";
        }

        void All_EllenfelStock() {
            Tervez = true; 
            for (int i = 0; i < table.GetLength(0); i++) {
                for (int j = 0; j < table.GetLength(1); j++) {
                    memory[0] = i; memory[1] = j;
                    if (VizsgaCheck(i, j, 2) !="0" && VizsgaCheck(i, j, 2).Split(':')[1] != Next.ToString()) Szabalyok(table[i,j,2].Split(':'));
                }
            }
            Console.WriteLine(KiiratT(3));
            Tervez = false;
        }
    }
}
