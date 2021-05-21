import os
import json
import yaml
from yaml.loader import FullLoader
from jsonschema import validate


HOURS_PER_DAY = 24
DAYS_PER_YEAR = 365
DAYS_PER_MONTH = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
DAYS_PER_WEEK = 7


def room_schedules():
    """
    Generates the zone schedules inspired by SIA 2024 from yaml into a single JSON
    Validate with the schedules schema.
    In YAMl because easier to write than JSON....

    Occupancy / Devices -> like SIA 2024.

    Setpoint:
    - setpoints: when occupied + 2 hours before occupied
    - otherwise setback

    Lighting:
    - for day hours: match occupancy
    - for night hours: 
      - fill evening hours from 19h until 24h with number of SIA 2024 lighting hours.
      - if some left over, fill morning hours backing up from 7h to 0h.
    """

    # Get the schedules yaml
    file_name = "sia2024_schedules.yaml"
    file_path = os.path.abspath(os.path.join(
        os.path.dirname(__file__), "..", "Resources", file_name))
    assert os.path.exists(file_path)
    with open(file_path, "r") as fp:
        schedules_sia2024 = yaml.load(fp, Loader=FullLoader)

    # Validate yaml
    file_name_schema = "sia2024_schedules_schema.yaml"
    file_path_schema = os.path.abspath(os.path.join(
        os.path.dirname(__file__), "..", "Schemas", file_name_schema))
    assert os.path.exists(file_path_schema)
    with open(file_path_schema, "r") as fp:
        schema = yaml.load(fp, Loader=FullLoader)
    validate(schedules_sia2024, schema)

    # One schedule set per type
    for i in range(len(schedules_sia2024))[::-1]:
        schedule = schedules_sia2024[i]
        if isinstance(schedule['RoomType'], list):
            room_types = schedules_sia2024.pop(i)['RoomType']
            for room_type in room_types:
                schedule['RoomType'] = room_type
                schedules_sia2024.append(dict(schedule))

    # Resort the keys as in SIA 2024
    schedules_sia2024.sort(key=lambda k: float(k['RoomType'].split(' ')[0]))

    # Generate schedules
    schedules = get_hourly_schedules(schedules_sia2024)

    # Dump as json
    out_file = os.path.join(os.path.dirname(
        __file__), "sia2024_schedules.json")
    with open(out_file, "w") as fp:
        json.dump(schedules, fp, indent=4, encoding="utf8")


def get_hourly_schedules(room_schedules):
    """
    Converts the encoded schedules for occupancy, devices, and lighting into hourly schedules.
    :param: The schedules JSON.
    :returns: Hourly schedules for occupancy (P), devices (A), and lighting (L)
    """
    schedules = {
        'RoomType': '',
        'YearlyProfile': [],
        'DaysOffPerWeek': 0,
        'OccupancySchedule': {'DailyProfile': [], 'Default': 0},
    }

    occupancy_schedule = room_schedules['OccupancySchedule']
    devices_schedule = room_schedules['DevicesSchedule']
    lighting_schedule = room_schedules['LightingSchedule']
    setpoint_schedule = room_schedules['SetpointSchedule']

    YEAR_INTIAL_WEEKDAY = 0  # indexed on 1
    HOUR_DAY_START = 8  # inclusive, from SIA 2024 corrected from 7h
    HOUR_DAY_END = 19  # exclusive, from SIA 2024 corrected from 18h

    # Other asserts are captured in schema validation
    assert 365 - room_schedules['DaysOffPerWeek'] * \
        52 == room_schedules['DaysUsedPerYear']

    def get_daily_lighting_occupied():
        # light hours = occupancy hours when light hours > 0
        n_day = lighting_schedule['HoursPerDay']
        n_night = lighting_schedule['HoursPerNight']

        # Night hours (when lighting is activated during night)
        # are filled first from 19h to 24h,
        # and, if extra, starting from 7h and moving backwards to 0h
        # The index tracks until when in the evening the night hours
        idx_night_start = HOURS_PER_DAY - 1 \
            if n_night > HOURS_PER_DAY - HOUR_DAY_END \
            else HOUR_DAY_END + n_night - 1

        L_daily_occupied = [0.0] * HOURS_PER_DAY
        for hour, occupancy in reversed(list(enumerate(occupancy_schedule['DailyProfile']))):
            if occupancy > 0.0:  # if occupied
                if hour >= HOUR_DAY_START - 1 and hour < HOUR_DAY_END-1:  # if during day
                    if n_day > 0:  # if lighting hours left to allocate
                        L_daily_occupied[hour] = 1.0
                        n_day -= 1
                else:  # during night
                    if n_night > 0 and hour < idx_night_start:  # if lighting hours left to allocate
                        L_daily_occupied[hour] = 1.0
                        n_night -= 1
        return L_daily_occupied
    
    def get_daily_setpoint_occupied():
        S_daily_occupied = [0.0] * HOURS_PER_DAY
        
        pre_occupied_hours = 2
        for hour, occupancy in reversed(list(enumerate(occupancy_schedule['DailyProfile']))):
            if occupancy > 0.0:
                pre_occupied_hours = 2
                S_daily_occupied[hour] = 1.0
            elif pre_occupied_hours > 0:
                S_daily_occupied[hour] = 1.0
                pre_occupied_hours -= 1
            else:
                S_daily_occupied[hour] = 0.5
                
        return S_daily_occupied
    

    O_daily_occupied = occupancy_schedule['DailyProfile']
    D_daily_occupied = devices_schedule['DailyProfile']
    L_daily_occupied = get_daily_lighting_occupied()
    S_daily_occupied = get_daily_setpoint_occupied()

    O_daily_unoccupied = [0.0] * HOURS_PER_DAY
    D_daily_unoccupied = [devices_schedule['LoadWhenUnoccupied']] * HOURS_PER_DAY
    L_daily_unoccupied = [0.0] * HOURS_PER_DAY
    S_daily_unoccupied = [0.5] * HOURS_PER_DAY

    weekdays_on = DAYS_PER_WEEK - room_schedules['DaysOffPerWeek']
    weekday = YEAR_INTIAL_WEEKDAY
    # TODO assert props, expand daily profiles and yearly to hourly timeseries
    
    P_hourly = []
    A_hourly = []
    L_hourly = []
    S_hourly = []
    
    for month, month_days in enumerate(DAYS_PER_MONTH):
        # TODO assumes days off / holidays all at once rather
        # than peppered through month. More appropriate for schools / summer / winter
        # but not really for national holidays...
        days_on = int(month_days * room_schedules['YearlyProfile'][month])

        for day in range(month_days):
            if weekday == DAYS_PER_WEEK:
                weekday = 0
            skip = weekday >= weekdays_on or day > days_on

            if skip: # unoccupied
                P_hourly.extend(O_daily_unoccupied)
                A_hourly.extend(D_daily_unoccupied)
                L_hourly.extend(L_daily_unoccupied)
                S_hourly.extend(S_daily_unoccupied)
            else:    # occupied
                P_hourly.extend(O_daily_occupied)
                A_hourly.extend(D_daily_occupied)
                L_hourly.extend(L_daily_occupied)
                S_hourly.extend(S_daily_occupied)

            weekday += 1

    return P_hourly, A_hourly, L_hourly, S_hourly


if __name__ == '__main__':
    room_schedules()
