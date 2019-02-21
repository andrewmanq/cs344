from tools.aima.search import *
from random import randrange
import math
import time

if(__name__=="__main__"):

    numCities = 4

    cityDict = dict(StartingNode=dict(EndingNode=10000))
    cityDict.pop(thirdCity=dict(cityDict=10, EndingNode=20))

    theMap = UndirectedGraph(cityDict)

    theProblem = GraphProblem()