"""
Shadow-range analysis using Rhino API

https://developer.rhino3d.com/5/api/RhinoCommonWin/html/T_Rhino_Render_Sun.htm

somehow it modifies properties in the Rhino Render viewport...

To-Do:
    - HIVE component that reads in the used weather file for deriving the location
    - set the location into the Rhino Sun class
    - HIVE input for day of the year
    - draw shadows for each hour of that day. Rhino Render only renders shadows of one particular moment, but
    we want a shadow-range analysis
    - Update the Rhino Render with these multiple shadows
"""