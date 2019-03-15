
from probability import JointProbDist, enumerate_joint_ask

# The Joint Probability Distribution Fig. 13.3 (from AIMA Python)
P = JointProbDist(['Toothache', 'Cavity', 'Catch'])
T, F = True, False
P[T, T, T] = 0.108; P[T, T, F] = 0.012
P[F, T, T] = 0.072; P[F, T, F] = 0.008
P[T, F, T] = 0.016; P[T, F, F] = 0.064
P[F, F, T] = 0.144; P[F, F, F] = 0.576

"""
Hand work:
P(cavity|catch) = P(cavity ^ catch) / P(catch)
P(catch) = .108 + .016 + .072 + .144 = .34
P(cavity ^ catch) = .108 + .072

Answer: 0.5294
"""
#PC = enumerate_joint_ask('Cavity', {'Catch': T}, P)
#print(PC.show_approx())


#---------------------------COIN PROBABILITY--------------------------|
P = JointProbDist(['coin1', 'coin2'])
H, T = True, False
P[H, H] = 1/4
P[H, T] = 1/4
P[T, H] = 1/4
P[T, T] = 1/4

answ = P('coin2', {'coin1' : H}, P)
print(answ.show_approx())
"""above code returns .529 -- statistically, the result of the first toss should not affect the second because they are exclusive.
    I can see why joint probability distributions don't always make sense in the right situations."""