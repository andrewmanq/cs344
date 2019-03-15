
from probability import BayesNet, enumeration_ask, elimination_ask, gibbs_ask

# Utility variables
T, F = True, False

happiness = BayesNet([
    ('Sunny', '', 0.7),
    ('Raise', '', 0.01),
    ('Happy', 'Sunny Raise', {(T, T): 1.0, (T, F): 0.7, (F, T): 0.9, (F, F): 0.1})
    ])

"""Uses all methods to compute the answer because why not?"""
def compute3methods(X, e, bn):

    # Compute P(Burglary | John and Mary both call).
    print("---")
    print("enumeration_ask: ")
    print(enumeration_ask(X, e, bn).show_approx())
    print("---")
    # elimination_ask() is a dynamic programming version of enumeration_ask().
    print("elimination_ask: ")
    print(elimination_ask(X, e, bn).show_approx())
    print("---")
    # gibbs_ask() is an approximation algorithm helps Bayesian Networks scale up.
    print("gibbs_ask: ")
    print(gibbs_ask(X, e, bn).show_approx())
    print("---")
    # See the explanation of the algorithms in AIMA Section 14.4.

"""Exercise 5.3

1a: P(Raise | Sunny) = .01, because these variables don't affect each other. Probability of a raise
    stays the same. 
"""
print("-------------------------------------P(Raise | Sunny)")
compute3methods('Raise', dict(Sunny=T), happiness)

"""
2a: P(Raise | Happy ^ Sunny)    *@ is alpha
    = @P(Raise, Happy, Sunny)
    = @ <P(Happy | Raise ^ Sunny) * P(Sunny) * P(Raise), P(Happy | !Raise ^ Sunny) * P(Sunny) * P(!Raise)>
    = @ <.007, .4851
    T: .0142, F: 0.986
    
    The chance of a raise has gone up, because the fact that he/she is happy means we have an insight into
    the results of each outcome.
"""
print("-------------------------------------P(Raise | Happy ^ Sunny)")
compute3methods('Raise', dict(Happy=T, Sunny=T), happiness)

"""
1b. P(Raise | happy)
The probability of a raise should go up slightly, thanks to the fact that the agent has been caused to
be happy (which usually has an explainable cause including raises)
"""
print("-------------------------------------P(Raise | happy)")
compute3methods('Raise', dict(Happy=T), happiness)

"""
1b. P(Raise | happy)
The probability of a raise should go up even more. Because sun is not causing the happiness, there
is much more opportunity for a raise to become the best explanation.
"""
print("-------------------------------------P(Raise | happy ∧ ¬sunny)")
compute3methods('Raise', dict(Happy=T, Sunny=F), happiness)

