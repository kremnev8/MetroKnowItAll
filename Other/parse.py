import xml.etree.ElementTree as ET
import json
import codecs
from PIL import ImageColor

metro = ET.parse('mcd1.svg')
svg = metro.getroot()

lines = []
stations = []
lastColor = "#6ac9c8"
lastId = 0

for station in svg:
	circle = station[1]
	color = circle.attrib["fill"]
	xPos = circle.attrib["x"]
	yPos = circle.attrib["y"]

	
	if color == "#333333":
		color = lastColor
	
	if color != lastColor:
		colorTuple = ImageColor.getcolor(lastColor, "RGB")
		
		lineData = {
			'lineId': lastId,
			'name': "Линия",
			'style': 0,
			'useSmoothCurves': False,
			"simpleLine": True,
			"lineColor": {
				"r": colorTuple[0] / 255.0,
				"g": colorTuple[1] / 255.0,
				"b": colorTuple[2] / 255.0,
				"a": 1
			},
			'stations': stations
		}
		lines.append(lineData)
		stations = []
		lastColor = color
		lastId = lastId + 1
	
	station = {
		"position": {
			"x": xPos,
			"y": yPos
		},
		"names": [
			"Станция"
		],
		"currentNameIndex": 0,
		"isOpen": True
	}
	stations.append(station)
	
colorTuple = ImageColor.getcolor(lastColor, "RGB")
	
lineData = {
			'lineId': lastId,
			'name': "Линия",
			'style': 0,
			'useSmoothCurves': False,
			"simpleLine": True,
			"lineColor": {
				"r": colorTuple[0] / 255.0,
				"g": colorTuple[1] / 255.0,
				"b": colorTuple[2] / 255.0,
				"a": 1
			},
			'stations': stations
		}
lines.append(lineData)
	
json_string = json.dumps({"lines": lines}, sort_keys=True, indent=4)

f = codecs.open("mcd.json", "w", "utf-8")
f.write(json_string)
f.close()
    