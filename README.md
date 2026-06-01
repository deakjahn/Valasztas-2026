# 2026-os országgyűlési választási eredmények — minden egyben

Szokás szerint: felmerült egy kérdés, keresgéltem a neten, és kiderült, hogy bár ma már közreadja a Nemzeti Választási Iroda
a választások [teljes adathalmazát](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok), de valahogy
mégsem igazán elemzésbarát: aligha lehetne bármilyen elemzésre könnyedén felhasználni. Nem egyben van az anyag,
hanem sok-sok fájlra szétbontva, és azok a szétbontott fájlok bizony ősöreg XLS-ek, néha CSV-k. Valami adatbázisból csak úgy
kiexportálva, vagy tudja a cica.

És nem elég a régi, nehezen használható formátum és a sok helyre elszórt adat, fontos adatok még így is hiányoznak. A leginkább
érthetetlen az, hogy az egyéni jelölteknél nincsen párt. Igen, nincsen párt. Az egyes OEVK-oldalak úgy közlik az eredményeket,
hogy egyszerűen megsorszámozzák a jelölteket: 01, 02, 03, 04, stb. jelölt, a címoldalon pedig hozzárendelik a sorszámot
a jelölt nevéhez. Párt nincs sehol.

Így hát némi töprengés után nekiláttam, és itt látható a végeredménye: a teljes választási adathalmaz egy
önmagában teljes JSON-ban. Minden eredmény, listás, egyéni, nemzetiségi egyszerre, hierarchikus rendben, a többféle
— kulcson alapuló véletlenszerű és a listát használó iterációs — feldolgozás megkönnyítése érdekében bizonyos adatok,
jellemzően objektumazonosítók redundáns tárolásával.

A részletek később jönnek, de a türelmetlenek az [`Adatok/Feldolgozott/ogy2026.json`](Adatok/Feldolgozott/ogy2026.json) fájlban
keressék a végeredményt.

## Státusz

Még friss, most készült. Szúrópróbaszerű ellenőrzéseket már végeztem, de bármiféle felhasználás előtt várnám a véleményeket,
ellenőrzéseket, kiigazításokat.

## Formátum

A fejléc a két azonosítón kívül az állandóan visszatérő elemeket írja le — ilyenek a pártok és a nemzetiségek:

```
{
  "year": 2026,
  "generated": "2026-05-31T20:59:09.9583934Z",
  "parties": {
    "1": {
      "code": 1,
      "abbreviation": "MKKP",
      "name": "Magyar Kétfarkú Kutya Párt"
    },
    "2": {
      "code": 2,
      "abbreviation": "TISZA",
      "name": "Tisztelet és Szabadság Párt"
    },
    ...
    "5": {
      "code": 5,
      "abbreviation": "FIDESZ-KDNP",
      "name": "FIDESZ - Magyar Polgári Szövetség-Kereszténydemokrata Néppárt"
    }
  },
  "nationalities": {
    "1": {
      "code": 1,
      "abbreviation": "OLÖ",
      "name": "Országos Lengyel Önkormányzat"
    },
    "2": {
      "code": 2,
      "abbreviation": "OÖÖ",
      "name": "Országos Örmény Önkormányzat"
    },
    ...
    "11": {
      "code": 11,
      "abbreviation": "MROÖ",
      "name": "Magyarországi Románok Országos Önkormányzata"
    },
    "12": {
      "code": 12,
      "abbreviation": "Országos Szlovén",
      "name": "Országos Szlovén Önkormányzat"
    }
  },
```

A tényleges adatok hierarchiájának legfelső szintjén a vármegyék állnak. Ez alá tartoznak az OEVK-k
(országgyűlési egyéni választókerületek). Ezek a néven és kódjukon kívül tartalmazzák a földrajzi középpontjuk
koordinátáit és a választókerület határát leíró poligon koordináta-sorozatát is.

```
  "counties": {
    "01": {
      "code": "01",
      "name": "Budapest",
      "oevks": {
        "01": {
          "code": "01",
          "name": "01. evk",
          "center": "47.490980 19.045150",
          "border": "47.5146939015652 19.0436777064605,47.5147366015652 19.0434745064606,...,47.514130201565 19.0452562064603",
```

Minden OEVK az elején leírja a benne induló egyéni jelölteket (a szavazatok később ezzel a kulcssal
hivatkoznak vissza a jelöltekre):

```
          "candidates": {
            "1": {
              "code": 1,
              "name": "HERFORT MARIETTA",
              "name2": "Herfort Marietta",
              "county": "01",
              "oevk": "01",
              "party": "DK"
            },
            ...
            "6": {
              "code": 6,
              "name": "TANÁCS ZOLTÁN",
              "name2": "Tanács Zoltán",
              "county": "01",
              "oevk": "01",
              "party": "TISZA"
            }
          },
```

Ezután következik az egyes szavazókörök, és az azokban leadott szavazatok adathalmaza. Minden szavazókörnek hivatalos
azonosítója van _vármegye-település-szavazókör-ellenőrzőszám_ formátumban, bár a település azonosítója máshol nem köszön vissza,
ezzel egyértelműen lehet azonosítani. A jelenlegi adatokból a címek további bontását és koordinátákat még nem sikerült
a szavazókörökhöz rendelni.

A — természetesen amúgy a JSON-ban nem használt — megjegyzések a szavazóköri jegyzőkönyvek és a feldolgozott XLS-fájlok
rubrikáira és jelmagyarázatára utalnak:

```
          "stations": {
            "01-001-001-5": {
              "code": "01-001-001-5",
              "oevk": "01",
              "settlement": "Budapest I. kerület",
              "description": "Bartók Béla út 27. (Gárdonyi Géza Ált. Isk.)",
              "name": "Gárdonyi Géza Ált. Isk.",
              "address": "Bartók Béla út 27.",
              "absentee": 0, // C = Külképviseleti névjegyzékben lévő választópolgárok száma
              "transfer": 0, // B = Az átjelentkezett választópolgárok száma
              "voters": 1156, // E = Választópolgárok száma összesen
              "voted": 1006, // J = Szavazó választópolgárok száma összesen
              "envelope": 0, // I = Átjelentkezéssel és külképviseleten szavazó választópolgárok beérkezett lezárt borítékjainak száma
              "register": 1156, // A = Szavazóköri névjegyzékben lévő választópolgárok száma
              "inperson": 1006, // F = Szavazókörben szavazó választópolgárok száma
              "notstamped": 0, // O = Urnában és a beérkezett lezárt borítékokban lévő, bélyegzőlenyomat nélküli szavazólapok száma
              "stamped": 1002, // K = Urnában és a beérkezett lezárt borítékokban lévő, lebélyegzett szavazólapok száma
              "difference": 0, // L = Eltérés a szavazóként megjelenetk számától (L=K-J; többlet:+/hiány:-)
              "invalid": 2, // M = Érvénytelen lebélyegzett szavazólapok száma
              "valid": 1000, // N = Érvényes szavazólapok száma
```

Az általános, egész szavazókörre érvényes statisztikai adatok után elsőként a listákra leadott szavazatok jelennek meg.
A `votes` kulcsai a fejlécben felsorolt pártok azonosítói. 

```
              "list": {
                "register": 1154,
                "inperson": 1004,
                "notstamped": 0,
                "stamped": 1001,
                "difference": -3,
                "invalid": 2,
                "valid": 999,
                "votes": {
                  "1": 26,
                  "2": 559,
                  "3": 49,
                  "4": 5,
                  "5": 360
                }
              },
```

Ezután következnek az egyéni képviselőjelöltekre leadott szavazatok, a statisztikai adatok értelmezése megegyezik a korábbival,
az egyes jelöltek kulcsa az OEVK fejlécében felsorolt jelöltekre hivatkozik vissza:

```
              "individual": {
                "register": 1156,
                "voters": 1156,
                "voted": 1006,
                "inperson": 1006,
                "notstamped": 1,
                "stamped": 1002,
                "difference": -4,
                "invalid": 1,
                "valid": 1001,
                "votes": {
                  "1": 8,
                  "2": 12,
                  "3": 0,
                  "4": 30,
                  "5": 402,
                  "6": 549
                }
              },
```

A szavazókör adatai a nemzetiségi listákra leadott szavazatokkal zárulnak. A `byList` kulcsai a fejlécben definiált
nemzetiségek kódjára hivatkoznak:

```
              "nationalities": {
                "register": 2,
                "inperson": 2,
                "notstamped": 0,
                "stamped": 1,
                "difference": 0,
                "invalid": 0,
                "valid": 1,
                "byList": {
                  "1": {
                    "register": 1,
                    "inperson": 1,
                    "notstamped": 0,
                    "stamped": 0,
                    "difference": -1,
                    "invalid": 0,
                    "valid": 0
                  },
                  "2": {
                    "register": 1,
                    "inperson": 1,
                    "notstamped": 0,
                    "stamped": 1,
                    "difference": 0,
                    "invalid": 0,
                    "valid": 1
                  }
                }
              }
            },
```

A szavazókör adatainak végeztével jön a következő:

```
            "01-001-002-1": {
              "code": "01-001-002-1",
              "oevk": "01",
              "settlement": "Budapest I. kerület",
              "description": "Bocskai út 47-49. (Bocskai István Ált. Isk.)",
              "name": "Bocskai István Ált. Isk.",
              "address": "Bocskai út 47-49.",
              "absentee": 0,
              "transfer": 0,
              "envelope": 0,
              ...
```

majd így tovább, a következő OEVK, és a következő vármegye.

## Felhasznált bemeneti fájlok

A konvertáló program az `Adatok/Eredeti` mappában levő fájlokat dolgozza fel. A fájlok szinte mindegyike a
[valasztas.hu ide vonatkozó oldaláról](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok)
származik. Az adatok legnagyobb része a

* `<vármegye> listás 2026.xls`
* `<vármegye> OEVK egyéni 2026.xls`

fájlpárból kerül ki (a Pest vármegyei listásba kézzel bele kellett javítani, mert a főoldala a többihez képest tartalmaz egy
felesleges sort).

Ahogy az elején említettem, a fenti adatok egyáltalán nem tartalmazzák az egyéni jelöltek pártját, ezért azt egy párhuzamos
adathalmazból kell kiemelni. Ez a fájl a [Jelölő szervezetek, jelöltek](https://vtr.valasztas.hu/ogy2026/jelolo-szervezetek?tab=jeloltek)
oldalról származik (a fájl letöltéskor kap időbélyeget a nevébe, tehát a lenti fájlnév nem stabil):

* `jeloltek_20260531.xls`

Az eredeti formátum CSV, az itt található XLS ahhoz képest már szűrt változat, mert az eredetiben a különféle köztes státuszú
jelöltek is megtalálhatók, nem csak a választáson végül ténylegesen elindultak.

Az egyes szavazókörök címét egy harmadik adathalmazból kellett kiemelni:

* `korzet.xls`

ráadásul ez — teljesen érthetetlen módon — nem is használja a szavazókörök egyedi azonosítóját, szerencsére ettől még
az egymáshoz rendelés sikeresen megtörténik.

A térképi megjelenítéshez szükséges adatok egy negyedik fájlból származnak:

* `oevk.json`

Az NVI megjelölésével ellentétben ez egyáltalán nem GeoJSON, csak egy sima koordináta-pár és -lista.

## Kimeneti fájlok

Az egyértelműen fontos kimenet az

* `ogy2026.json`

fájl, amely a választás teljes adatanyagát tartalmazza. Mivel a feldolgozás során amúgy is elő kellett állítani,
a program lementi az

* `ogy2026_jeloltek.json`

fájlt is, amelyben a jelöltek adatai találhatók. A kulcs — a névegyezésekből eredő ütközések elkerülésére —
a _név|vármegye|OEVK_ kombinációból áll. A neveken semmilyen átalakítás nem történt, minden úgy maradt, beleértve a
meglehetősen sajátos nagybetűs írásmódot, ahogy az eredeti fájlok tartalmazták.

## Felhasználás

A konvertáló program forráskódja MIT licenc alatt érhető el. A feldolgozott adatkészlet és a hozzá tartozó dokumentáció
CC BY 4.0 licenc alatt használható. Az adatkészlet a Nemzeti Választási Iroda nyilvánosan közzétett
2026-os országgyűlési választási [adataiból](https://www.valasztas.hu/ogy2026-letoltheto-es-tovabbfeldolgozhato-adatok) készült,
több forrásfájl összefésülésével, tisztításával és átalakításával.

Felhasználáskor, kérlek, tüntesd fel:
- az adatkészlet nevét,
- a repository URL-jét,
- valamint azt, hogy az eredeti forrásadatok a Nemzeti Választási Irodától származnak.

A projekt nem a Nemzeti Választási Iroda hivatalos kiadványa.