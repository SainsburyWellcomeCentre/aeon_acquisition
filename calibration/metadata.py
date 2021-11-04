from lxml import etree
import argparse
import json
import sys

parser = argparse.ArgumentParser(description="Exports device and experiment metadata for the specified workflow file.")
parser.add_argument('workflow', type=str, help="The path to the workflow file used for data acquisition.")
parser.add_argument('--indent', type=int, help="The optional indent level for JSON pretty printing.")
args = parser.parse_args()

ns = {
    'x' : 'https://bonsai-rx.org/2018/workflow',
    'xsi' : 'http://www.w3.org/2001/XMLSchema-instance'
}

def recursive_dict(element):
    return etree.QName(element).localname, \
        dict(map(recursive_dict, element)) or element.text

def list_metadata(elements):
    return list((recursive_dict(x)[1]) for x in elements)

root = etree.parse(args.workflow)
workflow = root.xpath('/x:WorkflowBuilder/x:Workflow/x:Nodes', namespaces=ns)[0]
hardware = workflow.xpath('./x:Expression[@xsi:type="GroupWorkflow" and ./x:Name[text()="Hardware"]]/x:Workflow/x:Nodes', namespaces=ns)[0]

video_controllers = hardware.xpath('./x:Expression[@Path="Extensions\VideoController.bonsai"]', namespaces=ns)
video_sources = hardware.xpath('./x:Expression[@Path="Extensions\VideoSource.bonsai"]', namespaces=ns)
audio_sources = hardware.xpath('./x:Expression[@Path="Extensions\AudioSource.bonsai"]', namespaces=ns)
patches = hardware.xpath('./x:Expression[@Path="Extensions\PatchController.bonsai"]', namespaces=ns)

metadata = {
    'VideoControllers' : list_metadata(video_controllers),
    'VideoSources' : list_metadata(video_sources),
    'AudioSources' : list_metadata(audio_sources),
    'Patches' : list_metadata(patches)
}

json.dump(metadata, sys.stdout, indent=args.indent)