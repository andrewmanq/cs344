"""
Run the various CSP solvers on the nQueens problem.
These calls are mostly copied/adapted from AIMA Python.

@author: kvlinden
@version 14feb2013
"""

from tools.aima.csp import backtracking_search, NQueensCSP, min_conflicts, mrv, \
    forward_checking, AC3
from tools.aima.search import depth_first_graph_search
import logging
import time

# 1. Set up the problem.
n = 50
problem = NQueensCSP(n)

start = time.time()

# 2. Solve the problem.
# There is a bug in the DFS code (even for 1-queens), so skip this one.
#solution = depth_first_graph_search(problem)
#solution = AC3(problem);
#solution = backtracking_search(problem)
solution = min_conflicts(problem, 130)

end = time.time()

# 3. Print the results.  
print
# Handle AC3 solutions (boolean values) first, they behave differently.
if type(solution) is bool:
    if solution and problem.goal_test(problem.infer_assignment()):
        print('AC3 Solution:')
    else:
        print('AC Failure:')
    print(problem.curr_domains)

# Handle other solutions next.
elif problem.goal_test(solution):
    print('Solution:')
    print(solution)
    problem.display(problem.infer_assignment())
else:
    print('Failed - domains: ' + str(problem.curr_domains))
    problem.display(problem.infer_assignment())

print('TIME CALC: ' + str(end - start) + '\n')