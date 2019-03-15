from probability import BayesNet, enumeration_ask, elimination_ask, gibbs_ask

# Utility variables
T, F = True, False

sprinklerProblem = BayesNet([
    ('Cloudy', '', 0.5),
    ('Sprinkler', 'Cloudy', {T: 0.10, F: 0.5}),
    ('Rain', 'Cloudy', {T: 0.80, F: 0.2}),
    ('WetGrass', 'Cloudy Rain', {(T, T): 0.9, (T, F): 0.9, (F, T): 0.9, (F, F): 0.0})
    ])

print("i. P(Cloudy) = ")
print("0.5")
"""This one is easy: Cloudiness is already defined as a 0.5 chance"""

print("\nii. (Sprinker | cloudy) = ")
print(enumeration_ask('Sprinkler', dict(Cloudy=T), sprinklerProblem).show_approx())
"""We know that the Sprinkler has a .1% chance of going off on a cloudy day."""

print("\niii. P(Cloudy| the sprinkler is running and it’s not raining) =")
print(enumeration_ask('Cloudy', dict(Sprinkler=T, Rain=F), sprinklerProblem).show_approx())

print("\niv. P(WetGrass | it’s cloudy, the sprinkler is running and it’s raining) =")
print(enumeration_ask('WetGrass', dict(Cloudy=T, Sprinkler=T, Rain=T), sprinklerProblem).show_approx())

print("\nv. P(Cloudy | the grass is not wet) =")
print(enumeration_ask('Cloudy', dict(WetGrass=F), sprinklerProblem).show_approx())

