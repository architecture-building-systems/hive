
from ghpythonlib.componentbase import dotnetcompiledcomponent
import Grasshopper, GhPython
import System
import GhPython
import System
import Rhino
import rhinoscriptsyntax as rs
import importlib

BADGER_CONFIG = {'description': 'Photovoltaic calculations', 'name': 'pv', 'id': '72eb1244-b95a-4624-9832-c6640a36fae9', 'components': [{'subcategory': 'pv', 'inputs': [{'nick-name': 'Gh', 'type': 'float', 'name': 'Gh', 'description': 'Global horizontal irradiation [kWh/m2]'}, {'nick-name': 'OF', 'type': 'float', 'name': 'OF', 'description': 'Orientation factor [%] (Default Value: 100%)'}, {'nick-name': u'\u03b7', 'type': 'float', 'name': 'eta', 'description': 'PV module efficiency [%] (Default Value: 18%)'}, {'nick-name': 'PR', 'type': 'float', 'name': 'PR', 'description': 'Performance ratio [%] (Default Value: 75%)'}, {'nick-name': 'A', 'type': 'float', 'name': 'A', 'description': 'PV area[m2] (Default Value: 100 m2)'}], 'description': 'A simplified PV calculation', 'outputs': [{'nick-name': 'E', 'type': 'float', 'name': 'E', 'description': 'PV electricity generation [kWh]'}], 'name': 'Simplified PV', 'id': 'b43f79db-a2bf-42a2-8b9f-5a0dada0b14f', 'main-module': 'simple_pv', 'category': '[hive]', 'class-name': 'SimplePV', 'abbreviation': 'simple-pv', 'main-function': 'simple_pv'}], 'version': '0.1', 'author': 'daren-thomas', 'include-files': ['simple_pv.py']}

PARAMETER_MAP = {
    'string': Grasshopper.Kernel.Parameters.Param_GenericObject,
    'float': Grasshopper.Kernel.Parameters.Param_Number,
    'number': Grasshopper.Kernel.Parameters.Param_Number,
}


def set_up_param(p, name, nickname, description):
    p.Name = name
    p.NickName = nickname
    p.Description = description
    p.Optional = True


for component in BADGER_CONFIG['components']:
    # dynamically create subclasses of ``component`` for each component in the badger file

    def __new__(cls):
        instance = Grasshopper.Kernel.GH_Component.__new__(cls,
                                                           component['name'],
                                                           component['abbreviation'],
                                                           component['description'],
                                                           component['category'],
                                                           component['subcategory'])
        return instance

    def RegisterInputParams(self, pManager):
        for input in component['inputs']:
            p = PARAMETER_MAP[input['type']]()
            set_up_param(p, input['name'], input['nick-name'], input['description'])
            p.Access = Grasshopper.Kernel.GH_ParamAccess.item
            self.Params.Input.Add(p)

    def RegisterOutputParams(self, pManager):
        for output in component['outputs']:
            p = PARAMETER_MAP[output['type']]()
            set_up_param(p, output['name'], output['nick-name'], output['description'])
            # p.Access = Grasshopper.Kernel.GH_ParamAccess.item
            self.Params.Output.Add(p)

    def get_Internal_Icon_24x24(self):
        o = "iVBORw0KGgoAAAANSUhEUgAAABgAAAAYCAYAAADgdz34AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAASDSURBVEhLnZQLUJRVFMcXvmUXRN47I4OQOk5NM2YSBGalmWYDueKEjizrg0DK5GXaSNhUoERINcLwEHeIQClCVEhwyEcSomBMJINoRKiIgMhrP2B3eazgv+/c3YJ1khjOzH/m7P3uOb9zzz17RU8ye5lM7ujkzDs4OGAqOcqcedprDJu+OTrN4ZNV53C9eQTV9UM496sWJ8s1yD0zgNTjPBJy+hCd1g15yCkGMYZN36i6kkoNlvhmwO05BTzlh7H+o068vc8g8v323seanc3sJMaw6RsFqYr78dRiBdwW+WOBuxIBcV0IPNDNRP6mz7rgG9E0c8D+b9V4yf8IFnoosVyhQlBSL4K/6mMif2tiD9ZG3Zw5IPxQL7Yk9LCEoclq7EjlsTPdIPJpbV3U9f8BcF5yc85buCR3YdOErKzmmfz+Rx9kDzLtyhpEeGa/ALiG+ZaWWCwSmcjTQsR7cCK5yIzz4pMzK3Hr3ijq/hzGpWtDKLuqw4kKLY5d0EJVpkVKiRZJxTrEHtchJt+g6Dwt9uRosC6ihp1g9E4BhmrjMHBWgd6jS3AmyAKeYpFaOII7Lv+uw4rQu3gzso1NBl0e9ZdaQFVSIkr66YlhxBWNMJFPa/KwKgboz7eFOnc2elXW6E6zwu1PJOwkDFB6aRAr32uFz652Nn40IXSJ1GdqBVVLCeNLRnHwp4dM5NOa77sVDDBQ6AT+mA36sgRAuhX+ipFOAPJK+/G6EUAzTmO4Ob4Dythb2BzXhG0HGhH8+U1sT2hA6Bf1CIqrxaa91fALq8AyvxTMd3XSa36cBz5PAHxjjZ6MWfjjw0knSP2+zwSwYV8rViqzH7nOddFTdVNpgZtMf/rgwkeaIhcTQEPkJEBsZrdJi3zCb2Cui4u+vDAWY90VGOsswVhHHsbb0jHemoDx29EYa9wBfV0ARqpWQVf2NAYK7ExaVPf+JEBEYqfJJa8KqWXV6VsKoDlpB12JA4bPOmLkZyeMlsuYyKc1+kZ7Hr/k34ItJgCB0W14Nfgu3gi7h7V7OrBcWc0AI42ZQmW20BTZs0RDZY4sKYl8llz4RntYe7KF9mTOQleKJa4qJwFWh7bg5W0t/97DUn/DZAzVJaD/OxsDRKhSW2wP7WmjBJ/W6BvtUedMVP/gkCWubBQbAGbmXrzrC0fx7OpfsGjNRTzvcwHuvucZQHtlNwuk6qgFlGyw0A7qPFt0ZdmgI202WpKs0RwnTE20JeojpajdLmHV57/G0R9NeCGEp8LsP54KAgyeD2JV0dGpv3SJVNV0RMnZU/EkIwB/6i12ZOorAwkTwv/wDDQXt2C4IQ0Pu2pARnuNYdM3CurN9Wb9pEsjEI0fzbgqWArlMg5H3pGgIUoyc8CD1AW4nyhB55dSdH4tZTBS4FIO/h4cAr05XN4gnjmgLd4G7fst0B5vgY4ECYORDivEULxojowAMUpfMZ8ZYI7Mni/cKkHrxxzuxIjRtFuMG2Ec6kI41Cg5VG3kULneHOkrpHAW9hrDpm8ymb2cAqm6qUR7aK8x7DETif4GWQPKTm78ggMAAAAASUVORK5CYII="
        return System.Drawing.Bitmap(System.IO.MemoryStream(System.Convert.FromBase64String(o)))

    def SolveInstance(self, DA):
        main_module = importlib.import_module(component['main-module'])
        if 'main-function' in component:
            main_function = getattr(main_module, component['main-function'])
        else:
            main_function = getattr(main_module, 'main')
        inputs = [self.marshal.GetInput(DA, i) for i in range(len(component['inputs']))]
        results = main_function(*inputs)
        if len(component['outputs']) == 1:
            self.marshal.SetOutput(results, DA, 0, True)
        elif len(component['outputs']) > 1:
            for i, r in enumerate(results):
                self.marshal.SetOutput(r, DA, i, True)

    globals()[component['name']] = type(component['class-name'], (dotnetcompiledcomponent,), {
        '__new__': __new__,
        'get_ComponentGuid': lambda self: System.Guid(component['id']),
        'RegisterInputParams': RegisterInputParams,
        'RegisterOutputParams': RegisterOutputParams,
        'get_Internal_Icon_24x24': get_Internal_Icon_24x24,
        'SolveInstance': SolveInstance,
    })


class AssemblyInfo(GhPython.Assemblies.PythonAssemblyInfo):
    def get_AssemblyName(self):
        return BADGER_CONFIG['name']

    def get_AssemblyDescription(self):
        return BADGER_CONFIG['description']

    def get_AssemblyVersion(self):
        return BADGER_CONFIG['version']

    def get_AuthorName(self):
        return BADGER_CONFIG['author']

    def get_Id(self):
        return System.Guid(BADGER_CONFIG['id'])

