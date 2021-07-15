import pytest
import json

import sia380

def test_main(**args):
    
    ## NOT WORKING YET
    
    # Arrange
    with open("testdata.json", 'r') as f:
        kwargs = json.loads(f.read(), encoding='utf-8')
    
    import sia380.datatree as dt
    kwargs['srf_irrad_obstr_tree'] = dt.DataTree([[0.0]*8760])
    kwargs['srf_irrad_unobstr_tree'] = dt.DataTree(kwargs['srf_irrad_unobstr_tree'])
    
    # Act
    results = sia380.main(**kwargs)
    
    # Assert
    Q_Heat, Q_Cool, Q_Elec, Q_T, Q_V, Q_i, Q_s, Q_T_op, Q_T_tr, Q_s_tr_tree = results
    results_no_solar = [Q_Heat, Q_Cool, Q_Elec, Q_T, Q_V, Q_i, Q_T_op, Q_T_tr]
    
    # TODO
    assert True
    
def visualise_results():
    
    t_start = 0
    t_end = 8760