
witch(X) :- burns(X).
burns(X) :- wood(X).
wood(X) :- floats(X).
floats(X) :- equalWeight(duck, X).
equalWeight(duck, woman).
#?- witch(woman).
# true.