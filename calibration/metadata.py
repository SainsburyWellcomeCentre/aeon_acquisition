from pathlib import Path
from lxml import etree
from git import Repo
import argparse
import json
import sys

parser = argparse.ArgumentParser(description="Exports device and experiment metadata for the specified workflow file.")
parser.add_argument('workflow', type=str, help="The path to the workflow file used for data acquisition.")
parser.add_argument('--indent', type=int, help="The optional indent level for JSON pretty printing.")
parser.add_argument('--allow-dirty', action="store_true", help="Optionally allow exporting metadata for modified repositories.")
parser.add_argument('--output', type=str, help="The optional path to the exported JSON file.")
args = parser.parse_args()
dname = Path(__file__).parent

repo = Repo(dname.parent)
if not args.allow_dirty and (repo.is_dirty() or len(repo.untracked_files) > 0):
    parser.error("all modifications to the acquisition repository must be committed before exporting metadata")

ns = {
    'x' : 'https://bonsai-rx.org/2018/workflow',
    'xsi' : 'http://www.w3.org/2001/XMLSchema-instance'
}

def recursive_dict(element):
    return etree.QName(element).localname, \
        dict(map(recursive_dict, element)) or element.text

def list_metadata(elements, key, **kwargs):
    metadata = []
    for x in elements:
        elem_metadata = {}
        elem_metadata = (recursive_dict(x)[1])
        elem_metadata.update(kwargs)
        elem_metadata['Name'] = elem_metadata.pop(key, key)
        metadata.append(elem_metadata)
    return metadata

root = etree.parse(args.workflow)
workflow = root.xpath('/x:WorkflowBuilder/x:Workflow/x:Nodes', namespaces=ns)[0]
hardware = workflow.xpath('./x:Expression[@xsi:type="GroupWorkflow" and ./x:Name[text()="Hardware"]]/x:Workflow/x:Nodes', namespaces=ns)[0]

video_controllers = hardware.xpath('./x:Expression[@Path="Extensions\VideoController.bonsai"]', namespaces=ns)
video_sources = hardware.xpath('./x:Expression[@Path="Extensions\VideoSource.bonsai"]', namespaces=ns)
audio_sources = hardware.xpath('./x:Expression[@Path="Extensions\AudioSource.bonsai"]', namespaces=ns)
patches = hardware.xpath('./x:Expression[@Path="Extensions\PatchController.bonsai"]', namespaces=ns)
weight_scales = hardware.xpath('./x:Expression[@Path="Extensions\WeightScale.bonsai"]', namespaces=ns)

metadata = {
    'Workflow' : args.workflow,
    'Revision' : repo.head.commit.hexsha,
    'Devices' : list_metadata(video_controllers, 'VideoController', Type='VideoController') +
                list_metadata(video_sources, 'FrameEvents', Type='VideoSource') +
                list_metadata(audio_sources, 'AudioAmbient', Type='AudioSource') +
                list_metadata(patches, 'PatchEvents', Type='Patch') +
                list_metadata(weight_scales, 'WeightEvents', Type='WeightScale')
}

if args.output:
    with open(args.output, "w") as outfile:
        json.dump(metadata, outfile, indent=args.indent)
else:
    json.dump(metadata, sys.stdout, indent=args.indent)