
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


#compute3methods('Burglary', dict(JohnCalls=T, MaryCalls=T), burglary)

"""1: P(Alarm | burglary ∧ ¬earthquake) - this one is easy because it's in the probability distribution (B=t | E=f | .94).
Essentially, the alarm is probably going to go off if an actual burglary happens."""
print("________________P(Alarm | burglary ∧ ¬earthquake):_____________________________________________")
compute3methods('Alarm', dict(Burglary=T, Earthquake=F), burglary)

"""#2: P(John | burglary ∧ ¬earthquake) - John calls for 90% of alarms. That means that if the alarm is triggered by
a burglary (which as stated above, is likely), then he will usually call."""
print("________________P(John | burglary ∧ ¬earthquake):______________________________________________")
compute3methods('JohnCalls', dict(Burglary=T, Earthquake=F), burglary)

"""P(Burglary | alarm) - Earthquakes are twice as likely as burglaries.
This means that roughly 2/3 of alarms are earthquakes."""
print("________________P(Burglary | alarm):___________________________________________________________")
compute3methods('Burglary', dict(Alarm=T), burglary)

"""P(Burglary | john ∧ mary) - Most alarms happen for earthquakes. The likelihood of both employees calling is
low to begin with, which makes the chances of that happening to a burglary even smaller."""
print("________________P(Burglary | john ∧ mary):___________________________________________________________")
compute3methods('Burglary', dict(JohnCalls=T, MaryCalls=T), burglary)

