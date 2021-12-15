import json
import os
import string
from mako.template import Template


def main(badger_file):
    
    # parameter_compiler.setup(rhino_version)
    # temporary create the helloworld dll adding the honey-badger.json to it
    with open(badger_file, mode='r') as bf:
        badger_file_contents = bf.read()
        badger_config = json.loads(badger_file_contents)

    badger_dir = os.path.abspath(os.path.dirname(badger_file))
    build_dir = os.path.join(badger_dir, '_build')
    if not os.path.exists(build_dir):
        os.makedirs(build_dir)

    badger_config = check_badger_config(badger_config, badger_dir)
    
    from mako.exceptions import RichTraceback
    for component in badger_config['components']:
        component['class_name'] = component['class-name']
        component['exposure'] = 'secondary'
        inputs = RegisterInputParams(component)
        outputs = RegisterOutputParams(component)
        set_inputs = SolvePrefix(component)
        set_outputs = SolveSuffix(component)
        
        try:
            mytemplate = Template(COMPONENT_TEMPLATE, strict_undefined=True, default_filters=['decode.utf8'], input_encoding='utf-8', output_encoding='utf-8')
            text = mytemplate.render(register_inputs= inputs,
                                    register_outputs = outputs,
                                    set_inputs= set_inputs,
                                    set_outputs= set_outputs,
                                    solve_instance= SOLVE_INSTANCE,
                                    **component
                                    # id="",
                                    # icon="",
                                    # abbreviation="",
                                    # description="",
                                    # nick_name="",
                                    # name="",
                                    # category="",
                                    # subcategory="",
                                    # exposure="",
                                    # class_name=""
                                    )
            
            filename = component['abbreviation'] + '.cs'
            path = os.path.join(os.path.dirname(config_path), filename)
            with open(path, 'w') as file:
                file.write(text)
        except:
            traceback = RichTraceback()
            for (filename, lineno, function, line) in traceback.traceback:
                print("File %s, line %s, in %s" % (filename, lineno, function))
                print(line, "\n")
            print("%s: %s" % (str(traceback.error.__class__.__name__), traceback.error))
    

GH_PARAMETER_MAP = {
    'arc': "Arc",
    'boolean': "Boolean",
    'box': "Box",
    'brep': "Brep",
    'circle': "Circle",
    'colour': "Colour",
    'complex': "Complex",
    'culture': "Culture",
    'curve': "Curve",
    'field': "Field",
    'filepath': "FilePath",
    'generic': "GenericObject",
    'geometry': "Geometry",
    'group': "Group",
    'guid': "Guid",
    'integer': "Integer",
    'interval': "Interval",
    'interval2d': "Interval2D",
    'latlonlocation': "LatLonLocation",
    'line': "Line",
    'matrix': "Matrix",
    'mesh': "Mesh",
    'meshface': "MeshFace",
    'meshparameters': "MeshParameters",
    'float': "Number",
    'oglshader': "OGLShader",
    'plane': "Plane",
    'point': "Point",
    'rectangle': "Rectangle",
    'scriptvariable': "ScriptVariable",
    'string': "Text",
    'json': "String",
    'structurepath': "StructurePath",
    'surface': "Surface",
    'time': "Time",
    'transform': "Transform",
    'vector': "Vector",
}

CS_TYPE_MAP = {
    'float': 'double',
    'string': 'string',
    'integer': 'int',
    'generic': 'object',
    'json': 'Dictionary<string, object>',
    'boolean': 'bool'
}

def RegisterInputParams(component):
    text = []
    for input in component['inputs']:
        input['nickname'] = input['nick-name']
        input['ghtype'] = GH_PARAMETER_MAP[input['type']]
        text.append('pManager.Add{ghtype}Parameter("{name}", "{nickname}", "{description}", GH_ParamAccess.{access});' \
            .format(**input))
    return text

def RegisterOutputParams(component):
    text = []
    for output in component['outputs']:
        output['nickname'] = output['nick-name']
        output['ghtype'] = GH_PARAMETER_MAP[output['type']]
        text.append('pManager.Add{ghtype}Parameter("{name}", "{nickname}", "{description}", GH_ParamAccess.{access});' \
            .format(**output))
    return text

def SolvePrefix(component):
    text = []
    for i, input in enumerate(component['inputs']):
        input['cstype'] = CS_TYPE_MAP[input['type']]
        if input['access'] == 'list':
            input['list'] = 'List'
            input['cstype'] = 'List<' + input['cstype'] + '>'
        else :
            input['list'] = ''

        if input['default'] is None: 
            template = u"{cstype} {name} = new {cstype}();\n\t\t\tif (!DA.GetData{list}({index}, ref {name})) return;"
        else:
            template = u"{cstype} {name} = DA.GetData{list}({index}, ref {name}) ?? {default};"
        text.append(template.format(index=i, **input))
    
    text.append('\n')
    
    for i, output in enumerate(component['outputs']):
        output['cstype'] = CS_TYPE_MAP[output['type']]
        if output['access'] == 'list':
            output['list'] = 'List'
            output['cstype'] = 'List<' + input['cstype'] + '>'
        else :
            output['list'] = ''
        template = u"var {name} = new {cstype}();"
        text.append(template.format(index=i, **output))
    
    return text

def SolveSuffix(component):
    text = []
    for i, output in enumerate(component['outputs']):
        output['list'] = 'List' if output['access'] == 'list' else ''
        template = u"DA.SetData{list}({index}, {name});"
        text.append(template.format(index=i, **output))
    return text

def check_badger_config(badger_config, badger_dir):
    """
    Make sure the badger file contains all the required info. Fill in default values if they don't exist yet.

    nick-names and defaults for inputs/outputs are added automatically.

    FIXME: this could also be done with some kind of json schema thing. For now, this provides enough info.
    """
    assert "name" in badger_config, "Badger file needs a name"
    assert "description" in badger_config, "Badger file needs a description"
    assert "version" in badger_config, "Badger file needs a version"
    assert "author" in badger_config, "Badger file needs an author"
    assert "id" in badger_config, "Badger file needs an id"
    assert "include-files" in badger_config, "Badger file needs to specify include-files"
    if not "include-install" in badger_config:
        badger_config["include-install"] = []
    assert "components" in badger_config, "Badger file needs to specify at least one component"
    for component in badger_config["components"]:
        assert "class-name" in component, "Component needs a class name"
        assert "name" in component, "Component needs a name"
        assert "abbreviation" in component, "Component needs an abbreviation"
        assert "description" in component, "Component needs a description"
        assert "category" in component, "Component needs a category"
        assert "subcategory" in component, "Component needs a subcategory"
        assert "id" in component, "Component needs an id"
        assert "main-module" in component, "Component needs a main-module"
        assert "inputs" in component, "Component needs inputs"
        assert "outputs" in component, "Component needs outputs"
        for input in component["inputs"]:
            assert "type" in input, "Input needs a type"
            assert "name" in input, "Input needs a name"
            assert "description" in input, "Input needs a description"
            if not "nick-name" in input:
                input["nick-name"] = input["name"]
            if not "default" in input:
                input["default"] = None
            if not "access" in input:
                input["access"] = "item"
            assert input["access"] in {"item", "list", "tree"}, "Input Access needs to be either 'item', 'list' or 'tree'"
        for output in component["outputs"]:
            assert "type" in output, "Input needs a type"
            assert "name" in output, "Input needs a name"
            assert "description" in output, "Input needs a description"
            if not "nick-name" in output:
                output["nick-name"] = output["name"]
            if not "access" in output:
                output["access"] = "item"
        if "icon" in component:
            # convert icon (a path) to a base64 string
            icon_path = os.path.join(badger_dir, component["icon"])
            assert os.path.exists(icon_path), "Could not find icon file: {}".format(icon_path)
            # bytes = System.IO.File.ReadAllBytes(icon_path)
            # icon_base64 = System.Convert.ToBase64String(bytes)
            # component["icon"] = icon_path
    return badger_config


COMPONENT_TEMPLATE = u"""
using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Types;
using Grasshopper.Kernel;
using Hive.IO.EnergySystems;

namespace Hive.IO.Gh${subcategory}
{
    public class ${abbreviation} : GH_Component
    {
        public ${abbreviation}()
          : base("${name}", "${abbreviation}",
              "${description}",
              "${category}", "${subcategory}")
        {
        }

        public override GH_Exposure Exposure => GH_Exposure.${exposure};


        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            % for i in register_inputs:
            ${i}
            % endfor        
        }


        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            % for o in register_outputs:
            ${o}
            % endfor
        }


        protected override void SolveInstance(IGH_DataAccess DA)
        {
            % for i in set_inputs:
            ${i}
            % endfor

            ${solve_instance}

            % for o in set_outputs:
            ${o}
            % endfor
        }


        protected override System.Drawing.Bitmap Icon => Properties.Resources.${icon};


        public override Guid ComponentGuid => new Guid("${id}"); 
       
    }
}
"""

SOLVE_INSTANCE = "// TODO"

if __name__ == '__main__':
    filename = "epw_reader\\Hive.Core.epw_reader.json"
    config_path = os.path.join(os.path.dirname(__file__), filename)
    main(config_path)
