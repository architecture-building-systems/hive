"""
Import an epw file and output timeseries for each physical quantity

format of an EPW: https://energyplus.net/sites/all/modules/custom/nrel_custom/pdfs/pdfs_v9.2.0/AuxiliaryPrograms.pdf

It's a .csv with following format:

LOCATION, row [0]:
[0] header (i.e. 'LOCATION'), [1] City, [2] State, [3] Country, [4] Source, [5] WMO (6-digit code),
[6] Latitude [deg] -90.0 to +90.0, [7] Longitude [deg] -180.0 to 180.0,
[8] Timezone -12 to 12, [9] Elevation [m]

Starting at row [8]:
[0] Year, [1] Month, [2] Day, [3] Hour, [4] Minute, [5] ???,
[6] Dry Bulb [deg C], [7] Dew Point [deg C], [8] Rel. hum. [%], [9] Atm. Station Press. [Pa],
[10] Extraterr. Hor. Rad. [Wh/m2], [11] Extraterr. Direct Normal Rad. [Wh/m2],
[12] Hor. Infrared Rad. Intensity [Wh/m2], [13] Global Hor. Rad. [Wh/m2],
[14] Dir. Norm. Rad. [Wh/m2], [15] Diff. Hor. Rad. [Wh/m2],
[16] Glob. Hor. Ill. [lux], [17] Dir. Norm. Ill. [lux], [18] Diff. Hor. Ill. [lux], [19] Zenith Luminance [Cd/m2],
[20] Wind Dir. [deg], [21] Wind Speed [m/s],
[22] Total Sky Cover, [23] Opaque Sky Cover, [24] Visibility [km], [25] Ceiling Height [m],
[26] Present Weather Observation, [27] Present Weather Codes,
[28] Precipitable Water [mm], [29] Aerosol Optical depth [0.001],
[30] Snow Depth [cm], [31] Days Since Last Snowfall, [32] Albedo,
[33] Liquid Precipitation Depth [mm], [34] Liquid Precipitation Quantity [hr]


to do:
- {"type": "string", "name": "Resolution", "nick-name": "Resolution", "description": "Time resolution of the output.
Default is 'hourly'. Alternatives: 'monthly', 'daily', 'quarter-hourly', 'five-minutes', 'minutes'. !!!NOT IMPLEMENTED"}
- GROUNDTEMPERATURE
"""

import csv


def main(path):
    result = epw_reader(path)
    return result


def epw_reader(path):
    with open(path) as csvfile:
        readCSV = csv.reader(csvfile, delimiter=',')
        rows = list(readCSV)

        latitude = rows[0][6]
        longitude = rows[0][7]
        city_country = rows[0][1], rows[0][3]

        horizon = (len(rows) - 8)
        drybulb = [0.0] * horizon
        dewpoint = [0.0] * horizon
        dni = [0.0] * horizon
        dhi = [0.0] * horizon
        rh = [0.0] * horizon
        for i in range(8, len(rows)):
            drybulb[i-8] = rows[i][6]
            dewpoint[i-8] = rows[i][7]
            dni[i-8] = rows[i][14]
            dhi[i-8] = rows[i][15]
            rh[i-8] = rows[i][8]
    return latitude, longitude, city_country, dni, dhi, drybulb, dewpoint, rh


if __name__ == '__main__':
    path = 'USA_CA_San.Francisco.Intl.AP.724940_TMY3.epw'
    main(path)