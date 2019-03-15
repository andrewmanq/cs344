from search import *
import math
import time
import random

"""My implementation of TSP uses a list of 2d coordinates, calculating the distance of the total trip."""
class TSP(Problem):
    """initial should be a long list of number tuples that serve as 2d coordinates"""
    def __init__(self, initial):

        self.initial = initial
        # random.shuffle(self.initial)

    """the only action an algorithm can use is switching two cities around.
    This function returns every pair of cities (except switching a city with itself)"""
    def actions(self, state):
        switchList = []
        size = len(state)
        for i in range(0, size):
            for j in range(0, size):
                if i != j:
                    newoption = [i, j]
                    switchList.append(newoption)
        return switchList

    """returns a list of coordinates with the corresponding cities switched around."""
    def result(self, state, action):
        newState = state.copy()
        newState[action[0]] = state[action[1]]
        newState[action[1]] = state[action[0]]

        return newState

    """returns the miles between points (as a negative number,
    so that the solver tries to minimize distance instead of increase it)"""
    def value(self, state):
        fullDist = 0
        size = len(state)
        for i in range(0, size):
            fullDist += findDist(state[i], state[(i + 1) % size])
        return -fullDist


def findDist(A, B):
    """finds the distance between 2d coordinates.
    I decided to use 2d vectors because it is far more memory-effective
    than making an interconnected undirected graph."""
    return math.sqrt((A[0]-A[1])**2
                     + (B[0]-B[1])**2)


if __name__ == "__main__":

    numCities = 20

    # cityList = [(0, 5), (23, 7), (9, 20), (1, 4), (30, 10), (10, 42)]
    cityList = []
    for i in range(0, numCities):
        cityList.append((random.randint(0, 100), random.randint(0, 100)))

    theProblem = TSP(cityList)

    print("original problem: " + str(cityList))
    print("original value: " + str(-theProblem.value(cityList)) + "miles\n")

    print("simulated annealing solution: ")
    anneal = simulated_annealing(theProblem, exp_schedule(k=20, lam=0.005, limit=10000))
    print(str(anneal)
          + '\nvalue: ' + str(-theProblem.value(anneal)) + " miles\n")

    print("hillclimbing solution: ")
    hill = hill_climbing(theProblem)

    print(str(hill)
          + '\nvalue: ' + str(-theProblem.value(hill)) + " miles\n")
