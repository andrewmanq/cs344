"""
Run the various CSP solvers on selected Sudoku puzzles.
These calls are mostly copied/adapted from AIMA Python.

@author: kvlinden
@version 14feb2013
"""
from tools.aima.csp import Sudoku, easy1, AC3, harder1, backtracking_search, mrv, \
    forward_checking, min_conflicts
from tools.aima.search import depth_first_graph_search
import time

# 1. Set up the puzzle.
#puzzle = Sudoku('483921657967345821251876493548132976729564138136798245372689514814253769695417382') # solved (Figure 6.4.b)
puzzle = Sudoku('..3.2.6..9..3.5..1..18.64....81.29..7.......8..67.82....26.95..8..2.3..9..5.1.3..') # easy (Figure 6.4.a)
#puzzle = Sudoku('4173698.5.3..........7......2.....6.....8.4......1.......6.3.7.5..2.....1.4......') # harder (csp.py)
#puzzle = Sudoku('1....7.9..3..2...8..96..5....53..9...1..8...26....4...3......1..4......7..7...3..') # hardest (AI Escargot)

print('Start:')
puzzle.display(puzzle.infer_assignment())

start = time.time()

# 2. Solve the puzzle.
#depth_first_graph_search(puzzle)
#AC3(puzzle)
backtracking_search(puzzle)
#min_conflicts(puzzle)

end = time.time()

# 3. Print the results.  
print
if puzzle.goal_test(puzzle.infer_assignment()):
    print('Solution:')
    puzzle.display(puzzle.infer_assignment())
else:
    print('Failed - domains: ' + str(puzzle.curr_domains))
    puzzle.display(puzzle.infer_assignment())

print('TIME CALC: ' +  str(end - start))


