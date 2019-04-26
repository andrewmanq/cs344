#EX1.4
killer(Butch).
married(Marsellus, Mia).
dead(Zed).
killed(X):-
	footmassage(X, Mia),
	alive(Marsellus).
	# representations are all name-based. Since Marcellus does the killing, he must be alive.
loves(X, Mia):- goodDancer(X).
eats(Jules, X):-
	nutritious(X),
	tasty(X).
	# Jules eats something when it is both nutritious and tasty.

#EX1.5
#1 - yes -- Ron is a wizard.
#2 - no -- witch is not defined.
#3 - no -- hermione is not defined
#4 - no -- hermione is not defined
#5 - yes -- harry is a quidditch player, which means he has a broom in addition to his wand.
#6 - Y = Harry -- lists the people who are wizards
#7 - Y not found -- there are no witches

# QUESTIONS

#b.	Yes. Modus Ponens is an implementation of [if X then Y]. This is symbolized by [then(Y):- if(X).] in prolog.
#c.	Both Propositional logic and Prolog implement boolean logic. Prolog's notation, however, can be much more readable.
#	horn clauses are also less verbose with rulesets while producing virually the same implementations as propositional logic.
#d. Yes. Telling the rules usually involves loading a .pl file. Asking requires interpreting and querying in the swi environment.