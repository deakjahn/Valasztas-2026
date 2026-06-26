# 2026-os országgyűlési választási eredmények — teljes adathalmaz egyben

**A teljes választási adathalmaz egy önmagában teljes, külső függőségektől mentes JSON-ban.** Minden eredmény — egyéni,
pártlistás, nemzetiségi és levélszavazás egyszerre, hierarchikus rendben, a többféle — kulcson alapuló véletlenszerű
és a listát használó iterációs — feldolgozás megkönnyítése érdekében bizonyos adatok, jellemzően objektumazonosítók
redundáns tárolásával. Az adatok között szerepelnek a vizuális megjelenítéshez szükséges térképes adatok is, vármegyei,
választókerületi és szavazóköri szinten egyaránt.

## Történet

Szokás szerint: felmerült egy kérdés, keresgéltem a neten, és kiderült, hogy bár ma már közreadja a Nemzeti Választási Iroda
a választások [elvileg teljes adathalmazát](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok),
de már az első benyomás is az volt, hogy valahogy mégsem igazán elemzésbarát: aligha lehetne bármire is könnyedén felhasználni.
Nem egyben van az anyag, hanem sok-sok fájlra szétbontva, és azok a szétbontott fájlok bizony ősöreg XLS-ek, néha CSV-k.

Így hát némi töprengés után nekiláttam az átalakításnak — és ennek során több meglepetés is ért. Az első rögtön az volt,
hogy az egyéni jelöltek adathalmazában nincsen párt. Igen, nincsen párt. Az egyes OEVK-oldalak úgy közlik az eredményeket,
hogy egyszerűen megsorszámozzák a jelölteket: 01, 02, 03, 04, stb. jelölt, a címoldalon pedig hozzárendelik a sorszámot
a jelölt nevéhez. Párt nincs sehol — másik fájlból kell előszerezni és összefésülni az információt.

A következő probléma az volt, hogy csak OEVK-szinten van térképi adat, se a szavazókörök, se a vármegyék megjelenítéséhez
nincsen. Látványos, színes térképes vizualizációt ennek hiányában igen nehéz lenne megoldani, tehát ezt is máshonnan
kell pótolni. Hogy honnan? A Választási Tájékoztató Rendszer, vagyis VTR [webes felületén](https://vtr.valasztas.hu/ogy2026/egyeni-valasztokeruletek/01/01/001/szavazohelyisegek/001?tab=szavazokor-adatai)
mindezek az adatok megjelennek, tehát a szerveren levő, a weboldal által használt JSON-adatfájlokból ki is nyerhető.

Elkészítettem tehát a teljes konverziót a teljes adathalmaz: a "letölthető és továbbfeldolgozható" fájlok, valamint a VTR nyilvános
weboldalai által betöltött JSON-állományok adatait tartalmazó fájlok együttes felhasználásával. Mielőtt nyilvánosságra
hoztam volna a teljes változatot, megkerestem az NVI-t és az állásfoglalásukat kértem, rákérdezve az esetleges feltételeikre
és követelményeikre is a második fájlkupac tekintetében.

Erre a kérésre válasz még nem érkezett, de a munkát folytattam, és a konvertert kiegészítettem az adatok konzisztenciájának
ellenőrzésével. Miután átveszi a számított adatokat a bemeneti fájlokból, a program a belső adatösszefüggéseket is ellenőrzi,
valamint párhuzamosan elvégzi az alacsonyabb szintű adatok újraösszegzését is: a szavazóköri adatokból az OEVK-adatokat,
az OEVK-adatokból a megyei adatokat, a megyei adatokból pedig az országos adatokat. Sőt, az ellenőrzés kiterjed
a többféle forrásból származó, elvileg azonos adatok összevetésére is.

És ekkor derült ki a legnagyobb probléma: **a valasztas.hu hivatalosan közreadott, „letölthető és továbbfeldolgozható”
XLS-fájljai önmagukban nem tartalmazzák a levélszavazás eredményadatait. A teljes országos listás eredmény reprodukálásához
a VTR-ben külön elérhető `LevelJkv.json` feldolgozása is szükséges.**

Az NVI-nek küldött kérdésemet kiegészítettem a fenti információval, és további állásfoglalást kértem. A VTR nyilvános JSON-adatainak
feldolgozott újraközlésével kapcsolatban 2026. június 5-én, majd pontosított kérdésekkel 2026. június 11-én kerestem meg őket.
A közzététel időpontjáig semmilyen válasz nem érkezett.

A végeredmény az [`Adatok/Feldolgozott/ogy2026.json`](Adatok/Feldolgozott/ogy2026.json) fájlban található.

## Státusz

A jelenlegi állapotban az összes [belső ellenőrzés](VALIDATION.md) hibátlanul lefut, így aránylag valószínűnek tűnik,
hogy az adathalmaz konzisztens és helyes. Ettől még bármiféle felhasználás előtt természetesen várnám a véleményeket,
ellenőrzéseket, kiigazításokat.

## Formátum

A fejléc az azonosítókon kívül az állandóan visszatérő elemeket írja le — ilyenek a pártok és a nemzetiségek. Ahol a forrásadat
vagy a hivatalos megjelenítés százalékos értéket is használ, ott az adat kételemű tömb: az első elem a tényleges egész érték,
a második elem a százalékos arány. Ahol a százalék nem egyértelmű, vagy a forrásadat sem közöl ilyet, ott csak a darabszám szerepel.

```jsonc
{
  "ev": 2026,
  "idopont": "2026-06-08T17:20:04.5030773Z",
  "nevjegyzek": 7303591, // A lakcímük szerinti szavazóköri névjegyzékben lévő választópolgárok
  "atjelent": 224081, // Belföldi szavazókörökbe átjelentkezett választópolgárok
  "kulkepv": 90730, // A külképviseleti névjegyzékben szereplő választópolgárok
  "level": 494244, // A levélben szavazók névjegyzékében szereplő választópolgárok
  "valaszto": 8112646, // Választópolgárok száma = nevjegyzek + atjelent + kulkepv + level
  "szavazo": [6408362,78.99],
  "nemszavazo": [1704284,21.01],
  "boritek": 6404403, // Urnában és beérkezett borítékokban lévő szavazólapok = ervenyes + ervenytelen
  "ervenyes": [6366733,99.41], // Érvényes szavazólapok
  "ervenytelen": [37670,0.59], // Érvénytelen szavazólapok
  "part_ervenyes": [6324862,99.34], // Pártlistákra leadott érvényes szavazatok száma összesen
  "partok": {
    "1": {
      "azon": 1,
      "rovid": "MKKP",
      "nev": "Magyar Kétfarkú Kutya Párt",
      "jelolt": 63,
      "mandatum": 0,
      "szavazat": [51965,0.82],
      "hazai": [50789,0.85],
      "level": [1176,0.35]
    },
    "2": {
      "azon": 2,
      "rovid": "TISZA",
      "nev": "Tisztelet és Szabadság Párt",
      "jelolt": 185,
      "mandatum": 45,
      "szavazat": [3385890,53.18],
      "hazai": [3339527,55.76],
      "level": [46363,13.82]
    },
    ...
    "5": {
      "azon": 5,
      "rovid": "FIDESZ-KDNP",
      "nev": "FIDESZ - Magyar Polgári Szövetség-Kereszténydemokrata Néppárt",
      "jelolt": 279,
      "mandatum": 42,
      "szavazat": [2458337,38.61],
      "hazai": [2175671,36.33],
      "level": [282666,84.23]
    }
  },
  "nemz_ervenyes": [41871,0.66], // Nemzetiségi listákra leadott érvényes szavazatok száma összesen
  "nemzetisegek": {
    "1": {
      "azon": 1,
      "rovid": "Bolgár Országos Önkormányzat",
      "nev": "Bolgár Országos Önkormányzat",
      "jelolt": 5,
      "mandatum": 0,
      "ervenyes": 108,
      "szavazo": 156
    },
    ...
    "12": {
      "azon": 12,
      "rovid": "OSZÖ",
      "nev": "Országos Szlovák Önkormányzat",
      "jelolt": 29,
      "mandatum": 0,
      "ervenyes": 902,
      "szavazo": 1160
    }
  },
```

A tényleges adatok hierarchiájának legfelső szintjén a vármegyék állnak. A statisztikai adatokon kívül tartalmazzák
a földrajzi középpontjuk koordinátáit és határukat leíró poligon koordináta-sorozatát is.

```jsonc
  "megyek": {
    "01": {
      "maz": "01",
      "nev": "Budapest",
      "kozeppont": "47.497912 19.040235",
      "korvonal": "47.5435 18.9262,47.5448 18.9295,47.5457 18.9305,...,47.5424 18.928,47.5435 18.9262",
      "nevjegyzek": 1181603, // A lakcímük szerinti szavazóköri névjegyzékben lévő választópolgárok
      "atjelent": 35106, // Belföldi szavazókörökbe átjelentkezett választópolgárok
      "atjelent_mashova": 64473, // Belföldön máshova átjelentkezettek száma
      "kulkepv": 29367, // A külképviseleti névjegyzékben szereplő választópolgárok
      "valaszto": 1246076, // Választópolgárok száma = nevjegyzek + atjelent + kulkepv
      "szavazo": [1030588,82.71],
      "nemszavazo": [215488,17.29],
      "boritek": 1030065, // Urnában és beérkezett borítékokban lévő szavazólapok = ervenyes + ervenytelen
      "ervenyes": [1025599,99.57], // Érvényes szavazólapok
      "ervenytelen": [4466,0.43], // Érvénytelen szavazólapok
      "part_ervenyes": [1023790,99.82], // Pártlistákra leadott érvényes szavazatok száma összesen
      "partok": {
        "2": [653903,63.76],
        "5": [290825,28.36],
        "3": [47120,4.59],
        "4": [17357,1.69],
        "1": [14585,1.42]
      },
      "nemz_ervenyes": [1809,0.18], // Nemzetiségi listákra leadott érvényes szavazatok száma összesen
      "nemzetisegek": {
        "7": [820,0.08],
        "6": [419,0.04],
        "10": [124,0.01],
        "8": [70,0.01],
        "2": [68,0.01],
        "4": [62,0.01],
        "9": [56,0.01],
        "5": [55,0.01],
        "3": [54,0.01],
        "11": [43,0.0],
        "1": [35,0.0],
        "12": [3,0.0]
      },
```

Ez alá tartoznak az OEVK-k (országgyűlési egyéni választókerületek). Ezek — a vármegyékhez hasonlóan — tartalmazzák
a statisztikai és térképi adatokat:

```jsonc
      "oevkk": {
        "01": {
          "oevk": "01",
          "nev": "01. evk",
          "kozeppont": "47.490980 19.045150",
          "korvonal": "47.5146939015652 19.0436777064605,47.5147366015652 19.0434745064606,...,47.514130201565 19.0452562064603",
          "nevjegyzek": 68236, // Választókerületi névjegyzékben szereplő választópolgárok aktuális száma
          "atjelent": 2851, // OEVK-ba átjelentkezett szavazók száma
          "atjelent_mashova": 7451, // Belföldön más OEVK-ba átjelentkezettek száma
          "kulkepv": 2827, // A külképviseleti névjegyzékben szereplő választópolgárok
          "valaszto": 73914, // Választópolgárok száma = nevjegyzek + atjelent + kulkepv
          "szavazo": [60313,81.6],
          "nemszavazo": [13601,18.4],
          "boritek": 60263, // Urnában és beérkezett borítékokban lévő szavazólapok = ervenyes + ervenytelen
          "ervenyes": [59956,99.49], // Érvényes szavazólapok
          "ervenytelen": [307,0.51], // Érvénytelen szavazólapok
```

Minden OEVK az elején leírja a benne induló egyéni jelölteket (a szavazatok később ezzel a kulcssal
hivatkoznak vissza a jelöltekre):

```jsonc
          "jeloltek": {
            "1": {
              "azon": 1,
              "nev": "HERFORT MARIETTA",
              "nev2": "Herfort Marietta",
              "maz": "01", // megyeazonosító
              "oevk": "01", // OEVK-azonosító
              "part": "DK",
              "szavazat": [770,1.28],
              "mandatum": false
            },
            ...
            "6": {
              "azon": 6,
              "nev": "TANÁCS ZOLTÁN",
              "nev2": "Tanács Zoltán",
              "maz": "01",
              "oevk": "01",
              "part": "TISZA",
              "szavazat": [37803,63.05],
              "mandatum": true
            }
          },
```

Ezután következik az egyes szavazókörök, és az azokban leadott szavazatok adathalmaza. Minden szavazókörnek formális
azonosítója van _vármegye-település-szavazókör-szám_ formátumban. Ezek is tartalmazzák a statisztikai és
térképi adatokat.

```jsonc
          "szavazokorok": {
            "01-001-001-5": {
              "azon": "01-001-001-5",
              "maz": "01", // megyeazonosító
              "taz": "001", // településazonosító
              "szk": "001", // szavazókör-azonosító
              "oevk": "01", // OEVK-azonosító
              "irsz": null,
              "telepules": "Budapest 01",
              "cim": "Úri utca 38.",
              "intezmeny": "Önkormányzat Intézménye",
              "leiras": "Úri utca 38. (Önkormányzat Intézménye)",
              "pozicio": "47.5013604 19.0317592",
              "korvonal": "47.5033 19.0282,47.5027 19.0288,47.5024 19.0292,...,47.5046 19.0269,47.5033 19.0282",
              "atjelent": 0, // B = Az átjelentkezett választópolgárok száma
              "kulkepv": 0, // C = Külképviseleti névjegyzékben lévő választópolgárok száma
              "boritek": 0, // I = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
```

Az általános, egész szavazókörre érvényes statisztikai adatok után elsőként az egyéni képviselőjelöltekre leadott szavazatok
jelennek meg. Az egyes jelöltek kulcsa az OEVK fejlécében felsorolt jelöltekre hivatkozik vissza:

```jsonc
              "egyeni": {
                "nevjegyzek": 1156, // A = Szavazóköri névjegyzékben lévő választópolgárok száma
                "valaszto": 1156, // E = Választópolgárok száma összesen
                "szavazo": 1006, // J = Szavazó választópolgárok száma összesen
                "megjelent": 1006, // F = Szavazókörben szavazó választópolgárok száma
                "belyegzo": 1002, // K = Urnában és a beérkezett lezárt borítékokban lévő, lebélyegzett szavazólapok száma
                "belyegzo_nelkul": 1, // O = Urnában és a beérkezett lezárt borítékokban lévő, bélyegzőlenyomat nélküli szavazólapok száma
                "elteres": -4, // L = Eltérés a szavazóként megjelentek számától (L=K-J; többlet:+/hiány:-)
                "ervenyes": [1001,99.9], // N = Érvényes szavazólapok száma
                "ervenytelen": [1,0.1], // M = Érvénytelen lebélyegzett szavazólapok száma
                "szav": {
                  "1": [8,0.8],
                  "2": [12,1.2],
                  "3": [0,0],
                  "4": [30,3],
                  "5": [402,40.16],
                  "6": [549,54.85]
                }
              },
```

Ezután a listákra leadott szavazatok jelennek meg. A statisztikai adatok mindkét listára együttesen vonatkoznak:

```jsonc
              "listas": {
                "nevjegyzek": 1156,
                "megjelent": 1006,
                "valaszto": 1156,
                "szavazo": [1006,87.02],
                "nemszavazo": [150,12.98],
                "boritek": 0,
                "belyegzo": 1002,
                "belyegzo_nelkul": 0,
                "elteres": 0,
                "ervenyes": [1000,99.8],
                "ervenytelen": [2,0.2],
```

Ezt követik a pártlistákra leadott szavazatok. A `szav` kulcsai a fejlécben felsorolt pártok azonosítói. 

```jsonc
                "partok": {
                  "nevjegyzek": 1154,
                  "megjelent": 1004,
                  "belyegzo": 1001,
                  "belyegzo_nelkul": 0,
                  "elteres": -3,
                  "ervenyes": [999,99.8],
                  "ervenytelen": [2,0.2],
                  "szav": {
                    "1": [26,2.6],
                    "2": [559,55.9],
                    "3": [49,4.9],
                    "4": [5,0.5],
                    "5": [360,36]
                  }
                },
```

A szavazókör adatai a nemzetiségi listákra leadott szavazatokkal zárulnak. A `szav` kulcsai a fejlécben definiált
nemzetiségek kódjára hivatkoznak. A `nemzetisegi.ervenyes` összesítő százaléka a valasztas.hu megjelenítésével összhangban
a pártlistás érvényes szavazatokhoz viszonyított arányt követi. Az egyes nemzetiségi részblokkokon belüli `ervenyes` és `ervenytelen`
százalékok az adott nemzetiségi szavazólapok belső érvényességi arányát mutatják.

```jsonc
                "nemzetisegi": {
                  "nevjegyzek": 2,
                  "megjelent": 2,
                  "belyegzo": 1,
                  "belyegzo_nelkul": 0,
                  "elteres": 0,
                  "ervenyes": [1,0.1],
                  "ervenytelen": [0,0],
                  "szav": {
                    "7": {
                      "nevjegyzek": 1,
                      "megjelent": 1,
                      "belyegzo": 0,
                      "belyegzo_nelkul": 0,
                      "elteres": -1,
                      "ervenyes": [0,0],
                      "ervenytelen": [0,0]
                    },
                    "8": {
                      "nevjegyzek": 1,
                      "megjelent": 1,
                      "belyegzo": 1,
                      "belyegzo_nelkul": 0,
                      "elteres": 0,
                      "ervenyes": [1,100],
                      "ervenytelen": [0,0]
                    }
                  }
                }
              }
            },
```

A szavazókör adatainak végeztével jön a következő:

```jsonc
            "01-001-002-1": {
              "azon": "01-001-002-1",
              "maz": "01", // megyeazonosító
              "taz": "001", // településazonosító
              "szk": "002", // szavazókör-azonosító
              "oevk": "01",
              "irsz": "1014",
              "telepules": "Budapest 01",
              "cim": "Tárnok utca 9-11.",
              "intezmeny": "Budavári Általános Iskola",
              "leiras": "Tárnok utca 9-11. (Budavári Általános Iskola)",
              "pozicio": "47.50051079223623 19.03455977364539",
              "korvonal": "47.5037 19.0299,47.5036 19.0301,47.5035 19.0302,...,47.5041 19.0296,47.5039 19.0298",
              ...
```

majd így tovább, a következő OEVK, és a következő vármegye.

És végezetül, a levélszavazás adatai:

```jsonc
 "levelszav": {
    "beerkezett": [356332,72.1], // Beérkezett levélszavazási iratok
    "beerkezett_ervenyes": [337439,94.7], // Feldolgozott, érvényes
    "beerkezett_ervenytelen": [18893,5.3], // Feldolgozott, érvénytelen
    "boritek": 336222, // Az érvényes szavazási iratokban lévő szavazólapok
    "elteres": -1217,
    "ervenyes": [335595,99.81], // Érvényes szavazólapok
    "ervenytelen": [627,0.19], // Érvénytelen szavazólapok
    "listas": {
      "5": [282666,84.23],
      "2": [46363,13.82],
      "3": [4859,1.45],
      "1": [1176,0.35],
      "4": [531,0.16]
    }
  }
}
```

## Felhasznált bemeneti fájlok

A felhasznált források több ponton nem ugyanazt az adatkört vagy nem ugyanazt az adatállapotot tükrözik. Ez legjobban a szavazókörök
leíró adataiból látszik: több szavazókör címe és elnevezése eltér a letölthető XLS-fájlokban és a VTR nyilvános weboldalai által
használt JSON-adatokban. A konverter ilyen esetekben a VTR aktuális webes megjelenítéséhez használt adatokat részesíti előnyben,
az eltéréseket pedig naplózza.

Általánosságban is az a véleményem alakult ki, hogy a közismert weboldal, a https://vtr.valasztas.hu mögött álló rendszer
jóval modernebb, naprakészebb. Ennek ellenére a konvertáló programban meghagytam mindkét adatforrást, és számos helyen össze is
hasonlítja a kettőt, hibát jelezve, ha eltérést talál: ez véleményem szerint segít a megbízhatóságon.

A konverter bemenetei két csoportra oszlanak:

### „Feldolgozható” XLS/CSV-fájlok

Az első csoport az NVI által külön letölthetőként közzétett XLS/CSV-fájlokból áll. Ezek az `Adatok/Eredeti` mappában levő fájlok
a [valasztas.hu ide vonatkozó oldaláról](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok)
és az egyes listák, választókerületek egyedi oldaláról származnak. Az utóbbiak formátuma CSV, a feldolgozás egységesítése és
megkönnyítése érdekében ezek is már XLS formátumban vannak.

* `partlistak_2026067.xls`, `nemzetisegi-listak_2026067.xls` — Az országos listák adatai. Az eredeti formátum CSV volt.

* `<vármegye> listás 2026.xls`, `<vármegye> OEVK egyéni 2026.xls` — Az adatok legnagyobb része ezekből a fájlpárokból kerül ki
(a Pest vármegyei listásba kézzel bele kellett javítani, mert a főoldala a többihez képest tartalmaz egy felesleges sort).

* `jeloltek_20260531.xls` — Mivel a fenti adatok egyáltalán nem tartalmazzák az egyéni jelöltek pártját, ezért azt egy
párhuzamos adathalmazból kell kiemelni. Ez a fájl a [Jelölő szervezetek, jelöltek](https://vtr.valasztas.hu/ogy2026/jelolo-szervezetek?tab=jeloltek)
oldalról származik (a fájl letöltéskor kap időbélyeget a nevébe, tehát a fájlnév nem stabil). Az eredeti formátum CSV volt,
az itt található XLS ahhoz képest már szűrt változat, mert az eredetiben a különféle köztes státuszú jelöltek is megtalálhatók,
nem csak a választáson végül ténylegesen elindultak.

* `oevk-valasztopolgarok_2026062.xls` — OEVK-szintű statisztikai adatok. Ez a fájl az [Egyéni választókerületek](https://vtr.valasztas.hu/ogy2026/egyeni-valasztokeruletek?filter=orszagos)
oldalról származik (a fájl letöltéskor kap időbélyeget a nevébe, tehát a fájlnév nem stabil). Az eredeti formátum CSV volt.

* `korzet.xls` — Az egyes szavazókörök címét egy harmadik adathalmazból kellett kiemelni. Ez nem is használja a szavazókörök
egyedi azonosítóját, szerencsére ettől még az egymáshoz rendelés megoldható.

* `oevk.json` — A választókerületek térképi megjelenítéshez szükséges adatok. Az NVI megjelölésével ellentétben
ez egyáltalán nem GeoJSON, csak egy sima koordináta-pár és -lista.

### Weboldalon használt JSON-fájlok

A második csoport a VTR nyilvános weboldalai által betöltött JSON-állományokból áll. Ezek nem kiegészítő kényelmi adatok:
a teljes, ellenőrizhető adathalmaz előállításához több elemük szükséges, különösen a levélszavazás eredményét tartalmazó
`LevelJkv.json`. Ezek a fájlok az `Adatok/Cache` mappába kerülnek. Mivel szükségesek a teljes kimeneti adathalmaz előállításához,
ezért a program hiányzó cache esetén letölti őket a VTR nyilvános weboldalai által használt URL-ekről, későbbi futások során pedig
már a helyi cache-ből dolgozik (a szavazóköri fájlok nagy száma miatt — típusonként 3177 állomány — az első futás hosszabb ideig
tarthat, a debug kimenet folyamatosan listázza az épp letöltött fájlokat, így a futás állapota jól követhető).

A nyers cache mérete és jellege miatt nem része a repositorynak; a kimeneti JSON csak a feldolgozott, a megfelelő objektumokhoz
rendelt adatokat tartalmazza.

* `Megyek.json` — a vármegyék térképi adatai és választási statisztikái.

* `OevkAdatok.json` — választókerületek elnevezési és választási statisztikái.

* `OevkJkv.json`, `ListasJkv.json` — választókerületek szavazási adatai, beleértve a jelöltek végleges eredményét
és a kiosztott mandátumokat. Ez utóbbi adatok sem szerepelnek sehol a feldolgozásra közreadott XLS-fájlokban.

* `Szk\Szavazokorok-<megye>-<település>.json`, `Topo\Szavkor-Topo-<megye>-<település>.json` — Szavazókörök címei és választási
statisztika adatai (az előbbiek számos helyen eltérnek a letölthető XLS-fájlokban található adatoktól), valamint
a szavazókörök térképi adatai. Fajtánként 3177 fájl, letöltésük hosszadalmas. A debug változat folyamatosan naplózza
a feldolgozott fájlokat, a visszajelzés kedvéért érdemes lehet azzal próbálni.

* `LevelJkv.json` — a levélszavazás adatai. Ezek teljes egészében hiányoznak a közreadott XLS-fájlokból, tehát azok önmagukban
ezért sem alkalmasak a teljes választási adathalmaz létrehozására.

## Kimeneti fájlok

* `ogy2026.json` — A fájl, amely a választás teljes adatanyagát tartalmazza, mérete 41 MB.

* `ogy2026_jeloltek.json` — Mivel a feldolgozás során amúgy is elő kellett állítani, a program lementi a jelöltek adatait.
A kulcs — a névegyezésekből eredő ütközések elkerülésére — a _név|vármegye|OEVK_ kombinációból áll.

* `ogy2026_telepulesek.json` — Mivel a feldolgozás során amúgy is elő kellett állítani, a program lementi a szavazókörrel
érintett települések adatait. A kulcs a _vármegye|település_ kombinációból áll.

* `ogy2026_schema.json` — A kimeneti fájl sémája JSON Schema formátumban.

## Hibakezelés

A konvertáló program hiba esetén szándékosan kivételt dob, hogy csak hibátlan adathalmaz és konverzió esetén jöhessen létre
a végső feldolgozott fájl. Az egyetlen eltérés ettől a szavazókörök postacíme és megnevezése: mivel a két feldolgozott adathalmaz,
az XLS és a JSON között [jó pár eltérés](https://github.com/deakjahn/Valasztas-2026/issues/2) található.
Ezeket a program csak naplózza, de a JSON-ban levő (tehát az élő VTR weboldalon használt) adatokat részesíti előnyben.

## Felhasználás

A konvertáló program forráskódja MIT licenc alatt érhető el. A feldolgozott adatkészlet és a hozzá tartozó dokumentáció
CC BY 4.0 licenc alatt használható. A feldolgozott adatkészlet licencelése nem érinti az eredeti forrásadatokra
esetlegesen vonatkozó jogokat vagy felhasználási feltételeket.

Az adatok forrása a Nemzeti Választási Iroda [adatai](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok)
és a VTR nyilvános adatközlése; a feldolgozás, az egységesítés és a JSON-szerkezet a projekt saját munkája.

Felhasználáskor, kérlek, tüntesd fel:
- az adatkészlet nevét,
- a repository URL-jét,
- valamint azt, hogy az eredeti forrásadatok a Nemzeti Választási Irodától származnak.

A projekt nem a Nemzeti Választási Iroda hivatalos kiadványa.