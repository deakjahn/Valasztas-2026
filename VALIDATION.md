# Belső ellenőrzések

Miután átveszi a számított adatokat a bemeneti fájlokból, a program a belső adatösszefüggéseket is ellenőrzi,
valamint párhuzamosan elvégzi az alacsonyabb szintű adatok újraösszegzését is: a szavazóköri adatokból az OEVK-adatokat,
az OEVK-adatokból a megyei adatokat, a megyei adatokból pedig az országos adatokat. Sőt, az ellenőrzés kiterjed
a többféle forrásból származó, elvileg azonos adatok összevetésére is.

Az ellenőrzéseket az `AssertError()` hívások tartalmazzák. A nagybetűvel jelölt adatok (A, B, C, stb) a jegyzőkönyvek rovatait jelentik.

## Szavazókör

### egyéni

* **EE = AE + B + C**  
választók száma = névjegyzékben szereplők + átjelentkezettek + külképviseleti

* számlálásra kijelölt szavazókörzetnél

  * **JE = FE + I**  
  szavazók száma = megjelentek + borítékok

  * **LE = KE – JE**  
  eltérés = lebélyegett – szavazók száma
   
* számlálásra ki nem jelölt szavazókörzetnél

  * **LE = KE – FE**  
  eltérés = lebélyegzett – megjelentek száma

* **KE = ME + NE**  
lebélyegzett = érvénytelen + érvényes

* **NE = Σ egyéni**  
érvényes = egyéni szavazatok összege

### listás

* **AL = AP + AN**  
névjegyzék = pártok névjegyzéke + nemzetiségek névjegyzéke

* **EL = AL + B + C**  
választók száma = névjegyzék + átjelentkezettek + külképviseleti

* **KP = MP + NP**  
pártok lebélyegzett = pártok érvénytelen + pártok érvényes

* **KL = KP + KN**  
lebélyegzett = pártok lebélyegzett + nemzetiségi lebélyegzett

* **NL = NP + NN**  
érvényes = pártok érvényes + nemzetiségi érvényes

* számlálásra kijelölt szavazókörzetnél

  * **JL = FL + IL**   
  szavazók száma = listás megjelentek + listás borítékok

* számlálásra ki nem jelölt szavazókörzetnél

  * **FL = FP + FN**  
  megjelentek = pártok megjelentek + nemzetiségi megjelentek

  * **OL = OP + ON**  
  bélyegző nélkül = pártok bélyegző nélkül + nemzetiségi bélyegző nélkül

  * **LP = KP - FP**  
  eltérés = párok lebélyegzett – pártos megjelentek

* **LN = 0**  
nemzetiségi eltérés = 0

* **NP = Σ párt**  
pártok érvényes = pártszavazatok összege

* **NN = Σ nemzetiség**  
nemzetiségi érvényes = nemzetiségi szavazatok összege

## OEVK

* Két helyről származó adatok egyezése: átjelentkezés, külképviselet, összes választópolgár, névjegyzék

* **E = A + B + C**  
választók száma = névjegyzékben szereplők + átjelentkezettek + külképviseleti

* **I = M + N**  
borítékok = érvénytelen + érvényes

* **N = Σ jelölt**  
érvényes = jelöltek szavazatainak összege

* **Σ jelölt**  
jelölt szavazatai = szavazókörökben jelöltre leadott szavazatok összege

## Vármegye

* **E = A + B + C**  
választók száma = névjegyzékben szereplők + átjelentkezettek + külképviseleti

* **I = M + N**  
borítékok = érvénytelen + érvényes

* **NL = NP + NN**  
érvényes = pártok érvényes + nemzetiségi érvényes

* **NP = Σ párt**  
pártok érvényes = pártok szavazatainak összege

* **NN = Σ nemzetiség**  
nemzetiségi érvényes = nemzetisége szavazatok összege

* **Σ külképviselet**  
külképviselet = OEVK külképviselet összege

* **Σ átjelentkezés**  
átjelentkezés = OEVK átjelentkezés összege

* **NP = Σ oevk**  
pártok érvényes = OEVK pártok érvényes összege

* **NN = Σ oevk**  
nemzetiségi érvényes = OEVK nemzetiségi érvényes összege

* **Σ oevk**  
párt szavazatai = OEVK párt szavazatainak összege

* **Σ oevk**  
nemzetiségi szavazatok = OEVK nemzetiségi szavazatok összege

## Levél

* Két helyről származó adatok egyezése: levélszavazatok száma

* bejövő szavazat = érvényes + érvénytelen

* **I = M + N**  
borítékok = érvénytelen + érvényes

* **NL = NP + NN**  
érvényes = pártok érvényes + nemzetiségi érvényes

* **NP = Σ párt**  
levél érvényes = pártok szavazatainak összege

* **NN = Σ nemzetiség**  
nemzetiségi érvényes = nemzetiségi szavazatok összege

## Országos

* **E = A + B + C + levél**  
választók száma = névjegyzékben szereplők + átjelentkezettek + külképviseleti + levélszavazat

* **I = M + N**  
borítékok = érvénytelen + érvényes

* **NL = NP + NN**  
érvényes = pártok érvényes + nemzetiségi érvényes

* **NP = Σ párt**  
pártok érvényes = pártlisták szavazatainak összege

* **NN = Σ nemzetiség**  
nemzetiségi érvényes = nemzetiségi listák szavazatainak összege

* **NP = levél + Σ megye**  
országos pártok érvényes = levél érvényes + megyék pártok érvényes összege

* **NN = Σ megye**  
országos nemzetiségi érvényes = megyék nemzetiségi érvényes összege

* **párt = Σ megye**  
párt szavazatai = párt levélszavazatai + megyék pártszavazatai

* **nemzetiség = Σ megye**  
nemzetiség szavazatai = megyék nemzetiségi szavazatai összege

## Pártok

* párt szavazatai = beföldi szavazatok + külképviseleti szavazatok
