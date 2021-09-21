# Sources

## Indoor Comfort

See:

- [201008_SIA2024_Raumdaten_Bestand.csv](201008_SIA2024_Raumdaten_Bestand.csv)
- [201008_SIA2024_Raumdaten_Standardwert.csv](201008_SIA2024_Raumdaten_Standardwert.csv)
- [201008_SIA2024_Raumdaten_Zielwert.csv](201008_SIA2024_Raumdaten_Zielwert.csv)

## Construction

Building Lifetime = 80 years.

### Walls

Bestandwand: 

- Stahlbeton Typ 4-1, 0.25m mit verputzter Aussenwärmedämmung Mineralwollplattten 40mm
- U-Wert nach UBAKUS = 0.76 W/m2K, leicht unter SIA- Wert
- Kosten: Annahme, nur ein Viertel der Dämmung, dennoch halber Preis wegen Arbeit: 189+158*0.5 = 268CHF/m2
- Emissionsne nach UBAKUS (ohne Entsorgung): 58kgCO2-eq/m2

Standardwand:

- Stahlbeton Typ 4-1, 0.25m mit verputzter Aussenwärmedämmung Mineralwollplattten 180mm
- U-Wert nach UBAKUS =  0.21 W/m2K, entspricht Standardwert SIA.
- Kosten gemäass CRB EAK: 189+158 = 347CHF/m2
- Emissionsne nach UBAKUS (ohne Entsorgung): 63kgCO2-eq/m2

Zielwand:

- Stahlbeton Typ 4-1, 0.25m mit verputzter Aussenwärmedämmung Mineralwollplattten 360mm
- U-Wert nach UBAKUS = 0.11 W/m2K, entspricht Zielwert SIA
- Kosten: Annahme, doppelte Dämmung, 1.5x Preis, da Installation kaum teurer: 189 +158*1.5 = 426CHF/m2
- Emissionsne nach UBAKUS (ohne Entsorgung): 69kgCO2-eq/m2

### Windows

Bestandfenster:

- U-Wert= 1.5 W/m2K
- Kosten: (So schlechte Fenster gibt es nicht mehr. Annahme 420CHF/m2
- Emissionen nach KBOB Fensterrechner (ohne Entsorgung): gibt es nicht, nehme gleiches wie Standard: 64 kgCO2-eq/m2

Standardfenster:

- IV-Holzfenster; einfeldrig, zweiflüglig (CRB EAK S194)
- U-Wert= 1.2 W/m2K
- Kosten gemäss CRB EAK: 467 CHF/m2
- Emissionen nach KBOB Fensterrechner (ohne Entsorgung): 64 kgCO2-eq/m2
https://treeze.ch/fileadmin/user_upload/calculators/637-Fensterrechner.htm

Zielfenster:

- U-Wert = 0.9 W/m2K
- Kosten relativ angepasst, wie nach unten genannter Quelle: 514 CHF/m2
https://www.fensterversand.com/?cid=25 
- Emissionen nach KBOB Fensterrechner (ohne Entsorgung): 79.6 kgCO2-eq/m2

## Systems (ConversionTech)

### Gas Boiler

### CHP

### Heat Pump

### PV

From [pv_efficiency.csv](pv_efficiency.csv)

|PV tech | electric efficiency | thermal efficiency | cost per m2 | cost per kWp | life cycle GHG factor kgCO2eq per m2 | life cycle GHG factor kgCO2eq per kWp | Lifetime |
| -|-|-|-|-|-|-|-|
|polycristalline | 0.206 | 0 | 633.656 | 3076 | 171.598 | 833 | 30|
|monocristalline | 0.228 | 0 | 701.328 | 3076 | 189.924 | 833 | 30|
|CIGS | 0.156 | 0 | 592.8 | 3800 | 121.68 | 780 | 30|
|CdTe | 0.182 | 0 | 691.6 | 3800 | 78.806 | 433 | 30|
|HIT | 0.209 | 0 | 642.884 | 3076 | 174.097 | 833 | 30 |

Source for carbon footprint of PV technologies: https://treeze.ch/fileadmin/user_upload/downloads/Publications/Case_Studies/Energy/Future-PV-LCA-IEA-PVPS-Task-12-March-2015.pdf

Ausgehend von einer 3kWp Anlage:

Si: 2500/3 = 833 kgCO2eq/kWp

CdTe: 1300/3 = 433 kgCO2eq/kWp

Zu den anderen umrechnen gemäss der folgenden Studie, da beide Studien vom gleichen Autor und gleiche Daten nutzen.
https://ec.europa.eu/environment/eussd/smgp/pdf/PEFCR_PV_electricity_v1.1.pdf

CIGS: 781 kgCO2eq/kWp

[Should be updated by a common source] 

Kosten: Quelle unbekannt.

### PVT

From [pvt_efficiency.csv](pvt_efficiency.csv)

|PVT tech | electric efficiency | thermal efficiency | cost per m2 | life cycle GHG factor kgCO2eq per m2|
|-|-|-|-|-|
|PVT_cheap | NaN | NaN | NaN | NaN|
|PVT_normal | NaN | NaN | NaN | NaN|
|PVT_high_tech | NaN | NaN | NaN | NaN |
|PVT | 0.17 | 0.3 | NaN | 400 |

Für embodied emissions: https://doi.org/10.1016/j.rser.2015.10.156 

Thermal efficiency: Diese hängt stark von der supply and return temperature ab. Der Wert für PVT wurde aus diversen Schätzungen bestimmt und kann im Prinzip nicht als Allgemein korrekter Wert angeschaut werden. 30%

### Solar Thermal ST

From [st_efficiency.csv](st_efficiency.csv)

|ST tech | electric efficiency | thermal efficiency | cost per kWp | cost per m2 | life cycle GHG factor kgCO2eq per kWp | life cycle GHG factor kgCO2eq per m2
|-|-|-|-|-|-|-|
|vacume_tube | 0 | 0.45 | 4444.444444 | 2000 | 462.2222222 | 208 |
|flat_plate | 0 | 0.45 | 4444.444444 | 2000 | 408.8888889 | 184 |

Kosten: etwa 2000CHF/m2 gemäss: https://tachionframework.com/603/client/res/603/docs/Usermanual_de.pdf

Efficiency: http://kollektorliste.ch/ Die Effizienz ist ähnlich für Röhren und Flachkollektoren und liegt bei ungefähr 0.45% (Nenneffizienz). Der Vakuumkollektor wird aber höhere Temperaturen ermöglichen. 

Embodied emissions: von KBOB: Röhrenkollektor 208kgCO2eq/m2, Flachkollektor 184kgCO2eq/m2
