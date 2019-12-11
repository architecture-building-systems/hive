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
"""


def main(path):
    return epw_reader(path)


def epw_reader(path):

    return 0.0
