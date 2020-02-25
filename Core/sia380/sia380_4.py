"""
Calculating electricity and domestic hot water demand
currently just multiplying usage hours times area times specific electricity demand

But in future built, according to SIA380-4?
"""


def sia380_4(nfa, t_L, t_A, t_V, L, A, V):

    return nfa * t_L * L + nfa * t_A * A + nfa * t_V * V
