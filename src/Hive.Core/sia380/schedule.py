# coding: utf-8

class DevicesSchedule:
    def __init__(self, DailyProfile, LoadWhenUnoccupied):
        self.DailyProfile = DailyProfile
        self.LoadWhenUnoccupied = LoadWhenUnoccupied


class LightingSchedule:
    def __init__(self, HoursPerNight, HoursPerDay):
        self.HoursPerNight = HoursPerNight
        self.HoursPerDay = HoursPerDay


class OccupancySchedule:
    def __init__(self, YearlyProfile, DailyProfile, DaysOffPerWeek, DaysUsedPerYear):
        self.YearlyProfile = YearlyProfile
        self.DailyProfile = DailyProfile
        self.DaysOffPerWeek = DaysOffPerWeek
        self.DaysUsedPerYear = DaysUsedPerYear