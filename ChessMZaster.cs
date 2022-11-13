class JatekFaSzintek
{
    Babu best, tempBab;
    int pont, alfa, beta;
    public static int melyseg = 6;
    public static LinkedList<GykListaElem>[] gyilokList = new LinkedList<GykListaElem>[melyseg + 1];
    int i = 0; int j = 0;
    public Boolean tudLepni() { return (best != null); }

    public JatekFaSzintek(TablaCuccok T, int feher, int sz, int a, int b)
    {
        Boolean PozElemez()
        {
            switch (feher * T.tabla[i, j])
            {
                case 1: tempBab = new Gyalog(T, new XY(i, j), sz, alfa, beta); break;
                case 2: tempBab = new Bastya(T, new XY(i, j), sz, alfa, beta); break;
                case 3: tempBab = new Lo(T, new XY(i, j), sz, alfa, beta); break;
                case 4: tempBab = new Futo(T, new XY(i, j), sz, alfa, beta); break;
                case 5: tempBab = new Kiralyno(T, new XY(i, j), sz, alfa, beta); break;
                case 6: tempBab = new Kiraly(T, new XY(i, j), sz, alfa, beta); break;
                default: goto nemlepheto;
            }
            if (tempBab.hovaTud.Count > 0)
            {
                if (feher < 0) alfa = tempBab.alfa; else beta = tempBab.beta;
                if ((feher == -1 && pont > tempBab.pont) || (feher == 1 && pont < tempBab.pont)) { pont = tempBab.pont; best = tempBab; }
                if (tempBab.kilepni) i = 100; //alfabeta miatt kilepes                        
                return true;
            }
            nemlepheto: return false;
        }

        best = null; alfa = a; beta = b; int tablaVege = 8;
        pont = (-feher) << 17;//* 100000;                    
        LinkedList<XY> gykLepve = new LinkedList<XY>();

        if (sz > 1 && gyilokList[sz].Count > 0)
        {
            LinkedListNode<GykListaElem> gyilokTeszt = gyilokList[sz].First;
            while (gyilokTeszt != null && i < 100)
            {
                GykListaElem bb = gyilokTeszt.Value;
                if (T.tabla[bb.honnan.x, bb.honnan.y] == bb.bsz && T.tabla[bb.hova.x, bb.hova.y] * feher <= 0)
                {
                    i = bb.honnan.x; j = bb.honnan.y;
                    LinkedListNode<XY> gykBeszur = gykLepve.First;
                    while (gykBeszur != null && gykBeszur.Value.x < i) gykBeszur = gykBeszur.Next;
                    while (gykBeszur != null && gykBeszur.Value.x <= i && gykBeszur.Value.y < j) gykBeszur = gykBeszur.Next;
                    if (gykBeszur == null) { gykLepve.AddLast(bb.honnan); PozElemez(); }
                    else
                    {
                        if (gykBeszur.Value.y != j || gykBeszur.Value.x != i) { gykLepve.AddBefore(gykBeszur, bb.honnan); PozElemez(); }
                    }
                }
                gyilokTeszt = gyilokTeszt.Next;
            }
            if (i < 100) { i = 0; j = 0; } //100=alfabéta találat a gyilkoslépések között
        }
        else /*if (sz == 1)  { i = T.cpu; tablavege = i + 1;}*/ if (sz == 1) { i = T.cpu * (8 / maxcpu); tablaVege = i + (8 / maxcpu); }

        while (i < tablaVege)
        {
            if (gykLepve.Count > 0 && XY.ee(gykLepve.First.Value, new XY(i, j))) gykLepve.RemoveFirst();
            else
            {
                if (PozElemez())
                    if (sz > 1) lock (gyilokList[sz])
                        {
                            LinkedListNode<GykListaElem> gyili = gyilokList[sz].First;
                            if (gyili == null || (((feher == -1 && gyili.Value.pont >= tempBab.pont) || (feher == 1 && gyili.Value.pont <= tempBab.pont))))
                            { gyilokList[sz].AddFirst(new GykListaElem(tempBab.babszam, tempBab.poz, tempBab.hovaTud[tempBab.ponthol - 1], tempBab.pont)); }
                            else
                            {
                                int gyk = 0; gyili = gyilokList[sz].Last;
                                while ((feher == -1 && gyili.Value.pont > tempBab.pont) || (feher == 1 && gyili.Value.pont < tempBab.pont))
                                { gyili = gyili.Previous; ++gyk; }
                                if (gyilokList[sz].Count - gyk < 35)
                                { gyilokList[sz].AddAfter(gyili, new GykListaElem(tempBab.babszam, tempBab.poz, tempBab.hovaTud[tempBab.ponthol - 1], tempBab.pont)); }
                            }
                        }
            }
            if (++j > 7) { ++i; j = 0; }
        }

        if (best == null && sz > 1)
        {
            XY k = T.kiralypoz[feher == 1 ? 1 : 0];
            if (tutimezo(T, k.x, k.y, feher)) //patt?
            {
                if (T.tablanbabukerteke * feher > 40) pont = T.tablanbabukerteke - feher * 40;
                else //ha jól állunk, ne legyen patt
                if (T.tablanbabukerteke * feher < -40) pont = T.tablanbabukerteke + feher * 40;
            }
        }
        //catch (ArgumentOutOfRangeException e) catch kapta el az alfabétát, de már nem :D     //if (sz == 1) T.ido = sw.Elapsed.TotalSeconds.ToString();
    }
    public JatekFaSzintek(TablaCuccok T, int feher) : this(T, feher, 0, 100000, -100000) { }
}
// ... 