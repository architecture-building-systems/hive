def ed_and_dhw(nfa_total, nfa_laundry, nfa_rooms, ann_el_demand, ann_el_req_light, ann_el_req_simple, washing_equip,
               washing_light, washing_vent, n_room_equip, light_in_adj_room, n_room_vent, dhw_demand, washing, adj_room):

    nfa = (nfa_total - nfa_laundry) - nfa_rooms
    electricity = (ann_el_demand * nfa) + (ann_el_req_light * nfa) + (ann_el_req_simple * nfa) + \
                  (washing_equip * nfa) + (washing_light * nfa) + (washing_vent * nfa) + \
                  (n_room_equip * nfa) + (light_in_adj_room * nfa) + (n_room_vent * nfa)
    dhw = (dhw_demand * nfa) + (washing * nfa) + (adj_room * nfa)

    return electricity, dhw
