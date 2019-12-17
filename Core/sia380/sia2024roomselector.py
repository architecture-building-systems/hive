"""
Returns SIA 2024 room data given a specific room type
"""
from __future__ import print_function


def show_room(room):
    return "\n".join("{key}: {value}".format(key=key, value=value)
                     for key, value in room.items())
