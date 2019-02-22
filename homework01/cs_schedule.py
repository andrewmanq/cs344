from csp import *
from search import *

def Schedule():
    """Return an instance of the Schedule."""
    Courses = 'cs108 cs112 cs212 cs214 cs216 cs323 cs262'.split()
    Faculty = 'Adams Bailey Norman VanderLinden'.split()
    Timeslots = 'mwf900 tth1030 mwf1030 tth100'.split()
    Classrooms = 'nh256 sb382'.split()
    variables = Courses
    domains = {}
    for var in variables:
        triples = []
        for fac in Faculty:
            for time in Timeslots:
                for room in Classrooms:
                    triples.append([fac, time, room])
        domains.append(var=triples)

    neighbors = {}
    for var in variables:

    def zebra_constraint(A, a, B, b, recurse=0):
        same = (a == b)
        next_to = abs(a - b) == 1
        if A == 'Englishman' and B == 'Red':
            return same
        if A == 'Spaniard' and B == 'Dog':
            return same
        if A == 'Chesterfields' and B == 'Fox':
            return next_to
        if A == 'Norwegian' and B == 'Blue':
            return next_to
        if A == 'Kools' and B == 'Yellow':
            return same
        if A == 'Winston' and B == 'Snails':
            return same
        if A == 'LuckyStrike' and B == 'OJ':
            return same
        if A == 'Ukranian' and B == 'Tea':
            return same
        if A == 'Japanese' and B == 'Parliaments':
            return same
        if A == 'Kools' and B == 'Horse':
            return next_to
        if A == 'Coffee' and B == 'Green':
            return same
        if A == 'Green' and B == 'Ivory':
            return a - 1 == b
        if recurse == 0:
            return zebra_constraint(B, b, A, a, 1)
        if ((A in Colors and B in Colors) or
                (A in Pets and B in Pets) or
                (A in Drinks and B in Drinks) or
                (A in Countries and B in Countries) or
                (A in Smokes and B in Smokes)):
            return not same
        raise Exception('error')
    return CSP(variables, domains, neighbors, zebra_constraint)