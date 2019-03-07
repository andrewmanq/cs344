from probability import BayesNet, enumeration_ask, elimination_ask, gibbs_ask

# Utility variables
T, F = True, False

sprinklerProblem = BayesNet([
    ('Cloudy', '', 0.5),
    ('Sprinkler', 'Cloudy', {T: 0.10, F: 0.5}),
    ('Rain', 'Cloudy', {T: 0.80, F: 0.2}),
    ('WetGrass', 'Cloudy Rain', {(T, T): 0.9, (T, F): 0.9, (F, T): 0.9, (F, F): 0.0})
    ])