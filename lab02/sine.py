"""
This module implements local search on a simple abs function variant.
The function is a linear function  with a single, discontinuous max value
(see the abs function variant in graphs.py).

@author: kvlinden, Andrew Quist
@version 11feb2019
"""
from tools.aima.search import Problem, hill_climbing, simulated_annealing, \
    exp_schedule, genetic_search
from random import randrange
import math
import time


class SineVariant(Problem):
    """
    State: x value for the abs function variant f(x)
    Move: a new x value delta steps from the current x (in both directions)
    """

    def __init__(self, initial, maximum=30.0, delta=0.001):
        self.initial = initial
        self.maximum = maximum
        self.delta = delta

    def actions(self, state):
        return [state + self.delta, state - self.delta]

    def result(self, stateIgnored, x):
        return x

    def value(self, x):
        return math.fabs(x * math.sin(x))
        #self.maximum / 2 - math.fabs(self.maximum / 2 - x)

if __name__ == '__main__':
    # Formulate a problem with a 2D hill function and a single maximum value.
    maximum = 30
    restarts = 10;
    finalAnnealingSolution = 0
    finalHillSolution = 0

    while restarts > 0:
        initial = randrange(0, maximum)
        p = SineVariant(initial, maximum, delta=1)
        print('Initial                      x: ' + str(p.initial)
              + '\t\tvalue: ' + str(p.value(initial))
              )

        # Solve the problem using hill-climbing.

        hill_solution = hill_climbing(p)
        if hill_solution > finalHillSolution:
            finalHillSolution = hill_solution

        # Solve the problem using simulated annealing.

        annealing_solution = simulated_annealing(
            p,
            exp_schedule(k=20, lam=0.005, limit=1000)
        )
        if annealing_solution > finalAnnealingSolution:
            finalAnnealingSolution = annealing_solution

        restarts -= 1

    print('Hill-climbing solution       x: ' + str(finalHillSolution)
          + '\tvalue: ' + str(p.value(finalHillSolution)))
    print('Simulated annealing solution x: ' + str(finalAnnealingSolution)
          + '\tvalue: ' + str(p.value(finalAnnealingSolution)))
