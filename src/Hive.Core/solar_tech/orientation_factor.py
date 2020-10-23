"""
Calculates solar orientation factors, depending on the

so it needs to calcalte two annual values: one on the flat zero degree, coz thats 100 percent OF,
then the value on the angled surface. Then you can calculate orientation factor for that angle
"""


def main(G_flat, G_angled):
    OF = G_angled / G_flat
    return OF
