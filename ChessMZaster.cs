class JatekfaSzintek
{
    Babu best;
    int pont, alfa, beta;
    public static int melyseg = 6;
    public static LinkedList<Babu>[] gyilokList = new LinkedList<Babu>[melyseg + 1];
    public Boolean tudLepni()
    {
        return (best != null);
    }
    public JatekfaSzintek(int feher, int sz, int a, int b)
    {
        best = null; alfa = a; beta = b; Babu tempBab = null;
        pont = (-feher) << 17;//* 100000;
        Boolean EloszorGyilokLepesek = (gyilokList[sz] != null);
        LinkedList<XY> gyklepve = new LinkedList<XY>();
        LinkedListNode<Babu> gyilokTeszt = null;
        if (EloszorGyilokLepesek) gyilokTeszt = gyilokList[sz].First; else if (sz > 1) gyilokList[sz] = new LinkedList<Babu>();
        int i = 0; int j = 0;
        while (i < 8)
        {
            if (EloszorGyilokLepesek)
                do
                {
                    if (gyilokTeszt == null) { i = 0; j = 0; EloszorGyilokLepesek = false; } //++gyklep>50
                    else
                    {
                        Babu bb = gyilokTeszt.Value;
                        if (tabla[bb.poz.x, bb.poz.y] == bb.babszam && tabla[bb.hovatud[bb.ponthol - 1].x, bb.hovatud[bb.ponthol - 1].y] * feher <= 0)
                        {
                            i = bb.poz.x; j = bb.poz.y;
                            LinkedListNode<XY> gykBeszur = gyklepve.First;
                            while (gykBeszur != null && gykBeszur.Value.x < i) gykBeszur = gykBeszur.Next;
                            while (gykBeszur != null && gykBeszur.Value.x <= i && gykBeszur.Value.y < j) gykBeszur = gykBeszur.Next;
                            if (gykBeszur == null) gyklepve.AddLast(new XY(i, j));
                            else
                            {
                                if (gykBeszur.Value.y == j && gykBeszur.Value.x == i) i = -1;   // már néztük ezt a pozíciót de a listából mégse szedjük ki, hátha máshova lépve... // gyilokTeszt = gyilokTeszt.Previous; i = -1; // gyilokList[sz].Remove(gyilokTeszt.Next);                                                    
                                else { gyklepve.AddBefore(gykBeszur, new XY(i, j)); }
                            }
                        }
                        else i = -1; //ha nem léphető meg a gyilkos lépés, -1el jelzem 
                        gyilokTeszt = gyilokTeszt.Next;
                    }
                } while (i < 0);

            if (gyklepve.Count > 0 && !EloszorGyilokLepesek && XY.ee(gyklepve.First.Value, new XY(i, j)))
                gyklepve.RemoveFirst();
            else
            {
                switch (feher * tabla[i, j])
                {
                    case 1: tempBab = new Gyalog(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    case 2: tempBab = new Bastya(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    case 3: tempBab = new Lo(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    case 4: tempBab = new Futo(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    case 5: tempBab = new Kiralyno(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    case 6: tempBab = new Kiraly(tabla[i, j], new XY(i, j), sz, alfa, beta); break;
                    default: goto nemlepheto; //hogy ne kelljen tempBab létrehozni majd null ellenőrizni = gyorsabb?
                }
                if (tempBab.hovatud.Count > 0)
                {
                    if (feher < 0) alfa = tempBab.alfa; else beta = tempBab.beta;
                    if ((feher == -1 && pont > tempBab.pont) || (feher == 1 && pont < tempBab.pont)) { pont = tempBab.pont; best = tempBab; }
                    if (tempBab.kilepni) i = 100; //alfabeta miatt kilepes
                    if (!EloszorGyilokLepesek && sz > 1) //&& kvn(gyilokminmaxpont[sz], tempBab.pont))
                    {
                        LinkedListNode<Babu> gyili = gyilokList[sz].First;
                        if (gyili == null) gyilokList[sz].AddFirst(tempBab);
                        else
                        {
                            int gyk = 0;
                            while (gyili != null && ++gyk < 41)
                                if ((feher == -1 && gyili.Value.pont > tempBab.pont) || (feher == 1 && gyili.Value.pont < tempBab.pont))
                                {
                                    gyilokList[sz].AddBefore(gyili, tempBab); gyk = 150;                                    
                                }
                                else gyili = gyili.Next;
                            if (gyk < 30) gyilokList[sz].AddLast(tempBab);
                        }                        
                    }
                }
            nemlepheto:;
            }
            if (!EloszorGyilokLepesek && ++j > 7) { ++i; j = 0; }
        }
        if (best == null && sz > 1)
        {
            XY k = kiralypoz[feher == 1 ? 1 : 0];
            if (tutimezo(k.x, k.y, feher)) //patt?
            {
                if (tablanbabukerteke * feher > 40) pont = tablanbabukerteke - feher * 40;
                else //ha jól állunk, ne legyen patt
                if (tablanbabukerteke * feher < -40) pont = tablanbabukerteke + feher * 40;
            }
        }
        //catch (ArgumentOutOfRangeException e) catch kapta el az alfabétát, de már nem :D                 
    }
    public JatekfaSzintek(int feher) : this(feher, 0, 100000, -100000) { }
}