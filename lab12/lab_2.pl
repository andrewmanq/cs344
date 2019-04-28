#a.

#EX 2.1

#Q1 - yes

#Q2 - no

#Q8 - X = bread - yes

#Q9 - no - either x or y must be instantiated

#Q14 - no

#EX2.2
#Explain how Prolog does its unification and reasoning.
#If you have issues getting the results you’d expect, are there things you can do to game the system?

#Prolog generates a series of queries for what it's looking for. If it finds that a term satisfies all queries, it terminates.
#each pairing of terms is symbolized by a new synthetic variable.
#Sometimes it helps to simplify a function instead of complicate with multiple branghing unification queries.

#b. Does inference in propositional logic use unification? Why or why not?
#Not explicitly. Since unification is part of predicate logic, it isn't as applicable to PL.

#c. Does Prolog inferencing use resolution? Why or why not?
#Yes. The actual way it breaks down problems by resolving multiple clauses into one new clause is exactly in line with resolution inferrence.