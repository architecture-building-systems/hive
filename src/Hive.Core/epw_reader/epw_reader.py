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
from __future__ import division
import System
import csv
import Grasshopper as gh
path = gh.Folders.AppDataFolder
import clr
import os
clr.AddReferenceToFileAndPath(os.path.join(path, "Libraries", "Hive.IO.gha"))
import Hive.IO.EnergySystems as ensys


# indexes into .epw data rows:
DRYBULB_INDEX = 6
DEWPOINT_INDEX = 7
GHI_INDEX = 13
DNI_INDEX = 14
DHI_INDEX = 15
RH_INDEX = 8


def main(path):
    result = epw_reader(path)
    return result


def epw_reader(path):
    drybulb = []
    dewpoint = []
    ghi = []
    dni = []
    dhi = []
    rh = []
    ghi_monthly = []
    drybulb_monthly = []
    rh_monthly = []

    latitude = None
    longitude = None
    city_country = None

    with open(path) as csvfile:
        for row in csv.reader(csvfile):
            if row[0] == "LOCATION":
                # read in location stuff here
                _, city, _, country, _, _, latitude, longitude, _, _ = row
                city_country = (city, country)
            elif not row[0].isdigit():
                # still parsing header portion of epw file
                continue
            else:
                drybulb.append(float(row[DRYBULB_INDEX]))
                dewpoint.append(float(row[DEWPOINT_INDEX]))
                ghi.append(float(row[GHI_INDEX]))
                dni.append(float(row[DNI_INDEX]))
                dhi.append(float(row[DHI_INDEX]))
                rh.append(float(row[RH_INDEX]))

    # let's just be sure we actually read an .epw file with a LOCATION entry
    assert latitude is not None
    assert longitude is not None

    # monthly data
    days_per_month = [31.0, 28.0, 31.0, 30.0, 31.0, 30.0, 31.0, 31.0, 30.0, 31.0, 30.0, 31.0]
    hours_per_day = 24
    total_months = 12
    hours_per_year = 8760
    for month in range(total_months):
        start_hour = int(hours_per_day * sum(days_per_month[0:month]))
        end_hour = int(hours_per_day * sum(days_per_month[0:month + 1]))
        hours_per_month = days_per_month[month] * hours_per_day
        ghi_monthly.append(sum(ghi[start_hour:end_hour]) / 1000)
        drybulb_monthly.append(sum(drybulb[start_hour:end_hour]) / hours_per_month)
        rh_monthly.append(sum(rh[start_hour:end_hour]) / hours_per_month)

    ambient_temp_carrier = ensys.Air(hours_per_year, System.Array[float](drybulb))

    return latitude, longitude, city_country, ghi, dni, dhi, drybulb, dewpoint, rh, \
           ghi_monthly, drybulb_monthly, rh_monthly, ambient_temp_carrier


if __name__ == "__main__":
    def mean(lst):
        return sum(lst) / len(lst)


    def test():
        import os
        path = os.path.join(os.path.dirname(__file__), "USA_CA_San.Francisco.Intl.AP.724940_TMY3.epw")
        (latitude, longitude, city_country, ghi, dni, dhi, drybulb, dewpoint, rh,
         ghi_monthly, drybulb_monthly, rh_monthly) = main(path)
        print(latitude, longitude, city_country, mean(dni), mean(dhi), mean(drybulb), mean(dewpoint), mean(rh))


    test()
