
from probability import BayesNet, enumeration_ask, elimination_ask, gibbs_ask

# Utility variables
T, F = True, False

# From AIMA code (probability.py) - Fig. 14.2 - burglary example
burglary = BayesNet([
    ('Burglary', '', 0.001),
    ('Earthquake', '', 0.002),
    ('Alarm', 'Burglary Earthquake', {(T, T): 0.95, (T, F): 0.94, (F, T): 0.29, (F, F): 0.001}),
    ('JohnCalls', 'Alarm', {T: 0.90, F: 0.05}),
    ('MaryCalls', 'Alarm', {T: 0.70, F: 0.01})
    ])


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


#compute3methods('Burglary', dict(JohnCalls=T, MaryCalls=T), burglary)

compute3methods('Alarm', dict(Burglary=T, Earthquake=F), burglary)

