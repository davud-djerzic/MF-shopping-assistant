# MF Shopping Assistant

MF Shopping Assistant je inovativan, pametan sistem dizajniran da unaprijedi korisnicko iskustvo prilikom kupovine u fizičkim trgovinama koristenjem kombinacije hardverskih i softverskih komponenti. 
Sistem kombinuje barkod skener, digitalnu vagu (load cell + HX711), Raspberry Pi 4 uredjaj i WinForms C# aplikaciju kako bi omogucio brzo, efikasno i tacno evidentiranje proizvoda, mjerenje kolicine i generisanje racuna.

Aplikacija je razvijena s ciljem da simulira pametan maloprodajni sistem u kojem korisnik samostalno skenira i vaga proizvode, bez potrebe za dodatnim osobljem.
Time se omogucava uvođenje koncepta self-checkout-a u manjim prodajnim objektima uz minimalna ulaganja.

## Ciljevi projekta

- Automatizacija i ubrzavanje procesa kupovine
- Povećanje tacnosti mjerenja i fakturisanja proizvoda na mjeru (npr. voce i povrce)
- Demonstracija integracije vise tehnologija i platformi u jedinstveni sistem

## Opis projekta

Aplikacija omogucava:

- Skeniranje proizvoda pomocu barkod skenera
- Automatsko ocitavanje tezine proizvoda putem HX711 
- Azuriranje kolicine i dodavanje artikala u korpu
- Generisanje i slanje PDF racuna putem e-maila
- TCP komunikaciju izmedju Python skripte i C# aplikacije

##  Tehnologije i infrastruktura

- C# WinForms (.NET 4.7.2)
- Python skripta za kalibraciju i ocitavanje podataka sa vage
- Raspberry Pi 4 + HX711 modul + load cell
- TCP/IP socket komunikacija
- iTextSharp za PDF generisanje

## Nacin rada

- Korisnik skenira proizvod koristeći barkod skener.
- Proizvod se automatski prepoznaje iz baze podataka i dodaje u korpu
- Na osnovu vrste artikla (voce i povrce), korisnik postavlja proizvod na vagu.
- Raspberry Pi putem Python skripte očitava težinu i šalje je TCP socketom aplikaciji.
- Aplikacija prikazuje količinu, izračunava cijenu i omogućava dodavanje u korpu.
- Nakon skeniranja svih proizvoda, korisnik može generisati PDF račun i primiti ga putem e-maila.

## Prednosti i inovacije

- Laka instalacija i skalabilnost – sistem se moze prilagoditi manjim prodavnicama
- Niski troskovi – koristi se dostupni hardver kao što su Raspberry Pi i standardni barkod skeneri
- Modularna arhitektura – omogucava lako dodavanje funkcionalnosti (npr. online placanje, prepoznavanje artikla kamerom)
- Dvosmjerna komunikacija – izmedju mikrokontrolera i desktop aplikacije u realnom vremenu

## Pokretanje aplikacije

Aplikacija se izvrsava **na Raspberry Pi-u**, dok je **baza podataka smjestena na laptopu**. Komunikacija između Raspberry Pi-a i baze vrsi se putem lokalne mreze koristeći IP adresu.

### Priprema

1. Uvjerite se da su **Raspberry Pi i laptop povezani na istu Wi-Fi mrezu**.
2. Na laptopu mora biti pokrenuta **baza podataka** (MySql).
3. Na Raspberry Pi-u mora biti instaliran **Mono paket** za pokretanje `.exe` fajlova:
   sudo apt update
   sudo apt install mono-complete

### Python skripta za kalibraciju i ocitavanje podataka sa vage

```python
import socket
import time
import struct

import RPi.GPIO as GPIO  # import GPIO
from hx711 import HX711  # import the class HX711

HOST = '127.0.0.1'
PORT = 50001

try:
    GPIO.setmode(GPIO.BCM)  # set GPIO pin mode to BCM numbering
    # Create an object hx which represents your real hx711 chip
    # Required input parameters are only 'dout_pin' and 'pd_sck_pin'
    hx = HX711(dout_pin=5, pd_sck_pin=6)
    # measure tare and save the value as offset for current channel
    # and gain selected. That means channel A and gain 128
    err = hx.zero()
    # check if successful
    if err:
        raise ValueError('Tare is unsuccessful.')

    reading = hx.get_raw_data_mean()
    if reading:  # always check if you get correct value or only False
        # now the value is close to 0
        print('Data subtracted by offset but still not converted to units:',
              reading)
    else:
        print('invalid data', reading)

    # In order to calculate the conversion ratio to some units, in my case I want grams,
    # you must have known weight.
    input('Put known weight on the scale and then press Enter')
    reading = hx.get_data_mean()
    if reading:
        print('Mean value from HX711 subtracted by offset:', reading)
        known_weight_grams = input(
            'Write how many grams it was and press Enter: ')
        try:
            value = float(known_weight_grams)
            print(value, 'grams')
        except ValueError:
            print('Expected integer or float and I have got:',
                  known_weight_grams)

        # set scale ratio for particular channel and gain which is
        # used to calculate the conversion to units. Required argument is only
        # scale ratio. Without arguments 'channel' and 'gain_A' it sets
        # the ratio for current channel and gain.
        ratio = reading / value  # calculate the ratio for channel A and gain 128
        hx.set_scale_ratio(ratio)  # set ratio for current channel
        print('Ratio is set.')
    else:
        raise ValueError('Cannot calculate mean value. Try debug mode. Variable reading:', reading)

    # Read data several times and return mean value
    # subtracted by offset and converted by scale ratio to
    # desired units. In my case in grams.
    print("Now, I will read data in infinite loop. To exit press 'CTRL + C'")
    input('Press Enter to begin reading')
    print('Current weight on the scale in grams is: ')
    with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as s:
        s.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        s.bind((HOST, PORT))
        s.listen()
        canRead = False
        while True:
            conn, addr = s.accept()
            with conn:
                while True:
                    try:
                        receivedData = conn.recv(1024).decode("utf-8").strip()
                        if receivedData == "STOP":
                            canRead = False
                            conn.sendall(struct.pack('f', 0.0))
                        if receivedData == "GET_WEIGHT":
                            canRead = True
                        
                        if receivedData == "CLOSE":
                            canRead = False
                        
                        while canRead:
                            try:
                                weight = hx.get_weight_mean(5)  
                                
                                if weight is not False:
                                    data = struct.pack('f', weight)
                                    conn.sendall(data)
                                    print(weight, 'g')
                                else:
                                    print("Invalid weight reading!")
                                    continue
                                time.sleep(0.3)
                            except BrokenPipeError:
                                break
                    except BrokenPipeError:
                        break

except (KeyboardInterrupt, SystemExit):
    print('Bye :)')

finally:
    GPIO.cleanup()
```
