from csp import *
from search import *


def Schedule():
    variables = "cs108 cs112 cs212 cs214 cs216 cs323 cs262".split()
    timeslots = "mwf900 tth1030 mwf1030 tth230 mwf130".split()
    classrooms = "nh253 sb382".split()
    # I know these aren't true to life, but does that really matter?
    assignments = {
        "cs108": "Adams",
        "cs112": "Bailey",
        "cs212": "Norman",
        "cs214": "VanderLinden",
        "cs216": "Adams",
        "cs323": "Bailey",
        "cs262": "Norman"
    }

    domains = {}
    for var in variables:
        domains[var] = []
        faculty = assignments[var]
        for t in timeslots:
            for r in classrooms:
                domains[var].append([faculty, t, r])

    neighbors = {}
    for course in variables:
        neighborlist = []
        for var in variables:
            if var != course:
                neighborlist.append(var)
        neighbors[course] = neighborlist

    def sched_constraint(A, a, B, b, recurse=0):
        """checks that there is only 1 class at a time in a room"""
        if a[1] == b[1] and a[2] == b[2]:
            return False
        """checks that profs are not teaching 2 classes in the same room"""
        if a[0] == b[0] and a[1] == b[1]:
            return False

        if recurse != 0:
            return True

        return sched_constraint(B, b, A, a, 1)

    return CSP(variables, domains, neighbors, sched_constraint)


def solveSchedule(algorithm, **args):
    print("Algorithm type: " + algorithm.__name__)
    newSched = Schedule()
    ans = algorithm(newSched, **args)
    # print(ans)
    for course in "cs108 cs112 cs212 cs214 cs216 cs323 cs262".split():
        print(course, end=' ')

        for (var, val) in ans.items():
            if var == course:
                print(val, end=' ')
        print()
    print()


solveSchedule(min_conflicts, max_steps=30)
solveSchedule(backtracking_search, select_unassigned_variable=mrv, inference=forward_checking)
ac3Sol = AC3(Schedule())
