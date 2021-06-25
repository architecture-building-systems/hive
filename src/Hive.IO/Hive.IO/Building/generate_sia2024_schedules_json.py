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
    
    print("Compiling / generating SIA 2024 Schedules...")
    
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
    schedules_all = []
    for s in schedules_sia2024:
        schedules_all.append(get_daily_schedules(s))

    # Dump as json
    out_file = os.path.join(os.path.dirname(
        __file__), "sia2024_schedules.json")
    with open(out_file, "w") as fp:
        json.dump(schedules_all, fp, indent=4, encoding="utf8")
        
    print("Done. Saved at {0}".format(out_file))


def get_daily_schedules(room_schedules):
    """
    Converts the encoded schedules for occupancy, devices, and lighting into hourly schedule.
    :param: The SIA 2024 schedules JSON.
    :returns: Daily schedules for occupancy, devices, lighting, setpoints
    """

    occupancy_schedule = room_schedules['OccupancySchedule']
    devices_schedule = room_schedules['DeviceSchedule']
    lighting_schedule = room_schedules['LightingSchedule']

    HOUR_DAY_START = 8  # inclusive, from SIA 2024 corrected from 7h
    HOUR_DAY_END = 19  # exclusive, from SIA 2024 corrected from 18h

    # Other asserts are captured in schema validation
    assert 365 - occupancy_schedule['DaysOffPerWeek'] * \
        52 == occupancy_schedule['DaysUsedPerYear']

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

    def get_daily_setpoint_occupied(lighting_daily):
        S_daily_occupied = [0.0] * HOURS_PER_DAY

        before_occupied_hours = 0
        for hour, (occupancy, lighting) in reversed(list(enumerate(zip(occupancy_schedule['DailyProfile'], lighting_daily)))):
            # if occupied and lit (people are active)
            if occupancy > 0.0 and lighting > 0.0:
                # 2h before occupied we want the setpoint to preheat/cool space
                before_occupied_hours = 2
                S_daily_occupied[hour] = 1.0
            elif before_occupied_hours > 0:  # use the setpoint within 2h before occupied
                S_daily_occupied[hour] = 1.0
                before_occupied_hours -= 1
            else:  # use setback
                S_daily_occupied[hour] = 0.5

        return S_daily_occupied

    O_daily_occupied = occupancy_schedule['DailyProfile']
    D_daily_occupied = devices_schedule['DailyProfile']
    L_daily_occupied = get_daily_lighting_occupied()
    S_daily_occupied = get_daily_setpoint_occupied(L_daily_occupied)

    return {
        "RoomType": room_schedules["RoomType"],
        "YearlyProfile": occupancy_schedule["YearlyProfile"],
        "DaysOffPerWeek": occupancy_schedule["DaysOffPerWeek"],
        "DaysUsedPerYear": occupancy_schedule["DaysUsedPerYear"],
        "LightingSchedule": {
            "DailyProfile": L_daily_occupied,
            "Default": 0.0
        },
        "OccupancySchedule": {
            "DailyProfile": O_daily_occupied,
            "Default": 0.0
        },
        "DeviceSchedule": {
            "DailyProfile": D_daily_occupied,
            "Default": devices_schedule['LoadWhenUnoccupied']
        },
        "SetpointSchedule": {
            "DailyProfile": S_daily_occupied,
            "Default": 0.0
        }
    }


if __name__ == '__main__':
    room_schedules()

    # TEST
    # expected = {
    #     "RoomType": "1.1 Wohnen Mehrfamilienhaus",
    #     "YearlyProfile": [
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8,
    #         0.8
    #     ],
    #     "DaysOffPerWeek": 0,
    #     "DaysUsedPerYear": 365,
    #     "LightingSchedule": {
    #         "DailyProfile": [
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			1.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			1.0,
    # 			1.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    # 			1.0,
    # 			1.0,
    # 			1.0,
    # 			1.0,
    # 			0.0,
    # 			0.0,
    # 			0.0,
    #         ],
    #         "Default": 0.0
    #     },
    #     "OccupancySchedule": {
    #         "DailyProfile": [
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             0.6,
    #             0.4,
    #             0,
    #             0,
    #             0,
    #             0,
    #             0.8,
    #             0.4,
    #             0,
    #             0,
    #             0,
    #             0.4,
    #             0.8,
    #             0.8,
    #             0.8,
    #             1.0,
    #             1.0,
    #             1.0
    #         ],
    #         "Default": 0.0
    #     },
    #     "DeviceSchedule": {
    #         "DailyProfile": [
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.2,
    #             0.8,
    #             0.2,
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.8,
    #             0.2,
    #             0.1,
    #             0.1,
    #             0.1,
    #             0.2,
    #             0.8,
    #             1.0,
    #             0.2,
    #             0.2,
    #             0.2,
    #             0.1
    #         ],
    #         "Default": 0.1
    #     },
    #     "SetpointSchedule": {
    #         "DailyProfile": [
    #             0.5,
    #             0.5,
    #             0.5,
    #             0.5,
    #             0.5,
    #             1.0,
    #             1.0,
    #             1.0,
    #             0.5,
    #             0.5,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             0.5,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             1.0,
    #             0.5,
    #             0.5,
    #             0.5
    #         ],
    #         "Default": 0.0
    #     }
    # }

    # out_file = os.path.join(os.path.dirname(__file__), "sia2024_schedules.json")
    # with open(out_file, "r", encoding="utf8") as fp:
    #     actual = json.loads(fp.read())[0]

    # assert actual == expected

    # Printing helpers
    # [print(t) for t in list(zip(actual['DeviceSchedule']['DailyProfile'],
    #                             actual['OccupancySchedule']['DailyProfile'],
    #                             actual['LightingSchedule']['DailyProfile'],
    #                             actual['SetpointSchedule']['DailyProfile'], ))]

    # [print(t) for t in zip(expected['SetpointSchedule']['DailyProfile'],actual['SetpointSchedule']['DailyProfile'])]
