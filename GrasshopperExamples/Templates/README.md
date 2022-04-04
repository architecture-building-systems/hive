This folder contains Grasshopper (.gh) template files for HIVE.

- `Hive_FullTemplate.gh` -> Main template. You should use this.
- `Hive_FullTemplate_Parametric.gh` -> Same as the main template, but with parametric input components.
- `Hive_Solar_Unobstructed.gh` -> Simple example with just the solar model for the unobstructed case (no ray tracing, just physics). Can change tilt and azimuth angles of a panel and generate annual hourly solar potentials instantly.
- `GhPython_HeatingDemand.gh` -> ghpython components that calculate heating demand. Same physics as used in Hive, but solves instantly because all the data processing / visualization / what-not... is cut out. Physics are from Swiss norm SIA 380.1