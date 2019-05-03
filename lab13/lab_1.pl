#EX3.2
directlyIn(katrina, olga).
directlyIn(olga, natasha).
directlyIn(natasha, irina).

in(X, Y) := directlyIn(X, Y).

in(X, Y) := directlyIn(X, Z),
			in(Z, Y).

#EX4.5
tran(eins,one).
tran(zwei,two).
tran(drei,three).
tran(vier,four).
tran(fuenf,five).
tran(sechs,six).
tran(sieben,seven).
tran(acht,eight).
tran(neun,nine). 

listTran([], []).
listtran([X|T1], [Y|T2]) :-
    tran(X, Y),
	listtran(T1, T2).