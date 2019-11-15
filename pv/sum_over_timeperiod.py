"""
Summarize electricity yield over specified time period
    - daily
    - monthly
    - year
"""


def main(sum_mode, elec):
    pvyield = [0.0]
    if sum_mode == "day":
        pvyield = daily_sum(elec)
    elif sum_mode == "month":
        pvyield = monthly_sum(elec)
    elif sum_mode == "year":
        pvyield = sum(elec)

    return pvyield


def monthly_sum(elec):
    # which year should be assumed for days per month?!
    # let's assume common year in Gregorian calendar
    elecpermonth = [0.0] * 12
    dayspermonth = [31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31]
    h = 0
    for i in range(0, 12):
        elecpermonth[i] = sum(elec[h:(h + dayspermonth[i]*24)])
        h += dayspermonth[i]*24
    return elecpermonth


def daily_sum(elec):
    elecperday = [0.0] * 365
    u = 0
    for i in range(0, len(elec)):
        elecperday[u] += elec[i]
        if (i+1) % 24 == 0:
            u += 1
    return elecperday


if __name__ == '__main__':
    elec = [1.0] * 8760 # kWh
    print(main("year", elec)) # should be 8760
    print(main("month", elec))
    print(main("day", elec))

    print(sum(main("month", elec))) # should be 8760
    print(sum(main("day", elec))) # should be 8760