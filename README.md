# Top250IMDBMovies
**Reikalavimai:**
* Visual Studio 2022
* MySQL serveris
* .NET Core SDK 6
   
**Projekto paleidimas:**
* MySQL serveryje turi būti sukurta „top250moviesdb“ duomenų bazė.
* „Visual Studio 2022“ aplinkoje atidaromas projektas:
  * „appsettings.json“ faile įrašomas „MySQL connection string“ ir API raktas gautas iš https://imdb-api.com.
  ![image](https://user-images.githubusercontent.com/98698257/233781528-2cf46f91-4304-4e25-ab1b-15f62d3556f1.png)
  * Išsaugojus „appsettings.json“ failą spaudžimas „ISS Express“ *dubug* mygtukas.
  * Pirmą kartą paleidus projektą yra sukuriamos MySQL duomenų bazės lentelės.
  * Šios lentelės yra užpildomos pradiniais duomenimis (užpildymas trunka apie 10 min.).
  * Užpildžius duomenų bazių lenteles duomenimis galima pradėti naudotis API užklausomis.

**Naudojimasis API užklausomis**
* Komanda skirta atlikti API užklausą:
 > {Domain or IP address}/api/Movies?genre={the name of the genre you are looking for}
* API užklausų pavyzdžiai:
  * Naudojantis „Swagger“:
  ![image](https://user-images.githubusercontent.com/98698257/233783027-08045ae5-72b1-42b2-88c6-f0530c9ca95e.png)
  * Naudojantis „Postman“:
  ![image](https://user-images.githubusercontent.com/98698257/233782930-465f8d1f-1738-4237-be3d-52858c07b9f4.png)

**Duomenų atnaujinimas**
* Duomenis atnaujinami kartą per savaitę (Pirmadieniais 0:00).
* Norėdami pratestuoti atnaujinimą „MyBackgroundService.cs“ faile pakeiskite atnaujinimo laiką. (Atnaujinimas trunka apie 10 min.)
![image](https://user-images.githubusercontent.com/98698257/233784493-4d71a344-cf8e-43bb-af17-58540c6b28ac.png)
