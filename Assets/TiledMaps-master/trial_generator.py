#python
from random import randint
from random import random
import math as math
import sys

#Create a random trial file


# print("a={0},b={1}".format(a, b))


def setup_file(lat, lon, zoom, mercX, mercY, features_string, outfile, pname, radius, bc_string, num_points):
	outString = ""
	outString += "@Lat\t{0}\n".format(lat)
	outString += "@Lon\t{0}\n".format(lon)
	outString += "@Zoom\t{0}\n".format(zoom)
	outString += "@mercX\t{0}\n".format(mercX)
	outString += "@mercY\t{0}\n".format(mercY)
	outString += "@functions\t{0}\n".format(features_string)
	outString += "@outfile\t{0}\n".format(outfile)
	outString += "@pname\t{0}\n".format(pname);
	
	for i in range(num_points):
		r = randint(0,radius)
		theta = random() * 2*math.pi
		x = r * math.cos(theta) + radius
		z = r * math.sin(theta) + radius
		o = random() * 2 + 1
		outString += "@coin\tid\t{0}\tx\t{1}\tz\t{2}\toffset\t{3}\t{4}\n".format(i,x,z,o,bc_string)
	
	return outString
	


pname = sys.argv[1]
fileNames =[
"combinedTestBeacons{0}{1}.txt",
"combinedTestCoins{0}{1}.txt",
"flightTestBeacons{0}{1}.txt",
"flightTestCoins{0}{1}.txt",
"roomTestBeacons{0}{1}.txt",
"roomTestCoins{0}{1}.txt",
"teleTestBeacons{0}{1}.txt",
"teleTestCoins{0}{1}.txt",
"flightTestBeaconsNM{0}{1}.txt",
"flightTestCoinsNM{0}{1}.txt",
"roomTestBeaconsNM{0}{1}.txt",
"roomTestCoinsNM{0}{1}.txt",
"teleTestBeaconsNM{0}{1}.txt",
"teleTestCoinsNM{0}{1}.txt"
]	
	
for i in range(10):	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "frtm", fileNames[0].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[0].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "frtm", fileNames[1].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[1].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "fm", fileNames[2].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[2].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "fm", fileNames[3].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[3].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "rm", fileNames[4].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[4].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "rm", fileNames[5].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[5].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "tm", fileNames[6].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[6].format(i,pname), 'w+'))
	
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "tm", fileNames[7].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[7].format(i,pname), 'w+'))
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "f", fileNames[8].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[8].format(i,pname), 'w+'))
	
	s = setup_file(51.17, -115.31, 11, 367, 383, "f", fileNames[9].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[9].format(i,pname), 'w+'))
		
	s = setup_file(51.17, -115.31, 11, 367, 383, "r", fileNames[10].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[10].format(i,pname), 'w+'))
		
	s = setup_file(51.17, -115.31, 11, 367, 383, "r", fileNames[11].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[11].format(i,pname), 'w+'))
		
	s = setup_file(51.17, -115.31, 11, 367, 383, "t", fileNames[12].format(i,pname)+ ".csv",pname, 512, "b", 10)
	print(s, file = open(fileNames[12].format(i,pname), 'w+'))
		
	s = setup_file(51.17, -115.31, 11, 367, 383, "t", fileNames[13].format(i,pname)+ ".csv",pname, 512, "c", 10)
	print(s, file = open(fileNames[13].format(i,pname), 'w+'))
	